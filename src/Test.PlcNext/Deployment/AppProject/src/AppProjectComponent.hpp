#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "AppProjectLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaComponentBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace AppProject
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Meta;

//#appcomponent
class AppProjectComponent : public ComponentBase, public MetaComponentBase, private Loggable<AppProjectComponent>
{
public: // typedefs

public: // construction/destruction
    AppProjectComponent(IApplication& application, const String& name);
    virtual ~AppProjectComponent() = default;

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
    AppProjectComponent(const AppProjectComponent& arg) = delete;
    AppProjectComponent& operator= (const AppProjectComponent& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields

public: /* Ports
           =====
           Component ports are defined in the following way:
           //#port
           //#name(NameOfPort)
           boolean portField;

           The name comment defines the name of the port and is optional. Default is the name of the field.
           Attributes which are defined for a component port are IGNORED. If component ports with attributes
           are necessary, define a single structure port where attributes can be defined foreach field of the
           structure.
        */
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class AppProjectComponent
inline AppProjectComponent::AppProjectComponent(IApplication& application, const String& name)
: ComponentBase(application, ::AppProject::AppProjectLibrary::GetInstance(), name, ComponentCategory::Custom)
, MetaComponentBase(::AppProject::AppProjectLibrary::GetInstance().GetNamespace())
{
}

inline IComponent::Ptr AppProjectComponent::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new AppProjectComponent(application, name));
}

} // end of namespace AppProject
