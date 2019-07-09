#include "AlmostAmbiguousComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"

namespace AlmostAmbiguous
{

void AlmostAmbiguousComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void AlmostAmbiguousComponent::LoadConfig()
{
    // load project config here
}

void AlmostAmbiguousComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void AlmostAmbiguousComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace AlmostAmbiguous
