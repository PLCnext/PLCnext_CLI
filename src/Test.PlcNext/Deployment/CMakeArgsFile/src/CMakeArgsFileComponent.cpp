#include "CMakeArgsFileComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "CMakeArgsFileLibrary.hpp"

namespace CMakeArgsFile
{

CMakeArgsFileComponent::CMakeArgsFileComponent(IApplication& application, const String& name)
: ComponentBase(application, ::CMakeArgsFile::CMakeArgsFileLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::CMakeArgsFile::CMakeArgsFileLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void CMakeArgsFileComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void CMakeArgsFileComponent::LoadConfig()
{
    // load project config here
}

void CMakeArgsFileComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void CMakeArgsFileComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace CMakeArgsFile
