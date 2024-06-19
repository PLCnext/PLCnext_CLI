#include "ProjectWithConfigFileComponentProgramProvider.hpp"
#include "ProjectWithConfigFileProgram.hpp"

namespace ProjectWithConfigFile
{

IProgram::Ptr ProjectWithConfigFileComponentProgramProvider::CreateProgramInternal(const String& programName, const String& programType)
{
    if (programType == "ProjectWithConfigFileProgram")
    { 
        return std::make_shared<::ProjectWithConfigFile::ProjectWithConfigFileProgram>(this->projectWithConfigFileComponent, programName);
    }

    // else unknown program
    return nullptr;
}

} // end of namespace ProjectWithConfigFile
