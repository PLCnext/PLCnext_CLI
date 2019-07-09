#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ns1 { namespace ns2 { namespace ProjectWithoutProj {

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ns2::WrongNamespace::MyComponent)
class OtherProgram : public ProgramBase, private Loggable<OtherProgram>
{
public: // typedefs

public: // construction/destruction
    OtherProgram(const String& name);
    OtherProgram(const OtherProgram& arg) = delete;
    virtual ~OtherProgram(void) = default;

public: // operators
    OtherProgram&  operator=(const OtherProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

protected: // fields and ports

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline OtherProgram::OtherProgram(const String& name) : ProgramBase(name)
{
}

}}} // end of namespace ns1::ns2::ProjectWithoutProj
