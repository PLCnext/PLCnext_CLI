#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "DoublePortDeclarationComponent.hpp"

namespace DoublePortDeclaration
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(DoublePortDeclaration::DoublePortDeclarationComponent)
class DoublePortDeclarationProgram : public ProgramBase, private Loggable<DoublePortDeclarationProgram>
{
public: // typedefs

public: // construction/destruction
    DoublePortDeclarationProgram(DoublePortDeclaration::DoublePortDeclarationComponent& doublePortDeclarationComponentArg, const String& name);
    DoublePortDeclarationProgram(const DoublePortDeclarationProgram& arg) = delete;
    virtual ~DoublePortDeclarationProgram() = default;

public: // operators
    DoublePortDeclarationProgram&  operator=(const DoublePortDeclarationProgram& arg) = delete;

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
        //#attributes(Output)
        //#name(CommentedPort)
        //boolean portField;
        
        //#port
        //#attributes(Input)
        //#name(NewPort)
        boolean portField;

        //#port
        //#attributes(Input)
        //#name(NewPortWithSpace)



        boolean portField2;

        //#port
        //Other userful comment
        //#attributes(Input)
        //Some hopefully useful comment
        //#name(NewPortWithOtherComment)
        //Even more comments
        boolean portField3;

private: // fields
    DoublePortDeclaration::DoublePortDeclarationComponent& doublePortDeclarationComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline DoublePortDeclarationProgram::DoublePortDeclarationProgram(DoublePortDeclaration::DoublePortDeclarationComponent& doublePortDeclarationComponentArg, const String& name)
: ProgramBase(name)
, doublePortDeclarationComponent(doublePortDeclarationComponentArg)
{
}

} // end of namespace DoublePortDeclaration
