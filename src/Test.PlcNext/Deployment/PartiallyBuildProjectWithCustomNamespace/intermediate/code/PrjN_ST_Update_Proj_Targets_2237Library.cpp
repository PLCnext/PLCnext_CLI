#include "PrjN_ST_Update_Proj_Targets_2237Library.hpp"
#include "Arp/System/Core/CommonTypeName.hxx"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "Component1.hpp"

namespace RNS_ST_Update_Proj_Targets_2237
{

PrjN_ST_Update_Proj_Targets_2237Library::PrjN_ST_Update_Proj_Targets_2237Library(AppDomain& appDomain)
    : MetaLibraryBase(appDomain, ARP_VERSION_CURRENT, typeDomain)
    , typeDomain(CommonTypeName<PrjN_ST_Update_Proj_Targets_2237Library>().GetNamespace())
{
    this->componentFactory.AddFactoryMethod(CommonTypeName<::RNS_ST_Update_Proj_Targets_2237::Component1>(), &::RNS_ST_Update_Proj_Targets_2237::Component1::Create);
    this->InitializeTypeDomain();
}

void PrjN_ST_Update_Proj_Targets_2237Library::Main(AppDomain& appDomain)
{
    SingletonBase::CreateInstance(appDomain);
}

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain)
{
    PrjN_ST_Update_Proj_Targets_2237Library::Main(appDomain);
    return  PrjN_ST_Update_Proj_Targets_2237Library::GetInstance();
}

} // end of namespace RNS_ST_Update_Proj_Targets_2237
