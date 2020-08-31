#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "MyProgram.hpp"
#include "ProgramWithArrayInitializedPortLibrary.hpp"

namespace ProgramWithArrayInitializedPort
{

using namespace Arp::Plc::Commons::Meta;

    void ProgramWithArrayInitializedPortLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // ProgramDefinition: ProgramWithArrayInitializedPort::MyComponent::MyProgram
                    DataType::Program, CTN<ProgramWithArrayInitializedPort::MyComponent::MyProgram>(), sizeof(::ProgramWithArrayInitializedPort::MyComponent::MyProgram), alignof(::ProgramWithArrayInitializedPort::MyComponent::MyProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "intArray", offsetof(::ProgramWithArrayInitializedPort::MyComponent::MyProgram, intArray), DataType::Int16 | DataType::Array, String::Empty, sizeof(int16), alignof(int16), { 12 }, StandardAttribute::Input | StandardAttribute::Retain },
                        { "Var_ARRAY_UDINT1", offsetof(::ProgramWithArrayInitializedPort::MyComponent::MyProgram, Var_ARRAY_UDINT1), DataType::UInt32 | DataType::Array, String::Empty, sizeof(uint32), alignof(uint32), { {3},{2} }, StandardAttribute::Retain },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

} // end of namespace ProgramWithArrayInitializedPort

