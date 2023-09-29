#pragma once
#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"
#include "CppSpecialComponent.hpp"

namespace CppSpecial
{

using namespace Arp;
using namespace Arp::System::Commons::Diagnostics::Logging;
using namespace Arp::Plc::Commons::Esm;

//#program
//#component(CppSpecial::CppSpecialComponent)
class CppSpecialProgram : public ProgramBase, private Loggable<CppSpecialProgram>
{
public: // typedefs

    CommandData(const FcnCode_T fcnCode, const CommandType cmdType)
        : functionCode{ fcnCode }, type{ cmdType } {};

    template<typename a_b_c,
        a_b cde,
#if (defined(ABC_DEFG) && (_HIJ_KLM < 1234))
        nop::int64_t qrstuv_wxyz,
#else
        typename a_b_c::int_type qrstuv_wxyz,
#endif
        unsigned short ab_cde,
        typename var_type = nop::int32_t >
    class test {};

    template <class T>
    struct
        has_to_string
    {
        enum e { value = to_string_detail::has_to_string_impl<T, is_output_streamable<T>::value>::value };
    };

public: // construction/destruction
    CppSpecialProgram(CppSpecial::CppSpecialComponent& cppSpecialComponentArg, const String& name);
    CppSpecialProgram(const CppSpecialProgram& arg) = delete;
    virtual ~CppSpecialProgram() = default;

public: // operators
    CppSpecialProgram&  operator=(const CppSpecialProgram& arg) = delete;

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


private: // fields
    CppSpecial::CppSpecialComponent& cppSpecialComponent;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ProgramBase
inline CppSpecialProgram::CppSpecialProgram(CppSpecial::CppSpecialComponent& cppSpecialComponentArg, const String& name)
: ProgramBase(name)
, cppSpecialComponent(cppSpecialComponentArg)
{
}

} // end of namespace CppSpecial
