#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ProgramWithDifferentPorts { namespace MyComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

struct Example {
	int32 value1;
	Bit value2;
	int64 value3[6], value4[1];
};

//#program
//#component(MyComponent)
class MyProgram : public ProgramBase, private Loggable<MyProgram>
{
public: // typedefs

public: // construction/destruction
    MyProgram(const String& name);
    MyProgram(const MyProgram& arg) = delete;
    virtual ~MyProgram(void) = default;

public: // operators
    MyProgram&  operator=(const MyProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

private: // fields
	//#port
	//#name(InCpp<INT)
	//#attributes(Input|Opc)
	Example examplePort1;
	//#port
	//#attributes(Output)
	Example examplePort2[1], examplePort3[5];
	//#port
	int32 examplePort4 = 3;
	//#port
	//#iecdatatype(LWORD)
	uint64 examplePort5[3];
	//#port
	uint64 examplePort6[3];
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase

}} // end of namespace ProgramWithDifferentPorts::MyComponent
