#include "InheritanceStructsComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "InheritanceStructsLibrary.hpp"

namespace InheritanceStructs
{

InheritanceStructsComponent::InheritanceStructsComponent(IApplication& application, const String& name)
: ComponentBase(application, ::InheritanceStructs::InheritanceStructsLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::InheritanceStructs::InheritanceStructsLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void InheritanceStructsComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void InheritanceStructsComponent::LoadConfig()
{
    // load project config here
}

void InheritanceStructsComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void InheritanceStructsComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void InheritanceStructsComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace InheritanceStructs
