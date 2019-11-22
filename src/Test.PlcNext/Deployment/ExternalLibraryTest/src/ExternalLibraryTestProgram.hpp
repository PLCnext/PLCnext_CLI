#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ExternalLibraryTestComponent.hpp"
#include "My/Company/Name/ExternalStruct.hpp"

namespace ExternalLibraryTest
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ExternalLibraryTest::ExternalLibraryTestComponent)
class ExternalLibraryTestProgram : public ProgramBase, private Loggable<ExternalLibraryTestProgram>
{
public: // typedefs

public: // construction/destruction
    ExternalLibraryTestProgram(ExternalLibraryTest::ExternalLibraryTestComponent& externalLibraryTestComponentArg, const String& name);
    ExternalLibraryTestProgram(const ExternalLibraryTestProgram& arg) = delete;
    virtual ~ExternalLibraryTestProgram() = default;

public: // operators
    ExternalLibraryTestProgram&  operator=(const ExternalLibraryTestProgram& arg) = delete;

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
        //#attributes(Output)
        //#name(ExternalPort)
        ExternalStruct port;

private: // fields
    ExternalLibraryTest::ExternalLibraryTestComponent& externalLibraryTestComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ExternalLibraryTestProgram::ExternalLibraryTestProgram(ExternalLibraryTest::ExternalLibraryTestComponent& externalLibraryTestComponentArg, const String& name)
: ProgramBase(name)
, externalLibraryTestComponent(externalLibraryTestComponentArg)
{
}

} // end of namespace ExternalLibraryTest
