#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "PortNameTooLongComponentProgramProvider.hpp"
#include "PortNameTooLongLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace PortNameTooLong
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

//#component
class PortNameTooLongComponent : public ComponentBase, public ProgramComponentBase, private Loggable<PortNameTooLongComponent>
{
public: // typedefs

public: // construction/destruction
    PortNameTooLongComponent(IApplication& application, const String& name);
    virtual ~PortNameTooLongComponent() = default;

public: // IComponent operations
    void Initialize() override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;

public: // ProgramComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
    PortNameTooLongComponent(const PortNameTooLongComponent& arg) = delete;
    PortNameTooLongComponent& operator= (const PortNameTooLongComponent& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields
    PortNameTooLongComponentProgramProvider programProvider;

public: /* Ports
           =====
           Component ports are defined in the following way:
           //#port
           //#name(NameOfPort)
           boolean portField;

           The name comment defines the name of the port and is optional. Default is the name of the field.
           Attributes which are defined for a component port are IGNORED. If component ports with attributes
           are necessary, define a single structure port where attributes can be defined foreach field of the
           structure.
        */
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class PortNameTooLongComponent
inline PortNameTooLongComponent::PortNameTooLongComponent(IApplication& application, const String& name)
: ComponentBase(application, ::PortNameTooLong::PortNameTooLongLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::PortNameTooLong::PortNameTooLongLibrary::GetInstance().GetNamespace(), programProvider)
{
}

inline IComponent::Ptr PortNameTooLongComponent::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new PortNameTooLongComponent(application, name));
}

} // end of namespace PortNameTooLong
