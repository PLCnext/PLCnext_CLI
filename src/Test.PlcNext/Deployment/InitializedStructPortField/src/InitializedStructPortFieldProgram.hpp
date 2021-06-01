#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "InitializedStructPortFieldComponent.hpp"

namespace InitializedStructPortField
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(InitializedStructPortField::InitializedStructPortFieldComponent)
class InitializedStructPortFieldProgram : public ProgramBase, private Loggable<InitializedStructPortFieldProgram>
{
public: // typedefs
	struct PortStruct
	{
		int32 Data;
	};

public: // construction/destruction
    InitializedStructPortFieldProgram(InitializedStructPortField::InitializedStructPortFieldComponent& initializedStructPortFieldComponentArg, const String& name);
    InitializedStructPortFieldProgram(const InitializedStructPortFieldProgram& arg) = delete;
    virtual ~InitializedStructPortFieldProgram() = default;

public: // operators
    InitializedStructPortFieldProgram&  operator=(const InitializedStructPortFieldProgram& arg) = delete;

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
		PortStruct PortName = PortStruct();

private: // fields
    InitializedStructPortField::InitializedStructPortFieldComponent& initializedStructPortFieldComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline InitializedStructPortFieldProgram::InitializedStructPortFieldProgram(InitializedStructPortField::InitializedStructPortFieldComponent& initializedStructPortFieldComponentArg, const String& name)
: ProgramBase(name)
, initializedStructPortFieldComponent(initializedStructPortFieldComponentArg)
{
}

} // end of namespace InitializedStructPortField
