#include "PortTypesInDifNamespacesComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "PortTypesInDifNamespacesLibrary.hpp"

namespace PortTypesInDifNamespaces
{

PortTypesInDifNamespacesComponent::PortTypesInDifNamespacesComponent(IApplication& application, const String& name)
: ComponentBase(application, ::PortTypesInDifNamespaces::PortTypesInDifNamespacesLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::PortTypesInDifNamespaces::PortTypesInDifNamespacesLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void PortTypesInDifNamespacesComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void PortTypesInDifNamespacesComponent::LoadConfig()
{
    // load project config here
}

void PortTypesInDifNamespacesComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void PortTypesInDifNamespacesComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void PortTypesInDifNamespacesComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace PortTypesInDifNamespaces
