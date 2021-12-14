#include "OtherNamespaceConstantComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "OtherNamespaceConstantLibrary.hpp"

namespace OtherNamespaceConstant
{

OtherNamespaceConstantComponent::OtherNamespaceConstantComponent(IApplication& application, const String& name)
: ComponentBase(application, ::OtherNamespaceConstant::OtherNamespaceConstantLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::OtherNamespaceConstant::OtherNamespaceConstantLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void OtherNamespaceConstantComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void OtherNamespaceConstantComponent::LoadConfig()
{
    // load project config here
}

void OtherNamespaceConstantComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void OtherNamespaceConstantComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void OtherNamespaceConstantComponent::PowerDown()
{
    // implement this only if data must be retained even on power down event
    // Available with 2021.6 FW
}

} // end of namespace OtherNamespaceConstant
