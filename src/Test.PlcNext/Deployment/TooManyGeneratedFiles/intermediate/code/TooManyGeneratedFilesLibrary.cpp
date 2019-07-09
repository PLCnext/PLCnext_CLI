#include "TooManyGeneratedFilesLibrary.hpp"
#include "Arp/System/Core/CommonTypeName.hxx"
#include "AccessComponent.hpp"
#include "TooManyGeneratedFilesComponent.hpp"

namespace TooManyGeneratedFiles
{

TooManyGeneratedFilesLibrary::TooManyGeneratedFilesLibrary(AppDomain& appDomain)
    : MetaLibraryBase(appDomain, ARP_VERSION_CURRENT, typeDomain)
    , typeDomain(CommonTypeName<TooManyGeneratedFilesLibrary>().GetNamespace())
{
    this->componentFactory.AddFactoryMethod(CommonTypeName<::TooManyGeneratedFiles::AccessComponent>(), &::TooManyGeneratedFiles::AccessComponent::Create);
    this->componentFactory.AddFactoryMethod(CommonTypeName<::TooManyGeneratedFiles::TooManyGeneratedFilesComponent>(), &::TooManyGeneratedFiles::TooManyGeneratedFilesComponent::Create);
    this->InitializeTypeDomain();
}

void TooManyGeneratedFilesLibrary::Main(AppDomain& appDomain)
{
    SingletonBase::CreateInstance(appDomain);
}

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain)
{
    TooManyGeneratedFilesLibrary::Main(appDomain);
    return  TooManyGeneratedFilesLibrary::GetInstance();
}

} // end of namespace TooManyGeneratedFiles
