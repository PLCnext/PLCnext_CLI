#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramProviderBase.hpp"

$(namespace.format.start)

using namespace Arp;
using namespace Arp::Plc::Commons::Esm;

//forwards
class $(name);

class $(name)ProgramProvider : public ProgramProviderBase
{

public:   // construction/destruction
    $(name)ProgramProvider($(name)& \l$(name)Arg);
    virtual ~$(name)ProgramProvider() = default;

public:   // IProgramProvider operations
    IProgram::Ptr CreateProgramInternal(const String& programName, const String& programType) override;

private:   // deleted methods
    $(name)ProgramProvider(const $(name)ProgramProvider& arg) = delete;
    $(name)ProgramProvider& operator=(const $(name)ProgramProvider& arg) = delete;

private: // fields
    $(name)& \l$(name);
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class $(name)ProgramProvider

inline $(name)ProgramProvider::$(name)ProgramProvider($(name)& \l$(name)Arg)
    : \l$(name)(\l$(name)Arg)
{
}

$(namespace.format.end) // end of namespace $(namespace)
