#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "PpDirectivesComponent.hpp"
#include "Directives.hpp"
#define SUM_VALUE (BASE_VALUE + 2)
#define MULTIPLIER SUM_VALUE * 3
#define multiply( f1, f2 ) ( f1 * f2 )

namespace PpDirectives
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(PpDirectives::PpDirectivesComponent)
class PpDirectivesProgram : public ProgramBase, private Loggable<PpDirectivesProgram>
{
public: // typedefs
#define CLASS_VALUE 5

public: // construction/destruction
    PpDirectivesProgram(PpDirectives::PpDirectivesComponent& ppDirectivesComponentArg, const String& name);
    PpDirectivesProgram(const PpDirectivesProgram& arg) = delete;
    virtual ~PpDirectivesProgram() = default;

public: // operators
    PpDirectivesProgram&  operator=(const PpDirectivesProgram& arg) = delete;

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
		StaticString<CLASS_VALUE> ClassValue;
		//#port
		StaticString<MULTIPLIER> CalculatedValue;
		//#port
		StaticString<BASE_VALUE> BaseValue;
		//#port
		StaticString<COMPONENT_VALUE> ComponentValue;
		//#port
		StaticString<__BYTE_ORDER__> CompilerMacroValue;

private: // fields
    PpDirectives::PpDirectivesComponent& ppDirectivesComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline PpDirectivesProgram::PpDirectivesProgram(PpDirectives::PpDirectivesComponent& ppDirectivesComponentArg, const String& name)
: ProgramBase(name)
, ppDirectivesComponent(ppDirectivesComponentArg)
{
}

} // end of namespace PpDirectives
