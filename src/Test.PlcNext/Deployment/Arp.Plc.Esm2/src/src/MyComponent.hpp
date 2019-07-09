#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Esm/IProgramComponent.hpp"

using namespace Arp;
using namespace Arp::System::Acf;

namespace Arp { namespace Plc { namespace Esm
{
	
//#component
class MyComponent : public ComponentBase
{
public: // typedefs

public: // construction/destruction
	MyComponent(IApplication& application, const String& name);
    virtual ~MyComponent(void) = default;

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
	MyComponent(const MyComponent& arg) = delete;
	MyComponent& operator= (const MyComponent& arg) = delete;

private: // fields
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class MyComponent
inline MyComponent::MyComponent(IApplication& application, const String& name)
: ComponentBase(application, name, ComponentCategory::Custom, Version(0))
{
}

inline IComponent::Ptr MyComponent::Create(IApplication& application, const String& componentName)
{
    return IComponent::Ptr(new MyComponent(application, componentName));
}

}}} // end of namespace Arp::Plc::Esm
