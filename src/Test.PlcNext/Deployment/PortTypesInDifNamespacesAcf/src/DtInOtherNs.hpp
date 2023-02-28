#pragma once
#include "Arp/System/Core/Arp.h"

namespace OtherNamespace
{
    using namespace Arp;
    using namespace Arp::Plc::Commons::Gds;

    class MyOtherEnums
    {
    public:
        enum EnumInOtherNs : int32 {
            e1 = 0,
            e2 = 1,
            e3 = 12
        };
    };

    class MyEnums
    {
    public:
        enum EnumTest : int32 {
            e1 = 0,
            e2 = 1,
            e3 = 12
        };
    };

    struct Test
    {
        uint8 i1;
        uint8 i2;
        int8 i3[10];
        MyEnums::EnumTest arrayOfEnums[3];
        MyOtherEnums::EnumInOtherNs i4;
        MyEnums::EnumTest i5;
        StaticString<> someString;
    };

    struct InOtherNamespace
    {
        uint16 i1;
        uint16 i2;
        int16 i3[10];
        Test innerStruct;
        MyOtherEnums::EnumInOtherNs i4;
        MyEnums::EnumTest i5;
        StaticString<> someString;
        StaticString<> myArrayOfString[3];
    };
}