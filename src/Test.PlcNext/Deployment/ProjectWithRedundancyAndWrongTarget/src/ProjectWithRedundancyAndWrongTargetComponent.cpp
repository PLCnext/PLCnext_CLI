#include "ProjectWithRedundancyAndWrongTargetComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ProjectWithRedundancyAndWrongTargetLibrary.hpp"

namespace ProjectWithRedundancyAndWrongTarget
{

ProjectWithRedundancyAndWrongTargetComponent::ProjectWithRedundancyAndWrongTargetComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ProjectWithRedundancyAndWrongTarget::ProjectWithRedundancyAndWrongTargetLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ProjectWithRedundancyAndWrongTarget::ProjectWithRedundancyAndWrongTargetLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void ProjectWithRedundancyAndWrongTargetComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ProjectWithRedundancyAndWrongTargetComponent::LoadConfig()
{
    // load project config here
}

void ProjectWithRedundancyAndWrongTargetComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ProjectWithRedundancyAndWrongTargetComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void ProjectWithRedundancyAndWrongTargetComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace ProjectWithRedundancyAndWrongTarget
