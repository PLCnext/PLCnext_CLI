#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ProgramWithSameProgramNamesComponent.hpp"

namespace ProgramWithSameProgramNames { namespace ns
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ProgramWithSameProgramNames::ProgramWithSameProgramNamesComponent)
class ProgramWithSameProgramNamesProgram : public ProgramBase, private Loggable<ProgramWithSameProgramNamesProgram>
{
public: // typedefs

public: // construction/destruction
    ProgramWithSameProgramNamesProgram(ProgramWithSameProgramNames::ProgramWithSameProgramNamesComponent& programWithSameProgramNamesComponentArg, const String& name);
    ProgramWithSameProgramNamesProgram(const ProgramWithSameProgramNamesProgram& arg) = delete;
    virtual ~ProgramWithSameProgramNamesProgram() = default;

public: // operators
    ProgramWithSameProgramNamesProgram&  operator=(const ProgramWithSameProgramNamesProgram& arg) = delete;

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
    ProgramWithSameProgramNames::ProgramWithSameProgramNamesComponent& programWithSameProgramNamesComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ProgramWithSameProgramNamesProgram::ProgramWithSameProgramNamesProgram(ProgramWithSameProgramNames::ProgramWithSameProgramNamesComponent& programWithSameProgramNamesComponentArg, const String& name)
: ProgramBase(name)
, programWithSameProgramNamesComponent(programWithSameProgramNamesComponentArg)
{
}

}} // end of namespace ProgramWithSameProgramNames.ns
