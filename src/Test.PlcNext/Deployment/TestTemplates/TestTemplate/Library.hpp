#pragma once
#include "Arp/System/Core/Arp.h"
#if ARP_ABI_VERSION_MAJOR < 2
#include "Arp/System/Core/AppDomain.hpp"
#include "Arp/System/Core/Singleton.hxx"
#include "Arp/System/Core/Library.h"
#endif
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeDomain.hpp"

$(namespace.format.start)

#if ARP_ABI_VERSION_MAJOR < 2
using namespace Arp::System::Acf;
#else
using namespace Arp::Base::Acf::Commons;
#endif
using namespace Arp::Plc::Commons::Meta;
using namespace Arp::Plc::Commons::Meta::TypeSystem;

class $(name.format.lastNamespacePart.format.escapeProjectName)Library : public MetaLibraryBase
#if ARP_ABI_VERSION_MAJOR < 2
    , public Singleton<$(name.format.lastNamespacePart.format.escapeProjectName)Library>
#endif
{
#if ARP_ABI_VERSION_MAJOR < 2
public: // typedefs
    typedef Singleton<$(name.format.lastNamespacePart.format.escapeProjectName)Library> SingletonBase;
#else
    $(name.format.lastNamespacePart.format.escapeProjectName)Library(void);
#endif
public: // construction/destruction
#if ARP_ABI_VERSION_MAJOR < 2
    $(name.format.lastNamespacePart.format.escapeProjectName)Library(AppDomain& appDomain);
    virtual ~$(name.format.lastNamespacePart.format.escapeProjectName)Library() = default;

public: // static operations (called through reflection)
    static void Main(AppDomain& appDomain);
#else
    static $(name.format.lastNamespacePart.format.escapeProjectName)Library& GetInstance();
#endif

private: // methods
    void InitializeTypeDomain();

#if ARP_ABI_VERSION_MAJOR < 2
private: // deleted methods
    $(name.format.lastNamespacePart.format.escapeProjectName)Library(const $(name.format.lastNamespacePart.format.escapeProjectName)Library& arg) = delete;
    $(name.format.lastNamespacePart.format.escapeProjectName)Library& operator= (const $(name.format.lastNamespacePart.format.escapeProjectName)Library& arg) = delete;
#endif
private:  // fields
    TypeDomain typeDomain;
};

#if ARP_ABI_VERSION_MAJOR < 2
extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain);
#endif
///////////////////////////////////////////////////////////////////////////////
// inline methods of class $(name.format.lastNamespacePart.format.escapeProjectName)Library

$(namespace.format.end) // end of namespace $(namespace)
