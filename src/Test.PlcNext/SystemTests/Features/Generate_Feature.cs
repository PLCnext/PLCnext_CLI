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
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.SystemTests.StepDefinitions;
using Test.PlcNext.SystemTests.Tools;
using Test.PlcNext.Tools;
using Xunit;
#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"User generates all necessary files via CLI with different parameters.")]
    public class Generate_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Generate_struct_typemeta_information_on_generate()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithOneStructPort"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("Example",
                                                                                 new []
                                                                                 {
                                                                                     new TypeMember("value1","int32"),
                                                                                     new TypeMember("value2","bit", 0),
                                                                                     new TypeMember("value3","int64", 6),
                                                                                     new TypeMember("value4","int64", 1),
                                                                                 }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_struct_typemeta_information_on_generate_from_inside_project()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithOneStructPort"),
                _ => Given_is_the_working_directory_PATH("ProgramWithOneStructPort"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("Example",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("value1","int32"),
                                                                                                       new TypeMember("value2","bit", 0),
                                                                                                       new TypeMember("value3","int64", 6),
                                                                                                       new TypeMember("value4","int64", 1),
                                                                                                   }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_struct_typemeta_information_with_attributes()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithOneAttributedStructPort"),
                _ => Given_is_the_working_directory_PATH("ProgramWithOneAttributedStructPort"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("Example",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("value1","int32", 0, "Input|Opc"),
                                                                                                       new TypeMember("value2","bit", 0, null),
                                                                                                   }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_struct_ins_struct_and_nested_structs()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithNestedAndStructInStruct"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("supernested",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("schnacken","bit"),
                                                                                                   }),
                                                                             new StructTypemetaStructure("nested",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("fiooba","supernested"),
                                                                                                   }),
                                                                             new StructTypemetaStructure("instruct",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("blubber","nested"),
                                                                                                       new TypeMember("test","supernested"),
                                                                                                   }))).RunAsync();
        }        

        [Scenario]
        [ClassData(typeof(ErrorDataGenerator))]
        public async Task Error_ERROR_on_generating_PROJECT(ErrorInformation error, string project)
        {
            await Runner.AddSteps(
                _ => Given_is_the_project(project),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_error_is_shown(error)).RunAsync();
        }

        [Scenario]
        public async Task Generate_codefiles_for_component()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithComponent"),
                _ => When_I_generate_all_codefiles(),
                _ => Then_codefiles_were_generated_for_the_component("MyComponent", new[] { "examplePort1", "examplePort2" })).RunAsync();
        }

        [Scenario]
        public async Task Delete_redundant_code_files()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("TooManyGeneratedFiles"),
                _ => When_I_generate_all_codefiles(),
                _ => Then_codefiles_were_not_generated_for_the_component("AccessComponent")).RunAsync();
        }

        [Scenario]
        public async Task Generate_all_meta_data_for_direct_struct_initalization()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithComponent"),
                _ => When_I_generate_all_codefiles(),
                _ => When_I_generate_all_metafiles(),
                _ => Then_codefiles_were_generated_for_the_component("OtherComponent", new[] { "examplePort" }),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("InlineStruct",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("fooba","boolean"),
                                                                                                   }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_files_for_component_with_correct_namespaces()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => When_I_generate_all_codefiles(),
                _ => Then_the_generated_components_namespace_starts_with_namespace("DComponent", "A.B.C.D")).RunAsync();
        }

        [Scenario]
        public async Task Generate_library_files_on_generate()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_generate_all_codefiles(),
                _ => Then_the_library_files_are_generated()).RunAsync();
        }

        [Scenario]
        public async Task Generate_applibrary_files_on_generate()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AppProject"),
                _ => When_I_generate_all_codefiles(),
                _ => Then_the_library_files_are_generated()
                ).RunAsync();
        }

        [Scenario]
        public async Task Generate_component_provider_files_on_generate()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("TwoComponentsAndPrograms"),
                _ => When_I_generate_all_codefiles(),
                _ => Then_the_provider_files_are_generated_for_component("MyComponent", new[] { "MyProgram", "MyProgram2" }),
                _ => Then_the_provider_files_are_generated_for_component("MyOtherComponent", new[] { "MyOtherProgram"})).RunAsync();
        }

        [Scenario]
        public async Task Generate_progmeta_information_on_generate_from_inside_project()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithDifferentPorts"),
                _ => Given_is_the_working_directory_PATH("ProgramWithDifferentPorts"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_there_are_progmeta_files_with_the_following_content(new ProgmetaData("MyProgram",
                                                                                  new [] {"MyComponent", "MyProgram"},
                                                                                  new []
                                                                                  {
                                                                                      new Portmeta("examplePort1","Example","Input|Opc"),
                                                                                      new Portmeta("examplePort2","Example","Output", 1),
                                                                                      new Portmeta("examplePort3","Example","Output", 5),
                                                                                      new Portmeta("examplePort4","int32","", 0),
                                                                                  }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_progmeta_for_enum_ports()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("EnumTest"),
                _ => Given_is_the_working_directory_PATH("EnumTest"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_there_are_progmeta_files_with_the_following_content(new ProgmetaData("EnumTestProgram",
                                                                                               new[] { "EnumTestComponent", "EnumTestProgram" },
                                                                                               new[]
                                                                                               {
                                                                                                   new Portmeta("OhMyPort","OhMy","Output"),
                                                                                                   new Portmeta("OtherPort","OtherEnum","Input"),
                                                                                                   new Portmeta("StructPort","EnumStruct","Input"),
                                                                                               }))).RunAsync();
        }

        [Scenario]
        public async Task UseCorrectPortInformationWhenPortIsDefinedElsewhere()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DoublePortDeclaration"),
                _ => Given_is_the_working_directory_PATH("DoublePortDeclaration"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_there_are_progmeta_files_with_the_following_content(new ProgmetaData("DoublePortDeclarationProgram",
                                                                                               new[] { "DoublePortDeclarationComponent", "DoublePortDeclarationProgram" },
                                                                                               new[]
                                                                                               {
                                                                                                   new Portmeta("NewPort","boolean","Input"),
                                                                                                   new Portmeta("NewPortWithSpace","boolean","Input"),
                                                                                                   new Portmeta("NewPortWithOtherComment","boolean","Input"),
                                                                                               }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_typemeta_for_enum_types()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("EnumTest"),
                _ => Given_is_the_working_directory_PATH("EnumTest"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("EnumStruct", new[]
                                                                                                   {
                                                                                                       new TypeMember("EnumValue", "OtherEnum"),
                                                                                                   }),
                                                                             new EnumTypemetaStructure("OhMy", "int32",
                                                                                                       new []
                                                                                                       {
                                                                                                           new EnumSymbol("Hoho",0), 
                                                                                                           new EnumSymbol("Haha",1), 
                                                                                                           new EnumSymbol("Hihi",12), 
                                                                                                       }),
                                                                             new EnumTypemetaStructure("OtherEnum", "int32",
                                                                                                       new[]
                                                                                                       {
                                                                                                           new EnumSymbol("What",0),
                                                                                                           new EnumSymbol("That",1),
                                                                                                           new EnumSymbol("Not",12),
                                                                                                       }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_port_information_with_array_initializer()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithArrayInitializedPort"),
                _ => Given_is_the_working_directory_PATH("ProgramWithArrayInitializedPort"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_there_are_progmeta_files_with_the_following_content(new ProgmetaData("MyProgram",
                                                                                               new[] { "MyComponent", "MyProgram" },
                                                                                               new[]
                                                                                               {
                                                                                                   new Portmeta("intArray","int16","Input|Retain", 12),
                                                                                               }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_compmeta_information_on_generate_from_inside_project()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("TwoComponentsAndPrograms"),
                _ => Given_is_the_working_directory_PATH("TwoComponentsAndPrograms"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_there_are_compmeta_files_with_the_following_content(new CompmetaData("MyComponent",
                                                                                new string[] { "MyComponent" },
                                                                                new[]
                                                                                {
                                                                                    new Portmeta("examplePort1","Example","Input|Opc"),
                                                                                    new Portmeta("examplePort2","Example","Output", 1),
                                                                                    new Portmeta("examplePort3","Example","Output", 5)
                                                                                },
                                                                                new[] { "MyProgram", "MyProgram2"}
                                                                                ), 
                                                                              new CompmetaData("MyOtherComponent",
                                                                                new string[] { "MyOtherComponent" },
                                                                                new[] { new Portmeta("examplePort4","int32","", 0) },
                                                                                new[] { "MyOtherProgram"}
                                                                                ))).RunAsync();
        }

        [Scenario]
        public async Task Generate_compmeta_information_for_ambigous_component()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AlmostAmbiguous"),
                _ => Given_is_the_working_directory_PATH("AlmostAmbiguous"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_there_are_compmeta_files_with_the_following_content(new CompmetaData("AlmostAmbiguousComponent",
                                                                                new string[] { "AlmostAmbiguousComponent" },
                                                                                new Portmeta[0], 
                                                                                new[] { "AlmostAmbiguousProgram" }
                                                                                ),
                                                                              new CompmetaData("AlmostAmbiguousComp",
                                                                                new string[] { "AlmostAmbiguousComp" },
                                                                                new Portmeta[0],
                                                                                new[] { "OtherProgram" }
                                                                                ))).RunAsync();
        }

        [Scenario]
        public async Task Generate_libmeta_on_generate()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_libmeta_file_is_generated_with_the_components("MyComponent")).RunAsync();
        }

        [Scenario]
        public async Task Generate_acfconfig_on_generate_for_appproject()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AppProject"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_acfconfig_file_is_generated_with_the_components("AppProject","AppProjectComponent")
                ).RunAsync();
        }

        [Scenario]
        public async Task Generate_typemeta_information_in_method()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Arp.Plc.Esm"),
                _ => Given_is_the_working_directory_PATH("Arp.Plc.Esm"),
                _ => When_I_generate_all_codefiles_with_the_source_directories("src/src"),
                _ => Then_the_typemeta_method_looks_like_NAME("EsmLibrary.meta.cpp")).RunAsync();
        }

        [Scenario]
        public async Task Generate_library_meta_information_for_enums()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("EnumTest"),
                _ => When_I_generate_all_codefiles(),
                _ => Then_the_typemeta_method_looks_like_NAME("EnumTestLibrary.meta.cpp")).RunAsync();
        }

        [Scenario]
        public async Task Generate_correct_meta_data_for_static_string()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Arp.Plc.Esm"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("TaskInfo",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("TaskInterval","int64"),
                                                                                                       new TypeMember("TaskPriority","int16"),
                                                                                                       new TypeMember("TaskWatchdogTime","int64"),
                                                                                                       new TypeMember("LastExecutionTime","int64"),
                                                                                                       new TypeMember("MaxExecutionTime","int64"),
                                                                                                       new TypeMember("LastTaskActivationDelay","int64"),
                                                                                                       new TypeMember("MaxTaskActivationDelay","int64"),
                                                                                                       new TypeMember("ExecutionTimeThreshold","int64"),
                                                                                                       new TypeMember("ExecutionTimeThresholdCount","uint32"),
                                                                                                       new TypeMember("TaskName","StaticString[80]",2),
                                                                                                   }))).RunAsync();
        }

        [Scenario]
        public async Task Generate_compmeta_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ComponentInSourceFolder"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_there_are_compmeta_files_with_the_following_content(new CompmetaData("Component",
                                                                                new [] { "Component" },
                                                                                new Portmeta[] { },
                                                                                new string[] { }
                                                                                ))).RunAsync();
        }

        [Scenario]
        public async Task Generate_compmeta_information_on_generate_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("TwoComponentsAndPrograms"),
                _ => Given_is_the_working_directory_PATH("TwoComponentsAndPrograms"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_there_are_compmeta_files_with_the_following_content(new CompmetaData("MyComponent",
                                                                                new string[] { "MyComponent" },
                                                                                new[]
                                                                                {
                                                                                    new Portmeta("examplePort1","Example","Input|Opc"),
                                                                                    new Portmeta("examplePort2","Example","Output", 1),
                                                                                    new Portmeta("examplePort3","Example","Output", 5)
                                                                                },
                                                                                new[] { "MyProgram", "MyProgram2" }
                                                                                ),
                                                                              new CompmetaData("MyOtherComponent",
                                                                                new string[] { "MyOtherComponent" },
                                                                                new[] { new Portmeta("examplePort4", "int32", "", 0) },
                                                                                new[] { "MyOtherProgram" }
                                                                                ))).RunAsync();
        }

        [Scenario]
        public async Task Generate_libmeta_on_generate_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_generate_all_files_from_inside_the_project_folder(),
                _ => Then_the_libmeta_file_is_generated_with_the_components("MyComponent")).RunAsync();
        }

        [Scenario]
        public async Task Generate_library_files_on_generate_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_generate_all_codefiles_from_inside_the_project_folder(),
                _ => Then_the_library_files_are_generated()).RunAsync();
        }

        [Scenario]
        public async Task Generate_library_files_on_generate_with_multiple_namespaces()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("MultipleNamespaces"),
                _ => Given_is_the_working_directory_PATH("MultipleNamespaces"),
                _ => When_I_generate_all_codefiles_with_the_source_directories("src","extern"),
                _ => Then_the_library_files_are_generated()).RunAsync();
        }
        [Scenario]
        public async Task Generate_struct_typemeta_information_for_other_namespace_structs()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("MultipleNamespaces"),
                _ => When_I_generate_all_metafiles_with_the_source_directories("src", "extern"),
                _ => Then_the_typemeta_file_contains_the_following_structure(new StructTypemetaStructure("CircuitControlOutputs",
                                                                                                   new[]
                                                                                                   {
                                                                                                       new TypeMember("CondTemp","float64"),
                                                                                                       new TypeMember("ConLiquidSubCooling","float64"),
                                                                                                       new TypeMember("CircuitState","uint16"),
                                                                                                       new TypeMember("AlarmWord","uint64"),
                                                                                                       new TypeMember("WarningWord","uint64"),
                                                                                                       new TypeMember("EvaShValvePosition","float64"),
                                                                                                       new TypeMember("EcoShValvePosition","float64"),
                                                                                                       new TypeMember("HGBValvePosition","float64"),
                                                                                                       new TypeMember("CPValvePosition","float64"),
                                                                                                   }))).RunAsync();
        }

        [Scenario]
        public async Task Error_when_entites_are_in_multiple_namespaces()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ComponentsInMultipleNamespaces"),
                _ => Given_is_the_working_directory_PATH("ComponentsInMultipleNamespaces"),
                _ => When_I_generate_all_codefiles_with_the_source_directories("src", "extern"),
                _ => Then_the_user_was_informed_that_there_are_multiple_root_namespaces()).RunAsync();
        }

        [Scenario]
        public async Task Generate_library_files_even_if_there_are_no_entities()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("NoEntities"),
                _ => Given_is_the_working_directory_PATH("NoEntities"),
                _ => When_I_generate_all_codefiles_from_inside_the_project_folder(),
                _ => Then_the_library_files_are_generated()).RunAsync();
        }

        [Scenario]
        public async Task Generate_component_provider_files_on_generate_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("TwoComponentsAndPrograms"),
                _ => Given_is_the_working_directory_PATH("TwoComponentsAndPrograms"),
                _ => When_I_generate_all_codefiles_from_inside_the_project_folder(),
                _ => Then_the_provider_files_are_generated_for_component("MyComponent", new[] { "MyProgram", "MyProgram2" }),
                _ => Then_the_provider_files_are_generated_for_component("MyOtherComponent", new[] { "MyOtherProgram" })).RunAsync();
        }

    }

    public class ErrorDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>
        {
            new object[] {new ErrorInformation("MyProgramImpl.hpp", 12, 5, "CPP0001"), "CodeFilesWithError" },
            new object[] {new ErrorInformation("MyProgramImpl.hpp", 17, 1, "CPP0002"), "CodeFilesWithError" },
            new object[] {new ErrorInformation("MyProgram1Impl.hpp", 53, 1, "CPP0005"), "CodeFilesWithError" },
            new object[] {new ErrorInformation("MyProgramImpl.hpp", 32, 19, "ARP0001"), "ProgramWithWrongPortAttributes" },
            new object[] {new ErrorInformation("MyProgramImpl.hpp", 32, 19, "ARP0001"), "ProgramWithWrongPortAttributes" },
            new object[] {new ErrorInformation("MyProgramImpl.hpp", 35, 19, "ARP0001"), "ProgramWithWrongPortAttributes" },
            new object[] {new ErrorInformation("MyComponentImpl.hpp", 15, 19, "ARP0001"), "ProgramWithWrongPortAttributes" },
            new object[] {new ErrorInformation("MyComponentImpl.hpp", 52, 19, "ARP0001"), "ProgramWithWrongPortAttributes" },
            new object[] {new ErrorInformation("PortNameErrorProgram.hpp", 35, 16, "ARP0001"), "PortNameError" },
        };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
