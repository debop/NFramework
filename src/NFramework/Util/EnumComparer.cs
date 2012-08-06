namespace NSoft.NFramework {
    /// <summary>
    /// Enum 형식에 대한 Comparer를 제공하는 Class입니다.
    /// </summary>
    public static class EnumComparer {
        /// <summary>
        /// 지정된 Enum 형식에 대한 Comparer를 제공합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EnumComparer<T> For<T>() where T : struct {
            return EnumComparer<T>.Instance;
        }
    }
}