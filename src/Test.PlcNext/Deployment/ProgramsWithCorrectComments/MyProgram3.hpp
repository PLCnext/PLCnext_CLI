#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ns1 { namespace ns2 { namespace ProjectWithoutProj { namespace CustomNamespace {

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ns2::ProjectWithoutProj::MyComponent)
class MyProgram3 : public ProgramBase, private Loggable<MyProgram3>
{
public: // typedefs

public: // construction/destruction
    MyProgram3(const String& name);
    MyProgram3(const MyProgram3& arg) = delete;
    virtual ~MyProgram3(void) = default;

public: // operators
    MyProgram3&  operator=(const MyProgram3& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

protected: // fields and ports

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyProgram3::MyProgram3(const String& name) : ProgramBase(name)
{
}

}}}} // end of namespace ns1::ns2::ProjectWithoutProj::CustomNamespace
