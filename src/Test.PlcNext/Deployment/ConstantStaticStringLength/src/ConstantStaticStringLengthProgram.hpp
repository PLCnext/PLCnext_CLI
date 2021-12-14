#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ConstantStaticStringLengthComponent.hpp"
#include "ConstHolder.hpp"

const int OuterConst = 70;

namespace ConstantStaticStringLength
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;
using namespace OtherFile;

const int InnerConst = 53;
const int MathConst = OuterConst+OtherFileConst;

//#program
//#component(ConstantStaticStringLength::ConstantStaticStringLengthComponent)
class ConstantStaticStringLengthProgram : public ProgramBase, private Loggable<ConstantStaticStringLengthProgram>
{
public: // typedefs
public: // construction/destruction
    ConstantStaticStringLengthProgram(ConstantStaticStringLength::ConstantStaticStringLengthComponent& constantStaticStringLengthComponentArg, const String& name);
    ConstantStaticStringLengthProgram(const ConstantStaticStringLengthProgram& arg) = delete;
    virtual ~ConstantStaticStringLengthProgram() = default;

public: // operators
    ConstantStaticStringLengthProgram&  operator=(const ConstantStaticStringLengthProgram& arg) = delete;

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
        StaticString<OuterConst> OuterConstPort;

        //#port
        StaticString<MathConst> MathConstPort;

        //#port
        StaticString<InnerConst> InnerConstPort;

        //#port
        StaticString<OtherFileConst> OtherFileConstPort;

private: // fields
    ConstantStaticStringLength::ConstantStaticStringLengthComponent& constantStaticStringLengthComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ConstantStaticStringLengthProgram::ConstantStaticStringLengthProgram(ConstantStaticStringLength::ConstantStaticStringLengthComponent& constantStaticStringLengthComponentArg, const String& name)
: ProgramBase(name)
, constantStaticStringLengthComponent(constantStaticStringLengthComponentArg)
{
}

} // end of namespace ConstantStaticStringLength
