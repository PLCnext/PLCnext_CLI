#include "ProjectWithRedundancyAttributeComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ProjectWithRedundancyAttributeLibrary.hpp"

namespace ProjectWithRedundancyAttribute
{

ProjectWithRedundancyAttributeComponent::ProjectWithRedundancyAttributeComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ProjectWithRedundancyAttribute::ProjectWithRedundancyAttributeLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void ProjectWithRedundancyAttributeComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ProjectWithRedundancyAttributeComponent::LoadConfig()
{
    // load project config here
}

void ProjectWithRedundancyAttributeComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ProjectWithRedundancyAttributeComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void ProjectWithRedundancyAttributeComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace ProjectWithRedundancyAttribute
