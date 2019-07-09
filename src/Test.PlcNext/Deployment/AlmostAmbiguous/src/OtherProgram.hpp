#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "AlmostAmbiguousComp.hpp"

namespace AlmostAmbiguous
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(AlmostAmbiguous::AlmostAmbiguousComp)
class OtherProgram : public ProgramBase, private Loggable<OtherProgram>
{
public: // typedefs

public: // construction/destruction
    OtherProgram(AlmostAmbiguous::AlmostAmbiguousComp& almostAmbiguousCompArg, const String& name);
    OtherProgram(const OtherProgram& arg) = delete;
    virtual ~OtherProgram() = default;

public: // operators
    OtherProgram&  operator=(const OtherProgram& arg) = delete;

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
    AlmostAmbiguous::AlmostAmbiguousComp& almostAmbiguousComp;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline OtherProgram::OtherProgram(AlmostAmbiguous::AlmostAmbiguousComp& almostAmbiguousCompArg, const String& name)
: ProgramBase(name)
, almostAmbiguousComp(almostAmbiguousCompArg)
{
}

} // end of namespace AlmostAmbiguous
