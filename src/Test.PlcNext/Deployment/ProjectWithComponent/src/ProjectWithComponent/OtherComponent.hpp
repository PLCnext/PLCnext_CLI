#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/IProgramComponent.hpp"
#include "Arp/Plc/Commons/Meta/IMetaComponent.hpp"
#include "Arp/Plc/Commons/Meta/DataInfoProvider.hpp"
#include "ProjectWithComponent\OtherComponentProgramProvider.hpp"
#include "ProjectWithComponent\ProjectWithComponentLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

namespace ProjectWithComponent { 
	
//#component
class OtherComponent : public ComponentBase, public IProgramComponent, public IMetaComponent
{
public: // typedefs
	//#port
	struct InlineStruct {
		bool fooba;
	} examplePort;

public: // construction/destruction
    OtherComponent(IApplication& application, const String& name);
    virtual ~OtherComponent(void) = default;

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

private: // OtherComponent.meta.cpp
	void RegisterComponentPorts(void);

private: // methods
    OtherComponent(const OtherComponent& arg) = delete;
    OtherComponent& operator= (const OtherComponent& arg) = delete;

public: // static factory operations
	static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& componentName);

public: // IProgramComponent operations
	IProgramProvider & GetProgramProvider(bool useBackgroundDomain)override;

public: // IMetaComponent operations
	IDataInfoProvider & GetDataInfoProvider(bool isChanging)override;
	IDataNavigator*     GetDataNavigator(void)override;

private: // fields and ports
	OtherComponentProgramProvider programProvider;
	DataInfoProvider	dataInfoProvider;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class OtherComponent
inline OtherComponent::OtherComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ProjectWithComponent::ProjectWithComponentLibrary::GetInstance(), name, ComponentCategory::Custom)
, dataInfoProvider(::ProjectWithComponent::ProjectWithComponentLibrary::GetInstance().GetNamespace(), &(this->programProvider))
{
}

#pragma region IProgramComponent implementation

inline IProgramProvider& OtherComponent::GetProgramProvider(bool /*useBackgroundDomain*/)
{
	return this->programProvider;
}

#pragma endregion

#pragma region IMetaComponent implementation

inline IDataInfoProvider& OtherComponent::GetDataInfoProvider(bool /*useBackgroundDomain*/)
{
	return this->dataInfoProvider;
}

inline IDataNavigator* OtherComponent::GetDataNavigator()
{
	return nullptr;
}

#pragma endregion

inline IComponent::Ptr OtherComponent::Create(Arp::System::Acf::IApplication& application, const String& componentName)
{
	return IComponent::Ptr(new OtherComponent(application, componentName));
}

} // end of namespace ProjectWithComponent
