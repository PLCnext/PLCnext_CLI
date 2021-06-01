#include "InitializedStructPortFieldComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "InitializedStructPortFieldLibrary.hpp"

namespace InitializedStructPortField
{

InitializedStructPortFieldComponent::InitializedStructPortFieldComponent(IApplication& application, const String& name)
: ComponentBase(application, ::InitializedStructPortField::InitializedStructPortFieldLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::InitializedStructPortField::InitializedStructPortFieldLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void InitializedStructPortFieldComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void InitializedStructPortFieldComponent::LoadConfig()
{
    // load project config here
}

void InitializedStructPortFieldComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void InitializedStructPortFieldComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace InitializedStructPortField
