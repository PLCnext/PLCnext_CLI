#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "EnumTestProgram.hpp"
#include "PortTypes.hpp"
#include "EnumTestLibrary.hpp"

namespace EnumTest
{

using namespace Arp::Plc::Commons::Meta;

    void EnumTestLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // TypeDefinition: EnumTest::EnumTestProgram::EnumStruct
                    DataType::Struct, CTN<EnumTest::EnumTestProgram::EnumStruct>(), sizeof(::EnumTest::EnumTestProgram::EnumStruct), alignof(::EnumTest::EnumTestProgram::EnumStruct), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "EnumValue", offsetof(::EnumTest::EnumTestProgram::EnumStruct, EnumValue), DataType::Enum | DataType::Int32, CTN<EnumTest::EnumTestProgram::OtherEnum>(), sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: EnumTest::EnumTestProgram
                    DataType::Program, CTN<EnumTest::EnumTestProgram>(), sizeof(::EnumTest::EnumTestProgram), alignof(::EnumTest::EnumTestProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "OhMyPort", offsetof(::EnumTest::EnumTestProgram, port), DataType::Enum | DataType::Int32, CTN<EnumTest::PortTypes::OhMy>(), sizeof(EnumTest::PortTypes::OhMy), alignof(EnumTest::PortTypes::OhMy), {  }, StandardAttribute::Output },
                        { "OtherPort", offsetof(::EnumTest::EnumTestProgram, port2), DataType::Enum | DataType::Int32, CTN<EnumTest::EnumTestProgram::OtherEnum>(), sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {  }, StandardAttribute::Input },
                        { "StructPort", offsetof(::EnumTest::EnumTestProgram, port3), DataType::Struct, CTN<EnumTest::EnumTestProgram::EnumStruct>(), sizeof(EnumTest::EnumTestProgram::EnumStruct), alignof(EnumTest::EnumTestProgram::EnumStruct), {  }, StandardAttribute::Input },
                    }
                },
            }
            // End TypeDefinitions
        );
        {
            TypeDefinition typeDefinition{DataType::Enum | DataType::Int32, CTN<EnumTest::PortTypes::OhMy>(), sizeof(EnumTest::PortTypes::OhMy), alignof(EnumTest::PortTypes::OhMy), StandardAttribute::None, {}};
            {
                FieldDefinition field{"Hoho", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::PortTypes::OhMy), alignof(EnumTest::PortTypes::OhMy), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::PortTypes::OhMy>::type>(EnumTest::PortTypes::OhMy::Hoho));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"Haha", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::PortTypes::OhMy), alignof(EnumTest::PortTypes::OhMy), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::PortTypes::OhMy>::type>(EnumTest::PortTypes::OhMy::Haha));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"Hihi", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::PortTypes::OhMy), alignof(EnumTest::PortTypes::OhMy), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::PortTypes::OhMy>::type>(EnumTest::PortTypes::OhMy::Hihi));
                typeDefinition.AddField(std::move(field));
            }
            typeDomain.AddTypeDefinition(std::move(typeDefinition));
        }
        {
            TypeDefinition typeDefinition{DataType::Enum | DataType::Int32, CTN<EnumTest::EnumTestProgram::OtherEnum>(), sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), StandardAttribute::None, {}};
            {
                FieldDefinition field{"What", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::What));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"That", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::That));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"Not", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::Not));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"Other", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::Other));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"More", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::More));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"Random", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::Random));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"Next", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::Next));
                typeDefinition.AddField(std::move(field));
            }
            {
                FieldDefinition field{"Last", 0, DataType::Enum | DataType::Int32, String::Empty, sizeof(EnumTest::EnumTestProgram::OtherEnum), alignof(EnumTest::EnumTestProgram::OtherEnum), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", static_cast<std::underlying_type<EnumTest::EnumTestProgram::OtherEnum>::type>(EnumTest::EnumTestProgram::OtherEnum::Last));
                typeDefinition.AddField(std::move(field));
            }
            typeDomain.AddTypeDefinition(std::move(typeDefinition));
        }
    }

} // end of namespace EnumTest

