#pragma once
#include "Arp/System/Core/Arp.h"

namespace ConsumableLibrary
{

using namespace Arp;

class ConsumableLibraryClass
{
public: // typedefs
    typedef std::shared_ptr<ConsumableLibraryClass> Ptr;

public: // construction/destruction
    /// <summary>Constructs an <see cref="ConsumableLibraryClass" /> instance.</summary>
    ConsumableLibraryClass(void) = default;
    /// <summary>Copy constructor.</summary>
    ConsumableLibraryClass(const ConsumableLibraryClass& arg) = default;
    /// <summary>Assignment operator.</summary>
    ConsumableLibraryClass& operator=(const ConsumableLibraryClass& arg) = default;
    /// <summary>Destructs this instance and frees all resources.</summary>
    ~ConsumableLibraryClass(void) = default;

public: // operators

public: // static operations

public: // setter/getter operations

public: // operations

protected: // operations

private: // static methods

private: // methods

public: // fields

private: // static fields

};

} // end of namespace ConsumableLibrary