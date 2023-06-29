#pragma once
#include "Arp/System/Core/Arp.h"

namespace PortTypesInDifNamespaces
{
    namespace NestedNamespace
    {
        
        class MyNestedEnums
        {
        public:
           enum EnumInNestedNs : int32 {
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
            uint32 i1;
            uint32 i2;
            int32 i3[5];
            MyEnums::EnumTest arrayOfEnums[3];
            MyNestedEnums::EnumInNestedNs i4;
            MyEnums::EnumTest i5;
            StaticString<> someString;
        };

        struct InNestedNs
        {
            uint32 u1;
            uint32 u2;
            int32 u3[5];
            Test innerStruct;
            MyNestedEnums::EnumInNestedNs u4;
            MyEnums::EnumTest u5;
            StaticString<> someString;
            StaticString<> myArrayOfString[3];
        };
    }
}