using System;

namespace NSoft.NFramework.Web.Tools
{
    public static partial class WebAppTool
    {
        /// <summary>
        /// Flag 형식의 Enum 에서 특정 Enum값이 <paramref name="flag"/> enum값을 가지는지 파악합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool EnumHasFlag(this Enum value, Enum flag)
        {
            if(value.GetType() == flag.GetType())
            {
                var flagHashCode = flag.GetHashCode();
                return (value.GetHashCode() & flagHashCode) == flagHashCode;
            }

            return false;
        }
    }
}