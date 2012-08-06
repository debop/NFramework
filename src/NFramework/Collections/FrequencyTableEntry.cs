using System;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// A generic structure storing the frequency information for each value
    /// </summary>
    [Serializable]
    public struct FrequencyTableEntry<T> where T : IComparable<T> {
        /// <summary>
        /// Counted value
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Absolute Frequency
        /// </summary>
        public int AbsoluteFrequency { get; set; }

        /// <summary>
        /// Relative Frequency
        /// </summary>
        public double RelativeFrequency { get; set; }

        /// <summary>
        /// Percentage
        /// </summary>
        public double Percentage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value counted</param>
        /// <param name="absFreq">absolute frequency</param>
        /// <param name="relFreq">relative frequency</param>
        /// <param name="percentage">sample size</param>
        public FrequencyTableEntry(T value, int absFreq, double relFreq, double percentage)
            : this() {
            Value = value;
            AbsoluteFrequency = absFreq;
            RelativeFrequency = relFreq;
            Percentage = percentage;
        }

        /// <summary>
        /// Returns frequencies of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("[{0}]=AbsouluteFreq=[{1}], RelativeFreq=[{2}], Percentage=[{3}]",
                                 Value, AbsoluteFrequency, RelativeFrequency, Percentage);
        }
    }
}