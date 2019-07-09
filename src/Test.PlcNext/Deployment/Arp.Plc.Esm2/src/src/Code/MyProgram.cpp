#include "MyProgram.hpp"
#include "Arp/System/Commons/Logging.h"
#include "Arp/System/Core/ByteConverter.hpp"

namespace Arp { namespace Plc { namespace Esm { namespace MyComponent {

MyProgram::MyProgram(const String& name) : ProgramBase(name)
{
   //AddPortInfo
}
 
void MyProgram::Execute()
{
   //implement program 
}

}}}} // end of namespace Arp::Plc::Esm::MyComponent
