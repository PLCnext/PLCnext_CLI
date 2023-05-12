#include "Defect_4334Component.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "Defect_4334Library.hpp"

namespace Defect_4334
{

Defect_4334Component::Defect_4334Component(IApplication& application, const String& name)
: ComponentBase(application, ::Defect_4334::Defect_4334Library::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::Defect_4334::Defect_4334Library::GetInstance().GetNamespace(), programProvider)
{
}

void Defect_4334Component::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void Defect_4334Component::LoadConfig()
{
    // load project config here
}

void Defect_4334Component::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void Defect_4334Component::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void Defect_4334Component::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace Defect_4334
