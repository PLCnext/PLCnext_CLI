#include "DuplicatePortNameComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "DuplicatePortNameLibrary.hpp"

namespace DuplicatePortName
{

DuplicatePortNameComponent::DuplicatePortNameComponent(IApplication& application, const String& name)
: ComponentBase(application, ::DuplicatePortName::DuplicatePortNameLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::DuplicatePortName::DuplicatePortNameLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void DuplicatePortNameComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void DuplicatePortNameComponent::LoadConfig()
{
    // load project config here
}

void DuplicatePortNameComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void DuplicatePortNameComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void DuplicatePortNameComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace DuplicatePortName
