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

	struct Example {
		int32 value1;
		Bit value2;
		int64 value3[6], value4[1];
	};
	
//#component
class MyComponent : public ComponentBase
{
public: // typedefs

public: // construction/destruction
    MyComponent(IApplication& application, const String& name);
    virtual ~MyComponent(void) = default;

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
	//#port
	//#attributes(Input|Opc)
	Example examplePort1;
	//#port
	//#attributes(Output)
	Example examplePort2[1], examplePort3[5];
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class SimulinkMode1Component
inline MyComponent::MyComponent(IApplication& application, const String& name)
: ComponentBase(application, name, ComponentCategory::Custom, Version(0))
{
}

} // end of namespace TwoComponentsAndPrograms
