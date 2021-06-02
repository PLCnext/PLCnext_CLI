#include "PpDirectivesComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "PpDirectivesLibrary.hpp"

namespace PpDirectives
{

PpDirectivesComponent::PpDirectivesComponent(IApplication& application, const String& name)
: ComponentBase(application, ::PpDirectives::PpDirectivesLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::PpDirectives::PpDirectivesLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void PpDirectivesComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void PpDirectivesComponent::LoadConfig()
{
    // load project config here
}

void PpDirectivesComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void PpDirectivesComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace PpDirectives
