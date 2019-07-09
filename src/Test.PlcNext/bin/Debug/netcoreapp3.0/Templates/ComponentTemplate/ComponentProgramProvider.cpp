#include "$(template.files.programProvider.format.include)"
$([foreach]program[in]related[of-type]program)
#include "$(program.template.files.program.format.include)"
$([end-foreach])

$(namespace.format.start)

IProgram::Ptr $(name)ProgramProvider::CreateProgramInternal(const String& programName, const String& programType)
{
$([foreach]program[in]related[of-type]program)
    if (programType == "$(program.name)")
    { 
        return std::make_shared<::$(program.fullName)>(this->\l$(name), programName);
    }
$([end-foreach])

    // else unknown program
    return nullptr;
}

$(namespace.format.end) // end of namespace $(namespace)
