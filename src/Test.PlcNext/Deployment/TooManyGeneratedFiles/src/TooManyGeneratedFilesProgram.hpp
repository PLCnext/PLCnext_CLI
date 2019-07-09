#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "TooManyGeneratedFilesComponent.hpp"

namespace TooManyGeneratedFiles
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(TooManyGeneratedFiles::TooManyGeneratedFilesComponent)
class TooManyGeneratedFilesProgram : public ProgramBase, private Loggable<TooManyGeneratedFilesProgram>
{
public: // typedefs

public: // construction/destruction
    TooManyGeneratedFilesProgram(TooManyGeneratedFiles::TooManyGeneratedFilesComponent& tooManyGeneratedFilesComponentArg, const String& name);
    TooManyGeneratedFilesProgram(const TooManyGeneratedFilesProgram& arg) = delete;
    virtual ~TooManyGeneratedFilesProgram() = default;

public: // operators
    TooManyGeneratedFilesProgram&  operator=(const TooManyGeneratedFilesProgram& arg) = delete;

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
    TooManyGeneratedFiles::TooManyGeneratedFilesComponent& tooManyGeneratedFilesComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline TooManyGeneratedFilesProgram::TooManyGeneratedFilesProgram(TooManyGeneratedFiles::TooManyGeneratedFilesComponent& tooManyGeneratedFilesComponentArg, const String& name)
: ProgramBase(name)
, tooManyGeneratedFilesComponent(tooManyGeneratedFilesComponentArg)
{
}

} // end of namespace TooManyGeneratedFiles
