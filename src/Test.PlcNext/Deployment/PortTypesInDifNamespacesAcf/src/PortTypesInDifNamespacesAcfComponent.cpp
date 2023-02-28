#include "PortTypesInDifNamespacesAcfComponent.hpp"
#include "Arp/Plc/Commons/Domain/PlcDomainProxy.hpp"
#include "PortTypesInDifNamespacesAcfLibrary.hpp"

namespace PortTypesInDifNamespacesAcf
{

using namespace Arp::Plc::Commons::Domain;

PortTypesInDifNamespacesAcfComponent::PortTypesInDifNamespacesAcfComponent(IApplication& application, const String& name)
: ComponentBase(application, ::PortTypesInDifNamespacesAcf::PortTypesInDifNamespacesAcfLibrary::GetInstance(), name, ComponentCategory::Custom)
, MetaComponentBase(::PortTypesInDifNamespacesAcf::PortTypesInDifNamespacesAcfLibrary::GetInstance().GetNamespace())
{
}

void PortTypesInDifNamespacesAcfComponent::Initialize()
{
    // never remove next line
    PlcDomainProxy::GetInstance().RegisterComponent(*this, true);
    
    // subscribe events from the event system (Nm) here
}

void PortTypesInDifNamespacesAcfComponent::SubscribeServices()
{
	// subscribe the services used by this component here
}

void PortTypesInDifNamespacesAcfComponent::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void PortTypesInDifNamespacesAcfComponent::SetupSettings()
{
    // never remove next line
    MetaComponentBase::SetupSettings();

	// setup firmware settings here
}

void PortTypesInDifNamespacesAcfComponent::PublishServices()
{
	// publish the services of this component here
}

void PortTypesInDifNamespacesAcfComponent::LoadConfig()
{
    // load project config here
}

void PortTypesInDifNamespacesAcfComponent::SetupConfig()
{
    // setup project config here
}

void PortTypesInDifNamespacesAcfComponent::ResetConfig()
{
    // implement this inverse to SetupConfig() and LoadConfig()
}

void PortTypesInDifNamespacesAcfComponent::Dispose()
{
    // never remove next line
    MetaComponentBase::Dispose();

	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void PortTypesInDifNamespacesAcfComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace PortTypesInDifNamespacesAcf
