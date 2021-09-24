#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "StaticStringTestComponent.hpp"

namespace StaticStringTest
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

#define MY_VALUE 42

struct MyStruct {
    public: bool something = true;
	StaticString<> someString;
	StaticWString<> someWString;
    //#iecdatatyPe(STRING)
	StaticString<23> someString23;
	StaticWString<109> someWString109;

    //#IECDataType(WORD)
    uint16 myData;

    uint16 myOtherData;

    StaticString<MY_VALUE> someStringFromDefine;
    StaticWString<MY_VALUE> someWStringFromDefine;
};

//#program
//#component(StaticStringTest::StaticStringTestComponent)
class StaticStringTestProgram : public ProgramBase, private Loggable<StaticStringTestProgram>
{
public: // typedefs

public: // construction/destruction
    StaticStringTestProgram(StaticStringTest::StaticStringTestComponent& staticStringTestComponentArg, const String& name);
    StaticStringTestProgram(const StaticStringTestProgram& arg) = delete;
    virtual ~StaticStringTestProgram() = default;

public: // operators
    StaticStringTestProgram&  operator=(const StaticStringTestProgram& arg) = delete;

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
        MyStruct portField;

        //#port
        //#attributes(Input)
        StaticString<> myDefaultStringPort;

        //#port
        //#attributes(Input)
        StaticWString<> myDefaultWStringPort;

        //#port
        //#attributes(Input)
        StaticString<10> myStringPort;

        //#port
        //#attributes(Input)
        StaticString<10> myStringPort2;

        //#port
        //#attributes(Input)
        StaticString<80> myStringPort3;

        //#port
        //#attributes(Input)
        StaticWString<100> myWStringPort;

        //#port
        //#attributes(Input)
        StaticWString<14> myWStringArrayPort[2];

        //#port
        StaticString<MY_VALUE> myStringFromDefine;
        
        //#port
        StaticWString<MY_VALUE> myWStringFromDefine;

private: // fields
    StaticStringTest::StaticStringTestComponent& staticStringTestComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline StaticStringTestProgram::StaticStringTestProgram(StaticStringTest::StaticStringTestComponent& staticStringTestComponentArg, const String& name)
: ProgramBase(name)
, staticStringTestComponent(staticStringTestComponentArg)
{
}

} // end of namespace StaticStringTest
