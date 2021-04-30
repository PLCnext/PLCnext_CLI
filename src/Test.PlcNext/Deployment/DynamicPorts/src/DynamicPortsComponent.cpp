#include "DynamicPortsComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "DynamicPortsLibrary.hpp"

namespace DynamicPorts
{

DynamicPortsComponent::DynamicPortsComponent(IApplication& application, const String& name)
: ComponentBase(application, ::DynamicPorts::DynamicPortsLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::DynamicPorts::DynamicPortsLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void DynamicPortsComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void DynamicPortsComponent::LoadConfig()
{
    // load project config here
}

void DynamicPortsComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void DynamicPortsComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace DynamicPorts
