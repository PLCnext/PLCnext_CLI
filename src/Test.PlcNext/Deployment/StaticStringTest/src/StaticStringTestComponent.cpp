#include "StaticStringTestComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "StaticStringTestLibrary.hpp"

namespace StaticStringTest
{

StaticStringTestComponent::StaticStringTestComponent(IApplication& application, const String& name)
: ComponentBase(application, ::StaticStringTest::StaticStringTestLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::StaticStringTest::StaticStringTestLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void StaticStringTestComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void StaticStringTestComponent::LoadConfig()
{
    // load project config here
}

void StaticStringTestComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void StaticStringTestComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace StaticStringTest
