#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Esm/IProgramComponent.hpp"

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Esm;

namespace TwoComponentsAndPrograms
{
	
//#component
class MyOtherComponent : public ComponentBase
{
public: // typedefs

public: // construction/destruction
    MyOtherComponent(IApplication& application, const String& name);
    virtual ~MyOtherComponent(void) = default;

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
    MyOtherComponent(const MyOtherComponent& arg) = delete;
    MyOtherComponent& operator= (const MyOtherComponent& arg) = delete;

private: // fields
	//#port
	int32 examplePort4 = 3;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class MyOtherComponent
inline MyOtherComponent::MyOtherComponent(IApplication& application, const String& name)
: ComponentBase(application, name, ComponentCategory::Custom, Version(0))
{
}

} // end of namespace TwoComponentsAndPrograms
