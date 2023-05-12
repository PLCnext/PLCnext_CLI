#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "Defect_4334Component.hpp"

namespace Defect_4334
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(Defect_4334::Defect_4334Component)
class Defect_4334Program : public ProgramBase, private Loggable<Defect_4334Program>
{
public: // typedefs

public: // construction/destruction
    Defect_4334Program(Defect_4334::Defect_4334Component& defect_4334ComponentArg, const String& name);
    Defect_4334Program(const Defect_4334Program& arg) = delete;
    virtual ~Defect_4334Program() = default;

public: // operators
    Defect_4334Program&  operator=(const Defect_4334Program& arg) = delete;

public: // properties

public: // operations
    //void    Execute() override;
    void Execute() override { /* this is used as a wrapper for a PLCnext independent implementation */ }

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
    //#attributes(Input|Retain)
    boolean portField;
    //#port
    //#attributes(Input|Retain)
    boolean portField1;
private: // fields
    Defect_4334::Defect_4334Component& defect_4334Component;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline Defect_4334Program::Defect_4334Program(Defect_4334::Defect_4334Component& defect_4334ComponentArg, const String& name)
: ProgramBase(name)
, defect_4334Component(defect_4334ComponentArg)
{
}

} // end of namespace Defect_4334
