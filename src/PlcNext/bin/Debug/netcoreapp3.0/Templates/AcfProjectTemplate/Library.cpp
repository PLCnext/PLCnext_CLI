#include "$(template.files.library.format.include)"
#include "Arp/System/Core/CommonTypeName.hxx"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
$([foreach]component[in]related[of-type]basecomponent)
#include "$(component.template.files.component.format.include)"
$([end-foreach])

$(namespace.format.start)

$(name.format.lastNamespacePart.format.escapeProjectName)Library::$(name.format.lastNamespacePart.format.escapeProjectName)Library(AppDomain& appDomain)
    : MetaLibraryBase(appDomain, ARP_VERSION_CURRENT, typeDomain)
    , typeDomain(CommonTypeName<$(name.format.lastNamespacePart.format.escapeProjectName)Library>().GetNamespace())
{
$([foreach]component[in]related[of-type]basecomponent)
    this->componentFactory.AddFactoryMethod(TypeName<::$(component.fullName)>(), &::$(component.fullName)::Create);
$([end-foreach])
    this->InitializeTypeDomain();
}

void $(name.format.lastNamespacePart.format.escapeProjectName)Library::Main(AppDomain& appDomain)
{
    SingletonBase::CreateInstance(appDomain);
}

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain)
{
    $(name.format.lastNamespacePart.format.escapeProjectName)Library::Main(appDomain);
    return  $(name.format.lastNamespacePart.format.escapeProjectName)Library::GetInstance();
}

$(namespace.format.end) // end of namespace $(namespace)
