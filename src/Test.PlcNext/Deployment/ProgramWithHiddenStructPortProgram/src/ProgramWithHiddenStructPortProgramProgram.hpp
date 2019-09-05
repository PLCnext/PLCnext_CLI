#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ProgramWithHiddenStructPortProgramComponent.hpp"

namespace ProgramWithHiddenStructPortProgram
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ProgramWithHiddenStructPortProgram::ProgramWithHiddenStructPortProgramComponent)
class ProgramWithHiddenStructPortProgramProgram : public ProgramBase, private Loggable<ProgramWithHiddenStructPortProgramProgram>
{
public: // typedefs
	//#attributes(Hidden)
	struct Example {
		//#attributes(Input)
		int32 value1;
		//#attributes(Output|Opc)
        //#name(NamedPort)
		bool value2;
	};

public: // construction/destruction
    ProgramWithHiddenStructPortProgramProgram(ProgramWithHiddenStructPortProgram::ProgramWithHiddenStructPortProgramComponent& programWithHiddenStructPortProgramComponentArg, const String& name);
    ProgramWithHiddenStructPortProgramProgram(const ProgramWithHiddenStructPortProgramProgram& arg) = delete;
    virtual ~ProgramWithHiddenStructPortProgramProgram() = default;

public: // operators
    ProgramWithHiddenStructPortProgramProgram&  operator=(const ProgramWithHiddenStructPortProgramProgram& arg) = delete;

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
		Example ports;

private: // fields
    ProgramWithHiddenStructPortProgram::ProgramWithHiddenStructPortProgramComponent& programWithHiddenStructPortProgramComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ProgramWithHiddenStructPortProgramProgram::ProgramWithHiddenStructPortProgramProgram(ProgramWithHiddenStructPortProgram::ProgramWithHiddenStructPortProgramComponent& programWithHiddenStructPortProgramComponentArg, const String& name)
: ProgramBase(name)
, programWithHiddenStructPortProgramComponent(programWithHiddenStructPortProgramComponentArg)
{
}

} // end of namespace ProgramWithHiddenStructPortProgram
