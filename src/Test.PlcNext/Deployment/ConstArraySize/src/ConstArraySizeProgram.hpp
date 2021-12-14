#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ConstArraySizeComponent.hpp"

namespace ConstArraySize
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

const int SizeConst = 13;
#define SIZE_DIRECTIVE 22

//#program
//#component(ConstArraySize::ConstArraySizeComponent)
class ConstArraySizeProgram : public ProgramBase, private Loggable<ConstArraySizeProgram>
{
public: // typedefs

public: // construction/destruction
    ConstArraySizeProgram(ConstArraySize::ConstArraySizeComponent& constArraySizeComponentArg, const String& name);
    ConstArraySizeProgram(const ConstArraySizeProgram& arg) = delete;
    virtual ~ConstArraySizeProgram() = default;

public: // operators
    ConstArraySizeProgram&  operator=(const ConstArraySizeProgram& arg) = delete;

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
        int32 ConstArray[SizeConst];
        
        //#port
        int32 DirectiveArray[SIZE_DIRECTIVE];

private: // fields
    ConstArraySize::ConstArraySizeComponent& constArraySizeComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ConstArraySizeProgram::ConstArraySizeProgram(ConstArraySize::ConstArraySizeComponent& constArraySizeComponentArg, const String& name)
: ProgramBase(name)
, constArraySizeComponent(constArraySizeComponentArg)
{
}

} // end of namespace ConstArraySize
