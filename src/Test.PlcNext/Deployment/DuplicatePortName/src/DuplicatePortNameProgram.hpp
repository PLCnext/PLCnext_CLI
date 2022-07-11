#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "DuplicatePortNameComponent.hpp"

namespace DuplicatePortName
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(DuplicatePortName::DuplicatePortNameComponent)
class DuplicatePortNameProgram : public ProgramBase, private Loggable<DuplicatePortNameProgram>
{
public: // typedefs

public: // construction/destruction
    DuplicatePortNameProgram(DuplicatePortName::DuplicatePortNameComponent& duplicatePortNameComponentArg, const String& name);
    DuplicatePortNameProgram(const DuplicatePortNameProgram& arg) = delete;
    virtual ~DuplicatePortNameProgram() = default;

public: // operators
    DuplicatePortNameProgram&  operator=(const DuplicatePortNameProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute() override;

public: 

        //#port
        //#attributes(Input|Retain)
        //#name(NameOfPort)
        boolean portField;

        //#port
        //#attributes(Input|Retain)
        boolean NameOfPort;

private: // fields
    DuplicatePortName::DuplicatePortNameComponent& duplicatePortNameComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline DuplicatePortNameProgram::DuplicatePortNameProgram(DuplicatePortName::DuplicatePortNameComponent& duplicatePortNameComponentArg, const String& name)
: ProgramBase(name)
, duplicatePortNameComponent(duplicatePortNameComponentArg)
{
}

} // end of namespace DuplicatePortName
