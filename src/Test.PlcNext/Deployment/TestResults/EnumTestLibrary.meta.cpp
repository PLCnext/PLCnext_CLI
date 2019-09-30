#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "EnumTestProgram.hpp"
#include "EnumTestLibrary.hpp"

namespace EnumTest
{

using namespace Arp::Plc::Commons::Meta;

    static const FieldDefinition EnumTest_EnumTestProgram_EnumStruct_EnumValue("EnumValue", offsetof(::EnumTest::EnumTestProgram::EnumStruct, EnumValue), DataType::Enum | DataType::Int32, "", sizeof(int32), alignof(int32), {  }, StandardAttribute::None);
    static const FieldDefinition EnumTest_EnumTestProgram_port("OhMyPort", offsetof(::EnumTest::EnumTestProgram, port), DataType::Enum | DataType::Int32, "", sizeof(int32), alignof(int32), {  }, StandardAttribute::Output);
    static const FieldDefinition EnumTest_EnumTestProgram_port2("OtherPort", offsetof(::EnumTest::EnumTestProgram, port2), DataType::Enum | DataType::Int32, "", sizeof(int32), alignof(int32), {  }, StandardAttribute::Input);
    static const FieldDefinition EnumTest_EnumTestProgram_port3("StructPort", offsetof(::EnumTest::EnumTestProgram, port3), DataType::Struct, CTN<EnumTest::EnumTestProgram::EnumStruct>(), sizeof(EnumTest::EnumTestProgram::EnumStruct), alignof(EnumTest::EnumTestProgram::EnumStruct), {  }, StandardAttribute::Input);
    
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
                        EnumTest_EnumTestProgram_EnumStruct_EnumValue,
                    }
                },
                {   // ProgramDefinition: EnumTest::EnumTestProgram
                    DataType::Program, CTN<EnumTest::EnumTestProgram>(), sizeof(::EnumTest::EnumTestProgram), alignof(::EnumTest::EnumTestProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        EnumTest_EnumTestProgram_port,
                        EnumTest_EnumTestProgram_port2,
                        EnumTest_EnumTestProgram_port3,
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace EnumTest

