#include "StandardAcfProjectComponent.hpp"

namespace StandardAcfProject
{

void StandardAcfProjectComponent::Initialize()
{
    // subscribe events from the event system (Es) here (use Arp::System::Es::EventManager)
}

void StandardAcfProjectComponent::SubscribeServices()
{
	// subscribe the services used by this component here
}

void StandardAcfProjectComponent::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void StandardAcfProjectComponent::SetupSettings()
{
	// setup firmware settings here
}

void StandardAcfProjectComponent::PublishServices()
{
	// publish the services of this component here
}

void StandardAcfProjectComponent::LoadConfig()
{
    // load project config here
}

void StandardAcfProjectComponent::SetupConfig()
{
    // never remove next line
    MetaComponentBase::SetupConfig();

    // setup project config here
}

void StandardAcfProjectComponent::ResetConfig()
{
    // never remove next line
    MetaComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void StandardAcfProjectComponent::Dispose()
{
	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void StandardAcfProjectComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

} // end of namespace StandardAcfProject
