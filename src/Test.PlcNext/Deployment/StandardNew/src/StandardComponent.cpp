#include "StandardComponent.hpp"

namespace Standard { 

void StandardComponent::Initialize(void)
{
}

void StandardComponent::LoadSettings(const String& /*settingsPath*/)
{
}

void StandardComponent::SetupSettings(void)
{
}

void StandardComponent::SubscribeServices(void)
{
}

void StandardComponent::LoadConfig(void)
{
}

void StandardComponent::SetupConfig(void)
{
	// DO NOT REMOVE THIS!
	StandardComponent::RegisterComponentPorts();
}

void StandardComponent::ResetConfig(void)
{
}

void StandardComponent::PublishServices(void)
{
}

void StandardComponent::Dispose(void)
{
}

void StandardComponent::PowerDown(void)
{
}

} // end of namespace Standard
