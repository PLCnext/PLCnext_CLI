#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "AlmostAmbiguousComponent.hpp"

namespace AlmostAmbiguous
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(AlmostAmbiguous::AlmostAmbiguousComponent)
class AlmostAmbiguousProgram : public ProgramBase, private Loggable<AlmostAmbiguousProgram>
{
public: // typedefs

public: // construction/destruction
    AlmostAmbiguousProgram(AlmostAmbiguous::AlmostAmbiguousComponent& almostAmbiguousComponentArg, const String& name);
    AlmostAmbiguousProgram(const AlmostAmbiguousProgram& arg) = delete;
    virtual ~AlmostAmbiguousProgram() = default;

public: // operators
    AlmostAmbiguousProgram&  operator=(const AlmostAmbiguousProgram& arg) = delete;

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
    AlmostAmbiguous::AlmostAmbiguousComponent& almostAmbiguousComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline AlmostAmbiguousProgram::AlmostAmbiguousProgram(AlmostAmbiguous::AlmostAmbiguousComponent& almostAmbiguousComponentArg, const String& name)
: ProgramBase(name)
, almostAmbiguousComponent(almostAmbiguousComponentArg)
{
}

} // end of namespace AlmostAmbiguous
