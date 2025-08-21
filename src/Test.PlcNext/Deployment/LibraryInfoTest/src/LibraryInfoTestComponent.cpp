#include "LibraryInfoTestComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "LibraryInfoTestLibrary.hpp"

namespace LibraryInfoTest
{
#if ARP_ABI_VERSION_MAJOR < 2
LibraryInfoTestComponent::LibraryInfoTestComponent(IApplication& application, const String& name)
: ComponentBase(application, ::LibraryInfoTest::LibraryInfoTestLibrary::GetInstance(), name, ComponentCategory::Custom)
    , programProvider(*this)
    , ProgramComponentBase(::LibraryInfoTest::LibraryInfoTestLibrary::GetInstance().GetNamespace(), programProvider)
#else
LibraryInfoTestComponent::LibraryInfoTestComponent(ILibrary& library, const String& name)
    : ComponentBase(library, name, ComponentCategory::Custom, GetDefaultStartOrder())
    , programProvider(*this)
    , ProgramComponentBase(::LibraryInfoTest::LibraryInfoTestLibrary::GetInstance().GetNamespace(), programProvider)
#endif
{
}

void LibraryInfoTestComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void LibraryInfoTestComponent::LoadConfig()
{
    // load project config here
}

void LibraryInfoTestComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void LibraryInfoTestComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void LibraryInfoTestComponent::PowerDown()
{
	// implement this only if data shall be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace LibraryInfoTest
