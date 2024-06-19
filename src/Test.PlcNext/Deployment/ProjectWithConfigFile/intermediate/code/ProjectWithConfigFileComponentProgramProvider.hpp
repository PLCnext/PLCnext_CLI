#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramProviderBase.hpp"

namespace ProjectWithConfigFile
{

using namespace Arp;
using namespace Arp::Plc::Commons::Esm;

//forwards
class ProjectWithConfigFileComponent;

class ProjectWithConfigFileComponentProgramProvider : public ProgramProviderBase
{

public:   // construction/destruction
    ProjectWithConfigFileComponentProgramProvider(ProjectWithConfigFileComponent& projectWithConfigFileComponentArg);
    virtual ~ProjectWithConfigFileComponentProgramProvider() = default;

public:   // IProgramProvider operations
    IProgram::Ptr CreateProgramInternal(const String& programName, const String& programType) override;

private:   // deleted methods
    ProjectWithConfigFileComponentProgramProvider(const ProjectWithConfigFileComponentProgramProvider& arg) = delete;
    ProjectWithConfigFileComponentProgramProvider& operator=(const ProjectWithConfigFileComponentProgramProvider& arg) = delete;

private: // fields
    ProjectWithConfigFileComponent& projectWithConfigFileComponent;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProjectWithConfigFileComponentProgramProvider

inline ProjectWithConfigFileComponentProgramProvider::ProjectWithConfigFileComponentProgramProvider(ProjectWithConfigFileComponent& projectWithConfigFileComponentArg)
    : projectWithConfigFileComponent(projectWithConfigFileComponentArg)
{
}

} // end of namespace ProjectWithConfigFile
