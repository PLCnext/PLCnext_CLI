#include "TooManyGeneratedFilesComponentProgramProvider.hpp"
#include "TooManyGeneratedFilesProgram.hpp"

namespace TooManyGeneratedFiles
{

IProgram::Ptr TooManyGeneratedFilesComponentProgramProvider::CreateProgramInternal(const String& programName, const String& programType)
{
    if (programType == "TooManyGeneratedFilesProgram")
    { 
        return std::make_shared<::TooManyGeneratedFiles::TooManyGeneratedFilesProgram>(this->tooManyGeneratedFilesComponent, programName);
    }

    // else unknown program
    return nullptr;
}

} // end of namespace TooManyGeneratedFiles
