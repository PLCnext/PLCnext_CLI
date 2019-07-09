#include "MyProgramImpl.hpp"
#include "Arp/System/Commons/Logging.h"
#include "Arp/System/Core/ByteConverter.hpp"

namespace ProgramWithNestedAndStructInStruct { namespace MyComponent {

MyProgramImpl::MyProgramImpl(const String& name) : ProgramBase(name)
{
   //AddPortInfo
}
 
void MyProgramImpl::Execute()
{
   //implement program 
}

}} // end of namespace MyLibrary::MyComponent
