#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Meta/TypeSystem/TypeSystem.h"
#include "StructWith1000FieldsProgram.hpp"
#include "StructWith1000FieldsLibrary.hpp"

using namespace Arp::Plc::Commons::Meta;

namespace { // anonymous namespace
    void AddFieldDefinitions_Sample0_0_7(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var0", offsetof(::StructWith1000Fields::Sample0, var0), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var1", offsetof(::StructWith1000Fields::Sample0, var1), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var2", offsetof(::StructWith1000Fields::Sample0, var2), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var3", offsetof(::StructWith1000Fields::Sample0, var3), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var4", offsetof(::StructWith1000Fields::Sample0, var4), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var5", offsetof(::StructWith1000Fields::Sample0, var5), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var6", offsetof(::StructWith1000Fields::Sample0, var6), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var7", offsetof(::StructWith1000Fields::Sample0, var7), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_8_15(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var8", offsetof(::StructWith1000Fields::Sample0, var8), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var9", offsetof(::StructWith1000Fields::Sample0, var9), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var10", offsetof(::StructWith1000Fields::Sample0, var10), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var11", offsetof(::StructWith1000Fields::Sample0, var11), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var12", offsetof(::StructWith1000Fields::Sample0, var12), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var13", offsetof(::StructWith1000Fields::Sample0, var13), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var14", offsetof(::StructWith1000Fields::Sample0, var14), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var15", offsetof(::StructWith1000Fields::Sample0, var15), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_16_23(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var16", offsetof(::StructWith1000Fields::Sample0, var16), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var17", offsetof(::StructWith1000Fields::Sample0, var17), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var18", offsetof(::StructWith1000Fields::Sample0, var18), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var19", offsetof(::StructWith1000Fields::Sample0, var19), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var20", offsetof(::StructWith1000Fields::Sample0, var20), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var21", offsetof(::StructWith1000Fields::Sample0, var21), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var22", offsetof(::StructWith1000Fields::Sample0, var22), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var23", offsetof(::StructWith1000Fields::Sample0, var23), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_24_31(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var24", offsetof(::StructWith1000Fields::Sample0, var24), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var25", offsetof(::StructWith1000Fields::Sample0, var25), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var26", offsetof(::StructWith1000Fields::Sample0, var26), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var27", offsetof(::StructWith1000Fields::Sample0, var27), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var28", offsetof(::StructWith1000Fields::Sample0, var28), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var29", offsetof(::StructWith1000Fields::Sample0, var29), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var30", offsetof(::StructWith1000Fields::Sample0, var30), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var31", offsetof(::StructWith1000Fields::Sample0, var31), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_32_39(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var32", offsetof(::StructWith1000Fields::Sample0, var32), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var33", offsetof(::StructWith1000Fields::Sample0, var33), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var34", offsetof(::StructWith1000Fields::Sample0, var34), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var35", offsetof(::StructWith1000Fields::Sample0, var35), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var36", offsetof(::StructWith1000Fields::Sample0, var36), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var37", offsetof(::StructWith1000Fields::Sample0, var37), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var38", offsetof(::StructWith1000Fields::Sample0, var38), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var39", offsetof(::StructWith1000Fields::Sample0, var39), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_40_47(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var40", offsetof(::StructWith1000Fields::Sample0, var40), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var41", offsetof(::StructWith1000Fields::Sample0, var41), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var42", offsetof(::StructWith1000Fields::Sample0, var42), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var43", offsetof(::StructWith1000Fields::Sample0, var43), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var44", offsetof(::StructWith1000Fields::Sample0, var44), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var45", offsetof(::StructWith1000Fields::Sample0, var45), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var46", offsetof(::StructWith1000Fields::Sample0, var46), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var47", offsetof(::StructWith1000Fields::Sample0, var47), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_48_55(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var48", offsetof(::StructWith1000Fields::Sample0, var48), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var49", offsetof(::StructWith1000Fields::Sample0, var49), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var50", offsetof(::StructWith1000Fields::Sample0, var50), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var51", offsetof(::StructWith1000Fields::Sample0, var51), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var52", offsetof(::StructWith1000Fields::Sample0, var52), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var53", offsetof(::StructWith1000Fields::Sample0, var53), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var54", offsetof(::StructWith1000Fields::Sample0, var54), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var55", offsetof(::StructWith1000Fields::Sample0, var55), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_56_63(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var56", offsetof(::StructWith1000Fields::Sample0, var56), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var57", offsetof(::StructWith1000Fields::Sample0, var57), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var58", offsetof(::StructWith1000Fields::Sample0, var58), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var59", offsetof(::StructWith1000Fields::Sample0, var59), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var60", offsetof(::StructWith1000Fields::Sample0, var60), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var61", offsetof(::StructWith1000Fields::Sample0, var61), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var62", offsetof(::StructWith1000Fields::Sample0, var62), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var63", offsetof(::StructWith1000Fields::Sample0, var63), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_64_71(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var64", offsetof(::StructWith1000Fields::Sample0, var64), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var65", offsetof(::StructWith1000Fields::Sample0, var65), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var66", offsetof(::StructWith1000Fields::Sample0, var66), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var67", offsetof(::StructWith1000Fields::Sample0, var67), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var68", offsetof(::StructWith1000Fields::Sample0, var68), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var69", offsetof(::StructWith1000Fields::Sample0, var69), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var70", offsetof(::StructWith1000Fields::Sample0, var70), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var71", offsetof(::StructWith1000Fields::Sample0, var71), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_72_79(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var72", offsetof(::StructWith1000Fields::Sample0, var72), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var73", offsetof(::StructWith1000Fields::Sample0, var73), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var74", offsetof(::StructWith1000Fields::Sample0, var74), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var75", offsetof(::StructWith1000Fields::Sample0, var75), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var76", offsetof(::StructWith1000Fields::Sample0, var76), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var77", offsetof(::StructWith1000Fields::Sample0, var77), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var78", offsetof(::StructWith1000Fields::Sample0, var78), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var79", offsetof(::StructWith1000Fields::Sample0, var79), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_80_87(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var80", offsetof(::StructWith1000Fields::Sample0, var80), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var81", offsetof(::StructWith1000Fields::Sample0, var81), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var82", offsetof(::StructWith1000Fields::Sample0, var82), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var83", offsetof(::StructWith1000Fields::Sample0, var83), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var84", offsetof(::StructWith1000Fields::Sample0, var84), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var85", offsetof(::StructWith1000Fields::Sample0, var85), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var86", offsetof(::StructWith1000Fields::Sample0, var86), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var87", offsetof(::StructWith1000Fields::Sample0, var87), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_88_95(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var88", offsetof(::StructWith1000Fields::Sample0, var88), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var89", offsetof(::StructWith1000Fields::Sample0, var89), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var90", offsetof(::StructWith1000Fields::Sample0, var90), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var91", offsetof(::StructWith1000Fields::Sample0, var91), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var92", offsetof(::StructWith1000Fields::Sample0, var92), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var93", offsetof(::StructWith1000Fields::Sample0, var93), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var94", offsetof(::StructWith1000Fields::Sample0, var94), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var95", offsetof(::StructWith1000Fields::Sample0, var95), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_96_103(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var96", offsetof(::StructWith1000Fields::Sample0, var96), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var97", offsetof(::StructWith1000Fields::Sample0, var97), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var98", offsetof(::StructWith1000Fields::Sample0, var98), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var99", offsetof(::StructWith1000Fields::Sample0, var99), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var100", offsetof(::StructWith1000Fields::Sample0, var100), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var101", offsetof(::StructWith1000Fields::Sample0, var101), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var102", offsetof(::StructWith1000Fields::Sample0, var102), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var103", offsetof(::StructWith1000Fields::Sample0, var103), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_104_111(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var104", offsetof(::StructWith1000Fields::Sample0, var104), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var105", offsetof(::StructWith1000Fields::Sample0, var105), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var106", offsetof(::StructWith1000Fields::Sample0, var106), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var107", offsetof(::StructWith1000Fields::Sample0, var107), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var108", offsetof(::StructWith1000Fields::Sample0, var108), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var109", offsetof(::StructWith1000Fields::Sample0, var109), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var110", offsetof(::StructWith1000Fields::Sample0, var110), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var111", offsetof(::StructWith1000Fields::Sample0, var111), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_112_119(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var112", offsetof(::StructWith1000Fields::Sample0, var112), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var113", offsetof(::StructWith1000Fields::Sample0, var113), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var114", offsetof(::StructWith1000Fields::Sample0, var114), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var115", offsetof(::StructWith1000Fields::Sample0, var115), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var116", offsetof(::StructWith1000Fields::Sample0, var116), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var117", offsetof(::StructWith1000Fields::Sample0, var117), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var118", offsetof(::StructWith1000Fields::Sample0, var118), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var119", offsetof(::StructWith1000Fields::Sample0, var119), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_120_127(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var120", offsetof(::StructWith1000Fields::Sample0, var120), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var121", offsetof(::StructWith1000Fields::Sample0, var121), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var122", offsetof(::StructWith1000Fields::Sample0, var122), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var123", offsetof(::StructWith1000Fields::Sample0, var123), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var124", offsetof(::StructWith1000Fields::Sample0, var124), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var125", offsetof(::StructWith1000Fields::Sample0, var125), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var126", offsetof(::StructWith1000Fields::Sample0, var126), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var127", offsetof(::StructWith1000Fields::Sample0, var127), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_128_135(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var128", offsetof(::StructWith1000Fields::Sample0, var128), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var129", offsetof(::StructWith1000Fields::Sample0, var129), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var130", offsetof(::StructWith1000Fields::Sample0, var130), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var131", offsetof(::StructWith1000Fields::Sample0, var131), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var132", offsetof(::StructWith1000Fields::Sample0, var132), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var133", offsetof(::StructWith1000Fields::Sample0, var133), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var134", offsetof(::StructWith1000Fields::Sample0, var134), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var135", offsetof(::StructWith1000Fields::Sample0, var135), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_136_143(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var136", offsetof(::StructWith1000Fields::Sample0, var136), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var137", offsetof(::StructWith1000Fields::Sample0, var137), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var138", offsetof(::StructWith1000Fields::Sample0, var138), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var139", offsetof(::StructWith1000Fields::Sample0, var139), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var140", offsetof(::StructWith1000Fields::Sample0, var140), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var141", offsetof(::StructWith1000Fields::Sample0, var141), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var142", offsetof(::StructWith1000Fields::Sample0, var142), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var143", offsetof(::StructWith1000Fields::Sample0, var143), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_144_151(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var144", offsetof(::StructWith1000Fields::Sample0, var144), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var145", offsetof(::StructWith1000Fields::Sample0, var145), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var146", offsetof(::StructWith1000Fields::Sample0, var146), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var147", offsetof(::StructWith1000Fields::Sample0, var147), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var148", offsetof(::StructWith1000Fields::Sample0, var148), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var149", offsetof(::StructWith1000Fields::Sample0, var149), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var150", offsetof(::StructWith1000Fields::Sample0, var150), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var151", offsetof(::StructWith1000Fields::Sample0, var151), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_152_159(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var152", offsetof(::StructWith1000Fields::Sample0, var152), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var153", offsetof(::StructWith1000Fields::Sample0, var153), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var154", offsetof(::StructWith1000Fields::Sample0, var154), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var155", offsetof(::StructWith1000Fields::Sample0, var155), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var156", offsetof(::StructWith1000Fields::Sample0, var156), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var157", offsetof(::StructWith1000Fields::Sample0, var157), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var158", offsetof(::StructWith1000Fields::Sample0, var158), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var159", offsetof(::StructWith1000Fields::Sample0, var159), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_160_167(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var160", offsetof(::StructWith1000Fields::Sample0, var160), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var161", offsetof(::StructWith1000Fields::Sample0, var161), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var162", offsetof(::StructWith1000Fields::Sample0, var162), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var163", offsetof(::StructWith1000Fields::Sample0, var163), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var164", offsetof(::StructWith1000Fields::Sample0, var164), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var165", offsetof(::StructWith1000Fields::Sample0, var165), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var166", offsetof(::StructWith1000Fields::Sample0, var166), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var167", offsetof(::StructWith1000Fields::Sample0, var167), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_168_175(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var168", offsetof(::StructWith1000Fields::Sample0, var168), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var169", offsetof(::StructWith1000Fields::Sample0, var169), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var170", offsetof(::StructWith1000Fields::Sample0, var170), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var171", offsetof(::StructWith1000Fields::Sample0, var171), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var172", offsetof(::StructWith1000Fields::Sample0, var172), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var173", offsetof(::StructWith1000Fields::Sample0, var173), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var174", offsetof(::StructWith1000Fields::Sample0, var174), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var175", offsetof(::StructWith1000Fields::Sample0, var175), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_176_183(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var176", offsetof(::StructWith1000Fields::Sample0, var176), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var177", offsetof(::StructWith1000Fields::Sample0, var177), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var178", offsetof(::StructWith1000Fields::Sample0, var178), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var179", offsetof(::StructWith1000Fields::Sample0, var179), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var180", offsetof(::StructWith1000Fields::Sample0, var180), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var181", offsetof(::StructWith1000Fields::Sample0, var181), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var182", offsetof(::StructWith1000Fields::Sample0, var182), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var183", offsetof(::StructWith1000Fields::Sample0, var183), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_184_191(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var184", offsetof(::StructWith1000Fields::Sample0, var184), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var185", offsetof(::StructWith1000Fields::Sample0, var185), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var186", offsetof(::StructWith1000Fields::Sample0, var186), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var187", offsetof(::StructWith1000Fields::Sample0, var187), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var188", offsetof(::StructWith1000Fields::Sample0, var188), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var189", offsetof(::StructWith1000Fields::Sample0, var189), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var190", offsetof(::StructWith1000Fields::Sample0, var190), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var191", offsetof(::StructWith1000Fields::Sample0, var191), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_192_199(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var192", offsetof(::StructWith1000Fields::Sample0, var192), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var193", offsetof(::StructWith1000Fields::Sample0, var193), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var194", offsetof(::StructWith1000Fields::Sample0, var194), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var195", offsetof(::StructWith1000Fields::Sample0, var195), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var196", offsetof(::StructWith1000Fields::Sample0, var196), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var197", offsetof(::StructWith1000Fields::Sample0, var197), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var198", offsetof(::StructWith1000Fields::Sample0, var198), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var199", offsetof(::StructWith1000Fields::Sample0, var199), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_200_207(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var200", offsetof(::StructWith1000Fields::Sample0, var200), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var201", offsetof(::StructWith1000Fields::Sample0, var201), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var202", offsetof(::StructWith1000Fields::Sample0, var202), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var203", offsetof(::StructWith1000Fields::Sample0, var203), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var204", offsetof(::StructWith1000Fields::Sample0, var204), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var205", offsetof(::StructWith1000Fields::Sample0, var205), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var206", offsetof(::StructWith1000Fields::Sample0, var206), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var207", offsetof(::StructWith1000Fields::Sample0, var207), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_208_215(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var208", offsetof(::StructWith1000Fields::Sample0, var208), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var209", offsetof(::StructWith1000Fields::Sample0, var209), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var210", offsetof(::StructWith1000Fields::Sample0, var210), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var211", offsetof(::StructWith1000Fields::Sample0, var211), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var212", offsetof(::StructWith1000Fields::Sample0, var212), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var213", offsetof(::StructWith1000Fields::Sample0, var213), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var214", offsetof(::StructWith1000Fields::Sample0, var214), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var215", offsetof(::StructWith1000Fields::Sample0, var215), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_216_223(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var216", offsetof(::StructWith1000Fields::Sample0, var216), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var217", offsetof(::StructWith1000Fields::Sample0, var217), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var218", offsetof(::StructWith1000Fields::Sample0, var218), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var219", offsetof(::StructWith1000Fields::Sample0, var219), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var220", offsetof(::StructWith1000Fields::Sample0, var220), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var221", offsetof(::StructWith1000Fields::Sample0, var221), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var222", offsetof(::StructWith1000Fields::Sample0, var222), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var223", offsetof(::StructWith1000Fields::Sample0, var223), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_224_231(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var224", offsetof(::StructWith1000Fields::Sample0, var224), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var225", offsetof(::StructWith1000Fields::Sample0, var225), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var226", offsetof(::StructWith1000Fields::Sample0, var226), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var227", offsetof(::StructWith1000Fields::Sample0, var227), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var228", offsetof(::StructWith1000Fields::Sample0, var228), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var229", offsetof(::StructWith1000Fields::Sample0, var229), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var230", offsetof(::StructWith1000Fields::Sample0, var230), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var231", offsetof(::StructWith1000Fields::Sample0, var231), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_232_239(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var232", offsetof(::StructWith1000Fields::Sample0, var232), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var233", offsetof(::StructWith1000Fields::Sample0, var233), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var234", offsetof(::StructWith1000Fields::Sample0, var234), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var235", offsetof(::StructWith1000Fields::Sample0, var235), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var236", offsetof(::StructWith1000Fields::Sample0, var236), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var237", offsetof(::StructWith1000Fields::Sample0, var237), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var238", offsetof(::StructWith1000Fields::Sample0, var238), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var239", offsetof(::StructWith1000Fields::Sample0, var239), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_240_247(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var240", offsetof(::StructWith1000Fields::Sample0, var240), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var241", offsetof(::StructWith1000Fields::Sample0, var241), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var242", offsetof(::StructWith1000Fields::Sample0, var242), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var243", offsetof(::StructWith1000Fields::Sample0, var243), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var244", offsetof(::StructWith1000Fields::Sample0, var244), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var245", offsetof(::StructWith1000Fields::Sample0, var245), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var246", offsetof(::StructWith1000Fields::Sample0, var246), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var247", offsetof(::StructWith1000Fields::Sample0, var247), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_248_255(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var248", offsetof(::StructWith1000Fields::Sample0, var248), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var249", offsetof(::StructWith1000Fields::Sample0, var249), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var250", offsetof(::StructWith1000Fields::Sample0, var250), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var251", offsetof(::StructWith1000Fields::Sample0, var251), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var252", offsetof(::StructWith1000Fields::Sample0, var252), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var253", offsetof(::StructWith1000Fields::Sample0, var253), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var254", offsetof(::StructWith1000Fields::Sample0, var254), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var255", offsetof(::StructWith1000Fields::Sample0, var255), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_256_263(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var256", offsetof(::StructWith1000Fields::Sample0, var256), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var257", offsetof(::StructWith1000Fields::Sample0, var257), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var258", offsetof(::StructWith1000Fields::Sample0, var258), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var259", offsetof(::StructWith1000Fields::Sample0, var259), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var260", offsetof(::StructWith1000Fields::Sample0, var260), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var261", offsetof(::StructWith1000Fields::Sample0, var261), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var262", offsetof(::StructWith1000Fields::Sample0, var262), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var263", offsetof(::StructWith1000Fields::Sample0, var263), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_264_271(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var264", offsetof(::StructWith1000Fields::Sample0, var264), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var265", offsetof(::StructWith1000Fields::Sample0, var265), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var266", offsetof(::StructWith1000Fields::Sample0, var266), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var267", offsetof(::StructWith1000Fields::Sample0, var267), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var268", offsetof(::StructWith1000Fields::Sample0, var268), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var269", offsetof(::StructWith1000Fields::Sample0, var269), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var270", offsetof(::StructWith1000Fields::Sample0, var270), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var271", offsetof(::StructWith1000Fields::Sample0, var271), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_272_279(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var272", offsetof(::StructWith1000Fields::Sample0, var272), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var273", offsetof(::StructWith1000Fields::Sample0, var273), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var274", offsetof(::StructWith1000Fields::Sample0, var274), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var275", offsetof(::StructWith1000Fields::Sample0, var275), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var276", offsetof(::StructWith1000Fields::Sample0, var276), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var277", offsetof(::StructWith1000Fields::Sample0, var277), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var278", offsetof(::StructWith1000Fields::Sample0, var278), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var279", offsetof(::StructWith1000Fields::Sample0, var279), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_280_287(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var280", offsetof(::StructWith1000Fields::Sample0, var280), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var281", offsetof(::StructWith1000Fields::Sample0, var281), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var282", offsetof(::StructWith1000Fields::Sample0, var282), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var283", offsetof(::StructWith1000Fields::Sample0, var283), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var284", offsetof(::StructWith1000Fields::Sample0, var284), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var285", offsetof(::StructWith1000Fields::Sample0, var285), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var286", offsetof(::StructWith1000Fields::Sample0, var286), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var287", offsetof(::StructWith1000Fields::Sample0, var287), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_288_295(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var288", offsetof(::StructWith1000Fields::Sample0, var288), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var289", offsetof(::StructWith1000Fields::Sample0, var289), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var290", offsetof(::StructWith1000Fields::Sample0, var290), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var291", offsetof(::StructWith1000Fields::Sample0, var291), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var292", offsetof(::StructWith1000Fields::Sample0, var292), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var293", offsetof(::StructWith1000Fields::Sample0, var293), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var294", offsetof(::StructWith1000Fields::Sample0, var294), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var295", offsetof(::StructWith1000Fields::Sample0, var295), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_296_303(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var296", offsetof(::StructWith1000Fields::Sample0, var296), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var297", offsetof(::StructWith1000Fields::Sample0, var297), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var298", offsetof(::StructWith1000Fields::Sample0, var298), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var299", offsetof(::StructWith1000Fields::Sample0, var299), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var300", offsetof(::StructWith1000Fields::Sample0, var300), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var301", offsetof(::StructWith1000Fields::Sample0, var301), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var302", offsetof(::StructWith1000Fields::Sample0, var302), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var303", offsetof(::StructWith1000Fields::Sample0, var303), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_304_311(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var304", offsetof(::StructWith1000Fields::Sample0, var304), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var305", offsetof(::StructWith1000Fields::Sample0, var305), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var306", offsetof(::StructWith1000Fields::Sample0, var306), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var307", offsetof(::StructWith1000Fields::Sample0, var307), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var308", offsetof(::StructWith1000Fields::Sample0, var308), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var309", offsetof(::StructWith1000Fields::Sample0, var309), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var310", offsetof(::StructWith1000Fields::Sample0, var310), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var311", offsetof(::StructWith1000Fields::Sample0, var311), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_312_319(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var312", offsetof(::StructWith1000Fields::Sample0, var312), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var313", offsetof(::StructWith1000Fields::Sample0, var313), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var314", offsetof(::StructWith1000Fields::Sample0, var314), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var315", offsetof(::StructWith1000Fields::Sample0, var315), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var316", offsetof(::StructWith1000Fields::Sample0, var316), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var317", offsetof(::StructWith1000Fields::Sample0, var317), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var318", offsetof(::StructWith1000Fields::Sample0, var318), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var319", offsetof(::StructWith1000Fields::Sample0, var319), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_320_327(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var320", offsetof(::StructWith1000Fields::Sample0, var320), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var321", offsetof(::StructWith1000Fields::Sample0, var321), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var322", offsetof(::StructWith1000Fields::Sample0, var322), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var323", offsetof(::StructWith1000Fields::Sample0, var323), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var324", offsetof(::StructWith1000Fields::Sample0, var324), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var325", offsetof(::StructWith1000Fields::Sample0, var325), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var326", offsetof(::StructWith1000Fields::Sample0, var326), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var327", offsetof(::StructWith1000Fields::Sample0, var327), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_328_335(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var328", offsetof(::StructWith1000Fields::Sample0, var328), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var329", offsetof(::StructWith1000Fields::Sample0, var329), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var330", offsetof(::StructWith1000Fields::Sample0, var330), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var331", offsetof(::StructWith1000Fields::Sample0, var331), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var332", offsetof(::StructWith1000Fields::Sample0, var332), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var333", offsetof(::StructWith1000Fields::Sample0, var333), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var334", offsetof(::StructWith1000Fields::Sample0, var334), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var335", offsetof(::StructWith1000Fields::Sample0, var335), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_336_343(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var336", offsetof(::StructWith1000Fields::Sample0, var336), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var337", offsetof(::StructWith1000Fields::Sample0, var337), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var338", offsetof(::StructWith1000Fields::Sample0, var338), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var339", offsetof(::StructWith1000Fields::Sample0, var339), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var340", offsetof(::StructWith1000Fields::Sample0, var340), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var341", offsetof(::StructWith1000Fields::Sample0, var341), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var342", offsetof(::StructWith1000Fields::Sample0, var342), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var343", offsetof(::StructWith1000Fields::Sample0, var343), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_344_351(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var344", offsetof(::StructWith1000Fields::Sample0, var344), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var345", offsetof(::StructWith1000Fields::Sample0, var345), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var346", offsetof(::StructWith1000Fields::Sample0, var346), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var347", offsetof(::StructWith1000Fields::Sample0, var347), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var348", offsetof(::StructWith1000Fields::Sample0, var348), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var349", offsetof(::StructWith1000Fields::Sample0, var349), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var350", offsetof(::StructWith1000Fields::Sample0, var350), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var351", offsetof(::StructWith1000Fields::Sample0, var351), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_352_359(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var352", offsetof(::StructWith1000Fields::Sample0, var352), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var353", offsetof(::StructWith1000Fields::Sample0, var353), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var354", offsetof(::StructWith1000Fields::Sample0, var354), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var355", offsetof(::StructWith1000Fields::Sample0, var355), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var356", offsetof(::StructWith1000Fields::Sample0, var356), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var357", offsetof(::StructWith1000Fields::Sample0, var357), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var358", offsetof(::StructWith1000Fields::Sample0, var358), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var359", offsetof(::StructWith1000Fields::Sample0, var359), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_360_367(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var360", offsetof(::StructWith1000Fields::Sample0, var360), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var361", offsetof(::StructWith1000Fields::Sample0, var361), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var362", offsetof(::StructWith1000Fields::Sample0, var362), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var363", offsetof(::StructWith1000Fields::Sample0, var363), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var364", offsetof(::StructWith1000Fields::Sample0, var364), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var365", offsetof(::StructWith1000Fields::Sample0, var365), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var366", offsetof(::StructWith1000Fields::Sample0, var366), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var367", offsetof(::StructWith1000Fields::Sample0, var367), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_368_375(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var368", offsetof(::StructWith1000Fields::Sample0, var368), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var369", offsetof(::StructWith1000Fields::Sample0, var369), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var370", offsetof(::StructWith1000Fields::Sample0, var370), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var371", offsetof(::StructWith1000Fields::Sample0, var371), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var372", offsetof(::StructWith1000Fields::Sample0, var372), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var373", offsetof(::StructWith1000Fields::Sample0, var373), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var374", offsetof(::StructWith1000Fields::Sample0, var374), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var375", offsetof(::StructWith1000Fields::Sample0, var375), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_376_383(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var376", offsetof(::StructWith1000Fields::Sample0, var376), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var377", offsetof(::StructWith1000Fields::Sample0, var377), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var378", offsetof(::StructWith1000Fields::Sample0, var378), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var379", offsetof(::StructWith1000Fields::Sample0, var379), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var380", offsetof(::StructWith1000Fields::Sample0, var380), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var381", offsetof(::StructWith1000Fields::Sample0, var381), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var382", offsetof(::StructWith1000Fields::Sample0, var382), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var383", offsetof(::StructWith1000Fields::Sample0, var383), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_384_391(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var384", offsetof(::StructWith1000Fields::Sample0, var384), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var385", offsetof(::StructWith1000Fields::Sample0, var385), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var386", offsetof(::StructWith1000Fields::Sample0, var386), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var387", offsetof(::StructWith1000Fields::Sample0, var387), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var388", offsetof(::StructWith1000Fields::Sample0, var388), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var389", offsetof(::StructWith1000Fields::Sample0, var389), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var390", offsetof(::StructWith1000Fields::Sample0, var390), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var391", offsetof(::StructWith1000Fields::Sample0, var391), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_392_399(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var392", offsetof(::StructWith1000Fields::Sample0, var392), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var393", offsetof(::StructWith1000Fields::Sample0, var393), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var394", offsetof(::StructWith1000Fields::Sample0, var394), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var395", offsetof(::StructWith1000Fields::Sample0, var395), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var396", offsetof(::StructWith1000Fields::Sample0, var396), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var397", offsetof(::StructWith1000Fields::Sample0, var397), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var398", offsetof(::StructWith1000Fields::Sample0, var398), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var399", offsetof(::StructWith1000Fields::Sample0, var399), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_400_407(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var400", offsetof(::StructWith1000Fields::Sample0, var400), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var401", offsetof(::StructWith1000Fields::Sample0, var401), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var402", offsetof(::StructWith1000Fields::Sample0, var402), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var403", offsetof(::StructWith1000Fields::Sample0, var403), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var404", offsetof(::StructWith1000Fields::Sample0, var404), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var405", offsetof(::StructWith1000Fields::Sample0, var405), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var406", offsetof(::StructWith1000Fields::Sample0, var406), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var407", offsetof(::StructWith1000Fields::Sample0, var407), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_408_415(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var408", offsetof(::StructWith1000Fields::Sample0, var408), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var409", offsetof(::StructWith1000Fields::Sample0, var409), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var410", offsetof(::StructWith1000Fields::Sample0, var410), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var411", offsetof(::StructWith1000Fields::Sample0, var411), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var412", offsetof(::StructWith1000Fields::Sample0, var412), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var413", offsetof(::StructWith1000Fields::Sample0, var413), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var414", offsetof(::StructWith1000Fields::Sample0, var414), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var415", offsetof(::StructWith1000Fields::Sample0, var415), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_416_423(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var416", offsetof(::StructWith1000Fields::Sample0, var416), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var417", offsetof(::StructWith1000Fields::Sample0, var417), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var418", offsetof(::StructWith1000Fields::Sample0, var418), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var419", offsetof(::StructWith1000Fields::Sample0, var419), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var420", offsetof(::StructWith1000Fields::Sample0, var420), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var421", offsetof(::StructWith1000Fields::Sample0, var421), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var422", offsetof(::StructWith1000Fields::Sample0, var422), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var423", offsetof(::StructWith1000Fields::Sample0, var423), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_424_431(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var424", offsetof(::StructWith1000Fields::Sample0, var424), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var425", offsetof(::StructWith1000Fields::Sample0, var425), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var426", offsetof(::StructWith1000Fields::Sample0, var426), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var427", offsetof(::StructWith1000Fields::Sample0, var427), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var428", offsetof(::StructWith1000Fields::Sample0, var428), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var429", offsetof(::StructWith1000Fields::Sample0, var429), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var430", offsetof(::StructWith1000Fields::Sample0, var430), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var431", offsetof(::StructWith1000Fields::Sample0, var431), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_432_439(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var432", offsetof(::StructWith1000Fields::Sample0, var432), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var433", offsetof(::StructWith1000Fields::Sample0, var433), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var434", offsetof(::StructWith1000Fields::Sample0, var434), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var435", offsetof(::StructWith1000Fields::Sample0, var435), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var436", offsetof(::StructWith1000Fields::Sample0, var436), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var437", offsetof(::StructWith1000Fields::Sample0, var437), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var438", offsetof(::StructWith1000Fields::Sample0, var438), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var439", offsetof(::StructWith1000Fields::Sample0, var439), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_440_447(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var440", offsetof(::StructWith1000Fields::Sample0, var440), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var441", offsetof(::StructWith1000Fields::Sample0, var441), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var442", offsetof(::StructWith1000Fields::Sample0, var442), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var443", offsetof(::StructWith1000Fields::Sample0, var443), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var444", offsetof(::StructWith1000Fields::Sample0, var444), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var445", offsetof(::StructWith1000Fields::Sample0, var445), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var446", offsetof(::StructWith1000Fields::Sample0, var446), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var447", offsetof(::StructWith1000Fields::Sample0, var447), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_448_455(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var448", offsetof(::StructWith1000Fields::Sample0, var448), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var449", offsetof(::StructWith1000Fields::Sample0, var449), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var450", offsetof(::StructWith1000Fields::Sample0, var450), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var451", offsetof(::StructWith1000Fields::Sample0, var451), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var452", offsetof(::StructWith1000Fields::Sample0, var452), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var453", offsetof(::StructWith1000Fields::Sample0, var453), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var454", offsetof(::StructWith1000Fields::Sample0, var454), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var455", offsetof(::StructWith1000Fields::Sample0, var455), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_456_463(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var456", offsetof(::StructWith1000Fields::Sample0, var456), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var457", offsetof(::StructWith1000Fields::Sample0, var457), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var458", offsetof(::StructWith1000Fields::Sample0, var458), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var459", offsetof(::StructWith1000Fields::Sample0, var459), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var460", offsetof(::StructWith1000Fields::Sample0, var460), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var461", offsetof(::StructWith1000Fields::Sample0, var461), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var462", offsetof(::StructWith1000Fields::Sample0, var462), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var463", offsetof(::StructWith1000Fields::Sample0, var463), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_464_471(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var464", offsetof(::StructWith1000Fields::Sample0, var464), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var465", offsetof(::StructWith1000Fields::Sample0, var465), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var466", offsetof(::StructWith1000Fields::Sample0, var466), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var467", offsetof(::StructWith1000Fields::Sample0, var467), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var468", offsetof(::StructWith1000Fields::Sample0, var468), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var469", offsetof(::StructWith1000Fields::Sample0, var469), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var470", offsetof(::StructWith1000Fields::Sample0, var470), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var471", offsetof(::StructWith1000Fields::Sample0, var471), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_472_479(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var472", offsetof(::StructWith1000Fields::Sample0, var472), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var473", offsetof(::StructWith1000Fields::Sample0, var473), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var474", offsetof(::StructWith1000Fields::Sample0, var474), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var475", offsetof(::StructWith1000Fields::Sample0, var475), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var476", offsetof(::StructWith1000Fields::Sample0, var476), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var477", offsetof(::StructWith1000Fields::Sample0, var477), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var478", offsetof(::StructWith1000Fields::Sample0, var478), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var479", offsetof(::StructWith1000Fields::Sample0, var479), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_480_487(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var480", offsetof(::StructWith1000Fields::Sample0, var480), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var481", offsetof(::StructWith1000Fields::Sample0, var481), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var482", offsetof(::StructWith1000Fields::Sample0, var482), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var483", offsetof(::StructWith1000Fields::Sample0, var483), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var484", offsetof(::StructWith1000Fields::Sample0, var484), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var485", offsetof(::StructWith1000Fields::Sample0, var485), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var486", offsetof(::StructWith1000Fields::Sample0, var486), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var487", offsetof(::StructWith1000Fields::Sample0, var487), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_488_495(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var488", offsetof(::StructWith1000Fields::Sample0, var488), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var489", offsetof(::StructWith1000Fields::Sample0, var489), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var490", offsetof(::StructWith1000Fields::Sample0, var490), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var491", offsetof(::StructWith1000Fields::Sample0, var491), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var492", offsetof(::StructWith1000Fields::Sample0, var492), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var493", offsetof(::StructWith1000Fields::Sample0, var493), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var494", offsetof(::StructWith1000Fields::Sample0, var494), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var495", offsetof(::StructWith1000Fields::Sample0, var495), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_496_503(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var496", offsetof(::StructWith1000Fields::Sample0, var496), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var497", offsetof(::StructWith1000Fields::Sample0, var497), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var498", offsetof(::StructWith1000Fields::Sample0, var498), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var499", offsetof(::StructWith1000Fields::Sample0, var499), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var500", offsetof(::StructWith1000Fields::Sample0, var500), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var501", offsetof(::StructWith1000Fields::Sample0, var501), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var502", offsetof(::StructWith1000Fields::Sample0, var502), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var503", offsetof(::StructWith1000Fields::Sample0, var503), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_504_511(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var504", offsetof(::StructWith1000Fields::Sample0, var504), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var505", offsetof(::StructWith1000Fields::Sample0, var505), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var506", offsetof(::StructWith1000Fields::Sample0, var506), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var507", offsetof(::StructWith1000Fields::Sample0, var507), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var508", offsetof(::StructWith1000Fields::Sample0, var508), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var509", offsetof(::StructWith1000Fields::Sample0, var509), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var510", offsetof(::StructWith1000Fields::Sample0, var510), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var511", offsetof(::StructWith1000Fields::Sample0, var511), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_512_519(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var512", offsetof(::StructWith1000Fields::Sample0, var512), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var513", offsetof(::StructWith1000Fields::Sample0, var513), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var514", offsetof(::StructWith1000Fields::Sample0, var514), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var515", offsetof(::StructWith1000Fields::Sample0, var515), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var516", offsetof(::StructWith1000Fields::Sample0, var516), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var517", offsetof(::StructWith1000Fields::Sample0, var517), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var518", offsetof(::StructWith1000Fields::Sample0, var518), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var519", offsetof(::StructWith1000Fields::Sample0, var519), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_520_527(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var520", offsetof(::StructWith1000Fields::Sample0, var520), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var521", offsetof(::StructWith1000Fields::Sample0, var521), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var522", offsetof(::StructWith1000Fields::Sample0, var522), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var523", offsetof(::StructWith1000Fields::Sample0, var523), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var524", offsetof(::StructWith1000Fields::Sample0, var524), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var525", offsetof(::StructWith1000Fields::Sample0, var525), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var526", offsetof(::StructWith1000Fields::Sample0, var526), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var527", offsetof(::StructWith1000Fields::Sample0, var527), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_528_535(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var528", offsetof(::StructWith1000Fields::Sample0, var528), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var529", offsetof(::StructWith1000Fields::Sample0, var529), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var530", offsetof(::StructWith1000Fields::Sample0, var530), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var531", offsetof(::StructWith1000Fields::Sample0, var531), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var532", offsetof(::StructWith1000Fields::Sample0, var532), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var533", offsetof(::StructWith1000Fields::Sample0, var533), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var534", offsetof(::StructWith1000Fields::Sample0, var534), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var535", offsetof(::StructWith1000Fields::Sample0, var535), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_536_543(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var536", offsetof(::StructWith1000Fields::Sample0, var536), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var537", offsetof(::StructWith1000Fields::Sample0, var537), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var538", offsetof(::StructWith1000Fields::Sample0, var538), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var539", offsetof(::StructWith1000Fields::Sample0, var539), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var540", offsetof(::StructWith1000Fields::Sample0, var540), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var541", offsetof(::StructWith1000Fields::Sample0, var541), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var542", offsetof(::StructWith1000Fields::Sample0, var542), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var543", offsetof(::StructWith1000Fields::Sample0, var543), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_544_551(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var544", offsetof(::StructWith1000Fields::Sample0, var544), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var545", offsetof(::StructWith1000Fields::Sample0, var545), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var546", offsetof(::StructWith1000Fields::Sample0, var546), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var547", offsetof(::StructWith1000Fields::Sample0, var547), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var548", offsetof(::StructWith1000Fields::Sample0, var548), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var549", offsetof(::StructWith1000Fields::Sample0, var549), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var550", offsetof(::StructWith1000Fields::Sample0, var550), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var551", offsetof(::StructWith1000Fields::Sample0, var551), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_552_559(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var552", offsetof(::StructWith1000Fields::Sample0, var552), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var553", offsetof(::StructWith1000Fields::Sample0, var553), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var554", offsetof(::StructWith1000Fields::Sample0, var554), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var555", offsetof(::StructWith1000Fields::Sample0, var555), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var556", offsetof(::StructWith1000Fields::Sample0, var556), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var557", offsetof(::StructWith1000Fields::Sample0, var557), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var558", offsetof(::StructWith1000Fields::Sample0, var558), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var559", offsetof(::StructWith1000Fields::Sample0, var559), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_560_567(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var560", offsetof(::StructWith1000Fields::Sample0, var560), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var561", offsetof(::StructWith1000Fields::Sample0, var561), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var562", offsetof(::StructWith1000Fields::Sample0, var562), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var563", offsetof(::StructWith1000Fields::Sample0, var563), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var564", offsetof(::StructWith1000Fields::Sample0, var564), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var565", offsetof(::StructWith1000Fields::Sample0, var565), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var566", offsetof(::StructWith1000Fields::Sample0, var566), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var567", offsetof(::StructWith1000Fields::Sample0, var567), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_568_575(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var568", offsetof(::StructWith1000Fields::Sample0, var568), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var569", offsetof(::StructWith1000Fields::Sample0, var569), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var570", offsetof(::StructWith1000Fields::Sample0, var570), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var571", offsetof(::StructWith1000Fields::Sample0, var571), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var572", offsetof(::StructWith1000Fields::Sample0, var572), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var573", offsetof(::StructWith1000Fields::Sample0, var573), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var574", offsetof(::StructWith1000Fields::Sample0, var574), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var575", offsetof(::StructWith1000Fields::Sample0, var575), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_576_583(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var576", offsetof(::StructWith1000Fields::Sample0, var576), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var577", offsetof(::StructWith1000Fields::Sample0, var577), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var578", offsetof(::StructWith1000Fields::Sample0, var578), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var579", offsetof(::StructWith1000Fields::Sample0, var579), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var580", offsetof(::StructWith1000Fields::Sample0, var580), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var581", offsetof(::StructWith1000Fields::Sample0, var581), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var582", offsetof(::StructWith1000Fields::Sample0, var582), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var583", offsetof(::StructWith1000Fields::Sample0, var583), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_584_591(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var584", offsetof(::StructWith1000Fields::Sample0, var584), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var585", offsetof(::StructWith1000Fields::Sample0, var585), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var586", offsetof(::StructWith1000Fields::Sample0, var586), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var587", offsetof(::StructWith1000Fields::Sample0, var587), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var588", offsetof(::StructWith1000Fields::Sample0, var588), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var589", offsetof(::StructWith1000Fields::Sample0, var589), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var590", offsetof(::StructWith1000Fields::Sample0, var590), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var591", offsetof(::StructWith1000Fields::Sample0, var591), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_592_599(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var592", offsetof(::StructWith1000Fields::Sample0, var592), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var593", offsetof(::StructWith1000Fields::Sample0, var593), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var594", offsetof(::StructWith1000Fields::Sample0, var594), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var595", offsetof(::StructWith1000Fields::Sample0, var595), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var596", offsetof(::StructWith1000Fields::Sample0, var596), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var597", offsetof(::StructWith1000Fields::Sample0, var597), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var598", offsetof(::StructWith1000Fields::Sample0, var598), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var599", offsetof(::StructWith1000Fields::Sample0, var599), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_600_607(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var600", offsetof(::StructWith1000Fields::Sample0, var600), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var601", offsetof(::StructWith1000Fields::Sample0, var601), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var602", offsetof(::StructWith1000Fields::Sample0, var602), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var603", offsetof(::StructWith1000Fields::Sample0, var603), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var604", offsetof(::StructWith1000Fields::Sample0, var604), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var605", offsetof(::StructWith1000Fields::Sample0, var605), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var606", offsetof(::StructWith1000Fields::Sample0, var606), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var607", offsetof(::StructWith1000Fields::Sample0, var607), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_608_615(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var608", offsetof(::StructWith1000Fields::Sample0, var608), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var609", offsetof(::StructWith1000Fields::Sample0, var609), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var610", offsetof(::StructWith1000Fields::Sample0, var610), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var611", offsetof(::StructWith1000Fields::Sample0, var611), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var612", offsetof(::StructWith1000Fields::Sample0, var612), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var613", offsetof(::StructWith1000Fields::Sample0, var613), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var614", offsetof(::StructWith1000Fields::Sample0, var614), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var615", offsetof(::StructWith1000Fields::Sample0, var615), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_616_623(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var616", offsetof(::StructWith1000Fields::Sample0, var616), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var617", offsetof(::StructWith1000Fields::Sample0, var617), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var618", offsetof(::StructWith1000Fields::Sample0, var618), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var619", offsetof(::StructWith1000Fields::Sample0, var619), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var620", offsetof(::StructWith1000Fields::Sample0, var620), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var621", offsetof(::StructWith1000Fields::Sample0, var621), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var622", offsetof(::StructWith1000Fields::Sample0, var622), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var623", offsetof(::StructWith1000Fields::Sample0, var623), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_624_631(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var624", offsetof(::StructWith1000Fields::Sample0, var624), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var625", offsetof(::StructWith1000Fields::Sample0, var625), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var626", offsetof(::StructWith1000Fields::Sample0, var626), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var627", offsetof(::StructWith1000Fields::Sample0, var627), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var628", offsetof(::StructWith1000Fields::Sample0, var628), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var629", offsetof(::StructWith1000Fields::Sample0, var629), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var630", offsetof(::StructWith1000Fields::Sample0, var630), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var631", offsetof(::StructWith1000Fields::Sample0, var631), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_632_639(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var632", offsetof(::StructWith1000Fields::Sample0, var632), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var633", offsetof(::StructWith1000Fields::Sample0, var633), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var634", offsetof(::StructWith1000Fields::Sample0, var634), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var635", offsetof(::StructWith1000Fields::Sample0, var635), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var636", offsetof(::StructWith1000Fields::Sample0, var636), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var637", offsetof(::StructWith1000Fields::Sample0, var637), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var638", offsetof(::StructWith1000Fields::Sample0, var638), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var639", offsetof(::StructWith1000Fields::Sample0, var639), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_640_647(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var640", offsetof(::StructWith1000Fields::Sample0, var640), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var641", offsetof(::StructWith1000Fields::Sample0, var641), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var642", offsetof(::StructWith1000Fields::Sample0, var642), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var643", offsetof(::StructWith1000Fields::Sample0, var643), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var644", offsetof(::StructWith1000Fields::Sample0, var644), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var645", offsetof(::StructWith1000Fields::Sample0, var645), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var646", offsetof(::StructWith1000Fields::Sample0, var646), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var647", offsetof(::StructWith1000Fields::Sample0, var647), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_648_655(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var648", offsetof(::StructWith1000Fields::Sample0, var648), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var649", offsetof(::StructWith1000Fields::Sample0, var649), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var650", offsetof(::StructWith1000Fields::Sample0, var650), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var651", offsetof(::StructWith1000Fields::Sample0, var651), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var652", offsetof(::StructWith1000Fields::Sample0, var652), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var653", offsetof(::StructWith1000Fields::Sample0, var653), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var654", offsetof(::StructWith1000Fields::Sample0, var654), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var655", offsetof(::StructWith1000Fields::Sample0, var655), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_656_663(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var656", offsetof(::StructWith1000Fields::Sample0, var656), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var657", offsetof(::StructWith1000Fields::Sample0, var657), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var658", offsetof(::StructWith1000Fields::Sample0, var658), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var659", offsetof(::StructWith1000Fields::Sample0, var659), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var660", offsetof(::StructWith1000Fields::Sample0, var660), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var661", offsetof(::StructWith1000Fields::Sample0, var661), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var662", offsetof(::StructWith1000Fields::Sample0, var662), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var663", offsetof(::StructWith1000Fields::Sample0, var663), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_664_671(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var664", offsetof(::StructWith1000Fields::Sample0, var664), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var665", offsetof(::StructWith1000Fields::Sample0, var665), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var666", offsetof(::StructWith1000Fields::Sample0, var666), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var667", offsetof(::StructWith1000Fields::Sample0, var667), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var668", offsetof(::StructWith1000Fields::Sample0, var668), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var669", offsetof(::StructWith1000Fields::Sample0, var669), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var670", offsetof(::StructWith1000Fields::Sample0, var670), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var671", offsetof(::StructWith1000Fields::Sample0, var671), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_672_679(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var672", offsetof(::StructWith1000Fields::Sample0, var672), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var673", offsetof(::StructWith1000Fields::Sample0, var673), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var674", offsetof(::StructWith1000Fields::Sample0, var674), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var675", offsetof(::StructWith1000Fields::Sample0, var675), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var676", offsetof(::StructWith1000Fields::Sample0, var676), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var677", offsetof(::StructWith1000Fields::Sample0, var677), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var678", offsetof(::StructWith1000Fields::Sample0, var678), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var679", offsetof(::StructWith1000Fields::Sample0, var679), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_680_687(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var680", offsetof(::StructWith1000Fields::Sample0, var680), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var681", offsetof(::StructWith1000Fields::Sample0, var681), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var682", offsetof(::StructWith1000Fields::Sample0, var682), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var683", offsetof(::StructWith1000Fields::Sample0, var683), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var684", offsetof(::StructWith1000Fields::Sample0, var684), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var685", offsetof(::StructWith1000Fields::Sample0, var685), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var686", offsetof(::StructWith1000Fields::Sample0, var686), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var687", offsetof(::StructWith1000Fields::Sample0, var687), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_688_695(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var688", offsetof(::StructWith1000Fields::Sample0, var688), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var689", offsetof(::StructWith1000Fields::Sample0, var689), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var690", offsetof(::StructWith1000Fields::Sample0, var690), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var691", offsetof(::StructWith1000Fields::Sample0, var691), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var692", offsetof(::StructWith1000Fields::Sample0, var692), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var693", offsetof(::StructWith1000Fields::Sample0, var693), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var694", offsetof(::StructWith1000Fields::Sample0, var694), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var695", offsetof(::StructWith1000Fields::Sample0, var695), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_696_703(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var696", offsetof(::StructWith1000Fields::Sample0, var696), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var697", offsetof(::StructWith1000Fields::Sample0, var697), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var698", offsetof(::StructWith1000Fields::Sample0, var698), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var699", offsetof(::StructWith1000Fields::Sample0, var699), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var700", offsetof(::StructWith1000Fields::Sample0, var700), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var701", offsetof(::StructWith1000Fields::Sample0, var701), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var702", offsetof(::StructWith1000Fields::Sample0, var702), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var703", offsetof(::StructWith1000Fields::Sample0, var703), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_704_711(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var704", offsetof(::StructWith1000Fields::Sample0, var704), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var705", offsetof(::StructWith1000Fields::Sample0, var705), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var706", offsetof(::StructWith1000Fields::Sample0, var706), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var707", offsetof(::StructWith1000Fields::Sample0, var707), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var708", offsetof(::StructWith1000Fields::Sample0, var708), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var709", offsetof(::StructWith1000Fields::Sample0, var709), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var710", offsetof(::StructWith1000Fields::Sample0, var710), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var711", offsetof(::StructWith1000Fields::Sample0, var711), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_712_719(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var712", offsetof(::StructWith1000Fields::Sample0, var712), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var713", offsetof(::StructWith1000Fields::Sample0, var713), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var714", offsetof(::StructWith1000Fields::Sample0, var714), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var715", offsetof(::StructWith1000Fields::Sample0, var715), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var716", offsetof(::StructWith1000Fields::Sample0, var716), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var717", offsetof(::StructWith1000Fields::Sample0, var717), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var718", offsetof(::StructWith1000Fields::Sample0, var718), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var719", offsetof(::StructWith1000Fields::Sample0, var719), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_720_727(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var720", offsetof(::StructWith1000Fields::Sample0, var720), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var721", offsetof(::StructWith1000Fields::Sample0, var721), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var722", offsetof(::StructWith1000Fields::Sample0, var722), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var723", offsetof(::StructWith1000Fields::Sample0, var723), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var724", offsetof(::StructWith1000Fields::Sample0, var724), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var725", offsetof(::StructWith1000Fields::Sample0, var725), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var726", offsetof(::StructWith1000Fields::Sample0, var726), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var727", offsetof(::StructWith1000Fields::Sample0, var727), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_728_735(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var728", offsetof(::StructWith1000Fields::Sample0, var728), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var729", offsetof(::StructWith1000Fields::Sample0, var729), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var730", offsetof(::StructWith1000Fields::Sample0, var730), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var731", offsetof(::StructWith1000Fields::Sample0, var731), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var732", offsetof(::StructWith1000Fields::Sample0, var732), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var733", offsetof(::StructWith1000Fields::Sample0, var733), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var734", offsetof(::StructWith1000Fields::Sample0, var734), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var735", offsetof(::StructWith1000Fields::Sample0, var735), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_736_743(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var736", offsetof(::StructWith1000Fields::Sample0, var736), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var737", offsetof(::StructWith1000Fields::Sample0, var737), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var738", offsetof(::StructWith1000Fields::Sample0, var738), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var739", offsetof(::StructWith1000Fields::Sample0, var739), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var740", offsetof(::StructWith1000Fields::Sample0, var740), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var741", offsetof(::StructWith1000Fields::Sample0, var741), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var742", offsetof(::StructWith1000Fields::Sample0, var742), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var743", offsetof(::StructWith1000Fields::Sample0, var743), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_744_751(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var744", offsetof(::StructWith1000Fields::Sample0, var744), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var745", offsetof(::StructWith1000Fields::Sample0, var745), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var746", offsetof(::StructWith1000Fields::Sample0, var746), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var747", offsetof(::StructWith1000Fields::Sample0, var747), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var748", offsetof(::StructWith1000Fields::Sample0, var748), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var749", offsetof(::StructWith1000Fields::Sample0, var749), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var750", offsetof(::StructWith1000Fields::Sample0, var750), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var751", offsetof(::StructWith1000Fields::Sample0, var751), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_752_759(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var752", offsetof(::StructWith1000Fields::Sample0, var752), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var753", offsetof(::StructWith1000Fields::Sample0, var753), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var754", offsetof(::StructWith1000Fields::Sample0, var754), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var755", offsetof(::StructWith1000Fields::Sample0, var755), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var756", offsetof(::StructWith1000Fields::Sample0, var756), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var757", offsetof(::StructWith1000Fields::Sample0, var757), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var758", offsetof(::StructWith1000Fields::Sample0, var758), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var759", offsetof(::StructWith1000Fields::Sample0, var759), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_760_767(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var760", offsetof(::StructWith1000Fields::Sample0, var760), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var761", offsetof(::StructWith1000Fields::Sample0, var761), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var762", offsetof(::StructWith1000Fields::Sample0, var762), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var763", offsetof(::StructWith1000Fields::Sample0, var763), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var764", offsetof(::StructWith1000Fields::Sample0, var764), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var765", offsetof(::StructWith1000Fields::Sample0, var765), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var766", offsetof(::StructWith1000Fields::Sample0, var766), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var767", offsetof(::StructWith1000Fields::Sample0, var767), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_768_775(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var768", offsetof(::StructWith1000Fields::Sample0, var768), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var769", offsetof(::StructWith1000Fields::Sample0, var769), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var770", offsetof(::StructWith1000Fields::Sample0, var770), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var771", offsetof(::StructWith1000Fields::Sample0, var771), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var772", offsetof(::StructWith1000Fields::Sample0, var772), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var773", offsetof(::StructWith1000Fields::Sample0, var773), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var774", offsetof(::StructWith1000Fields::Sample0, var774), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var775", offsetof(::StructWith1000Fields::Sample0, var775), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_776_783(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var776", offsetof(::StructWith1000Fields::Sample0, var776), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var777", offsetof(::StructWith1000Fields::Sample0, var777), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var778", offsetof(::StructWith1000Fields::Sample0, var778), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var779", offsetof(::StructWith1000Fields::Sample0, var779), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var780", offsetof(::StructWith1000Fields::Sample0, var780), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var781", offsetof(::StructWith1000Fields::Sample0, var781), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var782", offsetof(::StructWith1000Fields::Sample0, var782), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var783", offsetof(::StructWith1000Fields::Sample0, var783), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_784_791(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var784", offsetof(::StructWith1000Fields::Sample0, var784), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var785", offsetof(::StructWith1000Fields::Sample0, var785), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var786", offsetof(::StructWith1000Fields::Sample0, var786), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var787", offsetof(::StructWith1000Fields::Sample0, var787), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var788", offsetof(::StructWith1000Fields::Sample0, var788), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var789", offsetof(::StructWith1000Fields::Sample0, var789), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var790", offsetof(::StructWith1000Fields::Sample0, var790), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var791", offsetof(::StructWith1000Fields::Sample0, var791), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_792_799(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var792", offsetof(::StructWith1000Fields::Sample0, var792), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var793", offsetof(::StructWith1000Fields::Sample0, var793), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var794", offsetof(::StructWith1000Fields::Sample0, var794), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var795", offsetof(::StructWith1000Fields::Sample0, var795), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var796", offsetof(::StructWith1000Fields::Sample0, var796), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var797", offsetof(::StructWith1000Fields::Sample0, var797), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var798", offsetof(::StructWith1000Fields::Sample0, var798), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var799", offsetof(::StructWith1000Fields::Sample0, var799), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_800_807(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var800", offsetof(::StructWith1000Fields::Sample0, var800), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var801", offsetof(::StructWith1000Fields::Sample0, var801), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var802", offsetof(::StructWith1000Fields::Sample0, var802), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var803", offsetof(::StructWith1000Fields::Sample0, var803), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var804", offsetof(::StructWith1000Fields::Sample0, var804), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var805", offsetof(::StructWith1000Fields::Sample0, var805), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var806", offsetof(::StructWith1000Fields::Sample0, var806), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var807", offsetof(::StructWith1000Fields::Sample0, var807), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_808_815(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var808", offsetof(::StructWith1000Fields::Sample0, var808), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var809", offsetof(::StructWith1000Fields::Sample0, var809), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var810", offsetof(::StructWith1000Fields::Sample0, var810), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var811", offsetof(::StructWith1000Fields::Sample0, var811), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var812", offsetof(::StructWith1000Fields::Sample0, var812), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var813", offsetof(::StructWith1000Fields::Sample0, var813), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var814", offsetof(::StructWith1000Fields::Sample0, var814), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var815", offsetof(::StructWith1000Fields::Sample0, var815), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_816_823(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var816", offsetof(::StructWith1000Fields::Sample0, var816), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var817", offsetof(::StructWith1000Fields::Sample0, var817), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var818", offsetof(::StructWith1000Fields::Sample0, var818), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var819", offsetof(::StructWith1000Fields::Sample0, var819), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var820", offsetof(::StructWith1000Fields::Sample0, var820), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var821", offsetof(::StructWith1000Fields::Sample0, var821), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var822", offsetof(::StructWith1000Fields::Sample0, var822), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var823", offsetof(::StructWith1000Fields::Sample0, var823), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_824_831(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var824", offsetof(::StructWith1000Fields::Sample0, var824), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var825", offsetof(::StructWith1000Fields::Sample0, var825), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var826", offsetof(::StructWith1000Fields::Sample0, var826), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var827", offsetof(::StructWith1000Fields::Sample0, var827), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var828", offsetof(::StructWith1000Fields::Sample0, var828), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var829", offsetof(::StructWith1000Fields::Sample0, var829), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var830", offsetof(::StructWith1000Fields::Sample0, var830), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var831", offsetof(::StructWith1000Fields::Sample0, var831), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_832_839(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var832", offsetof(::StructWith1000Fields::Sample0, var832), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var833", offsetof(::StructWith1000Fields::Sample0, var833), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var834", offsetof(::StructWith1000Fields::Sample0, var834), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var835", offsetof(::StructWith1000Fields::Sample0, var835), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var836", offsetof(::StructWith1000Fields::Sample0, var836), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var837", offsetof(::StructWith1000Fields::Sample0, var837), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var838", offsetof(::StructWith1000Fields::Sample0, var838), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var839", offsetof(::StructWith1000Fields::Sample0, var839), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_840_847(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var840", offsetof(::StructWith1000Fields::Sample0, var840), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var841", offsetof(::StructWith1000Fields::Sample0, var841), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var842", offsetof(::StructWith1000Fields::Sample0, var842), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var843", offsetof(::StructWith1000Fields::Sample0, var843), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var844", offsetof(::StructWith1000Fields::Sample0, var844), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var845", offsetof(::StructWith1000Fields::Sample0, var845), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var846", offsetof(::StructWith1000Fields::Sample0, var846), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var847", offsetof(::StructWith1000Fields::Sample0, var847), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_848_855(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var848", offsetof(::StructWith1000Fields::Sample0, var848), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var849", offsetof(::StructWith1000Fields::Sample0, var849), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var850", offsetof(::StructWith1000Fields::Sample0, var850), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var851", offsetof(::StructWith1000Fields::Sample0, var851), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var852", offsetof(::StructWith1000Fields::Sample0, var852), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var853", offsetof(::StructWith1000Fields::Sample0, var853), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var854", offsetof(::StructWith1000Fields::Sample0, var854), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var855", offsetof(::StructWith1000Fields::Sample0, var855), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_856_863(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var856", offsetof(::StructWith1000Fields::Sample0, var856), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var857", offsetof(::StructWith1000Fields::Sample0, var857), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var858", offsetof(::StructWith1000Fields::Sample0, var858), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var859", offsetof(::StructWith1000Fields::Sample0, var859), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var860", offsetof(::StructWith1000Fields::Sample0, var860), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var861", offsetof(::StructWith1000Fields::Sample0, var861), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var862", offsetof(::StructWith1000Fields::Sample0, var862), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var863", offsetof(::StructWith1000Fields::Sample0, var863), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_864_871(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var864", offsetof(::StructWith1000Fields::Sample0, var864), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var865", offsetof(::StructWith1000Fields::Sample0, var865), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var866", offsetof(::StructWith1000Fields::Sample0, var866), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var867", offsetof(::StructWith1000Fields::Sample0, var867), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var868", offsetof(::StructWith1000Fields::Sample0, var868), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var869", offsetof(::StructWith1000Fields::Sample0, var869), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var870", offsetof(::StructWith1000Fields::Sample0, var870), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var871", offsetof(::StructWith1000Fields::Sample0, var871), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_872_879(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var872", offsetof(::StructWith1000Fields::Sample0, var872), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var873", offsetof(::StructWith1000Fields::Sample0, var873), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var874", offsetof(::StructWith1000Fields::Sample0, var874), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var875", offsetof(::StructWith1000Fields::Sample0, var875), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var876", offsetof(::StructWith1000Fields::Sample0, var876), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var877", offsetof(::StructWith1000Fields::Sample0, var877), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var878", offsetof(::StructWith1000Fields::Sample0, var878), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var879", offsetof(::StructWith1000Fields::Sample0, var879), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_880_887(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var880", offsetof(::StructWith1000Fields::Sample0, var880), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var881", offsetof(::StructWith1000Fields::Sample0, var881), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var882", offsetof(::StructWith1000Fields::Sample0, var882), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var883", offsetof(::StructWith1000Fields::Sample0, var883), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var884", offsetof(::StructWith1000Fields::Sample0, var884), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var885", offsetof(::StructWith1000Fields::Sample0, var885), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var886", offsetof(::StructWith1000Fields::Sample0, var886), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var887", offsetof(::StructWith1000Fields::Sample0, var887), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_888_895(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var888", offsetof(::StructWith1000Fields::Sample0, var888), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var889", offsetof(::StructWith1000Fields::Sample0, var889), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var890", offsetof(::StructWith1000Fields::Sample0, var890), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var891", offsetof(::StructWith1000Fields::Sample0, var891), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var892", offsetof(::StructWith1000Fields::Sample0, var892), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var893", offsetof(::StructWith1000Fields::Sample0, var893), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var894", offsetof(::StructWith1000Fields::Sample0, var894), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var895", offsetof(::StructWith1000Fields::Sample0, var895), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_896_903(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var896", offsetof(::StructWith1000Fields::Sample0, var896), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var897", offsetof(::StructWith1000Fields::Sample0, var897), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var898", offsetof(::StructWith1000Fields::Sample0, var898), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var899", offsetof(::StructWith1000Fields::Sample0, var899), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var900", offsetof(::StructWith1000Fields::Sample0, var900), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var901", offsetof(::StructWith1000Fields::Sample0, var901), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var902", offsetof(::StructWith1000Fields::Sample0, var902), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var903", offsetof(::StructWith1000Fields::Sample0, var903), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_904_911(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var904", offsetof(::StructWith1000Fields::Sample0, var904), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var905", offsetof(::StructWith1000Fields::Sample0, var905), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var906", offsetof(::StructWith1000Fields::Sample0, var906), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var907", offsetof(::StructWith1000Fields::Sample0, var907), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var908", offsetof(::StructWith1000Fields::Sample0, var908), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var909", offsetof(::StructWith1000Fields::Sample0, var909), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var910", offsetof(::StructWith1000Fields::Sample0, var910), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var911", offsetof(::StructWith1000Fields::Sample0, var911), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_912_919(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var912", offsetof(::StructWith1000Fields::Sample0, var912), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var913", offsetof(::StructWith1000Fields::Sample0, var913), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var914", offsetof(::StructWith1000Fields::Sample0, var914), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var915", offsetof(::StructWith1000Fields::Sample0, var915), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var916", offsetof(::StructWith1000Fields::Sample0, var916), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var917", offsetof(::StructWith1000Fields::Sample0, var917), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var918", offsetof(::StructWith1000Fields::Sample0, var918), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var919", offsetof(::StructWith1000Fields::Sample0, var919), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_920_927(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var920", offsetof(::StructWith1000Fields::Sample0, var920), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var921", offsetof(::StructWith1000Fields::Sample0, var921), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var922", offsetof(::StructWith1000Fields::Sample0, var922), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var923", offsetof(::StructWith1000Fields::Sample0, var923), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var924", offsetof(::StructWith1000Fields::Sample0, var924), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var925", offsetof(::StructWith1000Fields::Sample0, var925), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var926", offsetof(::StructWith1000Fields::Sample0, var926), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var927", offsetof(::StructWith1000Fields::Sample0, var927), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_928_935(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var928", offsetof(::StructWith1000Fields::Sample0, var928), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var929", offsetof(::StructWith1000Fields::Sample0, var929), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var930", offsetof(::StructWith1000Fields::Sample0, var930), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var931", offsetof(::StructWith1000Fields::Sample0, var931), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var932", offsetof(::StructWith1000Fields::Sample0, var932), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var933", offsetof(::StructWith1000Fields::Sample0, var933), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var934", offsetof(::StructWith1000Fields::Sample0, var934), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var935", offsetof(::StructWith1000Fields::Sample0, var935), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_936_943(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var936", offsetof(::StructWith1000Fields::Sample0, var936), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var937", offsetof(::StructWith1000Fields::Sample0, var937), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var938", offsetof(::StructWith1000Fields::Sample0, var938), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var939", offsetof(::StructWith1000Fields::Sample0, var939), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var940", offsetof(::StructWith1000Fields::Sample0, var940), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var941", offsetof(::StructWith1000Fields::Sample0, var941), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var942", offsetof(::StructWith1000Fields::Sample0, var942), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var943", offsetof(::StructWith1000Fields::Sample0, var943), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_944_951(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var944", offsetof(::StructWith1000Fields::Sample0, var944), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var945", offsetof(::StructWith1000Fields::Sample0, var945), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var946", offsetof(::StructWith1000Fields::Sample0, var946), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var947", offsetof(::StructWith1000Fields::Sample0, var947), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var948", offsetof(::StructWith1000Fields::Sample0, var948), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var949", offsetof(::StructWith1000Fields::Sample0, var949), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var950", offsetof(::StructWith1000Fields::Sample0, var950), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var951", offsetof(::StructWith1000Fields::Sample0, var951), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_952_959(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var952", offsetof(::StructWith1000Fields::Sample0, var952), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var953", offsetof(::StructWith1000Fields::Sample0, var953), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var954", offsetof(::StructWith1000Fields::Sample0, var954), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var955", offsetof(::StructWith1000Fields::Sample0, var955), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var956", offsetof(::StructWith1000Fields::Sample0, var956), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var957", offsetof(::StructWith1000Fields::Sample0, var957), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var958", offsetof(::StructWith1000Fields::Sample0, var958), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var959", offsetof(::StructWith1000Fields::Sample0, var959), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_960_967(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var960", offsetof(::StructWith1000Fields::Sample0, var960), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var961", offsetof(::StructWith1000Fields::Sample0, var961), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var962", offsetof(::StructWith1000Fields::Sample0, var962), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var963", offsetof(::StructWith1000Fields::Sample0, var963), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var964", offsetof(::StructWith1000Fields::Sample0, var964), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var965", offsetof(::StructWith1000Fields::Sample0, var965), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var966", offsetof(::StructWith1000Fields::Sample0, var966), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var967", offsetof(::StructWith1000Fields::Sample0, var967), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_968_975(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var968", offsetof(::StructWith1000Fields::Sample0, var968), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var969", offsetof(::StructWith1000Fields::Sample0, var969), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var970", offsetof(::StructWith1000Fields::Sample0, var970), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var971", offsetof(::StructWith1000Fields::Sample0, var971), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var972", offsetof(::StructWith1000Fields::Sample0, var972), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var973", offsetof(::StructWith1000Fields::Sample0, var973), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var974", offsetof(::StructWith1000Fields::Sample0, var974), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var975", offsetof(::StructWith1000Fields::Sample0, var975), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_976_983(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var976", offsetof(::StructWith1000Fields::Sample0, var976), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var977", offsetof(::StructWith1000Fields::Sample0, var977), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var978", offsetof(::StructWith1000Fields::Sample0, var978), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var979", offsetof(::StructWith1000Fields::Sample0, var979), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var980", offsetof(::StructWith1000Fields::Sample0, var980), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var981", offsetof(::StructWith1000Fields::Sample0, var981), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var982", offsetof(::StructWith1000Fields::Sample0, var982), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var983", offsetof(::StructWith1000Fields::Sample0, var983), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_984_991(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var984", offsetof(::StructWith1000Fields::Sample0, var984), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var985", offsetof(::StructWith1000Fields::Sample0, var985), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var986", offsetof(::StructWith1000Fields::Sample0, var986), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var987", offsetof(::StructWith1000Fields::Sample0, var987), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var988", offsetof(::StructWith1000Fields::Sample0, var988), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var989", offsetof(::StructWith1000Fields::Sample0, var989), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var990", offsetof(::StructWith1000Fields::Sample0, var990), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var991", offsetof(::StructWith1000Fields::Sample0, var991), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_992_999(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var992", offsetof(::StructWith1000Fields::Sample0, var992), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var993", offsetof(::StructWith1000Fields::Sample0, var993), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var994", offsetof(::StructWith1000Fields::Sample0, var994), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var995", offsetof(::StructWith1000Fields::Sample0, var995), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var996", offsetof(::StructWith1000Fields::Sample0, var996), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var997", offsetof(::StructWith1000Fields::Sample0, var997), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var998", offsetof(::StructWith1000Fields::Sample0, var998), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
        typeDefinition.AddField({ "var999", offsetof(::StructWith1000Fields::Sample0, var999), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }

    void AddFieldDefinitions_Sample0_1000_1000(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "var1000", offsetof(::StructWith1000Fields::Sample0, var1000), DataType::Int64, String::Empty, sizeof(int64), alignof(int64), {  }, StandardAttribute::None });
    }


    void AddFieldDefinitions_Sample0(TypeDefinition& typeDefinition)
    {
    AddFieldDefinitions_Sample0_0_7(typeDefinition);
    AddFieldDefinitions_Sample0_8_15(typeDefinition);
    AddFieldDefinitions_Sample0_16_23(typeDefinition);
    AddFieldDefinitions_Sample0_24_31(typeDefinition);
    AddFieldDefinitions_Sample0_32_39(typeDefinition);
    AddFieldDefinitions_Sample0_40_47(typeDefinition);
    AddFieldDefinitions_Sample0_48_55(typeDefinition);
    AddFieldDefinitions_Sample0_56_63(typeDefinition);
    AddFieldDefinitions_Sample0_64_71(typeDefinition);
    AddFieldDefinitions_Sample0_72_79(typeDefinition);
    AddFieldDefinitions_Sample0_80_87(typeDefinition);
    AddFieldDefinitions_Sample0_88_95(typeDefinition);
    AddFieldDefinitions_Sample0_96_103(typeDefinition);
    AddFieldDefinitions_Sample0_104_111(typeDefinition);
    AddFieldDefinitions_Sample0_112_119(typeDefinition);
    AddFieldDefinitions_Sample0_120_127(typeDefinition);
    AddFieldDefinitions_Sample0_128_135(typeDefinition);
    AddFieldDefinitions_Sample0_136_143(typeDefinition);
    AddFieldDefinitions_Sample0_144_151(typeDefinition);
    AddFieldDefinitions_Sample0_152_159(typeDefinition);
    AddFieldDefinitions_Sample0_160_167(typeDefinition);
    AddFieldDefinitions_Sample0_168_175(typeDefinition);
    AddFieldDefinitions_Sample0_176_183(typeDefinition);
    AddFieldDefinitions_Sample0_184_191(typeDefinition);
    AddFieldDefinitions_Sample0_192_199(typeDefinition);
    AddFieldDefinitions_Sample0_200_207(typeDefinition);
    AddFieldDefinitions_Sample0_208_215(typeDefinition);
    AddFieldDefinitions_Sample0_216_223(typeDefinition);
    AddFieldDefinitions_Sample0_224_231(typeDefinition);
    AddFieldDefinitions_Sample0_232_239(typeDefinition);
    AddFieldDefinitions_Sample0_240_247(typeDefinition);
    AddFieldDefinitions_Sample0_248_255(typeDefinition);
    AddFieldDefinitions_Sample0_256_263(typeDefinition);
    AddFieldDefinitions_Sample0_264_271(typeDefinition);
    AddFieldDefinitions_Sample0_272_279(typeDefinition);
    AddFieldDefinitions_Sample0_280_287(typeDefinition);
    AddFieldDefinitions_Sample0_288_295(typeDefinition);
    AddFieldDefinitions_Sample0_296_303(typeDefinition);
    AddFieldDefinitions_Sample0_304_311(typeDefinition);
    AddFieldDefinitions_Sample0_312_319(typeDefinition);
    AddFieldDefinitions_Sample0_320_327(typeDefinition);
    AddFieldDefinitions_Sample0_328_335(typeDefinition);
    AddFieldDefinitions_Sample0_336_343(typeDefinition);
    AddFieldDefinitions_Sample0_344_351(typeDefinition);
    AddFieldDefinitions_Sample0_352_359(typeDefinition);
    AddFieldDefinitions_Sample0_360_367(typeDefinition);
    AddFieldDefinitions_Sample0_368_375(typeDefinition);
    AddFieldDefinitions_Sample0_376_383(typeDefinition);
    AddFieldDefinitions_Sample0_384_391(typeDefinition);
    AddFieldDefinitions_Sample0_392_399(typeDefinition);
    AddFieldDefinitions_Sample0_400_407(typeDefinition);
    AddFieldDefinitions_Sample0_408_415(typeDefinition);
    AddFieldDefinitions_Sample0_416_423(typeDefinition);
    AddFieldDefinitions_Sample0_424_431(typeDefinition);
    AddFieldDefinitions_Sample0_432_439(typeDefinition);
    AddFieldDefinitions_Sample0_440_447(typeDefinition);
    AddFieldDefinitions_Sample0_448_455(typeDefinition);
    AddFieldDefinitions_Sample0_456_463(typeDefinition);
    AddFieldDefinitions_Sample0_464_471(typeDefinition);
    AddFieldDefinitions_Sample0_472_479(typeDefinition);
    AddFieldDefinitions_Sample0_480_487(typeDefinition);
    AddFieldDefinitions_Sample0_488_495(typeDefinition);
    AddFieldDefinitions_Sample0_496_503(typeDefinition);
    AddFieldDefinitions_Sample0_504_511(typeDefinition);
    AddFieldDefinitions_Sample0_512_519(typeDefinition);
    AddFieldDefinitions_Sample0_520_527(typeDefinition);
    AddFieldDefinitions_Sample0_528_535(typeDefinition);
    AddFieldDefinitions_Sample0_536_543(typeDefinition);
    AddFieldDefinitions_Sample0_544_551(typeDefinition);
    AddFieldDefinitions_Sample0_552_559(typeDefinition);
    AddFieldDefinitions_Sample0_560_567(typeDefinition);
    AddFieldDefinitions_Sample0_568_575(typeDefinition);
    AddFieldDefinitions_Sample0_576_583(typeDefinition);
    AddFieldDefinitions_Sample0_584_591(typeDefinition);
    AddFieldDefinitions_Sample0_592_599(typeDefinition);
    AddFieldDefinitions_Sample0_600_607(typeDefinition);
    AddFieldDefinitions_Sample0_608_615(typeDefinition);
    AddFieldDefinitions_Sample0_616_623(typeDefinition);
    AddFieldDefinitions_Sample0_624_631(typeDefinition);
    AddFieldDefinitions_Sample0_632_639(typeDefinition);
    AddFieldDefinitions_Sample0_640_647(typeDefinition);
    AddFieldDefinitions_Sample0_648_655(typeDefinition);
    AddFieldDefinitions_Sample0_656_663(typeDefinition);
    AddFieldDefinitions_Sample0_664_671(typeDefinition);
    AddFieldDefinitions_Sample0_672_679(typeDefinition);
    AddFieldDefinitions_Sample0_680_687(typeDefinition);
    AddFieldDefinitions_Sample0_688_695(typeDefinition);
    AddFieldDefinitions_Sample0_696_703(typeDefinition);
    AddFieldDefinitions_Sample0_704_711(typeDefinition);
    AddFieldDefinitions_Sample0_712_719(typeDefinition);
    AddFieldDefinitions_Sample0_720_727(typeDefinition);
    AddFieldDefinitions_Sample0_728_735(typeDefinition);
    AddFieldDefinitions_Sample0_736_743(typeDefinition);
    AddFieldDefinitions_Sample0_744_751(typeDefinition);
    AddFieldDefinitions_Sample0_752_759(typeDefinition);
    AddFieldDefinitions_Sample0_760_767(typeDefinition);
    AddFieldDefinitions_Sample0_768_775(typeDefinition);
    AddFieldDefinitions_Sample0_776_783(typeDefinition);
    AddFieldDefinitions_Sample0_784_791(typeDefinition);
    AddFieldDefinitions_Sample0_792_799(typeDefinition);
    AddFieldDefinitions_Sample0_800_807(typeDefinition);
    AddFieldDefinitions_Sample0_808_815(typeDefinition);
    AddFieldDefinitions_Sample0_816_823(typeDefinition);
    AddFieldDefinitions_Sample0_824_831(typeDefinition);
    AddFieldDefinitions_Sample0_832_839(typeDefinition);
    AddFieldDefinitions_Sample0_840_847(typeDefinition);
    AddFieldDefinitions_Sample0_848_855(typeDefinition);
    AddFieldDefinitions_Sample0_856_863(typeDefinition);
    AddFieldDefinitions_Sample0_864_871(typeDefinition);
    AddFieldDefinitions_Sample0_872_879(typeDefinition);
    AddFieldDefinitions_Sample0_880_887(typeDefinition);
    AddFieldDefinitions_Sample0_888_895(typeDefinition);
    AddFieldDefinitions_Sample0_896_903(typeDefinition);
    AddFieldDefinitions_Sample0_904_911(typeDefinition);
    AddFieldDefinitions_Sample0_912_919(typeDefinition);
    AddFieldDefinitions_Sample0_920_927(typeDefinition);
    AddFieldDefinitions_Sample0_928_935(typeDefinition);
    AddFieldDefinitions_Sample0_936_943(typeDefinition);
    AddFieldDefinitions_Sample0_944_951(typeDefinition);
    AddFieldDefinitions_Sample0_952_959(typeDefinition);
    AddFieldDefinitions_Sample0_960_967(typeDefinition);
    AddFieldDefinitions_Sample0_968_975(typeDefinition);
    AddFieldDefinitions_Sample0_976_983(typeDefinition);
    AddFieldDefinitions_Sample0_984_991(typeDefinition);
    AddFieldDefinitions_Sample0_992_999(typeDefinition);
    AddFieldDefinitions_Sample0_1000_1000(typeDefinition);
    }

    TypeDefinition GetTypeDefinitionSample0()
    {
        TypeDefinition result{ DataType::Struct, CTN<StructWith1000Fields::Sample0>(), sizeof(::StructWith1000Fields::Sample0), alignof(::StructWith1000Fields::Sample0), StandardAttribute::None, {} };
        result.ReserveFields(1001);
        AddFieldDefinitions_Sample0(result);
        return result;
    }

    void AddFieldDefinitions_StructWith1000FieldsProgram_0_0(TypeDefinition& typeDefinition)
    {
        typeDefinition.AddField({ "port", offsetof(::StructWith1000Fields::StructWith1000FieldsProgram, port), DataType::Struct, CTN<StructWith1000Fields::Sample0>(), sizeof(StructWith1000Fields::Sample0), alignof(StructWith1000Fields::Sample0), {  }, StandardAttribute::None });
    }


    void AddFieldDefinitions_StructWith1000FieldsProgram(TypeDefinition& typeDefinition)
    {
    AddFieldDefinitions_StructWith1000FieldsProgram_0_0(typeDefinition);
    }

    TypeDefinition GetTypeDefinitionStructWith1000FieldsProgram()
    {
        TypeDefinition result{ DataType::Program, CTN<StructWith1000Fields::StructWith1000FieldsProgram>(), sizeof(::StructWith1000Fields::StructWith1000FieldsProgram), alignof(::StructWith1000Fields::StructWith1000FieldsProgram), StandardAttribute::None, {} };
        result.ReserveFields(1);
        AddFieldDefinitions_StructWith1000FieldsProgram(result);
        return result;
    }
        

} // anonymous namespace


namespace StructWith1000Fields
{

    void StructWith1000FieldsLibrary::InitializeTypeDomain()
    {
        this->typeDomain.AddTypeDefinition(GetTypeDefinitionSample0());

        this->typeDomain.AddTypeDefinition(GetTypeDefinitionStructWith1000FieldsProgram());
    }

} // end of namespace StructWith1000Fields
