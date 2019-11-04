#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramProviderBase.hpp"

namespace RNS_ST_Update_Proj_Targets_2237
{

using namespace Arp;
using namespace Arp::Plc::Commons::Esm;

//forwards
class Component1;

class Component1ProgramProvider : public ProgramProviderBase
{

public:   // construction/destruction
    Component1ProgramProvider(Component1& component1Arg);
    virtual ~Component1ProgramProvider() = default;

public:   // IProgramProvider operations
    IProgram::Ptr CreateProgramInternal(const String& programName, const String& programType) override;

private:   // deleted methods
    Component1ProgramProvider(const Component1ProgramProvider& arg) = delete;
    Component1ProgramProvider& operator=(const Component1ProgramProvider& arg) = delete;

private: // fields
    Component1& component1;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class Component1ProgramProvider

inline Component1ProgramProvider::Component1ProgramProvider(Component1& component1Arg)
    : component1(component1Arg)
{
}

} // end of namespace RNS_ST_Update_Proj_Targets_2237
