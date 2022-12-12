using System;
using System.Linq;
using System.Reflection;

namespace Assist.Services.Utils
{
    public static class EnumUtils
    {

        public static T GetAttribute<T>(this Enum val)
            where T : Attribute
        {
            var type = val.GetType();
            var member = type.GetMember(val.ToString()).First();
            var attribute = member.GetCustomAttribute<T>();

            return attribute;
        }

    }
}
