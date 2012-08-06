using System;
using System.Linq;
using NSoft.NFramework.Numerics.Distributions;

namespace NSoft.NFramework.Numerics.Signals {
    public static partial class SignalGenerator {
        /// <summary>
        /// Samples a function randomly with the provided distribution.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="distribution">Random distribution of the real domain sample points.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        public static T[] Random<T>(Func<double, T> function, IContinuousDistribution distribution, int sampleCount) {
            function.ShouldNotBeNull("function");
            distribution.ShouldNotBeNull("distribution");
            sampleCount.ShouldBePositiveOrZero("sampleCount");

            return
                Enumerable
                    .Range(0, sampleCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(i => function(distribution.Sample()))
                    .ToArray();
        }

        /// <summary>
        /// Samples a function randomly with the provided distribution.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="distribution">Random distribution of the real domain sample points.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <param name="samplePoints">The real domain points where the samples are taken at.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public static T[] Random<T>(Func<double, T> function, IContinuousDistribution distribution, int sampleCount,
                                    out double[] samplePoints) {
            function.ShouldNotBeNull("function");
            distribution.ShouldNotBeNull("distribution");
            sampleCount.ShouldBePositiveOrZero("sampleCount");

            samplePoints = distribution.Samples().Take(sampleCount).ToArray();

            return
                samplePoints
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => function(x))
                    .ToArray();
        }

        /// <summary>
        /// Samples a two-domain function randomly with the provided distribution.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="distribution">Random distribution of the real domain sample points.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public static T[] Random<T>(Func<double, double, T> function, IContinuousDistribution distribution, int sampleCount) {
            function.ShouldNotBeNull("function");
            distribution.ShouldNotBeNull("distribution");
            sampleCount.ShouldBePositiveOrZero("sampleCount");

            return
                Enumerable
                    .Range(0, sampleCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(i => function(distribution.Sample(), distribution.Sample()))
                    .ToArray();
        }
    }
}