using System;
using System.Linq;
using System.Reflection;


namespace Assist.Objects.Enums
{
    public enum ELanguage
    {
        [Language("en-US")] 
        English = 0,

        [Language("es-ES")] 
        Spanish = 1,

        [Language("fr")] 
        French = 2,

        [Language("de")] 
        German = 3,

        [Language("zh-CN")] 
        Chinese_Simplified = 4,

        [Language("pt-BR")] 
        Portuguese = 5,

    }

    [AttributeUsage(AttributeTargets.Field)]
    public class LanguageAttribute : Attribute
    {
        public string Code { get; set; }

        public LanguageAttribute(string code)
        {
            Code = code;
        }
    }
}
