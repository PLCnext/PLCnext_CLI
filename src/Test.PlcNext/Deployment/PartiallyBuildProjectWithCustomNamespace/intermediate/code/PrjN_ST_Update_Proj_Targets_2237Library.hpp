#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Core/AppDomain.hpp"
#include "Arp/System/Core/Singleton.hxx"
#include "Arp/System/Core/Library.h"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeDomain.hpp"

namespace RNS_ST_Update_Proj_Targets_2237
{

using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Meta;
using namespace Arp::Plc::Commons::Meta::TypeSystem;

class PrjN_ST_Update_Proj_Targets_2237Library : public MetaLibraryBase, public Singleton<PrjN_ST_Update_Proj_Targets_2237Library>
{
public: // typedefs
    typedef Singleton<PrjN_ST_Update_Proj_Targets_2237Library> SingletonBase;

public: // construction/destruction
    PrjN_ST_Update_Proj_Targets_2237Library(AppDomain& appDomain);
    virtual ~PrjN_ST_Update_Proj_Targets_2237Library() = default;

public: // static operations (called through reflection)
    static void Main(AppDomain& appDomain);

private: // methods
    void InitializeTypeDomain();

private: // deleted methods
    PrjN_ST_Update_Proj_Targets_2237Library(const PrjN_ST_Update_Proj_Targets_2237Library& arg) = delete;
    PrjN_ST_Update_Proj_Targets_2237Library& operator= (const PrjN_ST_Update_Proj_Targets_2237Library& arg) = delete;

private:  // fields
    TypeDomain typeDomain;
};

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain);

///////////////////////////////////////////////////////////////////////////////
// inline methods of class PrjN_ST_Update_Proj_Targets_2237Library

} // end of namespace RNS_ST_Update_Proj_Targets_2237
