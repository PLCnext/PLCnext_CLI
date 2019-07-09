#include "OtherComponent.hpp"

namespace ProjectWithComponent { 

void OtherComponent::Initialize(void)
{
}

void OtherComponent::LoadSettings(const String& /*settingsPath*/)
{
}

void OtherComponent::SetupSettings(void)
{
}

void OtherComponent::SubscribeServices(void)
{
}

void OtherComponent::LoadConfig(void)
{
}

void OtherComponent::SetupConfig(void)
{
	// DO NOT REMOVE THIS!
	OtherComponent::RegisterComponentPorts();
}

void OtherComponent::ResetConfig(void)
{
}

void OtherComponent::PublishServices(void)
{
}

void OtherComponent::Dispose(void)
{
}

void OtherComponent::PowerDown(void)
{
}

} // end of namespace ProjectWithComponent
