#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace PlcNext.Common.Templates.Description
{
    public partial class TemplateDescription : MergableElement, IEquatable<TemplateDescription>
    {
        [XmlIgnore]
        public bool isHiddenSpecified { get; set; }

        [XmlIgnore]
        public bool isRootSpecified { get; set; }

        [XmlIgnore]
        public bool identifierSpecified { get; set; }

        [XmlIgnore]
        public bool isRelevantForProjectNamespaceSpecified { get; set; }

        public override string ToString()
        {
            return $"{nameof(name)}: {name}, {nameof(version)}: {version}, {nameof(basedOn)}: {basedOn}";
        }

        public bool Equals(TemplateDescription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(nameField, other.nameField);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TemplateDescription) obj);
        }

        public override int GetHashCode()
        {
            return (nameField != null ? nameField.GetHashCode() : 0);
        }

        public static bool operator ==(TemplateDescription left, TemplateDescription right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TemplateDescription left, TemplateDescription right)
        {
            return !Equals(left, right);
        }

        protected override string[] Unmergables => new[]
        {
            nameof(basedOn),
            nameof(isHidden),
            nameof(name),
            nameof(requiredCliVersion),
            nameof(supportedFirmwareVersions),
            nameof(version),
        };
    }

    public partial class templateReference : MergableElement, IEquatable<templateReference>
    {
        public override string ToString()
        {
            return $"{nameof(template)}: {template}";
        }

        [XmlIgnore]
        public bool templateSpecified { get; set; }

        public bool Equals(templateReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(templateField, other.templateField);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((templateReference) obj);
        }

        public override int GetHashCode()
        {
            return (templateField != null ? templateField.GetHashCode() : 0);
        }

        public static bool operator ==(templateReference left, templateReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(templateReference left, templateReference right)
        {
            return !Equals(left, right);
        }

        protected override string[] Unmergables => new[]
        {
            nameof(template),
        };
    }

    public partial class templateFile : MergableElement, IEquatable<templateFile>
    {
        [XmlIgnore]
        public bool pathSpecified { get; set; }

        [XmlIgnore]
        public bool keySpecified { get; set; }

        [XmlIgnore]
        public bool nameSpecified { get; set; }

        [XmlIgnore]
        public bool templateSpecified { get; set; }

        [XmlIgnore]
        public bool deployPathSpecified { get; set; }

        public bool Equals(templateFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(nameField, other.nameField);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((templateFile) obj);
        }

        public override int GetHashCode()
        {
            return (nameField != null ? nameField.GetHashCode() : 0);
        }

        public static bool operator ==(templateFile left, templateFile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(templateFile left, templateFile right)
        {
            return !Equals(left, right);
        }

        protected override string[] Unmergables => new[]
        {
            nameof(name),
        };
    }

    public partial class templateRelationshipBase : MergableElement, IEquatable<templateRelationshipBase>
    {
        [XmlIgnore]
        public bool nameSpecified { get; set; }

        public bool Equals(templateRelationshipBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(nameField, other.nameField);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((templateRelationshipBase) obj);
        }

        public override int GetHashCode()
        {
            return (nameField != null ? nameField.GetHashCode() : 0);
        }

        public static bool operator ==(templateRelationshipBase left, templateRelationshipBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(templateRelationshipBase left, templateRelationshipBase right)
        {
            return !Equals(left, right);
        }

        protected override string[] Unmergables => new[]
        {
            nameof(name),
        };
    }

    public partial class templateRelationship
    {
        [XmlIgnore]
        public bool typeSpecified { get; set; }

        [XmlIgnore]
        public bool multiplicitySpecified { get; set; }
    }

    public partial class templateRelationshipInstance
    {
        [XmlIgnore]
        public bool valueSpecified { get; set; }

        public override string ToString()
        {
            return $"{nameof(name)}: {name}, {nameof(value)}: {value}";
        }
    }

    public partial class templateArgumentBase : MergableElement, IEquatable<templateArgumentBase>
    {
        [XmlIgnore]
        public bool nameSpecified { get; set; }

        public bool Equals(templateArgumentBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(nameField, other.nameField);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((templateArgumentBase) obj);
        }

        public override int GetHashCode()
        {
            return (nameField != null ? nameField.GetHashCode() : 0);
        }

        public static bool operator ==(templateArgumentBase left, templateArgumentBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(templateArgumentBase left, templateArgumentBase right)
        {
            return !Equals(left, right);
        }

        protected override string[] Unmergables => new[]
        {
            nameof(name),
        };
    }

    public partial class templateGeneratedFile
    {
        [XmlIgnore]
        public bool aggregatedSpecified { get; set; }

        [XmlIgnore]
        public bool generatorSpecified { get; set; }
    }

    public partial class templateArgumentDefinition
    {
        [XmlIgnore]
        public bool @defaultSpecified { get; set; }

        [XmlIgnore]
        public bool formatSpecified { get; set; }

        [XmlIgnore]
        public bool hasvalueSpecified { get; set; }

        [XmlIgnore]
        public bool helpSpecified { get; set; }

        [XmlIgnore]
        public bool multiplicitySpecified { get; set; }

        [XmlIgnore]
        public bool shortnameSpecified { get; set; }

        [XmlIgnore]
        public bool separatorSpecified { get; set; }

        [XmlIgnore]
        public bool ValueRestrictionSpecified { get; set; }
    }

    public partial class templateArgumentInstance
    {
        [XmlIgnore]
        public bool valueSpecified { get; set; }
    }

    public abstract class MergableElement
    {
        protected abstract string[] Unmergables { get; }

        public void Merge(MergableElement baseElement)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (Unmergables.Contains(property.Name))
                {
                    continue;
                }
                if (property.PropertyType.IsArray)
                {
                    Array values = (Array) property.GetValue(this);
                    Array baseValues = (Array) property.GetValue(baseElement);
                    if (values == null)
                    {
                        property.SetValue(this, baseValues);
                        continue;
                    }
                    if (baseValues == null)
                    {
                        continue;
                    }

                    bool mergable = typeof(MergableElement).IsAssignableFrom(values.GetType().GetElementType());
                    IList result = MakeList(values.GetType().GetElementType());
                    foreach (object value in values)
                    {
                        if (mergable)
                        {
                            MergableElement baseValue = baseValues.OfType<MergableElement>().FirstOrDefault(m => m.Equals(value));
                            if (baseValue != null)
                            {
                                ((MergableElement)value).Merge(baseValue);
                            }
                        }
                        result.Add(value);
                    }

                    foreach (object additional in baseValues.Cast<object>().Except(values.Cast<object>()))
                    {
                        result.Add(additional);
                    }

                    property.SetValue(this, result.GetType().GetMethod("ToArray")?.Invoke(result,new object[0]));
                }
                else
                {
                    object value = property.GetValue(this);
                    object baseValue = property.GetValue(baseElement);
                    property.SetValue(this, MergeValues(value, baseValue, this.IsSpecified(property.Name)));
                }
            }

            IList MakeList(System.Type type)
            {
                System.Type listType = typeof(List<>);
                System.Type constructedListType = listType.MakeGenericType(type);

                return (IList) Activator.CreateInstance(constructedListType);
            }

            object MergeValues(object value, object baseValue, bool specified)
            {
                if (value is MergableElement mergableElement &&
                    baseValue is MergableElement baseMergableElement)
                {
                    mergableElement.Merge(baseMergableElement);
                    return mergableElement;
                }

                return !specified ? baseValue : value;
            }
        }
    }

    internal static class Helper
    {
        public static bool IsSpecified(this MergableElement obj, string propertyName)
        {
            PropertyInfo property = obj.GetType().GetProperty($"{propertyName}Specified");
            if (property != null && property.PropertyType == typeof(bool))
            {
                return (bool) property.GetValue(obj);
            }

            return true;
        }
    }
}
