#include "$(template.files.library.format.include)"
#if ARP_ABI_VERSION_MAJOR < 2
#include "Arp/System/Core/CommonTypeName.hxx"
#else
#include "Arp/Base/Core/CommonTypeName.hxx"
#include "$(name.format.lastNamespacePart.format.escapeProjectName)LibraryInfo.hpp"
#endif
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
$([foreach]component[in]related[of-type]basecomponent)
#include "$(component.template.files.component.format.include)"
$([end-foreach])

$(namespace.format.start)

#if ARP_ABI_VERSION_MAJOR < 2
$(name.format.lastNamespacePart.format.escapeProjectName)Library::$(name.format.lastNamespacePart.format.escapeProjectName)Library(AppDomain& appDomain)
    : MetaLibraryBase(appDomain, ARP_VERSION_CURRENT, typeDomain)
    , typeDomain(CommonTypeName<$(name.format.lastNamespacePart.format.escapeProjectName)Library>().GetNamespace())
#else
$(name.format.lastNamespacePart.format.escapeProjectName)Library::$(name.format.lastNamespacePart.format.escapeProjectName)Library()
    : MetaLibraryBase($(name.format.lastNamespacePart.format.escapeProjectName)LibraryVersion, typeDomain)
    , typeDomain(CommonTypeName<$(name.format.lastNamespacePart.format.escapeProjectName)Library>().GetNamespace())
#endif
{
#if ARP_ABI_VERSION_MAJOR < 2
$([foreach]component[in]related[of-type]basecomponent)
    this->componentFactory.AddFactoryMethod(CommonTypeName<::$(component.fullName)>(), &::$(component.fullName)::Create);
$([end-foreach])
#else
$([foreach]component[in]related[of-type]basecomponent) 
    this->AddComponentType<::$(component.fullName)>();
$([end-foreach])
#endif
    this->InitializeTypeDomain();
}

#if ARP_ABI_VERSION_MAJOR < 2
void $(name.format.lastNamespacePart.format.escapeProjectName)Library::Main(AppDomain& appDomain)
{
    SingletonBase::CreateInstance(appDomain);
}
#else
$(name.format.lastNamespacePart.format.escapeProjectName)Library& $(name.format.lastNamespacePart.format.escapeProjectName)Library::GetInstance()
{
    static $(name.format.lastNamespacePart.format.escapeProjectName)Library instance;
    return instance;
}
#endif


#if ARP_ABI_VERSION_MAJOR < 2
extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain)
{
    $(name.format.lastNamespacePart.format.escapeProjectName)Library::Main(appDomain);
    return  $(name.format.lastNamespacePart.format.escapeProjectName)Library::GetInstance();
}
$(namespace.format.end) // end of namespace $(namespace)
#else
$(namespace.format.end) // end of namespace $(namespace)
extern "C" ARP_EXPORT Arp::Base::Acf::Commons::ILibrary& $(name.format.escapeDotProjectName)_MainEntry()
{
    return  $(namespace.format.cppFullName)::$(name.format.lastNamespacePart.format.escapeProjectName)Library::GetInstance();
}
#endif

