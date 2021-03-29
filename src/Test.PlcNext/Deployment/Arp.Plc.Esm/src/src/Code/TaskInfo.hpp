#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/PlcTypes.h"

namespace Arp { namespace Plc { namespace Esm { namespace Data
{

using namespace Arp::Plc::Commons;
using namespace Arp::Plc::Commons::Gds;

class TaskInfo
{
public: // typedefs
    typedef std::shared_ptr<TaskInfo> Ptr;

public: // construction/destruction
    /// <summary>Constructs an <see cref="TaskInfo" /> instance.</summary>
    TaskInfo(void) = default;
    /// <summary>Copy constructor.</summary>
    TaskInfo(const TaskInfo& arg) = default;
    /// <summary>Assignment operator.</summary>
    TaskInfo& operator=(const TaskInfo& arg) = default;
    /// <summary>Destructs this instance and frees all resources.</summary>
    ~TaskInfo(void) = default;

public: // operators

public: // static operations

public: // setter/getter operations
    void SetLastTaskActivationDelay(int64 lastTaskActivationDelay);
    void SetLastExecutionTime(int64 lastExecutionTime);

public: // operations
    void ResetStatistics(void);

protected: // operations

private: // static methods

private: // methods

public: // fields
    using value_type = int32;
    
    int64               TaskInterval = 0;
    int16               TaskPriority = 0;
    int64               TaskWatchdogTime = 0;
    int64               LastExecutionTime = 0;
    int64               MinExecutionTime = std::numeric_limits<int64>::max();
    int64               MaxExecutionTime = 0;
    int64               LastTaskActivationDelay = 0;
    int64               MinTaskActivationDelay = std::numeric_limits<int64>::max();
    int64               MaxTaskActivationDelay = 0;
    int64               ExecutionTimeThreshold = 0;
    uint32              ExecutionTimeThresholdCount = 0;
    StaticString<80>    TaskName[2] = { "HollaDieWaldfee", "Das wird lustig!" };
    StaticString<100>    TaskName2;

private: // static fields

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class TaskInfo

}}}} // end of namespace Arp::Plc::Esm::Data
