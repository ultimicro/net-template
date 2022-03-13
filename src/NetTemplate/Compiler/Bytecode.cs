namespace NetTemplate.Compiler;

public enum Bytecode : byte
{
    Invalid = 0,

    // INSTRUCTION BYTECODES (byte is signed; use a short to keep 0..255)
    INSTR_LOAD_STR,
    INSTR_LOAD_ATTR,
    INSTR_LOAD_LOCAL, // load stuff like it, i, i0
    INSTR_LOAD_PROP,
    INSTR_LOAD_PROP_IND,
    INSTR_STORE_OPTION,
    INSTR_STORE_ARG,
    INSTR_NEW,  // create new template instance
    INSTR_NEW_IND,  // create new instance using value on stack
    INSTR_NEW_BOX_ARGS, // create new instance using args in Map on stack
    INSTR_SUPER_NEW,  // create new instance using value on stack
    INSTR_SUPER_NEW_BOX_ARGS, // create new instance using args in Map on stack
    INSTR_WRITE,
    INSTR_WRITE_OPT,
    INSTR_MAP,  // <a:b()>, <a:b():c()>, <a:{...}>
    INSTR_ROT_MAP,  // <a:b(),c()>
    INSTR_ZIP_MAP,  // <names,phones:{n,p | ...}>
    INSTR_BR,
    INSTR_BRF,
    INSTR_OPTIONS,  // push options map
    INSTR_ARGS,  // push args map
    INSTR_PASSTHRU,
    //INSTR_PASSTHRU_IND,
    INSTR_LIST,
    INSTR_ADD,
    INSTR_TOSTR,

    // Predefined functions
    INSTR_FIRST,
    INSTR_LAST,
    INSTR_REST,
    INSTR_TRUNC,
    INSTR_STRIP,
    INSTR_TRIM,
    INSTR_LENGTH,
    INSTR_STRLEN,
    INSTR_REVERSE,

    INSTR_NOT,
    INSTR_OR,
    INSTR_AND,

    INSTR_INDENT,
    INSTR_DEDENT,
    INSTR_NEWLINE,

    INSTR_NOOP, // do nothing
    INSTR_POP,
    INSTR_NULL, // push null value
    INSTR_TRUE, // push true
    INSTR_FALSE,

    // Combined instructions

    INSTR_WRITE_STR, // load_str n, write
    INSTR_WRITE_LOCAL, // TODO load_local n, write
}
