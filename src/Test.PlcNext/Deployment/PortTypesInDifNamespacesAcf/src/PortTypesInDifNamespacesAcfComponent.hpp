#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Meta/MetaComponentBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "DtInNestedNs.hpp"
#include "DtInOtherNs.hpp"

namespace PortTypesInDifNamespacesAcf
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Meta;
using namespace Arp::Plc::Commons::Gds;

using namespace PortTypesInDifNamespacesAcf::NestedNamespace;
using namespace OtherNamespace;

//#acfcomponent
class PortTypesInDifNamespacesAcfComponent : public ComponentBase, public MetaComponentBase, private Loggable<PortTypesInDifNamespacesAcfComponent>
{
public: // typedefs

public: // construction/destruction
    PortTypesInDifNamespacesAcfComponent(IApplication& application, const String& name);
    virtual ~PortTypesInDifNamespacesAcfComponent() = default;

public: // IComponent operations
    void Initialize() override;
    void SubscribeServices()override;
    void LoadSettings(const String& settingsPath)override;
    void SetupSettings()override;
    void PublishServices()override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;
    void Dispose()override;
    void PowerDown()override;

public: // MetaComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
    PortTypesInDifNamespacesAcfComponent(const PortTypesInDifNamespacesAcfComponent& arg) = delete;
    PortTypesInDifNamespacesAcfComponent& operator= (const PortTypesInDifNamespacesAcfComponent& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields

public: /* Ports
           =====
           Component ports are defined in the following way:

           //#attributes(Hidden)
           struct Ports 
           {
               //#name(NameOfPort)
               //#attributes(Input|Retain|Opc)
               Arp::boolean portField = false;
               // The GDS name is "<componentName>/NameOfPort" if the struct is declared as Hidden
               // otherwise the GDS name is "<componentName>/PORTS.NameOfPort"
               // If a component port is attributed with "Retain" additional measures need to be implemented. Fur further details refer to chapter "Component ports" in the topic "IComponent and IProgram" of https://www.plcnext.help
           };
           
           //#port
           Ports ports;

           Create one (and only one) instance of this struct.
           Apart from this single struct instance, it is recommended, that there should be no other Component variables 
           declared with the #port comment.
           The only attribute that is allowed on the struct instance is "Hidden", and this is optional. The attribute
           will hide the structure field and simulate that the struct fields are direct ports of the component. In the
           above example that would mean the component has only one port with the name "NameOfPort".
           When there are two struts with the attribute "Hidden" and both structs have a field with the same name, there
           will be an exception in the firmware. That is why only one struct is recommended. If multiple structs need to
           be used the "Hidden" attribute should be omitted.
           The struct can contain as many members as necessary.
           The #name comment can be applied to each member of the struct, and is optional.
           The #name comment defines the GDS name of an individual port element. If omitted, the member variable name is used as the GDS name.
           The members of the struct can be declared with any of the attributes allowed for a Program port.
        */

    //#attributes(Hidden)
    struct Ports
    {
        //#attributes(Output)
        InNestedNs outport1;

        //#attributes(Output)
        InOtherNamespace outport2;

        //#attributes(Output)
        OtherNamespace::Test outport3;

        //#attributes(Output)
        NestedNamespace::Test outport4;

        //#attributes(Output)
        MyNestedEnums::EnumInNestedNs outport5;

        //#attributes(Output)
        MyOtherEnums::EnumInOtherNs outport6;

        //#attributes(Output)
        OtherNamespace::MyEnums::EnumTest outport7;

        //#attributes(Output)
        NestedNamespace::MyEnums::EnumTest outport8;

        //#attributes(Input)
        StaticString<10> myStringPort;

        //#attributes(Input)
        StaticString<10> myStringPort2;

        //#attributes(Input)
        StaticString<> myArrayOfString[3];

        //#attributes(Output)
        OtherNamespace::MyEnums::EnumTest arrayOfOtherEnum[3];

        //#attributes(Output)
        NestedNamespace::MyEnums::EnumTest arrayOfNestedEnum[3];
    };

    //#port
    Ports ports;
};

inline IComponent::Ptr PortTypesInDifNamespacesAcfComponent::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new PortTypesInDifNamespacesAcfComponent(application, name));
}

} // end of namespace PortTypesInDifNamespacesAcf
