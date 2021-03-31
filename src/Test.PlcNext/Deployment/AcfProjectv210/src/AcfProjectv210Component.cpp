#include "AcfProjectv210Component.hpp"
#include "Arp/Plc/Commons/Domain/PlcDomainProxy.hpp"
#include "AcfProjectv210Library.hpp"

namespace AcfProjectv210
{

using namespace Arp::Plc::Commons::Domain;

AcfProjectv210Component::AcfProjectv210Component(IApplication& application, const String& name)
: ComponentBase(application, ::AcfProjectv210::AcfProjectv210Library::GetInstance(), name, ComponentCategory::Custom)
, MetaComponentBase(::AcfProjectv210::AcfProjectv210Library::GetInstance().GetNamespace())
{
}

void AcfProjectv210Component::Initialize()
{
    // never remove next line
    PlcDomainProxy::GetInstance().RegisterComponent(*this, false);
    
    // subscribe events from the event system (Nm) here
}

void AcfProjectv210Component::SubscribeServices()
{
	// subscribe the services used by this component here
}

void AcfProjectv210Component::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void AcfProjectv210Component::SetupSettings()
{
    // never remove next line
    MetaComponentBase::SetupSettings();

	// setup firmware settings here
}

void AcfProjectv210Component::PublishServices()
{
	// publish the services of this component here
}

void AcfProjectv210Component::LoadConfig()
{
    // load project config here
}

void AcfProjectv210Component::SetupConfig()
{
    // setup project config here
}

void AcfProjectv210Component::ResetConfig()
{
    // implement this inverse to SetupConfig() and LoadConfig()
}

void AcfProjectv210Component::Dispose()
{
    // never remove next line
    MetaComponentBase::Dispose();

	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void AcfProjectv210Component::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

} // end of namespace AcfProjectv210
