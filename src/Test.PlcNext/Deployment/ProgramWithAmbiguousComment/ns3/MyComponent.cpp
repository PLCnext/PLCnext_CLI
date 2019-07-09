#include "MyComponent.hpp"

namespace ns1 { namespace ns2 {

void MyComponent::Initialize(void)
{
}

void MyComponent::LoadSettings(const String& /*settingsPath*/)
{
}

void MyComponent::SetupSettings(void)
{
}

void MyComponent::SubscribeServices(void)
{
}

void MyComponent::LoadConfig(void)
{
}

void MyComponent::SetupConfig(void)
{
	// DO NOT REMOVE THIS!
	MyComponent::RegisterComponentPorts();
}

void MyComponent::ResetConfig(void)
{
}

void MyComponent::PublishServices(void)
{
}

void MyComponent::Dispose(void)
{
}

void MyComponent::PowerDown(void)
{
}

}} // end of namespace ns1::ns2
