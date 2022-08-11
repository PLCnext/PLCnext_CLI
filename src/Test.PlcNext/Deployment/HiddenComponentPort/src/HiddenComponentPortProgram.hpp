#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "HiddenComponentPortComponent.hpp"

namespace HiddenComponentPort
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(HiddenComponentPort::HiddenComponentPortComponent)
class HiddenComponentPortProgram : public ProgramBase, private Loggable<HiddenComponentPortProgram>
{
public: // typedefs

public: // construction/destruction
    HiddenComponentPortProgram(HiddenComponentPort::HiddenComponentPortComponent& hiddenComponentPortComponentArg, const String& name);
    HiddenComponentPortProgram(const HiddenComponentPortProgram& arg) = delete;
    virtual ~HiddenComponentPortProgram() = default;

public: // operators
    HiddenComponentPortProgram&  operator=(const HiddenComponentPortProgram& arg) = delete;

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
    HiddenComponentPort::HiddenComponentPortComponent& hiddenComponentPortComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline HiddenComponentPortProgram::HiddenComponentPortProgram(HiddenComponentPort::HiddenComponentPortComponent& hiddenComponentPortComponentArg, const String& name)
: ProgramBase(name)
, hiddenComponentPortComponent(hiddenComponentPortComponentArg)
{
}

} // end of namespace HiddenComponentPort
