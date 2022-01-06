#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ProjectWithRedundancyAndWrongTargetComponent.hpp"

namespace ProjectWithRedundancyAndWrongTarget
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ProjectWithRedundancyAndWrongTarget::ProjectWithRedundancyAndWrongTargetComponent)
class ProjectWithRedundancyAndWrongTargetProgram : public ProgramBase, private Loggable<ProjectWithRedundancyAndWrongTargetProgram>
{
public: // typedefs

public: // construction/destruction
    ProjectWithRedundancyAndWrongTargetProgram(ProjectWithRedundancyAndWrongTarget::ProjectWithRedundancyAndWrongTargetComponent& projectWithRedundancyAndWrongTargetComponentArg, const String& name);
    ProjectWithRedundancyAndWrongTargetProgram(const ProjectWithRedundancyAndWrongTargetProgram& arg) = delete;
    virtual ~ProjectWithRedundancyAndWrongTargetProgram() = default;

public: // operators
    ProjectWithRedundancyAndWrongTargetProgram&  operator=(const ProjectWithRedundancyAndWrongTargetProgram& arg) = delete;

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

        //#port
        //#attributes(Input|Redundant)
        boolean portField;

private: // fields
    ProjectWithRedundancyAndWrongTarget::ProjectWithRedundancyAndWrongTargetComponent& projectWithRedundancyAndWrongTargetComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ProjectWithRedundancyAndWrongTargetProgram::ProjectWithRedundancyAndWrongTargetProgram(ProjectWithRedundancyAndWrongTarget::ProjectWithRedundancyAndWrongTargetComponent& projectWithRedundancyAndWrongTargetComponentArg, const String& name)
: ProgramBase(name)
, projectWithRedundancyAndWrongTargetComponent(projectWithRedundancyAndWrongTargetComponentArg)
{
}

} // end of namespace ProjectWithRedundancyAndWrongTarget
