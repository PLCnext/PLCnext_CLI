#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/IProgramComponent.hpp"
#include "Arp/Plc/Commons/Meta/IMetaComponent.hpp"
#include "Arp/Plc/Commons/Meta/DataInfoProvider.hpp"
#include "Standard\StandardComponentProgramProvider.hpp"
#include "Standard\StandardLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

namespace Standard { 
	
//#component
class StandardComponent : public ComponentBase, public IProgramComponent, public IMetaComponent
{
public: // typedefs

public: // construction/destruction
    StandardComponent(IApplication& application, const String& name);
    virtual ~StandardComponent(void) = default;

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

private: // StandardComponent.meta.cpp
	void RegisterComponentPorts(void);

private: // methods
    StandardComponent(const StandardComponent& arg) = delete;
    StandardComponent& operator= (const StandardComponent& arg) = delete;

public: // static factory operations
	static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& componentName);

public: // IProgramComponent operations
	IProgramProvider & GetProgramProvider(bool useBackgroundDomain)override;

public: // IMetaComponent operations
	IDataInfoProvider & GetDataInfoProvider(bool isChanging)override;
	IDataNavigator*     GetDataNavigator(void)override;

private: // fields and ports
	StandardComponentProgramProvider programProvider;
	DataInfoProvider	dataInfoProvider;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class StandardComponent
inline StandardComponent::StandardComponent(IApplication& application, const String& name)
: ComponentBase(application, ::Standard::StandardLibrary::GetInstance(), name, ComponentCategory::Custom)
, dataInfoProvider(::Standard::StandardLibrary::GetInstance().GetNamespace(), &(this->programProvider))
{
}

#pragma region IProgramComponent implementation

inline IProgramProvider& StandardComponent::GetProgramProvider(bool /*useBackgroundDomain*/)
{
	return this->programProvider;
}

#pragma endregion

#pragma region IMetaComponent implementation

inline IDataInfoProvider& StandardComponent::GetDataInfoProvider(bool /*useBackgroundDomain*/)
{
	return this->dataInfoProvider;
}

inline IDataNavigator* StandardComponent::GetDataNavigator()
{
	return nullptr;
}

#pragma endregion

inline IComponent::Ptr StandardComponent::Create(Arp::System::Acf::IApplication& application, const String& componentName)
{
	return IComponent::Ptr(new StandardComponent(application, componentName));
}

} // end of namespace Standard
