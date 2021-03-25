#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "CommentedPortComponent.hpp"

namespace CommentedPort
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(CommentedPort::CommentedPortComponent)
class CommentedPortProgram : public ProgramBase, private Loggable<CommentedPortProgram>
{
public: // typedefs

public: // construction/destruction
    CommentedPortProgram(CommentedPort::CommentedPortComponent& commentedPortComponentArg, const String& name);
    CommentedPortProgram(const CommentedPortProgram& arg) = delete;
    virtual ~CommentedPortProgram() = default;

public: // operators
    CommentedPortProgram&  operator=(const CommentedPortProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute() override;

public: /* Ports
           =====
           Ports are defined in the following way:
           //#port
           //#attributes(Input|Retain)
           //#name(NameOfPort)
           boolean portField;

           The attributes comment define the port attributes and is optional.
           The name comment defines the name of the port and is optional. Default is the name of the field.
        */
		//#port
        //#attributes(Input)
        //int16 InPort;
        //#port
        //#attributes(Output)
        //#name(OutportNameinEngineer)
        //int16 OutPort;

private: // fields
    CommentedPort::CommentedPortComponent& commentedPortComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline CommentedPortProgram::CommentedPortProgram(CommentedPort::CommentedPortComponent& commentedPortComponentArg, const String& name)
: ProgramBase(name)
, commentedPortComponent(commentedPortComponentArg)
{
}

} // end of namespace CommentedPort
