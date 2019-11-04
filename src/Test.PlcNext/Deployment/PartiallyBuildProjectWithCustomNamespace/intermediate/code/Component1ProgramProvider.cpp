#include "Component1ProgramProvider.hpp"
#include "CppProgram11.hpp"

namespace RNS_ST_Update_Proj_Targets_2237
{

IProgram::Ptr Component1ProgramProvider::CreateProgramInternal(const String& programName, const String& programType)
{
    if (programType == "CppProgram11")
    { 
        return std::make_shared<::RNS_ST_Update_Proj_Targets_2237::CppProgram11>(this->component1, programName);
    }

    // else unknown program
    return nullptr;
}

} // end of namespace RNS_ST_Update_Proj_Targets_2237
