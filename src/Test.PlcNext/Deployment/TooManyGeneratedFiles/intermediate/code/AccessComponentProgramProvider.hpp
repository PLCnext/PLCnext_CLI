#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramProviderBase.hpp"

namespace TooManyGeneratedFiles
{

using namespace Arp;
using namespace Arp::Plc::Commons::Esm;

//forwards
class AccessComponent;

class AccessComponentProgramProvider : public ProgramProviderBase
{

public:   // construction/destruction
    AccessComponentProgramProvider(AccessComponent& accessComponentArg);
    virtual ~AccessComponentProgramProvider() = default;

public:   // IProgramProvider operations
    IProgram::Ptr CreateProgramInternal(const String& programName, const String& programType) override;

private:   // deleted methods
    AccessComponentProgramProvider(const AccessComponentProgramProvider& arg) = delete;
    AccessComponentProgramProvider& operator=(const AccessComponentProgramProvider& arg) = delete;

private: // fields
    AccessComponent& accessComponent;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class AccessComponentProgramProvider

inline AccessComponentProgramProvider::AccessComponentProgramProvider(AccessComponent& accessComponentArg)
    : accessComponent(accessComponentArg)
{
}

} // end of namespace TooManyGeneratedFiles
