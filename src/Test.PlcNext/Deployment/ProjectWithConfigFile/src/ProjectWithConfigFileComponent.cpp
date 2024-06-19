#include "ProjectWithConfigFileComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ProjectWithConfigFileLibrary.hpp"

namespace ProjectWithConfigFile
{

ProjectWithConfigFileComponent::ProjectWithConfigFileComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ProjectWithConfigFile::ProjectWithConfigFileLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ProjectWithConfigFile::ProjectWithConfigFileLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void ProjectWithConfigFileComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ProjectWithConfigFileComponent::LoadConfig()
{
    // load project config here
}

void ProjectWithConfigFileComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ProjectWithConfigFileComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

void ProjectWithConfigFileComponent::PowerDown()
{
	// implement this only if data must be retained even on power down event
	// will work only for PLCnext controllers with an "Integrated uninterruptible power supply (UPS)"
	// Available with 2021.6 FW
}

} // end of namespace ProjectWithConfigFile
