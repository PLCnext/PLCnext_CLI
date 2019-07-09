#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace CodeFilesWithError { namespace MyComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

struct Example {
	int32;
	Bit value2;
	int64 value3[6], int64 value4[4];
};

struct Example {
	Bit value2;
};

struct {
	Bit value2;
};

//#program
//#component(MyComponent)
class MyProgram1Impl : public ProgramBase, private Loggable<MyProgramImpl>
{
public: // typedefs

public: // construction/destruction
    MyProgram1Impl(const String& name);
    MyProgram1Impl(const MyProgramImpl& arg) = delete;
    virtual ~MyProgram1Impl(void) = default;

public: // operators
    MyProgram1Impl&  operator=(const MyProgram1Impl& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

protected: // fields and ports

}};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyProgram1Impl::MyProgram1Impl(const String& name) : ProgramBase(name)
{
}

}} // end of namespace CodeFilesWithError::MyComponent
