#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "HiddenPlmComponent.hpp"

namespace HiddenPlm
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(HiddenPlm::HiddenPlmComponent)
class HiddenPlmProgram : public ProgramBase, private Loggable<HiddenPlmProgram>
{
public: // typedefs

public: // construction/destruction
    HiddenPlmProgram(HiddenPlm::HiddenPlmComponent& hiddenPlmComponentArg, const String& name);
    HiddenPlmProgram(const HiddenPlmProgram& arg) = delete;
    virtual ~HiddenPlmProgram() = default;

public: // operators
    HiddenPlmProgram&  operator=(const HiddenPlmProgram& arg) = delete;

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
    HiddenPlm::HiddenPlmComponent& hiddenPlmComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline HiddenPlmProgram::HiddenPlmProgram(HiddenPlm::HiddenPlmComponent& hiddenPlmComponentArg, const String& name)
: ProgramBase(name)
, hiddenPlmComponent(hiddenPlmComponentArg)
{
}

} // end of namespace HiddenPlm
