#include "ConstArraySizeComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ConstArraySizeLibrary.hpp"

namespace ConstArraySize
{

ConstArraySizeComponent::ConstArraySizeComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ConstArraySize::ConstArraySizeLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ConstArraySize::ConstArraySizeLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void ConstArraySizeComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ConstArraySizeComponent::LoadConfig()
{
    // load project config here
}

void ConstArraySizeComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ConstArraySizeComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void ConstArraySizeComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace ConstArraySize
