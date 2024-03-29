﻿#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Meta/MetaComponentBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace AcfProjectv216
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Meta;

//#acfcomponent
class AcfProjectv216Component : public ComponentBase, public MetaComponentBase, private Loggable<AcfProjectv216Component>
{
public: // typedefs

#define MY_VALUE 42

    //#attributes(Hidden)
    struct Ports
    {
        //#name(NameOfPort)
        //#attributes(Input|Retain|Opc)
        Arp::boolean portField = false;

        StaticString<23> someString23;
        StaticWString<109> someWString109;
        StaticWString<MY_VALUE> someWStringFromDefine;

        Arp::int32 value2[10];
    };

public: // construction/destruction
    AcfProjectv216Component(IApplication& application, const String& name);
    virtual ~AcfProjectv216Component() = default;

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
    AcfProjectv216Component(const AcfProjectv216Component& arg) = delete;
    AcfProjectv216Component& operator= (const AcfProjectv216Component& arg) = delete;

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
           };
           
           //#port
           Ports ports;

           Create one (and only one) instance of this struct.
           Apart from this single struct instance, there must be no other Component variables declared with the #port comment.
           The only attribute that is allowed on the struct instance is "Hidden", and this is optional.
           The struct can contain as many members as necessary.
           The #name comment can be applied to each member of the struct, and is optional.
           The #name comment defines the GDS name of an individual port element. If omitted, the member variable name is used as the GDS name.
           The members of the struct can be declared with any of the attributes allowed for a Program port.
        */

    //#port
    Ports ports;

};

inline IComponent::Ptr AcfProjectv216Component::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new AcfProjectv216Component(application, name));
}

} // end of namespace AcfProjectv216
