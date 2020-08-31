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
                {   // TypeDefinition: My::Company::Name::ExternalStruct
                    DataType::Struct, CTN<My::Company::Name::ExternalStruct>(), sizeof(::My::Company::Name::ExternalStruct), alignof(::My::Company::Name::ExternalStruct), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "Member1", offsetof(::My::Company::Name::ExternalStruct, Member1), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "Member2", offsetof(::My::Company::Name::ExternalStruct, Member2), DataType::Int16, String::Empty, sizeof(int16), alignof(int16), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: ExternalLibraryTest::ExternalLibraryTestProgram
                    DataType::Program, CTN<ExternalLibraryTest::ExternalLibraryTestProgram>(), sizeof(::ExternalLibraryTest::ExternalLibraryTestProgram), alignof(::ExternalLibraryTest::ExternalLibraryTestProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "ExternalPort", offsetof(::ExternalLibraryTest::ExternalLibraryTestProgram, port), DataType::Struct, CTN<My::Company::Name::ExternalStruct>(), sizeof(My::Company::Name::ExternalStruct), alignof(My::Company::Name::ExternalStruct), {  }, StandardAttribute::Output },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace ExternalLibraryTest

