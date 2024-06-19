#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Core/AppDomain.hpp"
#include "Arp/System/Core/Singleton.hxx"
#include "Arp/System/Core/Library.h"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeDomain.hpp"

namespace ProjectWithConfigFile
{

using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Meta;
using namespace Arp::Plc::Commons::Meta::TypeSystem;

class ProjectWithConfigFileLibrary : public MetaLibraryBase, public Singleton<ProjectWithConfigFileLibrary>
{
public: // typedefs
    typedef Singleton<ProjectWithConfigFileLibrary> SingletonBase;

public: // construction/destruction
    ProjectWithConfigFileLibrary(AppDomain& appDomain);
    virtual ~ProjectWithConfigFileLibrary() = default;

public: // static operations (called through reflection)
    static void Main(AppDomain& appDomain);

private: // methods
    void InitializeTypeDomain();

private: // deleted methods
    ProjectWithConfigFileLibrary(const ProjectWithConfigFileLibrary& arg) = delete;
    ProjectWithConfigFileLibrary& operator= (const ProjectWithConfigFileLibrary& arg) = delete;

private:  // fields
    TypeDomain typeDomain;
};

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain);

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProjectWithConfigFileLibrary

} // end of namespace ProjectWithConfigFile
