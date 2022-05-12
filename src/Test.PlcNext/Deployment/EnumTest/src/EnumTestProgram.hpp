#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "EnumTestComponent.hpp"
#include "PortTypes.hpp"

namespace EnumTest
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(EnumTest::EnumTestComponent)
class EnumTestProgram : public ProgramBase, private Loggable<EnumTestProgram>
{
public: // typedefs
    enum OtherEnum : int32{
        What = 0,
        That = 1,
        Not = 12,
        Other = (OtherEnum::That + 5) * 2,
        More = Not | Other,
        Random,
        Next = 3 * (20 - (What | Not))
    };

    struct EnumStruct
    {
    public:
        OtherEnum EnumValue;
    };

public: // construction/destruction
    EnumTestProgram(EnumTest::EnumTestComponent& enumTestComponentArg, const String& name);
    EnumTestProgram(const EnumTestProgram& arg) = delete;
    virtual ~EnumTestProgram() = default;

public: // operators
    EnumTestProgram&  operator=(const EnumTestProgram& arg) = delete;

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
        //#attributes(Output)
        //#name(OhMyPort)
        PortTypes::OhMy port;
        
        //#port
        //#attributes(Input)
        //#name(OtherPort)
        OtherEnum port2;


        //#port
        //#attributes(Input)
        //#name(StructPort)
        EnumStruct port3;

private: // fields
    EnumTest::EnumTestComponent& enumTestComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline EnumTestProgram::EnumTestProgram(EnumTest::EnumTestComponent& enumTestComponentArg, const String& name)
: ProgramBase(name)
, enumTestComponent(enumTestComponentArg)
{
}

} // end of namespace EnumTest
