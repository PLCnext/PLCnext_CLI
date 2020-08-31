#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "EnumTestProgram.hpp"
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
                        { "EnumValue", offsetof(::EnumTest::EnumTestProgram::EnumStruct, EnumValue), DataType::Enum | DataType::Int32, String::Empty, sizeof(int32), alignof(int32), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: EnumTest::EnumTestProgram
                    DataType::Program, CTN<EnumTest::EnumTestProgram>(), sizeof(::EnumTest::EnumTestProgram), alignof(::EnumTest::EnumTestProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "OhMyPort", offsetof(::EnumTest::EnumTestProgram, port), DataType::Enum | DataType::Int32, String::Empty, sizeof(int32), alignof(int32), {  }, StandardAttribute::Output },
                        { "OtherPort", offsetof(::EnumTest::EnumTestProgram, port2), DataType::Enum | DataType::Int32, String::Empty, sizeof(int32), alignof(int32), {  }, StandardAttribute::Input },
                        { "StructPort", offsetof(::EnumTest::EnumTestProgram, port3), DataType::Struct, CTN<EnumTest::EnumTestProgram::EnumStruct>(), sizeof(EnumTest::EnumTestProgram::EnumStruct), alignof(EnumTest::EnumTestProgram::EnumStruct), {  }, StandardAttribute::Input },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace EnumTest

