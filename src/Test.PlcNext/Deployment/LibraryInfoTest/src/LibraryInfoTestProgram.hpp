#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "LibraryInfoTestComponent.hpp"

namespace LibraryInfoTest
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(LibraryInfoTest::LibraryInfoTestComponent)
class LibraryInfoTestProgram : public ProgramBase, private Loggable<LibraryInfoTestProgram>
{
public: // typedefs

public: // construction/destruction
    LibraryInfoTestProgram(LibraryInfoTest::LibraryInfoTestComponent& libraryInfoTestComponentArg, const String& name);
#if ARP_ABI_VERSION_MAJOR < 2
    LibraryInfoTestProgram(const LibraryInfoTestProgram& arg) = delete;
    virtual ~LibraryInfoTestProgram() = default;
#endif

public: // operators
#if ARP_ABI_VERSION_MAJOR < 2
    LibraryInfoTestProgram&  operator=(const LibraryInfoTestProgram& arg) = delete;
#endif

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
    LibraryInfoTest::LibraryInfoTestComponent& libraryInfoTestComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline LibraryInfoTestProgram::LibraryInfoTestProgram(LibraryInfoTest::LibraryInfoTestComponent& libraryInfoTestComponentArg, const String& name)
: ProgramBase(name)
, libraryInfoTestComponent(libraryInfoTestComponentArg)
{
}

} // end of namespace LibraryInfoTest
