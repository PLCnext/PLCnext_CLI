#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "DynamicPortsComponent.hpp"

namespace DynamicPorts
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(DynamicPorts::DynamicPortsComponent)
class DynamicPortsProgram : public ProgramBase, private Loggable<DynamicPortsProgram>
{
public: // typedefs
	struct nested{
			struct supernested{
				Bit schnacken;
			};
			supernested fiooba;
		};

    //#typeinformation
		struct instruct{
			nested blubber;
			nested::supernested test;
		};

public: // construction/destruction
    DynamicPortsProgram(DynamicPorts::DynamicPortsComponent& dynamicPortsComponentArg, const String& name);
    DynamicPortsProgram(const DynamicPortsProgram& arg) = delete;
    virtual ~DynamicPortsProgram() = default;

public: // operators
    DynamicPortsProgram&  operator=(const DynamicPortsProgram& arg) = delete;

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

    instruct exampleInput;

private: // fields
    DynamicPorts::DynamicPortsComponent& dynamicPortsComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline DynamicPortsProgram::DynamicPortsProgram(DynamicPorts::DynamicPortsComponent& dynamicPortsComponentArg, const String& name)
: ProgramBase(name)
, dynamicPortsComponent(dynamicPortsComponentArg)
{
}

} // end of namespace DynamicPorts
