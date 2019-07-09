#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace TwoComponentsAndPrograms { namespace MyComponent {

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

protected: // fields and ports

	//#port
	int32 examplePort5 = 5;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyProgram::MyProgram(const String& name) : ProgramBase(name)
{
}

}} // end of namespace TwoComponentsAndPrograms::MyComponent
