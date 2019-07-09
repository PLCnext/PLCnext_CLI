#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
$([no-duplicate-lines])
$([foreach]struct[in]portStructs)
#include "$(struct.file.format.include)"
$([end-foreach])
#include "$(template.files.library.format.include)"
$([end-no-duplicate-lines])

$(namespace.format.start)

using namespace Arp::Plc::Commons::Meta;
    
    void $(name.format.lastNamespacePart.format.escapeProjectName)Library::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
$([foreach]struct[in]portStructs)
                {   // TypeDefinition: $(struct.fullName)
                    DataType::Struct, $(struct.format.ctn), sizeof(::$(struct.fullName)), alignof(::$(struct.fullName)), $(struct.attributes.format.standardAttributes),
                    {
                        // FieldDefinitions:
$([foreach]field[in]struct.fields)
                        { "$(field.name)", offsetof(::$(struct.fullName), $(field.fieldName)), $(field.format.arpDataType), $(field.dataType.format.ctn), sizeof($(field.dataType.fullName)), alignof($(field.dataType.fullName)), { $(field.multiplicity) }, $(field.attributes.format.standardAttributes) },
$([end-foreach])
                    }
                },
$([end-foreach])
            }
            // End TypeDefinitions
        );
    }

$(namespace.format.end) // end of namespace $(namespace)

