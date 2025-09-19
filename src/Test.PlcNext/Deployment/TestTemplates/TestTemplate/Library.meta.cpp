#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
$([no-duplicate-lines])
$([foreach]program[in]hierarchy[of-type]program)
#include "$(program.template.files.program.format.include)"
$([end-foreach])
$([foreach]struct[in]portAndTypeInformationStructs)
#include "$(struct.file.format.include)"
$([end-foreach])
$([foreach]enum[in]portAndTypeInformationEnums)
#include "$(enum.file.format.include)"
$([end-foreach])
#include "$(template.files.library.format.include)"
$([end-no-duplicate-lines])

$(namespace.format.start)

using namespace Arp::Plc::Commons::Meta;

    void $(name.format.lastNamespacePart.format.escapeProjectName)Library::InitializeTypeDomain()
    {
$([foreach]struct[in]portAndTypeInformationStructs)$([if]$(struct.fullName.containsltgt))
        using $(struct.fullName.format.escapeTypenameForOffset) = $(struct.fullName);
$([end-if])$([end-foreach])
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
$([foreach]struct[in]portAndTypeInformationStructs)
                {   // TypeDefinition: $(struct.fullName)
                    DataType::Struct, CTN<$(struct.fullName)>(), sizeof(::$(struct.fullName)), alignof(::$(struct.fullName)), $(struct.attributes.format.standardAttributes),
                    {
                        // FieldDefinitions:
$([foreach]field[in]struct.fields)
                        { "$(field.name)", offsetof($([if]$(struct.fullName.containsltgt.negate))::$([end-if])$(struct.fullName.format.escapeTypenameForOffset), $(field.fieldName)), $(field.format.arpDataType), $(field.dataType.format.ctn), sizeof($(field.dataType.fullName)), alignof($(field.dataType.fullName)), { $(field.multiplicity.format.multiplicityConstantReplace.format.metaDimensions) }, $(field.attributes.format.standardAttributes) },
$([end-foreach])
                    }
                },
$([end-foreach])
$([foreach]program[in]hierarchy[of-type]program)
                {   // ProgramDefinition: $(program.fullName)
                    DataType::Program, CTN<$(program.fullName)>(), sizeof(::$(program.fullName)), alignof(::$(program.fullName)), StandardAttribute::None,
                    {
                        // FieldDefinitions:
$([foreach]port[in]program.ports)
                        { "$(port.name)", offsetof(::$(program.fullName), $(port.fieldName)), $(port.format.arpDataType), $(port.dataType.format.ctn), sizeof($(port.dataType.fullName)), alignof($(port.dataType.fullName)), { $(port.multiplicity.format.multiplicityConstantReplace.format.metaDimensions) }, $(port.attributes.format.standardAttributes) },
$([end-foreach])
                    }
                },
$([end-foreach])
$([if]$(minTargetVersion) < 20.3)
$([foreach]enum[in]portEnums)
                {
                    DataType::Enum | DataType::$(enum.baseType.name.format.knownDataTypes.format.stringConstantReplace.format.convertStaticString), CTN<$(enum.fullName)>(), sizeof($(enum.fullName)), alignof($(enum.fullName)), $(enum.attributes.format.standardAttributes), 
                    {
                        // FieldDefinitions:
$([foreach]symbol[in]enum.symbols)
                        { "$(symbol.name)", 0, DataType::Enum | DataType::$(enum.baseType.name.format.knownDataTypes.format.stringConstantReplace.format.convertStaticString), String::Empty, sizeof($(enum.fullName)), alignof($(enum.fullName)), {}, StandardAttribute::None },
$([end-foreach])
                    }
                },
$([end-foreach])
$([end-if])
            }
            // End TypeDefinitions
        );
$([if]$(minTargetVersion) >= 20.3)
$([foreach]enum[in]portAndTypeInformationEnums)
        {
            TypeDefinition typeDefinition{DataType::Enum | DataType::$(enum.baseType.name.format.knownDataTypes.format.stringConstantReplace.format.convertStaticString), CTN<$(enum.fullName)>(), sizeof($(enum.fullName)), alignof($(enum.fullName)), $(enum.attributes.format.standardAttributes), {}};
$([foreach]symbol[in]enum.symbols)
            {
                FieldDefinition field{"$(symbol.name)", 0, DataType::Enum | DataType::$(enum.baseType.name.format.knownDataTypes.format.stringConstantReplace.format.convertStaticString), String::Empty, sizeof($(enum.fullName)), alignof($(enum.fullName)), {}, StandardAttribute::None};
                field.GetChildTypeInfo().AddCustomAttribute("Value", (RscVariant<256>) static_cast<std::underlying_type<$(enum.fullName)>::type>($(enum.fullName)::$(symbol.name)));
                typeDefinition.AddField(std::move(field));
            }
$([end-foreach])
            typeDomain.AddTypeDefinition(std::move(typeDefinition));
        }
$([end-foreach])
$([end-if])
    }

$(namespace.format.end) // end of namespace $(namespace)

