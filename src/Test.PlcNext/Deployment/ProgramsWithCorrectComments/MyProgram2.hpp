#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ns1 { namespace ns2 { namespace ProjectWithoutProj { namespace CustomNamespace {

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ns1::ns2::ProjectWithoutProj::MyComponent)
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

}}}} // end of namespace ns1::ns2::ProjectWithoutProj::CustomNamespace
