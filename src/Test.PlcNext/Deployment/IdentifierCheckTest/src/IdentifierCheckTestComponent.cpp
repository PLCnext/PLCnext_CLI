#include "IdentifierCheckTestComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "IdentifierCheckTestLibrary.hpp"

namespace IdentifierCheckTest
{
#if ARP_ABI_VERSION_MAJOR < 2
IdentifierCheckTestComponent::IdentifierCheckTestComponent(IApplication& application, const String& name)
: ComponentBase(application, ::IdentifierCheckTest::IdentifierCheckTestLibrary::GetInstance(), name, ComponentCategory::Custom)
    , programProvider(*this)
    , ProgramComponentBase(::IdentifierCheckTest::IdentifierCheckTestLibrary::GetInstance().GetNamespace(), programProvider)
#else
IdentifierCheckTestComponent::IdentifierCheckTestComponent(ILibrary& library, const String& name)
    : ComponentBase(library, name, ComponentCategory::Custom, GetDefaultStartOrder())
    , programProvider(*this)
    , ProgramComponentBase(::IdentifierCheckTest::IdentifierCheckTestLibrary::GetInstance().GetNamespace(), programProvider)
#endif
{
}

void IdentifierCheckTestComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void IdentifierCheckTestComponent::LoadConfig()
{
    // load project config here
}

void IdentifierCheckTestComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void IdentifierCheckTestComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void IdentifierCheckTestComponent::PowerDown()
{
	// implement this only if data shall be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace IdentifierCheckTest
