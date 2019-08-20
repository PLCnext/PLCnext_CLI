#include "AcfProjectComponent.hpp"

namespace AcfProject
{

void AcfProjectComponent::Initialize()
{
    // subscribe events from the event system (Es) here (use Arp::System::Es::EventManager)
}

void AcfProjectComponent::SubscribeServices()
{
	// subscribe the services used by this component here
}

void AcfProjectComponent::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void AcfProjectComponent::SetupSettings()
{
	// setup firmware settings here
}

void AcfProjectComponent::PublishServices()
{
	// publish the services of this component here
}

void AcfProjectComponent::LoadConfig()
{
    // load project config here
}

void AcfProjectComponent::SetupConfig()
{
    // never remove next line
    MetaComponentBase::SetupConfig();

    // setup project config here
}

void AcfProjectComponent::ResetConfig()
{
    // never remove next line
    MetaComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void AcfProjectComponent::Dispose()
{
	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void AcfProjectComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

} // end of namespace AcfProject
