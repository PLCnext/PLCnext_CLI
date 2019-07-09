#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "MyCompProgramProvider.hpp"
#include "PseudoAmbiguousComponentsLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace PseudoAmbiguousComponents
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

//#component
class MyComp : public ComponentBase, public ProgramComponentBase, private Loggable<MyComp>
{
public: // typedefs

public: // construction/destruction
    MyComp(IApplication& application, const String& name);
    virtual ~MyComp() = default;

public: // IComponent operations
    void Initialize() override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;

public: // ProgramComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
    MyComp(const MyComp& arg) = delete;
    MyComp& operator= (const MyComp& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields
    MyCompProgramProvider programProvider;

public: /* Ports
           =====
           Ports are defined in the following way:
           //#port
           //#attributes(Input|Retain)
           //#name(NameOfPort)
           boolean portField;

           The attributes comment define the port attributes and is optional.
           The name comment defines the name of the port and is optional. Default is the name of the field
        */
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class MyComp
inline MyComp::MyComp(IApplication& application, const String& name)
: ComponentBase(application, ::PseudoAmbiguousComponents::PseudoAmbiguousComponentsLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::PseudoAmbiguousComponents::PseudoAmbiguousComponentsLibrary::GetInstance().GetNamespace(), programProvider)
{
}

inline IComponent::Ptr MyComp::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new MyComp(application, name));
}

} // end of namespace PseudoAmbiguousComponents
