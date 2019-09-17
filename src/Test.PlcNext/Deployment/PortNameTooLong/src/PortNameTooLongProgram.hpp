#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "PortNameTooLongComponent.hpp"

namespace PortNameTooLong
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(PortNameTooLong::PortNameTooLongComponent)
class PortNameTooLongProgram : public ProgramBase, private Loggable<PortNameTooLongProgram>
{
public: // typedefs

public: // construction/destruction
    PortNameTooLongProgram(PortNameTooLong::PortNameTooLongComponent& portNameTooLongComponentArg, const String& name);
    PortNameTooLongProgram(const PortNameTooLongProgram& arg) = delete;
    virtual ~PortNameTooLongProgram() = default;

public: // operators
    PortNameTooLongProgram&  operator=(const PortNameTooLongProgram& arg) = delete;

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
		//#name(NameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPortNameOfPort)
		boolean port;

private: // fields
    PortNameTooLong::PortNameTooLongComponent& portNameTooLongComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline PortNameTooLongProgram::PortNameTooLongProgram(PortNameTooLong::PortNameTooLongComponent& portNameTooLongComponentArg, const String& name)
: ProgramBase(name)
, portNameTooLongComponent(portNameTooLongComponentArg)
{
}

} // end of namespace PortNameTooLong
