///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Electronics GmbH
//
///////////////////////////////////////////////////////////////////////////////
#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Esm/Data/TaskInfo.hpp"

namespace Arp { namespace Plc { namespace Esm { namespace Data
{

class EsmInfo
{
public: // typedefs
    typedef std::shared_ptr<EsmInfo> Ptr;

public: // construction/destruction
    /// <summary>Constructs an <see cref="EsmInfo" /> instance.</summary>
    EsmInfo(void) = default;
    /// <summary>Copy constructor.</summary>
    EsmInfo(const EsmInfo& arg) = default;
    /// <summary>Assignment operator.</summary>
    EsmInfo& operator=(const EsmInfo& arg) = default;
    /// <summary>Destructs this instance and frees all resources.</summary>
    ~EsmInfo(void) = default;

public: // operators

public: // static operations

public: // setter/getter operations

public: // operations
    void IncrementTickCount(void);
    void ResetStatistics(void);

protected: // operations

private: // static methods

private: // methods

public: // fields
	//#attributes(Retain|Opc)
    uint32      TaskCount = 0;
    uint32      TickCount = 0;
    uint32      TickInterval = 0;
    TaskInfo    TaskInfos[16];

private: // static fields

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class EsmInfo

}}}} // end of namespace Arp::Plc::Esm::Data
