#include "CommentedPortComponent.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "CommentedPortLibrary.hpp"

namespace CommentedPort
{

CommentedPortComponent::CommentedPortComponent(IApplication& application, const String& name)
: ComponentBase(application, ::CommentedPort::CommentedPortLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::CommentedPort::CommentedPortLibrary::GetInstance().GetNamespace(), programProvider)
{
}

void CommentedPortComponent::Initialize()
{
    // never remove next line
    ProgramComponentBase::Initialize();

    // subscribe events from the event system (Nm) here
}

void CommentedPortComponent::LoadConfig()
{
    // load project config here
}

void CommentedPortComponent::SetupConfig()
{
    // never remove next line
    ProgramComponentBase::SetupConfig();

    // setup project config here
}

void CommentedPortComponent::ResetConfig()
{
    // never remove next line
    ProgramComponentBase::ResetConfig();

    // implement this inverse to SetupConfig() and LoadConfig()
}

} // end of namespace CommentedPort
