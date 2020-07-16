namespace My { namespace Company { namespace Name
{

class ExternalStruct
{
public: // typedefs
    typedef std::shared_ptr<ExternalStruct> Ptr;

public: // construction/destruction
    /// <summary>Constructs an <see cref="ExternalStruct" /> instance.</summary>
    ExternalStruct(void) = default;
    /// <summary>Copy constructor.</summary>
    ExternalStruct(const ExternalStruct& arg) = default;
    /// <summary>Assignment operator.</summary>
    ExternalStruct& operator=(const ExternalStruct& arg) = default;
    /// <summary>Destructs this instance and frees all resources.</summary>
    ~ExternalStruct(void) = default;

public: // fields
    int64               Member1 = 0;
    int16               Member2 = 0;

};

///////////////////////////////////////////////////////////////////////////////
// inline methods of class ExternalStruct

}}} // end of namespace My::Company::Name
