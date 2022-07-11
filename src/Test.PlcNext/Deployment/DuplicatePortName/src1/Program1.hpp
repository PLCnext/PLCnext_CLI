#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "DuplicatePortNameComponent.hpp"

namespace DuplicatePortName
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(DuplicatePortName::DuplicatePortNameComponent)
class Program1 : public ProgramBase, private Loggable<Program1>
{
public: // typedefs

public: // construction/destruction
    Program1(DuplicatePortName::DuplicatePortNameComponent& duplicatePortNameComponentArg, const String& name);
    Program1(const Program1& arg) = delete;
    virtual ~Program1() = default;

public: // operators
    Program1&  operator=(const Program1& arg) = delete;

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
        //#attributes(Input|Retain)
        //#name(NameOfPort)
        StaticString<> portField1;

        //#port
        //#attributes(Input|Retain)
        //#name(NameOfPort)
        boolean portField2;

private: // fields
    DuplicatePortName::DuplicatePortNameComponent& duplicatePortNameComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline Program1::Program1(DuplicatePortName::DuplicatePortNameComponent& duplicatePortNameComponentArg, const String& name)
: ProgramBase(name)
, duplicatePortNameComponent(duplicatePortNameComponentArg)
{
}

} // end of namespace DuplicatePortName
