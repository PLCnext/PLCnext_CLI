#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "StaticStringTestProgram.hpp"
#include "StaticStringTestLibrary.hpp"

namespace StaticStringTest
{

using namespace Arp::Plc::Commons::Meta;

    void StaticStringTestLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // TypeDefinition: StaticStringTest::MyStruct
                    DataType::Struct, CTN<StaticStringTest::MyStruct>(), sizeof(::StaticStringTest::MyStruct), alignof(::StaticStringTest::MyStruct), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "something", offsetof(::StaticStringTest::MyStruct, something), DataType::Boolean, String::Empty, sizeof(bool), alignof(bool), {  }, StandardAttribute::None },
                        { "someString", offsetof(::StaticStringTest::MyStruct, someString), DataType::StaticString, String::Empty, sizeof(StaticString<>), alignof(StaticString<>), {  }, StandardAttribute::None },
                        { "someWString", offsetof(::StaticStringTest::MyStruct, someWString), DataType::StaticWString, String::Empty, sizeof(StaticWString<>), alignof(StaticWString<>), {  }, StandardAttribute::None },
                        { "someString23", offsetof(::StaticStringTest::MyStruct, someString23), DataType::StaticString, String::Empty, sizeof(StaticString<23>), alignof(StaticString<23>), {  }, StandardAttribute::None },
                        { "someWString109", offsetof(::StaticStringTest::MyStruct, someWString109), DataType::StaticWString, String::Empty, sizeof(StaticWString<109>), alignof(StaticWString<109>), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: StaticStringTest::StaticStringTestProgram
                    DataType::Program, CTN<StaticStringTest::StaticStringTestProgram>(), sizeof(::StaticStringTest::StaticStringTestProgram), alignof(::StaticStringTest::StaticStringTestProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "NameOfPort", offsetof(::StaticStringTest::StaticStringTestProgram, portField), DataType::Struct, CTN<StaticStringTest::MyStruct>(), sizeof(StaticStringTest::MyStruct), alignof(StaticStringTest::MyStruct), {  }, StandardAttribute::Input | StandardAttribute::Retain },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace StaticStringTest

