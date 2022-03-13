namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Misc;

public class ErrorBufferAllErrors : ErrorBuffer
{
    public override void RuntimeError(TemplateMessage msg)
    {
        ErrorList.Add(msg);
    }
}
