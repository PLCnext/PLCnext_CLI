#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ProjectWithRedundancyAttributeComponent.hpp"

namespace ProjectWithRedundancyAttribute
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeComponent)
class ProjectWithRedundancyAttributeProgram : public ProgramBase, private Loggable<ProjectWithRedundancyAttributeProgram>
{
public: // typedefs

public: // construction/destruction
    ProjectWithRedundancyAttributeProgram(ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeComponent& projectWithRedundancyAttributeComponentArg, const String& name);
    ProjectWithRedundancyAttributeProgram(const ProjectWithRedundancyAttributeProgram& arg) = delete;
    virtual ~ProjectWithRedundancyAttributeProgram() = default;

public: // operators
    ProjectWithRedundancyAttributeProgram&  operator=(const ProjectWithRedundancyAttributeProgram& arg) = delete;

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
    ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeComponent& projectWithRedundancyAttributeComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ProjectWithRedundancyAttributeProgram::ProjectWithRedundancyAttributeProgram(ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeComponent& projectWithRedundancyAttributeComponentArg, const String& name)
: ProgramBase(name)
, projectWithRedundancyAttributeComponent(projectWithRedundancyAttributeComponentArg)
{
}

} // end of namespace ProjectWithRedundancyAttribute
