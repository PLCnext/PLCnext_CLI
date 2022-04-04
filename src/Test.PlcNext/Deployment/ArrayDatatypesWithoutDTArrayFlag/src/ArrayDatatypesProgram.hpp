#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "ArrayDatatypesComponent.hpp"

namespace ArrayDatatypes
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(ArrayDatatypes::ArrayDatatypesComponent)
class ArrayDatatypesProgram : public ProgramBase, private Loggable<ArrayDatatypesProgram>
{
public: // typedefs
struct TypeA {
    Arp::int16 value1;
    Arp::int16 value2[10];
};

struct TypeB {
    Arp::int32 value1;
    Arp::int32 value2[10];
};

struct TypeC {
    Arp::int32 value1;
    Arp::int32 value2[10];
};

struct Ports {
    TypeA myA;
    TypeB myB;
    TypeC myC;
}


public: // construction/destruction
    ArrayDatatypesProgram(ArrayDatatypes::ArrayDatatypesComponent& arrayDatatypesComponentArg, const String& name);
    ArrayDatatypesProgram(const ArrayDatatypesProgram& arg) = delete;
    virtual ~ArrayDatatypesProgram() = default;

public: // operators
    ArrayDatatypesProgram&  operator=(const ArrayDatatypesProgram& arg) = delete;

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
        Ports myPorts;

private: // fields
    ArrayDatatypes::ArrayDatatypesComponent& arrayDatatypesComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline ArrayDatatypesProgram::ArrayDatatypesProgram(ArrayDatatypes::ArrayDatatypesComponent& arrayDatatypesComponentArg, const String& name)
: ProgramBase(name)
, arrayDatatypesComponent(arrayDatatypesComponentArg)
{
}

} // end of namespace ArrayDatatypes
