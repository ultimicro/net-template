namespace NetTemplate;

/** Generic StringTemplate output writer filter.
 *
 *  Literals and the elements of expressions are emitted via Write().
 *  Separators are emitted via WriteSeparator() because they must be
 *  handled specially when wrapping lines (we don't want to wrap
 *  in between an element and it's separator).
 */
public interface ITemplateWriter
{
    /** Return the absolute char index into the output of the char
     *  we're about to Write.  Returns 0 if no char written yet.
     */
    int Index
    {
        get;
    }

    int LineWidth
    {
        get;
        set;
    }

    void PushIndentation(string indent);

    string PopIndentation();

    void PushAnchorPoint();

    void PopAnchorPoint();

    /** Write the string and return how many actual chars were written.
     *  With autoindentation and wrapping, more chars than length(str)
     *  can be emitted.  No wrapping is done.
     */
    int Write(string str);

    /** Same as Write, but wrap lines using the indicated string as the
     *  wrap character (such as "\n").
     */
    int Write(string str, string wrap);

    /** Because we evaluate Template instance by invoking exec() again, we
     *  can't pass options in.  So the WRITE instruction of an applied
     *  template (such as when we wrap in between template applications
     *  like &lt;data:{v|[&lt;v&gt;]}; wrap&gt;) we need to Write the wrap string
     *  before calling exec().  We expose just like for the separator.
     *  See Interpreter.WriteObject where it checks for Template instance.
     *  If POJO, WritePlainObject passes wrap to ITemplateWriter's
     *
     *     Write(String str, String wrap)
     *
     *  method.  Can't pass to exec().
     */
    int WriteWrap(string wrap);

    /** Write a separator.  Same as Write() except that a \n cannot
     *  be inserted before emitting a separator.
     */
    int WriteSeparator(string str);
}
