#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "CppProgram11.hpp"
#include "PrjN_ST_Update_Proj_Targets_2237Library.hpp"

namespace RNS_ST_Update_Proj_Targets_2237
{

using namespace Arp::Plc::Commons::Meta;

    
    void PrjN_ST_Update_Proj_Targets_2237Library::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // ProgramDefinition: RNS_ST_Update_Proj_Targets_2237::CppProgram11
                    DataType::Program, CTN<RNS_ST_Update_Proj_Targets_2237::CppProgram11>(), sizeof(::RNS_ST_Update_Proj_Targets_2237::CppProgram11), alignof(::RNS_ST_Update_Proj_Targets_2237::CppProgram11), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace RNS_ST_Update_Proj_Targets_2237

