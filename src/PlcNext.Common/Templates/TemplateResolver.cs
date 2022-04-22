#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.Common.Templates
{
    internal class TemplateResolver : ITemplateResolver
    {
        private readonly Regex templateAccessRegex = new Regex(@"\$\((?<text_control>[lu]:)?(?<content>(?:[^\.]+?)(?:\.[^\.]+?)*)\)", RegexOptions.Compiled);
        private readonly Regex controlSequenceFinder = new Regex(@"\$\(\[(?<expression>[^\]]+?)\][^\)]*?\)[\s\S]*?\$\(\[end-\1\]\)", RegexOptions.Compiled);
        private readonly Regex greedyContentFinder = 
            new Regex(@"\$\(\[(?<expression>[^\]]+?)\](?<parameter>(?:\$\([^\)]*?\)|[^\)])*?)\)(?<content>[\s\S]*)\$\(\[end-\1\]\)", RegexOptions.Compiled);
        private readonly Regex newlineSearcher = new Regex(@"\s*?(?:\r\n|\r|\n)", RegexOptions.Compiled);

        private readonly ITemplateRepository repository;

        public TemplateResolver(ITemplateRepository repository)
        {
            this.repository = repository;
        }

        public string Resolve(string stringToResolve, IEntityBase dataSource)
        {
            StringBuilder resolved = new StringBuilder();
            int index = 0;
            while (index != stringToResolve.Length - 1)
            {
                Match sequenceFound = controlSequenceFinder.Match(stringToResolve, index);
                if (!sequenceFound.Success)
                {
                    break;
                }

                resolved.Append(ResolveTemplateContent(stringToResolve.Substring(index, sequenceFound.Index - index)));

                string controlSequence = sequenceFound.Groups["expression"].Value;
                string startTag = $"$([{controlSequence}]";
                string endTag = $"$([end-{controlSequence}])";
                int length = sequenceFound.Length;
                index = sequenceFound.Index;
                int nestedSequences =
                    Count(stringToResolve.Substring(index + startTag.Length, length - startTag.Length), startTag);
                int endIndex = index + length;
                while (nestedSequences > 0)
                {
                    int newEnd = stringToResolve.Substring(endIndex)
                                                .IndexOf(endTag, StringComparison.OrdinalIgnoreCase);
                    if (newEnd < 0)
                    {
                        throw new ControlSequenceEndTagNotFoundException(controlSequence);
                    }

                    newEnd = endIndex + newEnd;

                    //Find nested tags between last end tag and this end tag
                    nestedSequences += Count(stringToResolve.Substring(endIndex, newEnd - endIndex), startTag);
                    nestedSequences--;
                    endIndex = newEnd + endTag.Length;
                }

                Match controlSequenceDataMatch = greedyContentFinder.Match(stringToResolve, index, endIndex - index);
                if (!controlSequenceDataMatch.Success)
                {
                    throw new InvalidOperationException("Should not happen");
                }

                resolved.Append(ResolveControlSequence(controlSequenceDataMatch.Groups["expression"].Value,
                                                       controlSequenceDataMatch.Groups["parameter"].Value,
                                                       controlSequenceDataMatch.Groups["content"].Value));
                if (controlSequenceDataMatch.Value.Contains('\n'))
                {
                    Match newlineMatch = newlineSearcher.Match(stringToResolve, endIndex);
                    if (newlineMatch.Success && newlineMatch.Index == endIndex)
                    {
                        endIndex += newlineMatch.Length;
                    }
                }

                index = endIndex;
            }

            resolved.Append(ResolveTemplateContent(stringToResolve.Substring(index)));
            return resolved.ToString();

            string ResolveControlSequence(string sequence, string parameter, string content)
            {
                switch (sequence.ToUpperInvariant())
                {
                    case "IF-EXIST":
                        return IfSequence(IfExistCondition);
                    case "IF-SPECIFIED":
                        return IfSequence(IfSpecifiedCondition);
                    case "IF":
                        return IfSequence(IfCondition);
                    case "FOREACH":
                        return ForeachSequence();
                    case "NO-DUPLICATE-LINES":
                        return RemoveDuplicateLinesSequence();
                    default:
                        throw new UnrecognizedControlSequenceException(sequence);
                }

                string RemoveDuplicateLinesSequence()
                {
                    if (!string.IsNullOrEmpty(parameter))
                    {
                        throw new NoDuplicateLinesParameterMismatchException();
                    }

                    string[] lines = (Resolve(content, dataSource)).Split(new[] {'\r', '\n'},
                                                                          StringSplitOptions.RemoveEmptyEntries);
                    string result = string.Join(Environment.NewLine, lines.Distinct());
                    return string.IsNullOrEmpty(result) ? string.Empty : result + Environment.NewLine;
                }

                string IfSequence(Func<bool> conditionCheck)
                {
                    if (string.IsNullOrEmpty(parameter))
                    {
                        throw new IfSequenceParameterMismatchException();
                    }
                    if ((content.StartsWith("\n", StringComparison.Ordinal) ||
                         content.StartsWith("\r\n", StringComparison.Ordinal)) &&
                        (content.EndsWith("\n", StringComparison.Ordinal) ||
                         content.EndsWith("\r\n", StringComparison.Ordinal)))
                    {
                        //This would lead to unwanted empty lines
                        content = content.TrimStart('\r').TrimStart('\n');
                    }

                    bool condition = conditionCheck();

                    return IfResult(condition);
                }

                bool IfExistCondition()
                {
                    CommandDefinition definition = dataSource.Value<CommandDefinition>();
                    Argument argument = definition?.Argument<Argument>(parameter);

                    return argument != null ||
                           dataSource.HasContent(parameter);
                }

                bool IfSpecifiedCondition()
                {
                    CommandDefinition definition = dataSource.Value<CommandDefinition>();
                    Argument argument = definition?.Argument<Argument>(parameter);
                    bool specified;
                    if (argument != null)
                    {
                        specified = argument.IsDefined;
                    }
                    else
                    {
                        specified = dataSource.HasContent(parameter) &&
                                    !string.IsNullOrEmpty(dataSource[parameter].Value<string>());
                    }

                    return specified;
                }

                bool IfCondition()
                {
                    string condition = Resolve(parameter, dataSource);
                    return condition.ResolveCondition();
                }

                string IfResult(bool condition)
                {
                    string result = string.Empty;
                    string[] elseSplit = content.Split(new[] {"$([else])"}, StringSplitOptions.RemoveEmptyEntries);
                    if (condition)
                    {
                        result = Resolve(elseSplit[0], dataSource);
                    }
                    else if (elseSplit.Length == 2)
                    {
                        result = Resolve(elseSplit[1], dataSource);
                    }

                    return result;
                }

                string ForeachSequence()
                {
                    StringBuilder foreachResult = new StringBuilder();
                    if ((content.StartsWith("\n", StringComparison.Ordinal) || 
                         content.StartsWith("\r\n", StringComparison.Ordinal)) &&
                        (content.EndsWith("\n", StringComparison.Ordinal) || 
                         content.EndsWith("\r\n", StringComparison.Ordinal)))
                    {
                        //This would lead to unwanted empty lines
                        content = content.TrimStart('\r').TrimStart('\n');
                    }

                    string[] nameSplit = parameter.Split(new[] {"[in]"}, StringSplitOptions.RemoveEmptyEntries);
                    if (nameSplit.Length != 2)
                    {
                        throw new ForeachSequenceParameterMismatchException(parameter);
                    }

                    string elementName = nameSplit[0].Trim();
                    string[] ofTypeSplit =
                        nameSplit[1].Split(new[] {"[of-type]"}, StringSplitOptions.RemoveEmptyEntries);
                    string splitPart = ofTypeSplit.Length == 2 ? ofTypeSplit[1] : ofTypeSplit[0];
                    string[] split = splitPart.Split(new[] { "[split]" }, StringSplitOptions.RemoveEmptyEntries);
                    string collection = ofTypeSplit.Length == 2 ? ofTypeSplit[0].Trim() : split[0].Trim();
                    string filter = string.Empty;
                    string splitSize = split.Length == 2 ? split[1].Trim() : string.Empty;
                    if (!int.TryParse(splitSize, out int chunksize))
                    {
                        chunksize = -1;
                    }
                    if (ofTypeSplit.Length == 2)
                    {
                        filter = split.Length == 2 ? split[0].Trim() : ofTypeSplit[1].Trim();
                    }

                    string[] path = collection.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                    IEnumerable<Entity> data = ResolveRecursively(path);

                    if (!string.IsNullOrEmpty(filter))
                    {
                        data = data.Where(TemplateEqualsFilter);
                    }

                    ForeachItemContainer container = new ForeachItemContainer(elementName);

                    if (chunksize > 0)
                    {
                        var query = data.Select((entity, idx) => new { idx, entity });
                        IEnumerable<IEnumerable<Entity>> dataChunks = query.GroupBy(x => x.idx / chunksize, a => a.entity);
                        using (dataSource.AddTemporaryDataSource(container))
                        using (dataSource.SkipCaching(elementName))
                        {
                            foreach (IEnumerable<Entity> chunk in dataChunks)
                            {

                                using (dataSource.SkipCaching(EntityKeys.ChunkStartKey))
                                using (dataSource.SkipCaching(EntityKeys.ChunkEndKey))
                                {
                                    string start = data.TakeWhile(x => !x.Equals(chunk.FirstOrDefault())).Count().ToString(CultureInfo.InvariantCulture);
                                    string end = data.TakeWhile(x => !x.Equals(chunk.LastOrDefault())).Count().ToString(CultureInfo.InvariantCulture);

                                    if (dataSource is Entity)
                                    {
                                        Entity dataSourceEntity = dataSource as Entity;
                                        container.Current = dataSourceEntity.Create(Guid.NewGuid().ToByteString(), chunk);
                                        using (container.Current.AddTemporaryDataSource(new DataChunk(start, end)))
                                            foreachResult.Append(Resolve(content, dataSource));
                                    }
                                    else
                                    {
                                        throw new FormattableException($"The datasource should be an entity but is of type {dataSource.GetType()}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (dataSource.AddTemporaryDataSource(container))
                        using (dataSource.SkipCaching(elementName))
                        {
                            foreach (Entity entity in data)
                            {
                                container.Current = entity;
                                foreachResult.Append(Resolve(content, dataSource));
                            }
                        }
                    }
                    return foreachResult.ToString();

                    bool TemplateEqualsFilter(Entity entity)
                    {
                        return entity.Template().TemplateNames(repository)
                                     .Any(n => n.Equals(filter, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            string ResolveTemplateContent(string resolvable)
            {
                string result = resolvable;
                Match controlSequenceMatch = templateAccessRegex.Match(resolvable);
                while (controlSequenceMatch.Success)
                {
                    string content = controlSequenceMatch.Groups["content"].Value;
                    if (content.StartsWith("[", StringComparison.Ordinal) &&
                        content.EndsWith("]", StringComparison.Ordinal))
                    {
                        throw new WildControlSequenceException(content);
                    }

                    string[] path = content.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                    string value = (ResolveRecursively(path)).Value<string>();
                    if (controlSequenceMatch.Groups["text_control"].Success)
                    {
                        string textControlSequence = controlSequenceMatch.Groups["text_control"].Value;
                        textControlSequence = textControlSequence.Substring(0, textControlSequence.Length - 1);
                        value = ResolveTextControlSequences(value, textControlSequence);
                    }
                    result = result.Replace(controlSequenceMatch.Value, value);
                    controlSequenceMatch = controlSequenceMatch.NextMatch();
                }

                return result;
            }

            IEntityBase ResolveRecursively(string[] path)
            {
                IEntityBase current = dataSource;
                foreach (string part in path)
                {
                    current = current[part];
                    if (current.HasValue<string>())
                    {
                        string value = Resolve(current.Value<string>(), current);
                        current.SetValue(value);
                    }
                }

                return current;
            }

            int Count(string data, string substring)
            {
                return (data.Length - data.Replace(substring, string.Empty).Length) / substring.Length;
            }
        }

        Task<string> ITemplateResolver.ResolveAsync(string stringToResolve, IEntityBase dataSource)
        {
            return Task.Factory.StartNew(() => Resolve(stringToResolve, dataSource),
                                         CancellationToken.None, TaskCreationOptions.LongRunning,
                                         TaskScheduler.Default);
        }

        private string ResolveTextControlSequences(string value, string textControlSequence)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(textControlSequence))
            {
                return value;
            }
            switch (textControlSequence.ToUpperInvariant())
            {
                case "L":
                    return value.Substring(0, 1).ToLowerInvariant() +
                           (value.Length > 1 ? value.Substring(1) : string.Empty);
                case "U":
                    return value.Substring(0, 1).ToUpperInvariant() +
                           (value.Length > 1 ? value.Substring(1) : string.Empty);
                default:
                    throw new UnrecognizedControlSequenceException(textControlSequence);
            }
        }
    }
}
