#include "ProjectWithConfigFileLibrary.hpp"
#include "Arp/System/Core/CommonTypeName.hxx"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "ProjectWithConfigFileComponent.hpp"

namespace ProjectWithConfigFile
{

ProjectWithConfigFileLibrary::ProjectWithConfigFileLibrary(AppDomain& appDomain)
    : MetaLibraryBase(appDomain, ARP_VERSION_CURRENT, typeDomain)
    , typeDomain(CommonTypeName<ProjectWithConfigFileLibrary>().GetNamespace())
{
    this->componentFactory.AddFactoryMethod(CommonTypeName<::ProjectWithConfigFile::ProjectWithConfigFileComponent>(), &::ProjectWithConfigFile::ProjectWithConfigFileComponent::Create);
    this->InitializeTypeDomain();
}

void ProjectWithConfigFileLibrary::Main(AppDomain& appDomain)
{
    SingletonBase::CreateInstance(appDomain);
}

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain)
{
    ProjectWithConfigFileLibrary::Main(appDomain);
    return  ProjectWithConfigFileLibrary::GetInstance();
}

} // end of namespace ProjectWithConfigFile
