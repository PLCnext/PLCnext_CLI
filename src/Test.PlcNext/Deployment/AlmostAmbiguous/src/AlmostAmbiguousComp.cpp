#include "AlmostAmbiguousComp.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"

namespace AlmostAmbiguous
{

void AlmostAmbiguousComp::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void AlmostAmbiguousComp::LoadConfig()
{
    // load project config here
}

void AlmostAmbiguousComp::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void AlmostAmbiguousComp::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace AlmostAmbiguous
