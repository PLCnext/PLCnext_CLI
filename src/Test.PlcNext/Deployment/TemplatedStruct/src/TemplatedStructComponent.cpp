#include "TemplatedStructComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "TemplatedStructLibrary.hpp"

namespace TemplatedStruct
{

TemplatedStructComponent::TemplatedStructComponent(IApplication& application, const String& name)
: ComponentBase(application, ::TemplatedStruct::TemplatedStructLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::TemplatedStruct::TemplatedStructLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void TemplatedStructComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void TemplatedStructComponent::LoadConfig()
{
    // load project config here
}

void TemplatedStructComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void TemplatedStructComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void TemplatedStructComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace TemplatedStruct
