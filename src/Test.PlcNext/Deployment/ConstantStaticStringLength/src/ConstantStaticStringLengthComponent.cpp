#include "ConstantStaticStringLengthComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ConstantStaticStringLengthLibrary.hpp"

namespace ConstantStaticStringLength
{

ConstantStaticStringLengthComponent::ConstantStaticStringLengthComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ConstantStaticStringLength::ConstantStaticStringLengthLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ConstantStaticStringLength::ConstantStaticStringLengthLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void ConstantStaticStringLengthComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ConstantStaticStringLengthComponent::LoadConfig()
{
    // load project config here
}

void ConstantStaticStringLengthComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ConstantStaticStringLengthComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void ConstantStaticStringLengthComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace ConstantStaticStringLength
