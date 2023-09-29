#include "CppSpecialComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "CppSpecialLibrary.hpp"

namespace CppSpecial
{

CppSpecialComponent::CppSpecialComponent(IApplication& application, const String& name)
: ComponentBase(application, ::CppSpecial::CppSpecialLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::CppSpecial::CppSpecialLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void CppSpecialComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void CppSpecialComponent::LoadConfig()
{
    // load project config here
}

void CppSpecialComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void CppSpecialComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void CppSpecialComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace CppSpecial
