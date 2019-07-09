#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/IProgramComponent.hpp"
#include "Arp/Plc/Commons/Meta/IMetaComponent.hpp"
#include "Arp/Plc/Commons/Meta/DataInfoProvider.hpp"
#include "Demo\MyComponentProgramProvider.hpp"
#include "Demo\DemoLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

namespace Demo { 
	
//#component
class MyComponent : public ComponentBase, public IProgramComponent, public IMetaComponent, private Loggable<MyComponent>
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

private: // MyComponent.meta.cpp
	void RegisterComponentPorts(void);

private: // methods
    MyComponent(const MyComponent& arg) = delete;
    MyComponent& operator= (const MyComponent& arg) = delete;

public: // static factory operations
	static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& componentName);

public: // IProgramComponent operations
	IProgramProvider & GetProgramProvider(bool useBackgroundDomain)override;

public: // IMetaComponent operations
	IDataInfoProvider & GetDataInfoProvider(bool isChanging)override;
	IDataNavigator*     GetDataNavigator(void)override;

private: // fields and ports
	MyComponentProgramProvider programProvider;
	DataInfoProvider	dataInfoProvider;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class MyComponent
inline MyComponent::MyComponent(IApplication& application, const String& name)
: ComponentBase(application, ::Demo::DemoLibrary::GetInstance(), name, ComponentCategory::Custom)
, dataInfoProvider(::Demo::DemoLibrary::GetInstance().GetNamespace(), &(this->programProvider))
{
}

#pragma region IProgramComponent implementation

inline IProgramProvider& MyComponent::GetProgramProvider(bool /*useBackgroundDomain*/)
{
	return this->programProvider;
}

#pragma endregion

#pragma region IMetaComponent implementation

inline IDataInfoProvider& MyComponent::GetDataInfoProvider(bool /*useBackgroundDomain*/)
{
	return this->dataInfoProvider;
}

inline IDataNavigator* MyComponent::GetDataNavigator()
{
	return nullptr;
}

#pragma endregion

inline IComponent::Ptr MyComponent::Create(Arp::System::Acf::IApplication& application, const String& componentName)
{
	return IComponent::Ptr(new MyComponent(application, componentName));
}

} // end of namespace Demo
