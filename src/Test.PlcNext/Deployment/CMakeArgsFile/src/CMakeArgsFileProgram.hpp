#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "CMakeArgsFileComponent.hpp"

namespace CMakeArgsFile
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(CMakeArgsFile::CMakeArgsFileComponent)
class CMakeArgsFileProgram : public ProgramBase, private Loggable<CMakeArgsFileProgram>
{
public: // typedefs

public: // construction/destruction
    CMakeArgsFileProgram(CMakeArgsFile::CMakeArgsFileComponent& cMakeArgsFileComponentArg, const String& name);
    CMakeArgsFileProgram(const CMakeArgsFileProgram& arg) = delete;
    virtual ~CMakeArgsFileProgram() = default;

public: // operators
    CMakeArgsFileProgram&  operator=(const CMakeArgsFileProgram& arg) = delete;

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

private: // fields
    CMakeArgsFile::CMakeArgsFileComponent& cMakeArgsFileComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline CMakeArgsFileProgram::CMakeArgsFileProgram(CMakeArgsFile::CMakeArgsFileComponent& cMakeArgsFileComponentArg, const String& name)
: ProgramBase(name)
, cMakeArgsFileComponent(cMakeArgsFileComponentArg)
{
}

} // end of namespace CMakeArgsFile
