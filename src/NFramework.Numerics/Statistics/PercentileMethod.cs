namespace NSoft.NFramework.Numerics.Statistics {
    public enum PercentileMethod {
        /// <summary>
        /// Using the method recommened my NIST,
        /// http://www.itl.nist.gov/div898/handbook/prc/section2/prc252.htm
        /// </summary>
        Nist = 0,

        /// <summary>
        /// Using the nearest rank, http://en.wikipedia.org/wiki/Percentile#Nearest_Rank
        /// </summary>
        Nearest,

        /// <summary>
        /// Using the same method as Excel does, 
        /// http://www.itl.nist.gov/div898/handbook/prc/section2/prc252.htm
        /// </summary>
        Excel,

        /// <summary>
        /// 두 개의 근접 Rank를 선형 보간법을 사용한다.
        /// http://en.wikipedia.org/wiki/Percentile#Linear_Interpolation_Between_Closest_Ranks
        /// </summary>
        Interpolation
    }
}