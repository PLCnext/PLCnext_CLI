#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace Standard { 

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(StandardComponent)
class StandardNewProgram : public ProgramBase, private Loggable<StandardNewProgram>
{
public: // typedefs

public: // construction/destruction
    StandardNewProgram(const String& name);
    StandardNewProgram(const StandardNewProgram& arg) = delete;
    virtual ~StandardNewProgram(void) = default;

public: // operators
    StandardNewProgram&  operator=(const StandardNewProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute(void)override;

protected: // fields and ports

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline StandardNewProgram::StandardNewProgram(const String& name) : ProgramBase(name)
{
}

} // end of namespace Standard::StandardComponent
