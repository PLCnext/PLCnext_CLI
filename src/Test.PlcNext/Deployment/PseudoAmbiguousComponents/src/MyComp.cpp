#include "MyComp.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"

namespace PseudoAmbiguousComponents
{

void MyComp::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void MyComp::LoadConfig()
{
    // load project config here
}

void MyComp::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void MyComp::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace PseudoAmbiguousComponents
