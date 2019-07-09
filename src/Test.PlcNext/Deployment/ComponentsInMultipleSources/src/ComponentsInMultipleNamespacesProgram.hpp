#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ComponentsInMultipleNamespacesComponent.hpp"

namespace ComponentsInMultipleNamespaces
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ComponentsInMultipleNamespaces::ComponentsInMultipleNamespacesComponent)
class ComponentsInMultipleNamespacesProgram : public ProgramBase, private Loggable<ComponentsInMultipleNamespacesProgram>
{
public: // typedefs

public: // construction/destruction
    ComponentsInMultipleNamespacesProgram(ComponentsInMultipleNamespaces::ComponentsInMultipleNamespacesComponent& componentsInMultipleNamespacesComponentArg, const String& name);
    ComponentsInMultipleNamespacesProgram(const ComponentsInMultipleNamespacesProgram& arg) = delete;
    virtual ~ComponentsInMultipleNamespacesProgram() = default;

public: // operators
    ComponentsInMultipleNamespacesProgram&  operator=(const ComponentsInMultipleNamespacesProgram& arg) = delete;

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
    ComponentsInMultipleNamespaces::ComponentsInMultipleNamespacesComponent& componentsInMultipleNamespacesComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ComponentsInMultipleNamespacesProgram::ComponentsInMultipleNamespacesProgram(ComponentsInMultipleNamespaces::ComponentsInMultipleNamespacesComponent& componentsInMultipleNamespacesComponentArg, const String& name)
: ProgramBase(name)
, componentsInMultipleNamespacesComponent(componentsInMultipleNamespacesComponentArg)
{
}

} // end of namespace ComponentsInMultipleNamespaces
