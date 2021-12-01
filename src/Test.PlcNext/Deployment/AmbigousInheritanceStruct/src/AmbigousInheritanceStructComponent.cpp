#include "AmbigousInheritanceStructComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "AmbigousInheritanceStructLibrary.hpp"

namespace AmbigousInheritanceStruct
{

AmbigousInheritanceStructComponent::AmbigousInheritanceStructComponent(IApplication& application, const String& name)
: ComponentBase(application, ::AmbigousInheritanceStruct::AmbigousInheritanceStructLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::AmbigousInheritanceStruct::AmbigousInheritanceStructLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void AmbigousInheritanceStructComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void AmbigousInheritanceStructComponent::LoadConfig()
{
    // load project config here
}

void AmbigousInheritanceStructComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void AmbigousInheritanceStructComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void AmbigousInheritanceStructComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace AmbigousInheritanceStruct
