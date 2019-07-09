#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ProgramWithOneAttributedStructPort { namespace MyComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

struct Example {
	//#attributes(Input|Opc)
	int32 value1;
	Bit value2;
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
	//#attributes(Input)
	Example exampleInput;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase

}} // end of namespace ProgramWithOneStructPort::MyComponent
