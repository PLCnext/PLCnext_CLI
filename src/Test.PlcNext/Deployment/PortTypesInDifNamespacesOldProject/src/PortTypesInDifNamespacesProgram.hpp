#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "PortTypesInDifNamespacesComponent.hpp"
#include "DtInNestedNs.hpp"
#include "DtInOtherNs.hpp"

namespace PortTypesInDifNamespaces
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

using namespace PortTypesInDifNamespaces::NestedNamespace;
using namespace OtherNamespace;

//#program
//#component(PortTypesInDifNamespaces::PortTypesInDifNamespacesComponent)
class PortTypesInDifNamespacesProgram : public ProgramBase, private Loggable<PortTypesInDifNamespacesProgram>
{
public: // typedefs

public: // construction/destruction
    PortTypesInDifNamespacesProgram(PortTypesInDifNamespaces::PortTypesInDifNamespacesComponent& portTypesInDifNamespacesComponentArg, const String& name);
    PortTypesInDifNamespacesProgram(const PortTypesInDifNamespacesProgram& arg) = delete;
    virtual ~PortTypesInDifNamespacesProgram() = default;

public: // operators
    PortTypesInDifNamespacesProgram&  operator=(const PortTypesInDifNamespacesProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute() override;

public: 
        //#port
        //#attributes(Output)
        InNestedNs outport1;

        //#port
        //#attributes(Output)
        InOtherNamespace outport2;

        //#port
        //#attributes(Output)
        OtherNamespace::Test outport3;

        //#port
        //#attributes(Output)
        NestedNamespace::Test outport4;

        //#port
        //#attributes(Output)
        MyNestedEnums::EnumInNestedNs outport5;

        //#port
        //#attributes(Output)
        MyOtherEnums::EnumInOtherNs outport6;

        //#port
        //#attributes(Output)
        OtherNamespace::MyEnums::EnumTest outport7;

        //#port
        //#attributes(Output)
        NestedNamespace::MyEnums::EnumTest outport8;

        //#port
        //#attributes(Input)
        StaticString<10> myStringPort;

        //#port
        //#attributes(Input)
        StaticString<10> myStringPort2;

        //#port
        //#attributes(Input)
        StaticString<> myArrayOfString[3];

        //#port
        //#attributes(Output)
        OtherNamespace::MyEnums::EnumTest arrayOfOtherEnum[3];

        //#port
        //#attributes(Output)
        NestedNamespace::MyEnums::EnumTest arrayOfNestedEnum[3];

private: // fields
    PortTypesInDifNamespaces::PortTypesInDifNamespacesComponent& portTypesInDifNamespacesComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline PortTypesInDifNamespacesProgram::PortTypesInDifNamespacesProgram(PortTypesInDifNamespaces::PortTypesInDifNamespacesComponent& portTypesInDifNamespacesComponentArg, const String& name)
: ProgramBase(name)
, portTypesInDifNamespacesComponent(portTypesInDifNamespacesComponentArg)
{
}

} // end of namespace PortTypesInDifNamespaces
