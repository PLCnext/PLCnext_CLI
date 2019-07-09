#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "$(component.template.files.component.format.include)"

$(namespace.format.start)

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//$(settings.AttributePrefix)program
//$(settings.AttributePrefix)component($(component.fullName.format.cppFullName))
class $(name) : public ProgramBase, private Loggable<$(name)>
{
public: // typedefs

public: // construction/destruction
    $(name)($(component.fullName.format.cppFullName)& \l$(component.name)Arg, const String& name);
    $(name)(const $(name)& arg) = delete;
    virtual ~$(name)() = default;

public: // operators
    $(name)&  operator=(const $(name)& arg) = delete;

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
    $(component.fullName.format.cppFullName)& \l$(component.name);

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline $(name)::$(name)($(component.fullName.format.cppFullName)& \l$(component.name)Arg, const String& name)
: ProgramBase(name)
, \l$(component.name)(\l$(component.name)Arg)
{
}

$(namespace.format.end) // end of namespace $(namespace)
