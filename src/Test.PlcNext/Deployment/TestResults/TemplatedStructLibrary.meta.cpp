#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "TemplatedStructProgram.hpp"
#include "TemplatedStructLibrary.hpp"

namespace TemplatedStruct
{

using namespace Arp::Plc::Commons::Meta;

    void TemplatedStructLibrary::InitializeTypeDomain()
    {
        using offsetof_TemplatedStruct_TemplatedStructProgram_Test_bool_int32_ = TemplatedStruct::TemplatedStructProgram::Test<bool, int32>;
        using offsetof_TemplatedStruct_TemplatedStructProgram_Test_int64_int16_ = TemplatedStruct::TemplatedStructProgram::Test<int64, int16>;
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // TypeDefinition: TemplatedStruct::TemplatedStructProgram::Test<bool, int32>
                    DataType::Struct, CTN<TemplatedStruct::TemplatedStructProgram::Test<bool, int32>>(), sizeof(::TemplatedStruct::TemplatedStructProgram::Test<bool, int32>), alignof(::TemplatedStruct::TemplatedStructProgram::Test<bool, int32>), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "flag", offsetof(offsetof_TemplatedStruct_TemplatedStructProgram_Test_bool_int32_, flag), DataType::Boolean, String::Empty, sizeof(bool), alignof(bool), {  }, StandardAttribute::None },
                        { "counter", offsetof(offsetof_TemplatedStruct_TemplatedStructProgram_Test_bool_int32_, counter), DataType::Int32, String::Empty, sizeof(int32), alignof(int32), {  }, StandardAttribute::None },
                    }
                },
                {   // TypeDefinition: TemplatedStruct::TemplatedStructProgram::Test<int64, int16>
                    DataType::Struct, CTN<TemplatedStruct::TemplatedStructProgram::Test<int64, int16>>(), sizeof(::TemplatedStruct::TemplatedStructProgram::Test<int64, int16>), alignof(::TemplatedStruct::TemplatedStructProgram::Test<int64, int16>), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "flag", offsetof(offsetof_TemplatedStruct_TemplatedStructProgram_Test_int64_int16_, flag), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "counter", offsetof(offsetof_TemplatedStruct_TemplatedStructProgram_Test_int64_int16_, counter), DataType::Int16, String::Empty, sizeof(int16), alignof(int16), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: TemplatedStruct::TemplatedStructProgram
                    DataType::Program, CTN<TemplatedStruct::TemplatedStructProgram>(), sizeof(::TemplatedStruct::TemplatedStructProgram), alignof(::TemplatedStruct::TemplatedStructProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "port1", offsetof(::TemplatedStruct::TemplatedStructProgram, port1), DataType::Struct, CTN<TemplatedStruct::TemplatedStructProgram::Test<bool, int32>>(), sizeof(TemplatedStruct::TemplatedStructProgram::Test<bool, int32>), alignof(TemplatedStruct::TemplatedStructProgram::Test<bool, int32>), {  }, StandardAttribute::Input },
                        { "NameOfPort", offsetof(::TemplatedStruct::TemplatedStructProgram, port2), DataType::Struct, CTN<TemplatedStruct::TemplatedStructProgram::Test<int64, int16>>(), sizeof(TemplatedStruct::TemplatedStructProgram::Test<int64, int16>), alignof(TemplatedStruct::TemplatedStructProgram::Test<int64, int16>), {  }, StandardAttribute::Output },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace TemplatedStruct

