#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "Component1.hpp"

namespace RNS_ST_Update_Proj_Targets_2237
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(RNS_ST_Update_Proj_Targets_2237::Component1)
class CppProgram11 : public ProgramBase, private Loggable<CppProgram11>
{
public: // typedefs

public: // construction/destruction
    CppProgram11(RNS_ST_Update_Proj_Targets_2237::Component1& component1Arg, const String& name);
    CppProgram11(const CppProgram11& arg) = delete;
    virtual ~CppProgram11() = default;

public: // operators
    CppProgram11&  operator=(const CppProgram11& arg) = delete;

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
    RNS_ST_Update_Proj_Targets_2237::Component1& component1;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline CppProgram11::CppProgram11(RNS_ST_Update_Proj_Targets_2237::Component1& component1Arg, const String& name)
: ProgramBase(name)
, component1(component1Arg)
{
}

} // end of namespace RNS_ST_Update_Proj_Targets_2237
