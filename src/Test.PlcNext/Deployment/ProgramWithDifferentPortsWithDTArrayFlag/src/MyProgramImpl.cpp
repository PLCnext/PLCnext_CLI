#include "MyProgram.hpp"
#include "Arp/System/Commons/Logging.h"
#include "Arp/System/Core/ByteConverter.hpp"

namespace ProgramWithDifferentPorts { namespace MyComponent {

MyProgram::MyProgram(const String& name) : ProgramBase(name)
{
   //AddPortInfo
}
 
void MyProgram::Execute()
{
   //implement program 
}

}} // end of namespace ProgramWithDifferentPorts::MyComponent
