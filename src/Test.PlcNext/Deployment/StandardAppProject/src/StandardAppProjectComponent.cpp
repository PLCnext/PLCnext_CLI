#include "StandardAppProjectComponent.hpp"

namespace StandardAppProject
{

void StandardAppProjectComponent::Initialize()
{
    // subscribe events from the event system (Es) here (use Arp::System::Es::EventManager)
}

void StandardAppProjectComponent::SubscribeServices()
{
	// subscribe the services used by this component here
}

void StandardAppProjectComponent::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void StandardAppProjectComponent::SetupSettings()
{
	// setup firmware settings here
}

void StandardAppProjectComponent::PublishServices()
{
	// publish the services of this component here
}

void StandardAppProjectComponent::LoadConfig()
{
    // load project config here
}

void StandardAppProjectComponent::SetupConfig()
{
    // never remove next line
    MetaComponentBase::SetupConfig();

    // setup project config here
}

void StandardAppProjectComponent::ResetConfig()
{
    // never remove next line
    MetaComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void StandardAppProjectComponent::Dispose()
{
	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void StandardAppProjectComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

} // end of namespace StandardAppProject
