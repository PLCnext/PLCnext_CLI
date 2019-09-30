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

    static const FieldDefinition Arp_Plc_Esm_Data_EsmData_EsmCount("EsmCount", offsetof(::Arp::Plc::Esm::Data::EsmData, EsmCount), DataType::UInt32, "", sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_EsmData_EsmInfos("EsmInfos", offsetof(::Arp::Plc::Esm::Data::EsmData, EsmInfos), DataType::Struct | DataType::Array, CTN<Arp::Plc::Esm::Data::EsmInfo>(), sizeof(Arp::Plc::Esm::Data::EsmInfo), alignof(Arp::Plc::Esm::Data::EsmInfo), { 2 }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_EsmInfo_TaskCount("TaskCount", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TaskCount), DataType::UInt32, "", sizeof(uint32), alignof(uint32), {  }, StandardAttribute::Retain | StandardAttribute::Opc);
    static const FieldDefinition Arp_Plc_Esm_Data_EsmInfo_TickCount("TickCount", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TickCount), DataType::UInt32, "", sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_EsmInfo_TickInterval("TickInterval", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TickInterval), DataType::UInt32, "", sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_EsmInfo_TaskInfos("TaskInfos", offsetof(::Arp::Plc::Esm::Data::EsmInfo, TaskInfos), DataType::Struct | DataType::Array, CTN<Arp::Plc::Esm::Data::TaskInfo>(), sizeof(Arp::Plc::Esm::Data::TaskInfo), alignof(Arp::Plc::Esm::Data::TaskInfo), { 16 }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_TaskInterval("TaskInterval", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskInterval), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_TaskPriority("TaskPriority", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskPriority), DataType::Int16, "", sizeof(int16), alignof(int16), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_TaskWatchdogTime("TaskWatchdogTime", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskWatchdogTime), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_LastExecutionTime("LastExecutionTime", offsetof(::Arp::Plc::Esm::Data::TaskInfo, LastExecutionTime), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_MaxExecutionTime("MaxExecutionTime", offsetof(::Arp::Plc::Esm::Data::TaskInfo, MaxExecutionTime), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_LastTaskActivationDelay("LastTaskActivationDelay", offsetof(::Arp::Plc::Esm::Data::TaskInfo, LastTaskActivationDelay), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_MaxTaskActivationDelay("MaxTaskActivationDelay", offsetof(::Arp::Plc::Esm::Data::TaskInfo, MaxTaskActivationDelay), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_ExecutionTimeThreshold("ExecutionTimeThreshold", offsetof(::Arp::Plc::Esm::Data::TaskInfo, ExecutionTimeThreshold), DataType::Int64, "", sizeof(int64), alignof(int64), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_ExecutionTimeThresholdCount("ExecutionTimeThresholdCount", offsetof(::Arp::Plc::Esm::Data::TaskInfo, ExecutionTimeThresholdCount), DataType::UInt32, "", sizeof(uint32), alignof(uint32), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_TaskName("TaskName", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskName), DataType::StaticString | DataType::Array, "", sizeof(StaticString<80>), alignof(StaticString<80>), { 2 }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_Data_TaskInfo_TaskName2("TaskName2", offsetof(::Arp::Plc::Esm::Data::TaskInfo, TaskName2), DataType::StaticString, "", sizeof(StaticString<100>), alignof(StaticString<100>), {  }, StandardAttribute::None);
    static const FieldDefinition Arp_Plc_Esm_MyComponent_MyProgram_exampleInput("exampleInput", offsetof(::Arp::Plc::Esm::MyComponent::MyProgram, exampleInput), DataType::Struct, CTN<Arp::Plc::Esm::Data::EsmData>(), sizeof(Arp::Plc::Esm::Data::EsmData), alignof(Arp::Plc::Esm::Data::EsmData), {  }, StandardAttribute::Input);
    
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
                        Arp_Plc_Esm_Data_EsmData_EsmCount,
                        Arp_Plc_Esm_Data_EsmData_EsmInfos,
                    }
                },
                {   // TypeDefinition: Arp::Plc::Esm::Data::EsmInfo
                    DataType::Struct, CTN<Arp::Plc::Esm::Data::EsmInfo>(), sizeof(::Arp::Plc::Esm::Data::EsmInfo), alignof(::Arp::Plc::Esm::Data::EsmInfo), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        Arp_Plc_Esm_Data_EsmInfo_TaskCount,
                        Arp_Plc_Esm_Data_EsmInfo_TickCount,
                        Arp_Plc_Esm_Data_EsmInfo_TickInterval,
                        Arp_Plc_Esm_Data_EsmInfo_TaskInfos,
                    }
                },
                {   // TypeDefinition: Arp::Plc::Esm::Data::TaskInfo
                    DataType::Struct, CTN<Arp::Plc::Esm::Data::TaskInfo>(), sizeof(::Arp::Plc::Esm::Data::TaskInfo), alignof(::Arp::Plc::Esm::Data::TaskInfo), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        Arp_Plc_Esm_Data_TaskInfo_TaskInterval,
                        Arp_Plc_Esm_Data_TaskInfo_TaskPriority,
                        Arp_Plc_Esm_Data_TaskInfo_TaskWatchdogTime,
                        Arp_Plc_Esm_Data_TaskInfo_LastExecutionTime,
                        Arp_Plc_Esm_Data_TaskInfo_MaxExecutionTime,
                        Arp_Plc_Esm_Data_TaskInfo_LastTaskActivationDelay,
                        Arp_Plc_Esm_Data_TaskInfo_MaxTaskActivationDelay,
                        Arp_Plc_Esm_Data_TaskInfo_ExecutionTimeThreshold,
                        Arp_Plc_Esm_Data_TaskInfo_ExecutionTimeThresholdCount,
                        Arp_Plc_Esm_Data_TaskInfo_TaskName,
                        Arp_Plc_Esm_Data_TaskInfo_TaskName2,
                    }
                },
                {   // ProgramDefinition: Arp::Plc::Esm::MyComponent::MyProgram
                    DataType::Program, CTN<Arp::Plc::Esm::MyComponent::MyProgram>(), sizeof(::Arp::Plc::Esm::MyComponent::MyProgram), alignof(::Arp::Plc::Esm::MyComponent::MyProgram), StandardAttribute::None,
                    {
                        // FieldDefinitions:
                        Arp_Plc_Esm_MyComponent_MyProgram_exampleInput,
                    }
                },
            }
            // End TypeDefinitions
        );
    }

}}} // end of namespace Arp.Plc.Esm

