#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "MyComponent.hpp"

namespace PseudoAmbiguousComponents
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(PseudoAmbiguousComponents::MyComponent)
class PseudoAmbiguousComponentsProgram : public ProgramBase, private Loggable<PseudoAmbiguousComponentsProgram>
{
public: // typedefs

public: // construction/destruction
    PseudoAmbiguousComponentsProgram(PseudoAmbiguousComponents::MyComponent& myComponentArg, const String& name);
    PseudoAmbiguousComponentsProgram(const PseudoAmbiguousComponentsProgram& arg) = delete;
    virtual ~PseudoAmbiguousComponentsProgram() = default;

public: // operators
    PseudoAmbiguousComponentsProgram&  operator=(const PseudoAmbiguousComponentsProgram& arg) = delete;

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
    PseudoAmbiguousComponents::MyComponent& myComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline PseudoAmbiguousComponentsProgram::PseudoAmbiguousComponentsProgram(PseudoAmbiguousComponents::MyComponent& myComponentArg, const String& name)
: ProgramBase(name)
, myComponent(myComponentArg)
{
}

} // end of namespace PseudoAmbiguousComponents
