#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "AmbigousInheritanceStructComponent.hpp"

namespace AmbigousInheritanceStruct
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

class BaseClass
{
    public:
        int32 Base = 13;
};

class NonDistinctBase
{
    public:
        int32 Base = 5;
};

class MultipleInheritance : public BaseClass, public NonDistinctBase
{
    public:
        boolean Fooba = true;
};

class PrivateMultipleInheritance : public BaseClass, private NonDistinctBase
{
    public:
        boolean Fooba = true;
};

//#program
//#component(AmbigousInheritanceStruct::AmbigousInheritanceStructComponent)
class AmbigousInheritanceStructProgram : public ProgramBase, private Loggable<AmbigousInheritanceStructProgram>
{
public: // typedefs

public: // construction/destruction
    AmbigousInheritanceStructProgram(AmbigousInheritanceStruct::AmbigousInheritanceStructComponent& ambigousInheritanceStructComponentArg, const String& name);
    AmbigousInheritanceStructProgram(const AmbigousInheritanceStructProgram& arg) = delete;
    virtual ~AmbigousInheritanceStructProgram() = default;

public: // operators
    AmbigousInheritanceStructProgram&  operator=(const AmbigousInheritanceStructProgram& arg) = delete;

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
        MultipleInheritance Multi;
        
        //#port
        //#attributes(Output)
        PrivateMultipleInheritance Private;

private: // fields
    AmbigousInheritanceStruct::AmbigousInheritanceStructComponent& ambigousInheritanceStructComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline AmbigousInheritanceStructProgram::AmbigousInheritanceStructProgram(AmbigousInheritanceStruct::AmbigousInheritanceStructComponent& ambigousInheritanceStructComponentArg, const String& name)
: ProgramBase(name)
, ambigousInheritanceStructComponent(ambigousInheritanceStructComponentArg)
{
}

} // end of namespace AmbigousInheritanceStruct
