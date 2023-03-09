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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CommandLine;
using CommandLine.Text;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Tools;
using TypeInfo = System.Reflection.TypeInfo;

namespace PlcNext.CommandLine
{
    internal class VerbTypeFactory
    {
        private readonly string name;
        private readonly string helpText;
        private bool useChildVerbsAsCategory;
        private readonly List<Option> options = new List<Option>();
        private readonly List<Example> examples = new List<Example>();
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        private Type baseType = typeof(DynamicVerb);
        
        private static ModuleBuilder moduleBuilder;

        private static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (moduleBuilder == null)
                {
                    AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid()
                                                                                                                 .ToString("N",
                                                                                                                      CultureInfo.InvariantCulture)), 
                                                                                            AssemblyBuilderAccess.Run);
                    moduleBuilder = assemblyBuilder.DefineDynamicModule("DefaultModule");
                }
                return moduleBuilder;
            }
        }

        private VerbTypeFactory(string name, string helpText)
        {
            this.name = name;
            this.helpText = helpText;
        }

        public VerbTypeFactory SetBaseType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type baseType)
        {
            this.baseType = baseType;
            return this;
        }

        public VerbTypeFactory EnableUseChildVerbsAsCategory()
        {
            useChildVerbsAsCategory = true;
            return this;
        }

        public VerbTypeFactory AddOption(string name, char shortName, bool mandatory, string help,
                                         OptionValueType optionValueType, string setName)
        {
            options.Add(new Option(name, shortName, mandatory, help, optionValueType, setName));
            return this;
        }

        public VerbTypeFactory AddExample(string command, string description)
        {
            examples.Add(new Example(command, description));
            return this;
        }

        public static VerbTypeFactory Create(string name, string helpText)
        {
            return new VerbTypeFactory(name, helpText);
        }

        public Type Build()
        {
            TypeInfo typeInfo = CompileResultTypeInfo();
            return typeInfo.AsType();

            TypeInfo CompileResultTypeInfo()
            {
                TypeBuilder typeBuilder = GetTypeBuilder();
                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                                     MethodAttributes.RTSpecialName);
                if (useChildVerbsAsCategory)
                {
                    AddUseChildVerbsAsCategory();
                }
                AddVerbAttribute();
                AddOptions();
                AddExamples();

                TypeInfo objectTypeInfo = typeBuilder.CreateTypeInfo();
                return objectTypeInfo;

                TypeBuilder GetTypeBuilder()
                {
                    string typeSignature = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
                    TypeBuilder result = ModuleBuilder.DefineType(typeSignature,
                                                                  TypeAttributes.Public |
                                                                  TypeAttributes.Class |
                                                                  TypeAttributes.AutoClass |
                                                                  TypeAttributes.AnsiClass |
                                                                  TypeAttributes.BeforeFieldInit |
                                                                  TypeAttributes.AutoLayout,
                                                                  baseType);
                    return result;
                }

                void AddUseChildVerbsAsCategory()
                {
                    Type childVerbsAsCategory = typeof(UseChildVerbsAsCategoryAttribute);
                    ConstructorInfo constructor = childVerbsAsCategory.GetConstructor(Array.Empty<Type>());
                    typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(constructor, Array.Empty<object>()));
                }

                void AddVerbAttribute()
                {
                    Type verbAttribute = typeof(VerbAttribute);
                    ConstructorInfo constructor = verbAttribute.GetConstructor(new[] {typeof(string), typeof(bool)});
                    PropertyInfo helpTextProperty = verbAttribute.GetProperty("HelpText");
                    typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(constructor, new object[] {name, false},
                                                                              new[] {helpTextProperty},
                                                                              new object[] {helpText}));
                }

                void AddExamples()
                {
                    if (!examples.Any())
                    {
                        return;
                    }
                    Type propertyType = typeof(IEnumerable<UsageExample>);
                    PropertyBuilder builder = CreateExampleProperty();
                    AddUsageAttribute();

                    void AddUsageAttribute()
                    {
                        Type usageAttributeType = typeof(UsageAttribute);
                        ConstructorInfo constructor = usageAttributeType.GetConstructor(Array.Empty<Type>());
                        CustomAttributeBuilder usageAttributeBuilder = new CustomAttributeBuilder(constructor, Array.Empty<object>());
                        builder.SetCustomAttribute(usageAttributeBuilder);
                    }

                    PropertyBuilder CreateExampleProperty()
                    {
                        string propertyName = Guid.NewGuid().ToByteString();

                        PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                        MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Static, propertyType, Type.EmptyTypes);
                        ILGenerator getIl = getPropMthdBldr.GetILGenerator();
                        ConstructorInfo constructor = typeof(UsageExample).GetConstructor(new []{typeof(string), typeof(string)});

                        //create new array
                        getIl.Emit(OpCodes.Ldc_I4, examples.Count);
                        getIl.Emit(OpCodes.Newarr, typeof(UsageExample));

                        //set array items
                        for (int i = 0; i < examples.Count; i++)
                        {
                            getIl.Emit(OpCodes.Dup);
                            getIl.Emit(OpCodes.Ldc_I4, i);

                            //create usage example object
                            getIl.Emit(OpCodes.Ldstr, examples[i].Description);
                            getIl.Emit(OpCodes.Ldstr, examples[i].Command);
                            getIl.Emit(OpCodes.Newobj, constructor);

                            getIl.Emit(OpCodes.Stelem_Ref);
                        }

                        //Return
                        getIl.Emit(OpCodes.Ret);

                        propertyBuilder.SetGetMethod(getPropMthdBldr);
                        return propertyBuilder;
                    }
                }

                void AddOptions()
                {
                    foreach (Option option in options)
                    {
                        Type propertyType = GetPropertyType(option.ValueType);
                        PropertyBuilder propertyBuilder = CreateProperty(option.Name, propertyType);
                        AddOptionAttribute(propertyBuilder, option);
                    }

                    void AddOptionAttribute(PropertyBuilder propertyBuilder, Option option)
                    {
                        Type optionAttribute = typeof(OptionAttribute);
                        PropertyInfo helpTextProperty = optionAttribute.GetProperty("HelpText");
                        PropertyInfo separatorProperty = optionAttribute.GetProperty("Separator");
                        PropertyInfo requiredProperty = optionAttribute.GetProperty("Required");
                        PropertyInfo setNameProperty = optionAttribute.GetProperty("SetName");
                        ConstructorInfo constructor = optionAttribute.GetConstructor(
                            option.ShortName != default(char)
                                ? new[] {typeof(char), typeof(string)}
                                : new[] {typeof(string)});
                        object[] constructorArgs = option.ShortName != default(char)
                                                       ? new object[] {option.ShortName, option.Name}
                                                       : new object[] {option.Name};
                        List<PropertyInfo> namedProperties = new List<PropertyInfo>(new[] { helpTextProperty, requiredProperty });
                        List<object> propertiesValues = new List<object>(new object[] { option.Help, option.Mandatory });
                        if (option.ValueType == OptionValueType.MultipleValue && !option.Separator.Equals(' '))
                        {
                            namedProperties.Add(separatorProperty);
                            propertiesValues.Add(option.Separator);
                        }

                        if (!string.IsNullOrEmpty(option.SetName))
                        {
                            namedProperties.Add(setNameProperty);
                            propertiesValues.Add(option.SetName);
                        }

                        CustomAttributeBuilder builder = new CustomAttributeBuilder(constructor, constructorArgs, namedProperties.ToArray(),
                                                                                    propertiesValues.ToArray());
                        propertyBuilder.SetCustomAttribute(builder);
                    }

                    Type GetPropertyType(OptionValueType optionValueType)
                    {
                        switch (optionValueType)
                        {
                            case OptionValueType.WithoutValue:
                                return typeof(bool);
                            case OptionValueType.SingleValue:
                                return typeof(string);
                            case OptionValueType.MultipleValue:
                                return typeof(IEnumerable<string>);
                            default:
                                throw new ArgumentException("Unkown option value type", nameof(optionValueType));
                        }
                    }

                    PropertyBuilder CreateProperty(string propertyName, Type propertyType)
                    {
                        FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

                        PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                        MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
                        ILGenerator getIl = getPropMthdBldr.GetILGenerator();

                        getIl.Emit(OpCodes.Ldarg_0);
                        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                        getIl.Emit(OpCodes.Ret);

                        MethodBuilder setPropMthdBldr =
                            typeBuilder.DefineMethod("set_" + propertyName,
                                            MethodAttributes.Public |
                                            MethodAttributes.SpecialName |
                                            MethodAttributes.HideBySig,
                                            null, new[] { propertyType });

                        ILGenerator setIl = setPropMthdBldr.GetILGenerator();
                        Label modifyProperty = setIl.DefineLabel();
                        Label exitSet = setIl.DefineLabel();

                        setIl.MarkLabel(modifyProperty);
                        setIl.Emit(OpCodes.Ldarg_0);
                        setIl.Emit(OpCodes.Ldarg_1);
                        setIl.Emit(OpCodes.Stfld, fieldBuilder);

                        setIl.Emit(OpCodes.Nop);
                        setIl.MarkLabel(exitSet);
                        setIl.Emit(OpCodes.Ret);

                        propertyBuilder.SetGetMethod(getPropMthdBldr);
                        propertyBuilder.SetSetMethod(setPropMthdBldr);
                        return propertyBuilder;
                    }
                }
            }
        }

        internal void AddOption(string name, char shortName, bool mandatory, string help, OptionValueType valueType, string setName, char separator)
        {
            options.Add(new Option(name, shortName, mandatory, help, valueType, setName, separator));
        }

        public static void ClearStaticCaches()
        {
            moduleBuilder = null;
        }

        private class Example
        {
            public Example(string command, string description)
            {
                Command = command;
                Description = description;
            }

            public string Command { get; }
            public string Description { get; }
        }

        private class Option
        {
            public string Name { get; }
            public char ShortName { get; }
            public bool Mandatory { get; }
            public string Help { get; }
            public OptionValueType ValueType { get; }
            public string SetName { get; }
            public char Separator { get; }

            public Option(string name, char shortName, bool mandatory, string help, OptionValueType optionValueType,
                          string setName)
            {
                Name = name;
                ShortName = shortName;
                Mandatory = mandatory;
                Help = help;
                ValueType = optionValueType;
                SetName = setName;
            }

            public Option(string name, char shortName, bool mandatory, string help, OptionValueType optionValueType, string setName, char separator) : this(name, shortName, mandatory, help, optionValueType, setName)
            {
                Separator = separator;
            }
        }
    }

    public enum OptionValueType
    {
        WithoutValue,
        SingleValue,
        MultipleValue
    }
}
