#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramProviderBase.hpp"

namespace TooManyGeneratedFiles
{

using namespace Arp;
using namespace Arp::Plc::Commons::Esm;

//forwards
class TooManyGeneratedFilesComponent;

class TooManyGeneratedFilesComponentProgramProvider : public ProgramProviderBase
{

public:   // construction/destruction
    TooManyGeneratedFilesComponentProgramProvider(TooManyGeneratedFilesComponent& tooManyGeneratedFilesComponentArg);
    virtual ~TooManyGeneratedFilesComponentProgramProvider() = default;

public:   // IProgramProvider operations
    IProgram::Ptr CreateProgramInternal(const String& programName, const String& programType) override;

private:   // deleted methods
    TooManyGeneratedFilesComponentProgramProvider(const TooManyGeneratedFilesComponentProgramProvider& arg) = delete;
    TooManyGeneratedFilesComponentProgramProvider& operator=(const TooManyGeneratedFilesComponentProgramProvider& arg) = delete;

private: // fields
    TooManyGeneratedFilesComponent& tooManyGeneratedFilesComponent;
};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class TooManyGeneratedFilesComponentProgramProvider

inline TooManyGeneratedFilesComponentProgramProvider::TooManyGeneratedFilesComponentProgramProvider(TooManyGeneratedFilesComponent& tooManyGeneratedFilesComponentArg)
    : tooManyGeneratedFilesComponent(tooManyGeneratedFilesComponentArg)
{
}

} // end of namespace TooManyGeneratedFiles
