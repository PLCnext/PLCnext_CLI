#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ProgramWithArrayInitializedPort { namespace MyComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

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
	//#attributes(Input|Retain)
    int16 intArray[12] = {0,1,2,3,4,5,6,7,8,9,10,11};
    
    //#port
    //#attributes(retain)
    uint32 Var_ARRAY_UDINT1[3][2]={0, 0};
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase

}} // end of namespace ProgramWithOneStructPort::MyComponent
