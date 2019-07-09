#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/System/Core/AppDomain.hpp"
#include "Arp/System/Core/Singleton.hxx"
#include "Arp/System/Core/Library.h"
#include "Arp/Plc/Commons/Meta/MetaLibraryBase.hpp"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeDomain.hpp"

namespace TooManyGeneratedFiles
{

using namespace Arp::System::Acf;
using namespace Arp::Plc::Commons::Meta;
using namespace Arp::Plc::Commons::Meta::TypeSystem;

class TooManyGeneratedFilesLibrary : public MetaLibraryBase, public Singleton<TooManyGeneratedFilesLibrary>
{
public: // typedefs
    typedef Singleton<TooManyGeneratedFilesLibrary> SingletonBase;

public: // construction/destruction
    TooManyGeneratedFilesLibrary(AppDomain& appDomain);
    virtual ~TooManyGeneratedFilesLibrary() = default;

public: // static operations (called through reflection)
    static void Main(AppDomain& appDomain);

private: // methods
    void InitializeTypeDomain();

private: // deleted methods
    TooManyGeneratedFilesLibrary(const TooManyGeneratedFilesLibrary& arg) = delete;
    TooManyGeneratedFilesLibrary& operator= (const TooManyGeneratedFilesLibrary& arg) = delete;

private:  // fields
    TypeDomain typeDomain;
};

extern "C" ARP_CXX_SYMBOL_EXPORT ILibrary& ArpDynamicLibraryMain(AppDomain& appDomain);

///////////////////////////////////////////////////////////////////////////////
// inline methods of class TooManyGeneratedFilesLibrary

} // end of namespace TooManyGeneratedFiles
