#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ProgramWithNestedAndStructInStruct { namespace MyComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

//#program
//#component(MyComponent)
class MyProgramImpl : public ProgramBase, private Loggable<MyProgramImpl>
{
public: // typedefs
	struct nested{
		struct supernested{
			bit schnacken;
		};
		supernested fiooba;
	};

	struct instruct{
		nested blubber;
		nested::supernested test;
	};

public: // construction/destruction
    MyProgramImpl(const String& name);
    MyProgramImpl(const MyProgramImpl& arg) = delete;
    virtual ~MyProgramImpl(void) = default;

public: // operators
    MyProgramImpl&  operator=(const MyProgramImpl& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

private: // fields
	//#port
	//#attributes(Input)
	instruct exampleInput;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase

}} // end of namespace ProgramWithOneStructPort::MyComponent
