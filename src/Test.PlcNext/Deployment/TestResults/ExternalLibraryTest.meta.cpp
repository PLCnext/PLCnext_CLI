#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "ExternalLibraryTestProgram.hpp"
#include "My/Company/Name/ExternalStruct.hpp"
#include "ExternalLibraryTestLibrary.hpp"

namespace ExternalLibraryTest
{

using namespace Arp::Plc::Commons::Meta;

    static const FieldDefinition My_Company_Name_ExternalStruct_Member1("Member1", offsetof(::My::Company::Name::ExternalStruct, Member1), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition My_Company_Name_ExternalStruct_Member2("Member2", offsetof(::My::Company::Name::ExternalStruct, Member2), DataType::Int16, "", sizeof(int16), alignof(int16), {  }, StandardAttribute::None);
    static const FieldDefinition ExternalLibraryTest_ExternalLibraryTestProgram_port("ExternalPort", offsetof(::ExternalLibraryTest::ExternalLibraryTestProgram, port), DataType::Struct, CTN<My::Company::Name::ExternalStruct>(), sizeof(My::Company::Name::ExternalStruct), alignof(My::Company::Name::ExternalStruct), {  }, StandardAttribute::Output);
    
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
                        My_Company_Name_ExternalStruct_Member1,
                        My_Company_Name_ExternalStruct_Member2,
                    }
                },
                {   // ProgramDefinition: ExternalLibraryTest::ExternalLibraryTestProgram
                    DataType::Program, CTN<ExternalLibraryTest::ExternalLibraryTestProgram>(), sizeof(::ExternalLibraryTest::ExternalLibraryTestProgram), alignof(::ExternalLibraryTest::ExternalLibraryTestProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        ExternalLibraryTest_ExternalLibraryTestProgram_port,
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace ExternalLibraryTest

