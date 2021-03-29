#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "FullyQualifiedEnumComponent.hpp"

namespace FullyQualifiedEnum
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(FullyQualifiedEnum::FullyQualifiedEnumComponent)
class FullyQualifiedEnumProgram : public ProgramBase, private Loggable<FullyQualifiedEnumProgram>
{
public: // typedefs
	enum class QualityState : Arp::uint8 {
        None = 0x00,
        Good = 0x01,
        Bad = 0xFF,
	};
public: // construction/destruction
    FullyQualifiedEnumProgram(FullyQualifiedEnum::FullyQualifiedEnumComponent& fullyQualifiedEnumComponentArg, const String& name);
    FullyQualifiedEnumProgram(const FullyQualifiedEnumProgram& arg) = delete;
    virtual ~FullyQualifiedEnumProgram() = default;

public: // operators
    FullyQualifiedEnumProgram&  operator=(const FullyQualifiedEnumProgram& arg) = delete;

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
        //#attributes(Input|Retain)
		QualityState MyPort;

private: // fields
    FullyQualifiedEnum::FullyQualifiedEnumComponent& fullyQualifiedEnumComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline FullyQualifiedEnumProgram::FullyQualifiedEnumProgram(FullyQualifiedEnum::FullyQualifiedEnumComponent& fullyQualifiedEnumComponentArg, const String& name)
: ProgramBase(name)
, fullyQualifiedEnumComponent(fullyQualifiedEnumComponentArg)
{
}

} // end of namespace FullyQualifiedEnum
