#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ns1 { namespace ns2 { namespace ProjectWithoutProj { namespace CustomNamespace {

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

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

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyProgram::MyProgram(const String& name) : ProgramBase(name)
{
}

}}}} // end of namespace ns1::ns2::ProjectWithoutProj::CustomNamespace
