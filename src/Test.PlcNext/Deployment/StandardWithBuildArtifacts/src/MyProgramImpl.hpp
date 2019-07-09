#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace StandardWithBuildArtifacts { namespace MyComponent {

using namespace Arp;
using namespace Arp::Plc::Esm;

//#program
//#component(MyComponentImpl)
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

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline MyProgramImpl::MyProgramImpl(const String& name) : ProgramBase(name)
{
}

}} // end of namespace StandardWithBuildArtifacts::MyComponent
