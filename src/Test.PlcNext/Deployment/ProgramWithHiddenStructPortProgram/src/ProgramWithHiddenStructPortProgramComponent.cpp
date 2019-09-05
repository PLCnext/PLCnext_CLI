#include "ProgramWithHiddenStructPortProgramComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"

namespace ProgramWithHiddenStructPortProgram
{

void ProgramWithHiddenStructPortProgramComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void ProgramWithHiddenStructPortProgramComponent::LoadConfig()
{
    // load project config here
}

void ProgramWithHiddenStructPortProgramComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void ProgramWithHiddenStructPortProgramComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace ProgramWithHiddenStructPortProgram
