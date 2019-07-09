#include "AppProjectComponent.hpp"

namespace AppProject
{

void AppProjectComponent::Initialize()
{
    // subscribe events from the event system (Es) here (use Arp::System::Es::EventManager)
}

void AppProjectComponent::SubscribeServices()
{
	// subscribe the services used by this component here
}

void AppProjectComponent::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void AppProjectComponent::SetupSettings()
{
	// setup firmware settings here
}

void AppProjectComponent::PublishServices()
{
	// publish the services of this component here
}

void AppProjectComponent::LoadConfig()
{
    // load project config here
}

void AppProjectComponent::SetupConfig()
{
    // never remove next line
    MetaComponentBase::SetupConfig();

    // setup project config here
}

void AppProjectComponent::ResetConfig()
{
    // never remove next line
    MetaComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void AppProjectComponent::Dispose()
{
	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void AppProjectComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

} // end of namespace AppProject
