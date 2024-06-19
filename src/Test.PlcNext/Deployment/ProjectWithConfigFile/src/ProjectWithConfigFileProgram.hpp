#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ProjectWithConfigFileComponent.hpp"

namespace ProjectWithConfigFile
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ProjectWithConfigFile::ProjectWithConfigFileComponent)
class ProjectWithConfigFileProgram : public ProgramBase, private Loggable<ProjectWithConfigFileProgram>
{
public: // typedefs

public: // construction/destruction
    ProjectWithConfigFileProgram(ProjectWithConfigFile::ProjectWithConfigFileComponent& projectWithConfigFileComponentArg, const String& name);
    ProjectWithConfigFileProgram(const ProjectWithConfigFileProgram& arg) = delete;
    virtual ~ProjectWithConfigFileProgram() = default;

public: // operators
    ProjectWithConfigFileProgram&  operator=(const ProjectWithConfigFileProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute() override;

public: /* Ports
           =====
           Ports are defined in the following way:
           //#port
           //#attributes(Input|Retain)
           //#name(NameOfPort)
           boolean portField;

           The attributes comment define the port attributes and is optional.
           The name comment defines the name of the port and is optional. Default is the name of the field.
        */

private: // fields
    ProjectWithConfigFile::ProjectWithConfigFileComponent& projectWithConfigFileComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ProjectWithConfigFileProgram::ProjectWithConfigFileProgram(ProjectWithConfigFile::ProjectWithConfigFileComponent& projectWithConfigFileComponentArg, const String& name)
: ProgramBase(name)
, projectWithConfigFileComponent(projectWithConfigFileComponentArg)
{
}

} // end of namespace ProjectWithConfigFile
