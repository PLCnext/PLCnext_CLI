#include "$(template.files.component.format.include)"
#include "Arp/Plc/Commons/Domain/PlcDomainProxy.hpp"
#include "$(root.template.files.library.name)"

$(namespace.format.start)

using namespace Arp::Plc::Commons::Domain;

$(name)::$(name)(IApplication& application, const String& name)
: ComponentBase(application, ::$(root.namespace.format.cppFullName)::$(root.name.format.lastNamespacePart.format.escapeProjectName)Library::GetInstance(), name, ComponentCategory::Custom)
, MetaComponentBase(::$(root.namespace.format.cppFullName)::$(root.name.format.lastNamespacePart.format.escapeProjectName)Library::GetInstance().GetNamespace())
{
}

void $(name)::Initialize()
{
    // never remove next line
    PlcDomainProxy::GetInstance().RegisterComponent(*this, false);
    
    // subscribe events from the event system (Nm) here
}

void $(name)::SubscribeServices()
{
	// subscribe the services used by this component here
}

void $(name)::LoadSettings(const String& /*settingsPath*/)
{
	// load firmware settings here
}

void $(name)::SetupSettings()
{
    // never remove next line
    MetaComponentBase::SetupSettings();

	// setup firmware settings here
}

void $(name)::PublishServices()
{
	// publish the services of this component here
}

void $(name)::LoadConfig()
{
    // load project config here
}

void $(name)::SetupConfig()
{
    // setup project config here
}

void $(name)::ResetConfig()
{
    // implement this inverse to SetupConfig() and LoadConfig()
}

void $(name)::Dispose()
{
    // never remove next line
    MetaComponentBase::Dispose();

	// implement this inverse to SetupSettings(), LoadSettings() and Initialize()
}

void $(name)::PowerDown()
{
	// implement this only if data must be retained even on power down event
}

$(namespace.format.end) // end of namespace $(namespace)
