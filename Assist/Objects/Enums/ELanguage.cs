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

        [Language("fr-FR")] 
        French = 2,

        [Language("ja-JP")] 
        Japanese = 3,

        [Language("pt-BR")] 
        Portuguese = 4,


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
