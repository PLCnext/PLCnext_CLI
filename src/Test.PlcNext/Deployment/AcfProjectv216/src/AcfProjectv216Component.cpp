#include "AcfProjectv216Component.hpp"
#include "Arp/Plc/Commons/Domain/PlcDomainProxy.hpp"
#include "AcfProjectv216Library.hpp"

namespace AcfProjectv216
{

using namespace Arp::Plc::Commons::Domain;

AcfProjectv216Component::AcfProjectv216Component(IApplication& application, const String& name)
: ComponentBase(application, ::AcfProjectv216::AcfProjectv216Library::GetInstance(), name, ComponentCategory::Custom)
, MetaComponentBase(::AcfProjectv216::AcfProjectv216Library::GetInstance().GetNamespace())
{
}

void AcfProjectv216Component::Initialize()
{
    // never remove next line
    PlcDomainProxy::GetInstance().RegisterComponent(*this, false);
    
    // subscribe events from the event system (Nm) here
}

void AcfProjectv216Component::SubscribeServices()
{
	// subscribe the services used by this component here
}

void AcfProjectv216Component::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void AcfProjectv216Component::SetupSettings()
{
    // never remove next line
    MetaComponentBase::SetupSettings();

	// setup firmware settings here
}

void AcfProjectv216Component::PublishServices()
{
	// publish the services of this component here
}

void AcfProjectv216Component::LoadConfig()
{
    // load project config here
}

void AcfProjectv216Component::SetupConfig()
{
    // setup project config here
}

void AcfProjectv216Component::ResetConfig()
{
    // implement this inverse to SetupConfig() and LoadConfig()
}

void AcfProjectv216Component::Dispose()
{
    // never remove next line
    MetaComponentBase::Dispose();

	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void AcfProjectv216Component::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

} // end of namespace AcfProjectv216
