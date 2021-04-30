#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "DynamicPortsProgram.hpp"
#include "DynamicPortsLibrary.hpp"

namespace DynamicPorts
{

using namespace Arp::Plc::Commons::Meta;

    void DynamicPortsLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // TypeDefinition: DynamicPorts::DynamicPortsProgram::instruct
                    DataType::Struct, CTN<DynamicPorts::DynamicPortsProgram::instruct>(), sizeof(::DynamicPorts::DynamicPortsProgram::instruct), alignof(::DynamicPorts::DynamicPortsProgram::instruct), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "blubber", offsetof(::DynamicPorts::DynamicPortsProgram::instruct, blubber), DataType::Struct, CTN<DynamicPorts::DynamicPortsProgram::nested>(), sizeof(DynamicPorts::DynamicPortsProgram::nested), alignof(DynamicPorts::DynamicPortsProgram::nested), {  }, StandardAttribute::None },
                        { "test", offsetof(::DynamicPorts::DynamicPortsProgram::instruct, test), DataType::Struct, CTN<DynamicPorts::DynamicPortsProgram::nested::supernested>(), sizeof(DynamicPorts::DynamicPortsProgram::nested::supernested), alignof(DynamicPorts::DynamicPortsProgram::nested::supernested), {  }, StandardAttribute::None },
                    }
                },
                {   // TypeDefinition: DynamicPorts::DynamicPortsProgram::nested
                    DataType::Struct, CTN<DynamicPorts::DynamicPortsProgram::nested>(), sizeof(::DynamicPorts::DynamicPortsProgram::nested), alignof(::DynamicPorts::DynamicPortsProgram::nested), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "fiooba", offsetof(::DynamicPorts::DynamicPortsProgram::nested, fiooba), DataType::Struct, CTN<DynamicPorts::DynamicPortsProgram::nested::supernested>(), sizeof(DynamicPorts::DynamicPortsProgram::nested::supernested), alignof(DynamicPorts::DynamicPortsProgram::nested::supernested), {  }, StandardAttribute::None },
                    }
                },
                {   // TypeDefinition: DynamicPorts::DynamicPortsProgram::nested::supernested
                    DataType::Struct, CTN<DynamicPorts::DynamicPortsProgram::nested::supernested>(), sizeof(::DynamicPorts::DynamicPortsProgram::nested::supernested), alignof(::DynamicPorts::DynamicPortsProgram::nested::supernested), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "schnacken", offsetof(::DynamicPorts::DynamicPortsProgram::nested::supernested, schnacken), DataType::Bit, String::Empty, sizeof(Bit), alignof(Bit), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: DynamicPorts::DynamicPortsProgram
                    DataType::Program, CTN<DynamicPorts::DynamicPortsProgram>(), sizeof(::DynamicPorts::DynamicPortsProgram), alignof(::DynamicPorts::DynamicPortsProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace DynamicPorts

