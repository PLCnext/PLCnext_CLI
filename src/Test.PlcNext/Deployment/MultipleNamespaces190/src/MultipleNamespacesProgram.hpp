#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "MultipleNamespacesComponent.hpp"
#include "CircuitControl.hpp"

namespace MultipleNamespaces
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;
using namespace CircuitControl;

//#program
//#component(MultipleNamespaces::MultipleNamespacesComponent)
class MultipleNamespacesProgram : public ProgramBase, private Loggable<MultipleNamespacesProgram>
{
public: // typedefs

public: // construction/destruction
    MultipleNamespacesProgram(MultipleNamespaces::MultipleNamespacesComponent& multipleNamespacesComponentArg, const String& name);
    MultipleNamespacesProgram(const MultipleNamespacesProgram& arg) = delete;
    virtual ~MultipleNamespacesProgram() = default;

public: // operators
    MultipleNamespacesProgram&  operator=(const MultipleNamespacesProgram& arg) = delete;

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
        //#attributes(Output)
        CircuitControl::CircuitControlOutputs OutPort;

private: // fields
    MultipleNamespaces::MultipleNamespacesComponent& multipleNamespacesComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MultipleNamespacesProgram::MultipleNamespacesProgram(MultipleNamespaces::MultipleNamespacesComponent& multipleNamespacesComponentArg, const String& name)
: ProgramBase(name)
, multipleNamespacesComponent(multipleNamespacesComponentArg)
{
}

} // end of namespace MultipleNamespaces
