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

using namespace Arp::Plc::Commons::Meta;

namespace { // anonymous namespace
$([foreach]struct[in]portAndTypeInformationStructs)$([if]$(struct.fullName.containsltgt))
    using $(struct.fullName.format.escapeTypenameForOffset) = $(struct.fullName);
$([end-if])$([end-foreach])
$([foreach]struct[in]portAndTypeInformationStructs)
$([foreach]chunk[in]struct.fields[split]8)
    void AddFieldDefinitions_$(struct.name.format.escapeTypenameForOffset)_$(chunk.start)_$(chunk.end)(TypeDefinition& typeDefinition)
    {
$([foreach]field[in]chunk)
        typeDefinition.AddField({ "$(field.name)", offsetof($([if]$(struct.fullName.containsltgt.negate))::$([end-if])$(struct.fullName.format.escapeTypenameForOffset), $(field.fieldName)), $(field.format.arpDataType), $(field.dataType.format.ctn), sizeof($(field.dataType.fullName)), alignof($(field.dataType.fullName)), { $(field.multiplicity.format.multiplicityConstantReplace.format.metaDimensions) }, $(field.attributes.format.standardAttributes) });
$([end-foreach])
    }

$([end-foreach])

    void AddFieldDefinitions_$(struct.name.format.escapeTypenameForOffset)(TypeDefinition& typeDefinition)
    {
$([foreach]chunk[in]struct.fields[split]8)
    AddFieldDefinitions_$(struct.name.format.escapeTypenameForOffset)_$(chunk.start)_$(chunk.end)(typeDefinition);
$([end-foreach])
    }

    TypeDefinition GetTypeDefinition$(struct.name.format.escapeTypenameForOffset)()
    {
        TypeDefinition result{ DataType::Struct, $(struct.format.ctn), sizeof(::$(struct.fullName)), alignof(::$(struct.fullName)), $(struct.attributes.format.standardAttributes), {} };
        result.ReserveFields($(struct.fields.count));
        AddFieldDefinitions_$(struct.name.format.escapeTypenameForOffset)(result);
        return result;
    }
$([end-foreach])

$([foreach]program[in]hierarchy[of-type]program)
$([foreach]chunk[in]program.ports[split]8)
    void AddFieldDefinitions_$(program.name)_$(chunk.start)_$(chunk.end)(TypeDefinition& typeDefinition)
    {
$([foreach]port[in]chunk)
        typeDefinition.AddField({ "$(port.name)", offsetof(::$(program.fullName), $(port.fieldName)), $(port.format.arpDataType), $(port.dataType.format.ctn), sizeof($(port.dataType.fullName)), alignof($(port.dataType.fullName)), { $(port.multiplicity.format.multiplicityConstantReplace.format.metaDimensions) }, $(port.attributes.format.standardAttributes) });
$([end-foreach])
    }

$([end-foreach])

    void AddFieldDefinitions_$(program.name)(TypeDefinition& typeDefinition)
    {
$([foreach]chunk[in]program.ports[split]8)
    AddFieldDefinitions_$(program.name)_$(chunk.start)_$(chunk.end)(typeDefinition);
$([end-foreach])
    }

    TypeDefinition GetTypeDefinition$(program.name)()
    {
        TypeDefinition result{ DataType::Program, $(program.format.ctn), sizeof(::$(program.fullName)), alignof(::$(program.fullName)), StandardAttribute::None, {} };
        result.ReserveFields($(program.ports.count));
        AddFieldDefinitions_$(program.name)(result);
        return result;
    }
$([end-foreach])
        

} // anonymous namespace


$(namespace.format.start)

    void $(name.format.lastNamespacePart.format.escapeProjectName)Library::InitializeTypeDomain()
    {
$([foreach]struct[in]portAndTypeInformationStructs)
        this->typeDomain.AddTypeDefinition(GetTypeDefinition$(struct.name.format.escapeTypenameForOffset)());
$([end-foreach])

$([foreach]program[in]hierarchy[of-type]program)
        this->typeDomain.AddTypeDefinition(GetTypeDefinition$(program.name)());
$([end-foreach])
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
    }

$(namespace.format.end) // end of namespace $(namespace)
