#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "PortNameTooShortComponent.hpp"

namespace PortNameTooShort
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(PortNameTooShort::PortNameTooShortComponent)
class PortNameTooShortProgram : public ProgramBase, private Loggable<PortNameTooShortProgram>
{
public: // typedefs

public: // construction/destruction
    PortNameTooShortProgram(PortNameTooShort::PortNameTooShortComponent& portNameTooShortComponentArg, const String& name);
    PortNameTooShortProgram(const PortNameTooShortProgram& arg) = delete;
    virtual ~PortNameTooShortProgram() = default;

public: // operators
    PortNameTooShortProgram&  operator=(const PortNameTooShortProgram& arg) = delete;

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
		//#name(A)
		boolean port;

private: // fields
    PortNameTooShort::PortNameTooShortComponent& portNameTooShortComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline PortNameTooShortProgram::PortNameTooShortProgram(PortNameTooShort::PortNameTooShortComponent& portNameTooShortComponentArg, const String& name)
: ProgramBase(name)
, portNameTooShortComponent(portNameTooShortComponentArg)
{
}

} // end of namespace PortNameTooShort
