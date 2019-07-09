#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace EmptyWithLibmeta { namespace MyComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

struct Example {
	int32 value1;
	Bit value2;
	int64 value3[6], int64 value4[4];
};

//#program
//#component(MyComponent)
class MyProgramImpl : public ProgramBase, private Loggable<MyProgramImpl>
{
public: // typedefs

public: // construction/destruction
    MyProgramImpl(const String& name);
    MyProgramImpl(const MyProgramImpl& arg) = delete;
    virtual ~MyProgramImpl(void) = default;

public: // operators
    MyProgramImpl&  operator=(const MyProgramImpl& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

protected: // fields and ports
	//#port
	//#attributes(IN)
	Example exampleInput;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyProgramImpl::MyProgramImpl(const String& name) : ProgramBase(name)
{
}

}} // end of namespace EmptyWithLibmeta::MyComponent
