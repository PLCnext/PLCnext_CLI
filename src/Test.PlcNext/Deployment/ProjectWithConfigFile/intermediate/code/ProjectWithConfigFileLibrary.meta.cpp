#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "ProjectWithConfigFileProgram.hpp"
#include "ProjectWithConfigFileLibrary.hpp"

namespace ProjectWithConfigFile
{

using namespace Arp::Plc::Commons::Meta;

    void ProjectWithConfigFileLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // ProgramDefinition: ProjectWithConfigFile::ProjectWithConfigFileProgram
                    DataType::Program, CTN<ProjectWithConfigFile::ProjectWithConfigFileProgram>(), sizeof(::ProjectWithConfigFile::ProjectWithConfigFileProgram), alignof(::ProjectWithConfigFile::ProjectWithConfigFileProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace ProjectWithConfigFile

