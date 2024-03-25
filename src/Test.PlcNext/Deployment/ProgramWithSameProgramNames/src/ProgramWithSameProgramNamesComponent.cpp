#include "ProgramWithSameProgramNamesComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ProgramWithSameProgramNamesLibrary.hpp"

namespace ProgramWithSameProgramNames
{

ProgramWithSameProgramNamesComponent::ProgramWithSameProgramNamesComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ProgramWithSameProgramNames::ProgramWithSameProgramNamesLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ProgramWithSameProgramNames::ProgramWithSameProgramNamesLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void ProgramWithSameProgramNamesComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ProgramWithSameProgramNamesComponent::LoadConfig()
{
    // load project config here
}

void ProgramWithSameProgramNamesComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ProgramWithSameProgramNamesComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void ProgramWithSameProgramNamesComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace ProgramWithSameProgramNames
