#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "Code/MyProgram.hpp"
#include "Code/EsmData.hpp"
#include "Code/EsmInfo.hpp"
#include "Code/TaskInfo.hpp"
#include "EsmLibrary.hpp"

namespace Arp { namespace Plc { namespace Esm
{

using namespace Arp::Plc::Commons::Meta;

    void EsmLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinitions
        (
            // Begin TypeDefinitions
            {
                {   // TypeDefinition: Arp::Plc::Esm::Data::EsmData
                    DataType::Struct, CTN<Arp::Plc::Esm::Data::EsmData>(), sizeof(::Arp::Plc::Esm::Data::EsmData), alignof(::Arp::Plc::Esm::Data::EsmData), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "EsmCount", offsetof(::Arp::Plc::Esm::Data::EsmData, EsmCount), DataType::UInt32, String::Empty, sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None },
                        { "EsmInfos", offsetof(::Arp::Plc::Esm::Data::EsmData, EsmInfos), DataType::Struct | DataType::Array, CTN<Arp::Plc::Esm::Data::EsmInfo>(), sizeof(Arp::Plc::Esm::Data::EsmInfo), alignof(Arp::Plc::Esm::Data::EsmInfo), { 2 }, StandardAttribute::None },
                    }
                },
                {   // TypeDefinition: Arp::Plc::Esm::Data::EsmInfo
                    DataType::Struct, CTN<Arp::Plc::Esm::Data::EsmInfo>(), sizeof(::Arp::Plc::Esm::Data::EsmInfo), alignof(::Arp::Plc::Esm::Data::EsmInfo), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "TaskCount", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TaskCount), DataType::UInt32, String::Empty, sizeof(uint32), alignof(uint32), {  }, StandardAttribute::Retain | StandardAttribute::Opc },
                        { "TickCount", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TickCount), DataType::UInt32, String::Empty, sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None },
                        { "TickInterval", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TickInterval), DataType::UInt32, String::Empty, sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None },
                        { "TaskInfos", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TaskInfos), DataType::Struct | DataType::Array, CTN<Arp::Plc::Esm::Data::TaskInfo>(), sizeof(Arp::Plc::Esm::Data::TaskInfo), alignof(Arp::Plc::Esm::Data::TaskInfo), { 16 }, StandardAttribute::None },
                    }
                },
                {   // TypeDefinition: Arp::Plc::Esm::Data::TaskInfo
                    DataType::Struct, CTN<Arp::Plc::Esm::Data::TaskInfo>(), sizeof(::Arp::Plc::Esm::Data::TaskInfo), alignof(::Arp::Plc::Esm::Data::TaskInfo), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "TaskInterval", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskInterval), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "TaskPriority", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskPriority), DataType::Int16, String::Empty, sizeof(int16), alignof(int16), {  }, StandardAttribute::None },
                        { "TaskWatchdogTime", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskWatchdogTime), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "LastExecutionTime", offsetof(::Arp::Plc::Esm::Data::TaskInfo, LastExecutionTime), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "MinExecutionTime", offsetof(::Arp::Plc::Esm::Data::TaskInfo, MinExecutionTime), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "MaxExecutionTime", offsetof(::Arp::Plc::Esm::Data::TaskInfo, MaxExecutionTime), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "LastTaskActivationDelay", offsetof(::Arp::Plc::Esm::Data::TaskInfo, LastTaskActivationDelay), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "MinTaskActivationDelay", offsetof(::Arp::Plc::Esm::Data::TaskInfo, MinTaskActivationDelay), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "MaxTaskActivationDelay", offsetof(::Arp::Plc::Esm::Data::TaskInfo, MaxTaskActivationDelay), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "ExecutionTimeThreshold", offsetof(::Arp::Plc::Esm::Data::TaskInfo, ExecutionTimeThreshold), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None },
                        { "ExecutionTimeThresholdCount", offsetof(::Arp::Plc::Esm::Data::TaskInfo, ExecutionTimeThresholdCount), DataType::UInt32, String::Empty, sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None },
                        { "TaskName", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskName), DataType::StaticString | DataType::Array, String::Empty, sizeof(StaticString<80>), alignof(StaticString<80>), { 2 }, StandardAttribute::None },
                        { "TaskName2", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskName2), DataType::StaticString, String::Empty, sizeof(StaticString<100>), alignof(StaticString<100>), {  }, StandardAttribute::None },
                    }
                },
                {   // ProgramDefinition: Arp::Plc::Esm::MyComponent::MyProgram
                    DataType::Program, CTN<Arp::Plc::Esm::MyComponent::MyProgram>(), sizeof(::Arp::Plc::Esm::MyComponent::MyProgram), alignof(::Arp::Plc::Esm::MyComponent::MyProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        { "exampleInput", offsetof(::Arp::Plc::Esm::MyComponent::MyProgram, exampleInput), DataType::Struct, CTN<Arp::Plc::Esm::Data::EsmData>(), sizeof(Arp::Plc::Esm::Data::EsmData), alignof(Arp::Plc::Esm::Data::EsmData), {  }, StandardAttribute::Input },
                    }
                },
            }
            // End TypeDefinitions
        );
    }

}}} // end of namespace Arp.Plc.Esm

