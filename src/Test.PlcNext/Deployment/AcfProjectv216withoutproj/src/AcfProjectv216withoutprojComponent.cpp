#include "AcfProjectv216withoutprojComponent.hpp"
#include "Arp/Plc/Commons/Domain/PlcDomainProxy.hpp"
#include "AcfProjectv216withoutprojLibrary.hpp"

namespace AcfProjectv216withoutproj
{

using namespace Arp::Plc::Commons::Domain;

AcfProjectv216withoutprojComponent::AcfProjectv216withoutprojComponent(IApplication& application, const String& name)
: ComponentBase(application, ::AcfProjectv216withoutproj::AcfProjectv216withoutprojLibrary::GetInstance(), name, ComponentCategory::Custom)
, MetaComponentBase(::AcfProjectv216withoutproj::AcfProjectv216withoutprojLibrary::GetInstance().GetNamespace())
{
}

void AcfProjectv216withoutprojComponent::Initialize()
{
    // never remove next line
    PlcDomainProxy::GetInstance().RegisterComponent(*this, false);
    
    // subscribe events from the event system (Nm) here
}

void AcfProjectv216withoutprojComponent::SubscribeServices()
{
	// subscribe the services used by this component here
}

void AcfProjectv216withoutprojComponent::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void AcfProjectv216withoutprojComponent::SetupSettings()
{
    // never remove next line
    MetaComponentBase::SetupSettings();

	// setup firmware settings here
}

void AcfProjectv216withoutprojComponent::PublishServices()
{
	// publish the services of this component here
}

void AcfProjectv216withoutprojComponent::LoadConfig()
{
    // load project config here
}

void AcfProjectv216withoutprojComponent::SetupConfig()
{
    // setup project config here
}

void AcfProjectv216withoutprojComponent::ResetConfig()
{
    // implement this inverse to SetupConfig() and LoadConfig()
}

void AcfProjectv216withoutprojComponent::Dispose()
{
    // never remove next line
    MetaComponentBase::Dispose();

	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void AcfProjectv216withoutprojComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

} // end of namespace AcfProjectv216withoutproj
