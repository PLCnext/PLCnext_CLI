#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Core/AppDomain.hpp"
#include "Arp/System/Core/Singleton.hxx"
#include "Arp/System/Core/Library.h"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeDomain.hpp"

$(namespace.format.start)

using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Meta;
using namespace Arp::Plc::Commons::Meta::TypeSystem;

class $(name.format.lastNamespacePart.format.escapeProjectName)Library : public MetaLibraryBase, public Singleton<$(name.format.lastNamespacePart.format.escapeProjectName)Library>
{
public: // typedefs
    typedef Singleton<$(name.format.lastNamespacePart.format.escapeProjectName)Library> SingletonBase;

public: // construction/destruction
    $(name.format.lastNamespacePart.format.escapeProjectName)Library(AppDomain& appDomain);
    virtual ~$(name.format.lastNamespacePart.format.escapeProjectName)Library() = default;

public: // static operations (called through reflection)
    static void Main(AppDomain& appDomain);

private: // methods
    void InitializeTypeDomain();

private: // deleted methods
    $(name.format.lastNamespacePart.format.escapeProjectName)Library(const $(name.format.lastNamespacePart.format.escapeProjectName)Library& arg) = delete;
    $(name.format.lastNamespacePart.format.escapeProjectName)Library& operator= (const $(name.format.lastNamespacePart.format.escapeProjectName)Library& arg) = delete;

private:  // fields
    TypeDomain typeDomain;
};

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain);

$(namespace.format.end) // end of namespace $(namespace)
