#include "$(template.files.component.format.include)"

$(namespace.format.start)

void $(name)::RegisterComponentPorts()
{
$([foreach]port[in]ports)
    this->dataInfoProvider.AddRoot("$(port.name)", this->$(port.fieldName));
$([end-foreach])
}

$(namespace.format.end) // end of namespace $(namespace)