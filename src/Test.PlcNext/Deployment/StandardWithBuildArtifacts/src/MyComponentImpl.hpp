#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Esm/IProgramComponent.hpp"

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Esm;

namespace StandardWithBuildArtifacts
{
	
//#component
class MyComponentImpl : public ComponentBase
{
public: // typedefs

public: // construction/destruction
    MyComponentImpl(IApplication& application, const String& name);
    virtual ~MyComponentImpl(void) = default;

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
    MyComponentImpl(const MyComponentImpl& arg) = delete;
    MyComponentImpl& operator= (const MyComponentImpl& arg) = delete;

private: // fields
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class SimulinkMode1Component
inline MyComponentImpl::MyComponentImpl(IApplication& application, const String& name)
: ComponentBase(application, name, ComponentCategory::Custom, Version(0))
{
}

} // end of namespace StandardWithBuildArtifacts
