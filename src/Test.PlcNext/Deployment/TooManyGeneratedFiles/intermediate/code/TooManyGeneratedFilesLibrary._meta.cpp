#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "TooManyGeneratedFilesProgram.hpp"
#include "TooManyGeneratedFilesLibrary.hpp"

namespace TooManyGeneratedFiles
{

using namespace Arp::Plc::Commons::Meta;

    void TooManyGeneratedFilesLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // ProgramDefinition: TooManyGeneratedFiles::TooManyGeneratedFilesProgram
                    DataType::Program, CTN<TooManyGeneratedFiles::TooManyGeneratedFilesProgram>(), sizeof(::TooManyGeneratedFiles::TooManyGeneratedFilesProgram), alignof(::TooManyGeneratedFiles::TooManyGeneratedFilesProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace TooManyGeneratedFiles

