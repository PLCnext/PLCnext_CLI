#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "ProjectWithRedundancyAttributeProgram.hpp"
#include "ProjectWithRedundancyAttributeLibrary.hpp"

namespace ProjectWithRedundancyAttribute
{

using namespace Arp::Plc::Commons::Meta;

    void ProjectWithRedundancyAttributeLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // ProgramDefinition: ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeProgram
                    DataType::Program, CTN<ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeProgram>(), sizeof(::ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeProgram), alignof(::ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "portField", offsetof(::ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeProgram, portField), DataType::Boolean, String::Empty, sizeof(boolean), alignof(boolean), {  }, StandardAttribute::Input | StandardAttribute::Redundant },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace ProjectWithRedundancyAttribute

