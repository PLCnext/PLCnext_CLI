#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "AlmostAmbiguousComponentProgramProvider.hpp"
#include "AlmostAmbiguousLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace AlmostAmbiguous
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

//#component
class AlmostAmbiguousComponent : public ComponentBase, public ProgramComponentBase, private Loggable<AlmostAmbiguousComponent>
{
public: // typedefs

public: // construction/destruction
    AlmostAmbiguousComponent(IApplication& application, const String& name);
    virtual ~AlmostAmbiguousComponent() = default;

public: // IComponent operations
    void Initialize() override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;

public: // ProgramComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
    AlmostAmbiguousComponent(const AlmostAmbiguousComponent& arg) = delete;
    AlmostAmbiguousComponent& operator= (const AlmostAmbiguousComponent& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields
    AlmostAmbiguousComponentProgramProvider programProvider;

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
// inline methods of class AlmostAmbiguousComponent
inline AlmostAmbiguousComponent::AlmostAmbiguousComponent(IApplication& application, const String& name)
: ComponentBase(application, ::AlmostAmbiguous::AlmostAmbiguousLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::AlmostAmbiguous::AlmostAmbiguousLibrary::GetInstance().GetNamespace(), programProvider)
{
}

inline IComponent::Ptr AlmostAmbiguousComponent::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new AlmostAmbiguousComponent(application, name));
}

} // end of namespace AlmostAmbiguous
