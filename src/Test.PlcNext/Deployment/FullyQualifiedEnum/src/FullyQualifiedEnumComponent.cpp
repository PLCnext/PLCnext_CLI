#include "FullyQualifiedEnumComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "FullyQualifiedEnumLibrary.hpp"

namespace FullyQualifiedEnum
{

FullyQualifiedEnumComponent::FullyQualifiedEnumComponent(IApplication& application, const String& name)
: ComponentBase(application, ::FullyQualifiedEnum::FullyQualifiedEnumLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::FullyQualifiedEnum::FullyQualifiedEnumLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void FullyQualifiedEnumComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void FullyQualifiedEnumComponent::LoadConfig()
{
    // load project config here
}

void FullyQualifiedEnumComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void FullyQualifiedEnumComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace FullyQualifiedEnum
