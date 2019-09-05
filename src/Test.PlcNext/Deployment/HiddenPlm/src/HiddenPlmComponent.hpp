#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "HiddenPlmComponentProgramProvider.hpp"
#include "HiddenPlmLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace HiddenPlm
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

//#component
class HiddenPlmComponent : public ComponentBase, public ProgramComponentBase, private Loggable<HiddenPlmComponent>
{
public: // typedefs
	//#attributes(Hidden)
	struct Example {
		//#attributes(Input)
		int32 value1;
		//#attributes(Output|Opc)
        //#name(NamedPort)
		bool value2;
	};

public: // construction/destruction
    HiddenPlmComponent(IApplication& application, const String& name);
    virtual ~HiddenPlmComponent() = default;

public: // IComponent operations
    void Initialize() override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;

public: // ProgramComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
    HiddenPlmComponent(const HiddenPlmComponent& arg) = delete;
    HiddenPlmComponent& operator= (const HiddenPlmComponent& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields
    HiddenPlmComponentProgramProvider programProvider;

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
		//#port
		Example ports;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class HiddenPlmComponent
inline HiddenPlmComponent::HiddenPlmComponent(IApplication& application, const String& name)
: ComponentBase(application, ::HiddenPlm::HiddenPlmLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::HiddenPlm::HiddenPlmLibrary::GetInstance().GetNamespace(), programProvider)
{
}

inline IComponent::Ptr HiddenPlmComponent::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new HiddenPlmComponent(application, name));
}

} // end of namespace HiddenPlm
