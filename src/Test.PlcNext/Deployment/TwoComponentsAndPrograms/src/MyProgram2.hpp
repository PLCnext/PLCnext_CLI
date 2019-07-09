#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace TwoComponentsAndPrograms { namespace MyComponent { 

using namespace Arp;
using namespace Arp::Plc::Esm;

//#program
//#component(MyComponent)
class MyProgram2 : public ProgramBase, private Loggable<MyProgram2>
{
public: // typedefs

public: // construction/destruction
    MyProgram2(const String& name);
    MyProgram2(const MyProgram2& arg) = delete;
    virtual ~MyProgram2(void) = default;

public: // operators
    MyProgram2&  operator=(const MyProgram2& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

protected: // fields and ports

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyProgram2::MyProgram2(const String& name) : ProgramBase(name)
{
}

}} // end of namespace TwoComponentsAndPrograms::MyComponent
