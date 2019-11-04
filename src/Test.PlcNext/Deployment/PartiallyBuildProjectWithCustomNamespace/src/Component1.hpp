#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "Component1ProgramProvider.hpp"
#include "PrjN_ST_Update_Proj_Targets_2237Library.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace RNS_ST_Update_Proj_Targets_2237
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

//#component
class Component1 : public ComponentBase, public ProgramComponentBase, private Loggable<Component1>
{
public: // typedefs

public: // construction/destruction
    Component1(IApplication& application, const String& name);
    virtual ~Component1() = default;

public: // IComponent operations
    void Initialize() override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;

public: // ProgramComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
    Component1(const Component1& arg) = delete;
    Component1& operator= (const Component1& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields
    Component1ProgramProvider programProvider;

public: /* Ports
           =====
           Component ports are defined in the following way:

           //#port
           //#attributes(Hidden)
           struct PORTS {
               //#name(NameOfPort)
               //#attributes(Input|Retain|Opc)
               Arp::boolean portField = false;
               // The GDS name is "<componentName>/NameOfPort" if the struct is declared as Hidden
               // otherwise the GDS name is "<componentName>/PORTS.NameOfPort"
           } ports;

           Create one (and only one) instance of this struct.
           Apart from this single struct instance, there must be no other Component variables declared with the #port comment.
           The only attribute that is allowed on the struct instance is "Hidden", and this is optional.
           The struct can contain as many members as necessary.
           The #name comment can be applied to each member of the struct, and is optional.
           The #name comment defines the GDS name of an individual port element. If omitted, the member variable name is used as the GDS name.
           The members of the struct can be declared with any of the attributes allowed for a Program port.
        */
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class Component1
inline Component1::Component1(IApplication& application, const String& name)
: ComponentBase(application, ::RNS_ST_Update_Proj_Targets_2237::PrjN_ST_Update_Proj_Targets_2237Library::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::RNS_ST_Update_Proj_Targets_2237::PrjN_ST_Update_Proj_Targets_2237Library::GetInstance().GetNamespace(), programProvider)
{
}

inline IComponent::Ptr Component1::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new Component1(application, name));
}

} // end of namespace RNS_ST_Update_Proj_Targets_2237
