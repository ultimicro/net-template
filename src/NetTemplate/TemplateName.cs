namespace NetTemplate;

using System;
using Path = System.IO.Path;
using Regex = System.Text.RegularExpressions.Regex;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;

public class TemplateName
{
    private static readonly Regex NameFormatRegex = new Regex(@"^/?([a-z0-9_]+/)*[a-z0-9_]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly string _name;

    public TemplateName(string name)
    {
        if (name == null)
            throw new ArgumentNullException("name");
        if (name.Length == 0)
            throw new ArgumentException();
        if (!NameFormatRegex.IsMatch(name))
            throw new ArgumentException();

        _name = name;
    }

    public bool IsRooted
    {
        get
        {
            return _name.Length > 0 && _name[0] == '/';
        }
    }

    public bool IsTemplate
    {
        get
        {
            return _name.Length > 0 && _name[_name.Length - 1] != '/';
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }
    }

    public static implicit operator TemplateName(string name)
    {
        return new TemplateName(name);
    }

    public static string GetTemplatePath(string localPathRoot, TemplateName templateName)
    {
        if (!templateName.IsRooted)
            return Path.Combine(localPathRoot, templateName.Name);

        return Path.Combine(localPathRoot, templateName.Name.Substring(1));
    }
}
