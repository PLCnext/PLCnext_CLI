///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Electronics GmbH
//
///////////////////////////////////////////////////////////////////////////////
#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/Data/EsmInfo.hpp"
#include <memory>

namespace Arp { namespace Plc { namespace Esm { namespace Internal
{

//forwards
class EsmDomain;

}}}} // end of namespace Arp::Plc::Esm::Internal

namespace Arp { namespace Plc { namespace Esm { namespace Data
{

using namespace Arp::Plc::Esm::Internal;

class EsmData
{
public: // typedefs
    typedef std::shared_ptr<EsmData> Ptr;
    typedef std::shared_ptr<const EsmData> ConstPtr;

public: // construction/destruction
    /// <summary>Constructs an <see cref="EsmData" /> instance.</summary>
    EsmData(void) = default;
    /// <summary>Copy constructor.</summary>
    EsmData(const EsmData& arg) = default;
    /// <summary>Assignment operator.</summary>
    EsmData& operator=(const EsmData& arg) = default;
    /// <summary>Destructs this instance and frees all resources.</summary>
    ~EsmData(void) = default;

public: // operators

public: // static operations

public: // setter/getter operations

public: // operations
    void Connect(EsmDomain& esmDomain);

protected: // operations

private: // static methods

private: // methods

public: // fields
    uint32  EsmCount = 0;
    EsmInfo EsmInfos[2];

private: // static fields
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ComponentData

}}}} // end of namespace Arp::Plc::Esm::Data
