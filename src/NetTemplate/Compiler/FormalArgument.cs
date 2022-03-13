namespace NetTemplate.Compiler;

using Antlr.Runtime;

/** Represents the name of a formal argument defined in a template:
 *
 *  test(a,b,x=defaultvalue) ::= "&lt;a&gt; &lt;n&gt; &lt;x&gt;"
 *
 *  Each template has a set of these formal arguments or uses
 *  a placeholder object: UNKNOWN (indicating that no arguments
 *  were specified such as when we create a template with "new Template(...)").
 *
 *  Note: originally, I tracked cardinality as well as the name of an
 *  attribute.  I'm leaving the code here as I suspect something may come
 *  of it later.  Currently, though, cardinality is not used.
 */
public class FormalArgument
{
    /*
        // the following represent bit positions emulating a cardinality bitset.
        public static final int OPTIONAL = 1;     // a?
        public static final int REQUIRED = 2;     // a
        public static final int ZERO_OR_MORE = 4; // a*
        public static final int ONE_OR_MORE = 8;  // a+
        public static final String[] suffixes = {
            null,
            "?",
            "",
            null,
            "*",
            null,
            null,
            null,
            "+"
        };
        protected int cardinality = REQUIRED;
         */

    private readonly string name;

    private int index; // which argument is it? from 0..n-1

    /** If they specified default value x=y, store the token here */
    private readonly IToken defaultValueToken;
    private object defaultValue; // x="str", x=true, x=false
    private CompiledTemplate compiledDefaultValue; // x={...}

    public FormalArgument(string name)
    {
        this.name = name;
    }

    public FormalArgument(string name, IToken defaultValueToken)
    {
        this.name = name;
        this.defaultValueToken = defaultValueToken;
    }

    public string Name
    {
        get
        {
            return name;
        }
    }

    public int Index
    {
        get
        {
            return index;
        }

        internal set
        {
            index = value;
        }
    }

    public IToken DefaultValueToken
    {
        get
        {
            return defaultValueToken;
        }
    }

    public object DefaultValue
    {
        get
        {
            return defaultValue;
        }

        set
        {
            defaultValue = value;
        }
    }

    public CompiledTemplate CompiledDefaultValue
    {
        get
        {
            return compiledDefaultValue;
        }

        internal set
        {
            compiledDefaultValue = value;
        }
    }

    /*
    public static String getCardinalityName(int cardinality) {
        switch (cardinality) {
            case OPTIONAL : return "optional";
            case REQUIRED : return "exactly one";
            case ZERO_OR_MORE : return "zero-or-more";
            case ONE_OR_MORE : return "one-or-more";
            default : return "unknown";
        }
    }
    */

    public override bool Equals(object o)
    {
        if (o == null || !(o is FormalArgument))
        {
            return false;
        }

        FormalArgument other = (FormalArgument)o;
        if (!this.name.Equals(other.name))
        {
            return false;
        }

        // only check if there is a default value; that's all
        return !((this.defaultValueToken != null && other.defaultValueToken == null) ||
               (this.defaultValueToken == null && other.defaultValueToken != null));
    }

    public override int GetHashCode()
    {
        return name.GetHashCode() + defaultValueToken.GetHashCode();
    }

    public override string ToString()
    {
        if (defaultValueToken != null)
            return name + "=" + defaultValueToken.Text;

        return name;
    }
}
