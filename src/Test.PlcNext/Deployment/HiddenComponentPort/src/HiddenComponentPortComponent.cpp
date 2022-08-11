#include "HiddenComponentPortComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "HiddenComponentPortLibrary.hpp"

namespace HiddenComponentPort
{

HiddenComponentPortComponent::HiddenComponentPortComponent(IApplication& application, const String& name)
: ComponentBase(application, ::HiddenComponentPort::HiddenComponentPortLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::HiddenComponentPort::HiddenComponentPortLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void HiddenComponentPortComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void HiddenComponentPortComponent::LoadConfig()
{
    // load project config here
}

void HiddenComponentPortComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void HiddenComponentPortComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void HiddenComponentPortComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace HiddenComponentPort
