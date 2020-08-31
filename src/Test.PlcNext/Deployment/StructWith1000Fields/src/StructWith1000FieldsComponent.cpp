#include "StructWith1000FieldsComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "StructWith1000FieldsLibrary.hpp"

namespace StructWith1000Fields
{

StructWith1000FieldsComponent::StructWith1000FieldsComponent(IApplication& application, const String& name)
: ComponentBase(application, ::StructWith1000Fields::StructWith1000FieldsLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::StructWith1000Fields::StructWith1000FieldsLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void StructWith1000FieldsComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void StructWith1000FieldsComponent::LoadConfig()
{
    // load project config here
}

void StructWith1000FieldsComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void StructWith1000FieldsComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace StructWith1000Fields
