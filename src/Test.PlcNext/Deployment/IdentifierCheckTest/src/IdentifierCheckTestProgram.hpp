#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "IdentifierCheckTestComponent.hpp"

namespace IdentifierCheckTest
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(IdentifierCheckTest::IdentifierCheckTestComponent)
class IdentifierCheckTestProgram : public ProgramBase, private Loggable<IdentifierCheckTestProgram>
{
public: // typedefs
    enum OtherEnum : int32 {
        What = 0,
        That = 1,
        Nothing = 12,
        Other = (OtherEnum::That + 5) * 2,  ///< some comment.
        More = Nothing | Other,
        Random,                             ///< my comment
        Next = 3 * (20 - (What | Nothing)),
        Last = 512                          ///< other comment.
    };

    struct TypeA {
        Arp::int16 program;
        Arp::int16 value2[10];
    };

    struct Ports {
        TypeA myA;
    }

public: // construction/destruction
    IdentifierCheckTestProgram(IdentifierCheckTest::IdentifierCheckTestComponent& identifierCheckTestComponentArg, const String& name);
#if ARP_ABI_VERSION_MAJOR < 2
    IdentifierCheckTestProgram(const IdentifierCheckTestProgram& arg) = delete;
    virtual ~IdentifierCheckTestProgram() = default;
#endif

public: // operators
#if ARP_ABI_VERSION_MAJOR < 2
    IdentifierCheckTestProgram&  operator=(const IdentifierCheckTestProgram& arg) = delete;
#endif

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
        Ports myPorts;

        //#port
        //#attributes(Input)
        //#name(OtherPort)
        OtherEnum port2;

private: // fields
    IdentifierCheckTest::IdentifierCheckTestComponent& identifierCheckTestComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline IdentifierCheckTestProgram::IdentifierCheckTestProgram(IdentifierCheckTest::IdentifierCheckTestComponent& identifierCheckTestComponentArg, const String& name)
: ProgramBase(name)
, identifierCheckTestComponent(identifierCheckTestComponentArg)
{
}

} // end of namespace IdentifierCheckTest
