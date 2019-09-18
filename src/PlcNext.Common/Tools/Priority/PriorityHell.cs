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
using System.Linq;
using System.Text;

namespace PlcNext.Common.Tools.Priority
{
    internal class PriorityHell : IPriorityMaster
    {
        public IReadOnlyCollection<T> SortPriorities<T>(IEnumerable<T> unsortedEnumerable) where T : IPrioritySubject
        {
            List<T> sortedSubjects = new List<T>();
            List<T> unsortedSubjects = new List<T>(unsortedEnumerable);
            while (unsortedSubjects.Any())
            {
                T subject = FindResolvableSubject();
                if (subject == null)
                {
                    throw new FormattableException($"Could not find any resolved priority subjects:{Environment.NewLine}" +
                                                   $"{PrintSubjectState()}");
                }

                InsertSubject(subject);
            }

            return sortedSubjects;

            T FindResolvableSubject()
            {
                return unsortedSubjects.FirstOrDefault(s => (SubjectIdentifier.IsNullOrNone(s.HigherPrioritySubject) || 
                                                             sortedSubjects.Any(s2 => s2.SubjectIdentifier == s.HigherPrioritySubject)) &&
                                                            (SubjectIdentifier.IsNullOrNone(s.LowerPrioritySubject) ||
                                                             sortedSubjects.Any(s2 => s2.SubjectIdentifier == s.LowerPrioritySubject)));
            }

            void InsertSubject(T subject)
            {
                int predecessorPosition = SubjectIdentifier.IsNullOrNone(subject.HigherPrioritySubject)
                                              ? int.MinValue
                                              : sortedSubjects.TakeWhile(s => s.SubjectIdentifier != subject.HigherPrioritySubject).Count();
                int successorPosition = SubjectIdentifier.IsNullOrNone(subject.LowerPrioritySubject)
                                            ? int.MaxValue
                                            : sortedSubjects.TakeWhile(s => s.SubjectIdentifier != subject.LowerPrioritySubject).Count();
                if (successorPosition <= predecessorPosition)
                {
                    throw new FormattableException($"Could not insert subject {SubjectToString(subject)} into sorted collection:{Environment.NewLine}" +
                                                   $"{PrintSubjectState()}");
                }

                sortedSubjects.Insert(Math.Min(sortedSubjects.Count, successorPosition), subject);
                unsortedSubjects.Remove(subject);
            }

            string SubjectToString(T subject)
            {
                return $"Subject \"{subject.SubjectIdentifier}\" [Predecessor: \"{subject.HigherPrioritySubject}\";  Successor: \"{subject.LowerPrioritySubject}\"]";
            }

            string PrintSubjectState()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Sorted subjects:");
                builder.AppendLine("================");
                builder.AppendLine();
                foreach (T subject in sortedSubjects)
                {
                    PrintSubject(subject);
                }
                builder.AppendLine("Unsorted subjects:");
                builder.AppendLine("================");
                builder.AppendLine();
                foreach (T subject in unsortedSubjects)
                {
                    PrintSubject(subject);
                }

                return builder.ToString();

                void PrintSubject(T subject)
                {
                    builder.AppendLine($"Subject \"{subject.SubjectIdentifier}\"");
                    builder.AppendLine($"|_ Predecessor: \"{subject.HigherPrioritySubject}\"");
                    builder.AppendLine($"|_ Successor: \"{subject.LowerPrioritySubject}\"");
                    builder.AppendLine();
                }
            }
        }
    }
}
