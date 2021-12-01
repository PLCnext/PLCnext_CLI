#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "InheritanceStructsComponent.hpp"

namespace InheritanceStructs
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

class BaseClass
{
    public:
        int32 Base = 13;
};

class NextLevel : public BaseClass
{
    public:
        int16 MidLevel = 5;
};

class DistinctBase
{
    public:
        boolean OtherBase = false;
};

class MultipleInheritance : public BaseClass, public DistinctBase
{
    public:
        boolean Fooba = true;
};

class PrivateInheritance : public BaseClass, private DistinctBase
{
    public:
        boolean Fooba = true;
};

class MultiLevel : public NextLevel
{
    public:
        boolean Fooba = true;
};

class SimpleInherited : public BaseClass
{
    public:
        boolean Fooba = true;
};

//#program
//#component(InheritanceStructs::InheritanceStructsComponent)
class InheritanceStructsProgram : public ProgramBase, private Loggable<InheritanceStructsProgram>
{
public: // typedefs

public: // construction/destruction
    InheritanceStructsProgram(InheritanceStructs::InheritanceStructsComponent& inheritanceStructsComponentArg, const String& name);
    InheritanceStructsProgram(const InheritanceStructsProgram& arg) = delete;
    virtual ~InheritanceStructsProgram() = default;

public: // operators
    InheritanceStructsProgram&  operator=(const InheritanceStructsProgram& arg) = delete;

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
        SimpleInherited SimplePort;
        
        //#port
        //#attributes(Output)
        MultiLevel MultiLevelPort;
        
        //#port
        //#attributes(Output)
        PrivateInheritance PrivateMultiPort;
        
        //#port
        //#attributes(Output)
        MultipleInheritance MultiplePort;

private: // fields
    InheritanceStructs::InheritanceStructsComponent& inheritanceStructsComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline InheritanceStructsProgram::InheritanceStructsProgram(InheritanceStructs::InheritanceStructsComponent& inheritanceStructsComponentArg, const String& name)
: ProgramBase(name)
, inheritanceStructsComponent(inheritanceStructsComponentArg)
{
}

} // end of namespace InheritanceStructs
