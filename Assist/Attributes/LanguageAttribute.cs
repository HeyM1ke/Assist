using System;

namespace Assist.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class LanguageAttribute : Attribute
{
    public string Code { get; set; }

    public LanguageAttribute(string code)
    {
        Code = code;
    }
}
