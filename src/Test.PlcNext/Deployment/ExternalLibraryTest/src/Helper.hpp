namespace Special { namespace Namespace
{
    class Helper
    {
    public: // typedefs
        typedef std::shared_ptr<Helper> Ptr;

    public: // construction/destruction
        /// <summary>Constructs an <see cref="Helper" /> instance.</summary>
        Helper(void) = default;
        /// <summary>Copy constructor.</summary>
        Helper(const Helper& arg) = default;
        /// <summary>Assignment operator.</summary>
        Helper& operator=(const Helper& arg) = default;
        /// <summary>Destructs this instance and frees all resources.</summary>
        ~Helper(void) = default;

    public: // fields
        bool               Member1 = false;

    };
}}