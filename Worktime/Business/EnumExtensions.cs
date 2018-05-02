using System;
using System.ComponentModel;
using System.Linq;

namespace Worktime.Business
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     Returns the value of the given attribute of the enum
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        /// <summary>
        ///     Returns the description of the enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var description = GetAttribute<DescriptionAttribute>(value);
            return description?.Description;
        }
    }
}