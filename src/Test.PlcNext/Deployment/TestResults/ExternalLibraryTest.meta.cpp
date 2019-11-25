#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "ExternalLibraryTestProgram.hpp"
#include "My/Company/Name/ExternalStruct.hpp"
#include "ExternalLibraryTestLibrary.hpp"

namespace ExternalLibraryTest
{

using namespace Arp::Plc::Commons::Meta;

    void ExternalLibraryTestLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // TypeDefinition: ExternalLibraryTest::ExternalStruct
                    DataType::Struct, CTN<ExternalLibraryTest::ExternalStruct>(), sizeof(::ExternalLibraryTest::ExternalStruct), alignof(::ExternalLibraryTest::ExternalStruct), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "Member1", offsetof(::ExternalLibraryTest::ExternalStruct, Member1), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "Member2", offsetof(::ExternalLibraryTest::ExternalStruct, Member2), DataType::Int16, "", sizeof(int16), alignof(int16), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: ExternalLibraryTest::ExternalLibraryTestProgram
                    DataType::Program, CTN<ExternalLibraryTest::ExternalLibraryTestProgram>(), sizeof(::ExternalLibraryTest::ExternalLibraryTestProgram), alignof(::ExternalLibraryTest::ExternalLibraryTestProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "ExternalPort", offsetof(::ExternalLibraryTest::ExternalLibraryTestProgram, port), DataType::Struct, CTN<ExternalLibraryTest::ExternalStruct>(), sizeof(ExternalLibraryTest::ExternalStruct), alignof(ExternalLibraryTest::ExternalStruct), {  }, StandardAttribute::Output },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace ExternalLibraryTest

