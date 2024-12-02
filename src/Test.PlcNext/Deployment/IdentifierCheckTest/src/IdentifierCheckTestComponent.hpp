#pragma once
#include "Arp/System/Core/Arp.h"
#if ARP_ABI_VERSION_MAJOR < 2
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#else
#include "Arp/Base/Acf/Commons/ComponentBase.hpp"
#endif
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "IdentifierCheckTestComponentProgramProvider.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace IdentifierCheckTest
{

using namespace Arp;
#if ARP_ABI_VERSION_MAJOR < 2
using namespace Arp::System::Acf;
#else
using namespace Arp::Base::Acf::Commons;
#endif
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

//#component
class IdentifierCheckTestComponent : public ComponentBase, public ProgramComponentBase, private Loggable<IdentifierCheckTestComponent>
{
public: // typedefs

public: // construction/destruction
#if ARP_ABI_VERSION_MAJOR < 2
    IdentifierCheckTestComponent(IApplication& application, const String& name);
    virtual ~IdentifierCheckTestComponent() = default;
#else
    IdentifierCheckTestComponent(ILibrary& library, const String& name);
#endif

public: // IComponent operations
    void Initialize() override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;
    void PowerDown() override;

public: // ProgramComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
#if ARP_ABI_VERSION_MAJOR < 2
    IdentifierCheckTestComponent(const IdentifierCheckTestComponent& arg) = delete;
    IdentifierCheckTestComponent& operator= (const IdentifierCheckTestComponent& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);
#endif

private: // fields
    IdentifierCheckTestComponentProgramProvider programProvider;

public: /* Ports
           =====
           Component ports are defined in the following way:

           //#attributes(Hidden)
           struct Ports 
           {
               //#name(NameOfPort)
               //#attributes(Input|Retain|Opc)
               Arp::boolean portField = false;
               // The GDS name is "<componentName>/NameOfPort" if the struct is declared as Hidden
               // otherwise the GDS name is "<componentName>/PORTS.NameOfPort"
			   // If a component port is attributed with "Retain" additional measures need to be implemented. Fur further details refer to chapter "Component ports" in the topic "IComponent and IProgram" of https://www.plcnext.help
           };
           
           //#port
           Ports ports;

           Create one (and only one) instance of this struct.
           Apart from this single struct instance, it is recommended, that there should be no other Component variables 
           declared with the #port comment.
           The only attribute that is allowed on the struct instance is "Hidden", and this is optional. The attribute
           will hide the structure field and simulate that the struct fields are direct ports of the component. In the
           above example that would mean the component has only one port with the name "NameOfPort".
           When there are two struts with the attribute "Hidden" and both structs have a field with the same name, there
           will be an exception in the firmware. That is why only one struct is recommended. If multiple structs need to
           be used the "Hidden" attribute should be omitted.
           The struct can contain as many members as necessary.
           The #name comment can be applied to each member of the struct, and is optional.
           The #name comment defines the GDS name of an individual port element. If omitted, the member variable name is used as the GDS name.
           The members of the struct can be declared with any of the attributes allowed for a Program port.
        */
};

#if ARP_ABI_VERSION_MAJOR < 2
inline IComponent::Ptr IdentifierCheckTestComponent::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new IdentifierCheckTestComponent(application, name));
}
#endif
} // end of namespace IdentifierCheckTest
