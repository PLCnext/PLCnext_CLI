#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Esm/IProgramComponent.hpp"

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Esm;

namespace Library
{
	
//#component
class Component : public ComponentBase
{
public: // typedefs

public: // construction/destruction
    Component(IApplication& application, const String& name);
    virtual ~Component(void) = default;

public: // static factory operations
    static IComponent::Ptr Create(IApplication& application, const String& componentName);

public: // IComponent operations
    void Initialize(void)override;
    void LoadSettings(const String& settingsPath)override;
    void SetupSettings(void)override;
    void SubscribeServices(void)override;
    void LoadConfig(void)override;
    void SetupConfig(void)override;
    void ResetConfig(void)override;
    void PublishServices(void)override;
    void Dispose(void)override;
    void PowerDown(void)override;

private: // methods
    Component(const Component& arg) = delete;
    Component& operator= (const Component& arg) = delete;

private: // fields
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class SimulinkMode1Component
inline Component::Component(IApplication& application, const String& name)
: ComponentBase(application, name, ComponentCategory::Custom, Version(0))
{
}

inline IComponent::Ptr Component::Create(IApplication& application, const String& componentName)
{
    return IComponent::Ptr(new Component(application, componentName));
}

} // end of namespace Library
