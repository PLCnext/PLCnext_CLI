#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "OtherNamespaceConstantComponent.hpp"

namespace OtherNamespaceConstant { namespace NotInUsingNamespace
{
const int OtherNamespaceConst = 53;
}}

namespace OtherNamespaceConstant
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

constexpr int f() { return 13; }
const int ExpressionConst = f();

//#program
//#component(OtherNamespaceConstant::OtherNamespaceConstantComponent)
class OtherNamespaceConstantProgram : public ProgramBase, private Loggable<OtherNamespaceConstantProgram>
{
public: // typedefs

public: // construction/destruction
    OtherNamespaceConstantProgram(OtherNamespaceConstant::OtherNamespaceConstantComponent& otherNamespaceConstantComponentArg, const String& name);
    OtherNamespaceConstantProgram(const OtherNamespaceConstantProgram& arg) = delete;
    virtual ~OtherNamespaceConstantProgram() = default;

public: // operators
    OtherNamespaceConstantProgram&  operator=(const OtherNamespaceConstantProgram& arg) = delete;

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
        StaticString<OtherNamespaceConst> OtherNamespaceConstPort;
        
        //#port
        StaticString<ExpressionConst> ExpressionConstPort;

private: // fields
    OtherNamespaceConstant::OtherNamespaceConstantComponent& otherNamespaceConstantComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline OtherNamespaceConstantProgram::OtherNamespaceConstantProgram(OtherNamespaceConstant::OtherNamespaceConstantComponent& otherNamespaceConstantComponentArg, const String& name)
: ProgramBase(name)
, otherNamespaceConstantComponent(otherNamespaceConstantComponentArg)
{
}

} // end of namespace OtherNamespaceConstant
