#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace TwoComponentsAndPrograms { namespace MyOtherComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

//#program
//#component(MyOtherComponent)
class MyOtherProgram : public ProgramBase, private Loggable<MyOtherProgram>
{
public: // typedefs

public: // construction/destruction
    MyOtherProgram(const String& name);
    MyOtherProgram(const MyOtherProgram& arg) = delete;
    virtual ~MyOtherProgram(void) = default;

public: // operators
    MyOtherProgram&  operator=(const MyOtherProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

protected: // fields and ports

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyOtherProgram::MyOtherProgram(const String& name) : ProgramBase(name)
{
}

}} // end of namespace TwoComponentsAndPrograms::MyOtherComponent
