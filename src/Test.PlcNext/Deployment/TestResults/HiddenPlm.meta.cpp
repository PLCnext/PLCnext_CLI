#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "HiddenPlmProgram.hpp"
#include "HiddenPlmComponent.hpp"
#include "HiddenPlmLibrary.hpp"

namespace HiddenPlm
{

using namespace Arp::Plc::Commons::Meta;

    void HiddenPlmLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // TypeDefinition: HiddenPlm::HiddenPlmComponent::Example
                    DataType::Struct, CTN<HiddenPlm::HiddenPlmComponent::Example>(), sizeof(::HiddenPlm::HiddenPlmComponent::Example), alignof(::HiddenPlm::HiddenPlmComponent::Example), StandardAttribute::Hidden,
                    {
                        // FieldDefinitions:
                        { "value1", offsetof(::HiddenPlm::HiddenPlmComponent::Example, value1), DataType::Int32, String::Empty, sizeof(int32), alignof(int32), {  }, StandardAttribute::Input },
                        { "NamedPort", offsetof(::HiddenPlm::HiddenPlmComponent::Example, value2), DataType::Boolean, String::Empty, sizeof(bool), alignof(bool), {  }, StandardAttribute::Output | StandardAttribute::Opc },
                    }
                },
                {   // ProgramDefinition: HiddenPlm::HiddenPlmProgram
                    DataType::Program, CTN<HiddenPlm::HiddenPlmProgram>(), sizeof(::HiddenPlm::HiddenPlmProgram), alignof(::HiddenPlm::HiddenPlmProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace HiddenPlm

