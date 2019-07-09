#include "MultipleNamespacesComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"

namespace ComponentsInMultipleNamespaces
{

void MultipleNamespacesComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void MultipleNamespacesComponent::LoadConfig()
{
    // load project config here
}

void MultipleNamespacesComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void MultipleNamespacesComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace MultipleNamespaces
