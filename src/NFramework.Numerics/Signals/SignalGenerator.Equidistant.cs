using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Numerics.Signals {
    public static partial class SignalGenerator {
        /// <summary>
        /// Samples a function equidistant within the provided interval.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="intervalBegin">The real domain interval begin where to start sampling.</param>
        /// <param name="intervalEnd">The real domain interval end where to stop sampling.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        public static T[] EquidistantInterval<T>(Func<double, T> function, double intervalBegin, double intervalEnd, int sampleCount) {
            function.ShouldNotBeNull("function");
            sampleCount.ShouldBePositiveOrZero("sampleCount");

            if(sampleCount == 0)
                return new T[0];

            if(sampleCount == 1)
                return new T[] { function((intervalBegin + intervalEnd) / 2.0) };

            var step = (intervalEnd - intervalBegin) / (sampleCount - 1);

            return
                Enumerable.Range(0, sampleCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(i => (i != sampleCount - 1) ? function(intervalBegin + step * i) : function(intervalEnd))
                    .ToArray();
        }

        /// <summary>
        /// Samples a function equidistant within the provided interval.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="intervalBegin">The real domain interval begin where to start sampling.</param>
        /// <param name="intervalEnd">The real domain interval end where to stop sampling.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <param name="samplePoints">The real domain points where the samples are taken at.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public static T[] EquidistantInterval<T>(Func<double, T> function, double intervalBegin, double intervalEnd, int sampleCount,
                                                 out double[] samplePoints) {
            function.ShouldNotBeNull("function");
            sampleCount.ShouldBePositiveOrZero("sampleCount");

            if(sampleCount == 0) {
                samplePoints = new double[0];
                return new T[0];
            }

            if(sampleCount == 1) {
                samplePoints = new double[] { (intervalBegin + intervalEnd) / 2.0 };
                return new T[] { function(samplePoints[0]) };
            }

            var step = (intervalEnd - intervalBegin) / (sampleCount - 1);

            samplePoints = PLinqTool.Generate(sampleCount, i => intervalBegin + i * step).ToArray();
            samplePoints[samplePoints.Length - 1] = intervalEnd;

            var result =
                samplePoints
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => function(x))
                    .ToArray();

            result[result.Length - 1] = function(intervalEnd);

            return result;
        }

        /// <summary>
        /// Samples a periodic function equidistant within one period, but omits the last sample such that the sequence
        /// can be concatenated together.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="periodLength">The real domain full period length.</param>
        /// <param name="periodOffset">The real domain offset where to start the sampling period.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static T[] EquidistantPeriodic<T>(Func<double, T> function, double periodLength, double periodOffset, int sampleCount) {
            function.ShouldNotBeNull("function");
            sampleCount.ShouldBePositive("sampleCount");

            var step = periodLength / sampleCount;

            return
                Enumerable.Range(0, sampleCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(i => function(periodOffset + i * step))
                    .ToArray();
        }

        /// <summary>
        /// Samples a periodic function equidistant within one period, but omits the last sample such that the sequence
        /// can be concatenated together.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="periodLength">The real domain full period length.</param>
        /// <param name="periodOffset">The real domain offset where to start the sampling period.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <param name="samplePoints">The real domain points where the samples are taken at.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static T[] EquidistantPeriodic<T>(Func<double, T> function, double periodLength, double periodOffset, int sampleCount,
                                                 out double[] samplePoints) {
            function.ShouldNotBeNull("function");
            sampleCount.ShouldBePositive("sampleCount");

            var step = periodLength / sampleCount;

            samplePoints = PLinqTool.Generate(sampleCount, i => periodOffset + i * step).ToArray();

            return
                samplePoints
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => function(x))
                    .ToArray();
        }

        /// <summary>
        /// Samples a function equidistant starting from the provided location with a fixed step length.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="start">The real domain location offset where to start sampling.</param>
        /// <param name="step">The real domain step length between the equidistant samples.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static T[] EquidistantStartingAt<T>(Func<double, T> function, double start, double step, int sampleCount) {
            function.ShouldNotBeNull("function");
            sampleCount.ShouldBePositiveOrZero("sampleCount");

            return
                ParallelEnumerable
                    .Range(0, sampleCount)
                    .AsParallel()
                    .AsOrdered()
                    .Select(i => function(start + i * step))
                    .ToArray();
        }

        /// <summary>
        /// Samples a function equidistant starting from the provided location with a fixed step length.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="start">The real domain location offset where to start sampling.</param>
        /// <param name="step">The real domain step length between the equidistant samples.</param>
        /// <param name="sampleCount">The number of samples to generate.</param>
        /// <param name="samplePoints">The real domain points where the samples are taken at.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample vector.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static T[] EquidistantStartingAt<T>(Func<double, T> function, double start, double step, int sampleCount,
                                                   out double[] samplePoints) {
            function.ShouldNotBeNull("function");
            sampleCount.ShouldBePositiveOrZero("sampleCount");


            samplePoints = PLinqTool.Generate(start, step, sampleCount, x => x).ToArray();

            return
                samplePoints
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => function(x))
                    .ToArray();
        }

        /// <summary>
        /// Samples a function equidistant continuously starting from the provided location with a fixed step length.
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="start">The real domain location offset where to start sampling.</param>
        /// <param name="step">The real domain step length between the equidistant samples.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated sample enumerator.</returns>
        /// <exception cref="ArgumentNullException" />
        public static IEnumerable<T> EquidistantContinuous<T>(Func<double, T> function, double start, double step) {
            function.ShouldNotBeNull("function");

            var current = start;

            while(true) {
                yield return function(current);
                current += step;
            }
        }

        /// <summary>
        /// Samples a function equidistant with the provided start and step length to an integer-domain function
        /// </summary>
        /// <param name="function">The real-domain function to sample.</param>
        /// <param name="start">The real domain location where to start sampling.</param>
        /// <param name="step">The real domain step length between the equidistant samples.</param>
        /// <typeparam name="T">The value type of the function to sample.</typeparam>
        /// <returns>The generated samples integer-domain function.</returns>
        /// <exception cref="ArgumentNullException" />
        public static Func<int, T> EquidistantToFunction<T>(Func<double, T> function, double start, double step) {
            function.ShouldNotBeNull("function");

            return k => function(start + k * step);
        }
    }
}