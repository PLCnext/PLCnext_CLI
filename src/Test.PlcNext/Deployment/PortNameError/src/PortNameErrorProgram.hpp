#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "PortNameErrorComponent.hpp"

namespace PortNameError
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(PortNameError::PortNameErrorComponent)
class PortNameErrorProgram : public ProgramBase, private Loggable<PortNameErrorProgram>
{
public: // typedefs

public: // construction/destruction
    PortNameErrorProgram(PortNameError::PortNameErrorComponent& portNameErrorComponentArg, const String& name);
    PortNameErrorProgram(const PortNameErrorProgram& arg) = delete;
    virtual ~PortNameErrorProgram() = default;

public: // operators
    PortNameErrorProgram&  operator=(const PortNameErrorProgram& arg) = delete;

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
        //#name(bad.character)
        boolean port;

private: // fields
    PortNameError::PortNameErrorComponent& portNameErrorComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline PortNameErrorProgram::PortNameErrorProgram(PortNameError::PortNameErrorComponent& portNameErrorComponentArg, const String& name)
: ProgramBase(name)
, portNameErrorComponent(portNameErrorComponentArg)
{
}

} // end of namespace PortNameError
