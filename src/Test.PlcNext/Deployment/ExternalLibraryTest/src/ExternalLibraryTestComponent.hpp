﻿#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Acf/ComponentBase.hpp"
#include "Arp/System/Acf/IApplication.hpp"
#include "Arp/Plc/Commons/Esm/ProgramComponentBase.hpp"
#include "ExternalLibraryTestComponentProgramProvider.hpp"
#include "ExternalLibraryTestLibrary.hpp"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/System/Commons/Logging.h"

namespace ExternalLibraryTest
{

using namespace Arp;
using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Esm;
using namespace Arp::Plc::Commons::Meta;

//#component
class ExternalLibraryTestComponent : public ComponentBase, public ProgramComponentBase, private Loggable<ExternalLibraryTestComponent>
{
public: // typedefs

public: // construction/destruction
    ExternalLibraryTestComponent(IApplication& application, const String& name);
    virtual ~ExternalLibraryTestComponent() = default;

public: // IComponent operations
    void Initialize() override;
    void LoadConfig() override;
    void SetupConfig() override;
    void ResetConfig() override;

public: // ProgramComponentBase operations
    void RegisterComponentPorts() override;

private: // methods
    ExternalLibraryTestComponent(const ExternalLibraryTestComponent& arg) = delete;
    ExternalLibraryTestComponent& operator= (const ExternalLibraryTestComponent& arg) = delete;

public: // static factory operations
    static IComponent::Ptr Create(Arp::System::Acf::IApplication& application, const String& name);

private: // fields
    ExternalLibraryTestComponentProgramProvider programProvider;

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
// inline methods of class ExternalLibraryTestComponent
inline ExternalLibraryTestComponent::ExternalLibraryTestComponent(IApplication& application, const String& name)
: ComponentBase(application, ::ExternalLibraryTest::ExternalLibraryTestLibrary::GetInstance(), name, ComponentCategory::Custom)
, programProvider(*this)
, ProgramComponentBase(::ExternalLibraryTest::ExternalLibraryTestLibrary::GetInstance().GetNamespace(), programProvider)
{
}

inline IComponent::Ptr ExternalLibraryTestComponent::Create(Arp::System::Acf::IApplication& application, const String& name)
{
    return IComponent::Ptr(new ExternalLibraryTestComponent(application, name));
}

} // end of namespace ExternalLibraryTest
