#include "ArrayDatatypesComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ArrayDatatypesLibrary.hpp"

namespace ArrayDatatypes
{

ArrayDatatypesComponent::ArrayDatatypesComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ArrayDatatypes::ArrayDatatypesLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ArrayDatatypes::ArrayDatatypesLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void ArrayDatatypesComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ArrayDatatypesComponent::LoadConfig()
{
    // load project config here
}

void ArrayDatatypesComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ArrayDatatypesComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void ArrayDatatypesComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace ArrayDatatypes
