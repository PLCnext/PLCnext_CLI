#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "TemplatedStructComponent.hpp"

namespace TemplatedStruct
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(TemplatedStruct::TemplatedStructComponent)
class TemplatedStructProgram : public ProgramBase, private Loggable<TemplatedStructProgram>
{
public: // typedefs

    template<typename TFlag, typename TCount>
    struct Test
    {
        TFlag flag;
        TCount counter;
    };

    //this is not supported as port type but should not throw a parser exception
    template <typename TFlag, size_t TCount>
    struct OtherTest
    {
        TFlag flag;
    };

    //this is not supported as port type but should not throw a parser exception
    template <typename TFunction, typename ...Args>
    struct OtherTest2
    {
        TFunction xyz;
    };

public: // construction/destruction
    TemplatedStructProgram(TemplatedStruct::TemplatedStructComponent& templatedStructComponentArg, const String& name);
    TemplatedStructProgram(const TemplatedStructProgram& arg) = delete;
    virtual ~TemplatedStructProgram() = default;

public: // operators
    TemplatedStructProgram&  operator=(const TemplatedStructProgram& arg) = delete;

public: // properties

public: // operations
    void    Execute() override;

public: /* Ports
           =====
           Ports are defined in the following way:
           //#port
           //#attributes(Input|Retain)
           //#name(NameOfPort)
           boolean portField;

           The attributes comment define the port attributes and is optional.
           The name comment defines the name of the port and is optional. Default is the name of the field.
        */

        //#port
        //#attributes(Input)
        Test<bool, int32> port1;

        //#port
        //#attributes(Output)
        //#name(NameOfPort)
        Test<int64, int16> port2;

        Test<Test<int16>> port3;

private: // fields
    TemplatedStruct::TemplatedStructComponent& templatedStructComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline TemplatedStructProgram::TemplatedStructProgram(TemplatedStruct::TemplatedStructComponent& templatedStructComponentArg, const String& name)
: ProgramBase(name)
, templatedStructComponent(templatedStructComponentArg)
{
}

} // end of namespace TemplatedStruct
