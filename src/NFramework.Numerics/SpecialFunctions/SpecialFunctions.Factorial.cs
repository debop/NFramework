﻿using System;

namespace NSoft.NFramework.Numerics {
    public static partial class SpecialFunctions {
        /// <summary>
        /// Computes the factorial function x -> x! of an integer number > 0. The function can represent all number up
        /// to 22! exactly, all numbers up to 170! using a double representation. All larger values will overflow.
        /// </summary>
        /// <returns>A value value! for value > 0</returns>
        /// <remarks>
        /// If you need to multiply or divide various such factorials, consider using the logarithmic version 
        /// <see cref="FactorialLn"/> instead so you can add instead of multiply and subtract instead of divide, and
        /// then exponentiate the result using <see cref="System.Math.Exp"/>. This will also circumvent the problem that
        /// factorials become very large even for small parameters.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static double Factorial(int x) {
            x.ShouldBePositiveOrZero("x");

            return x < _factorialCache.Length ? _factorialCache[x] : Double.PositiveInfinity;
        }

        /// <summary>
        /// Computes the logarithmic factorial function x -> ln(x!) of an integer number > 0.
        /// </summary>
        /// <returns>A value value! for value > 0</returns>
        public static double FactorialLn(int x) {
            x.ShouldBePositiveOrZero("x");

            if(x <= 1) {
                return 0d;
            }

            if(x < _factorialCache.Length) {
                return Math.Log(_factorialCache[x]);
            }

            return GammaLn(x + 1.0);
        }

        /// <summary>
        /// Computes the binomial coefficient: n choose k.
        /// </summary>
        /// <param name="n">A nonnegative value n.</param>
        /// <param name="k">A nonnegative value h.</param>
        /// <returns>The binomial coefficient: n choose k.</returns>
        public static double Binomial(int n, int k) {
            if(k < 0 || n < 0 || k > n) {
                return 0.0;
            }

            return Math.Floor(0.5 + Math.Exp(FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k)));
        }

        /// <summary>
        /// Computes the natural logarithm of the binomial coefficient: ln(n choose k).
        /// </summary>
        /// <param name="n">A nonnegative value n.</param>
        /// <param name="k">A nonnegative value h.</param>
        /// <returns>The logarithmic binomial coefficient: ln(n choose k).</returns>
        public static double BinomialLn(int n, int k) {
            if(k < 0 || n < 0 || k > n) {
                return Double.NegativeInfinity;
            }

            return FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k);
        }

        /// <summary>
        /// Computes the multinomial coefficient: n choose n1, n2, n3, ...
        /// </summary>
        /// <param name="n">A nonnegative value n.</param>
        /// <param name="ni">An array of nonnegative values that sum to <paramref name="n"/>.</param>
        /// <returns>The multinomial coefficient.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="ni"/> is <see langword="null" />.</exception>   
        /// <exception cref="ArgumentException">If <paramref name="n"/> or any of the <paramref name="ni"/> are negative.</exception>
        /// <exception cref="ArgumentException">If the sum of all <paramref name="ni"/> is not equal to <paramref name="n"/>.</exception>
        public static double Multinomial(int n, int[] ni) {
            n.ShouldBePositiveOrZero("n");
            ni.ShouldNotBeNull("ni");

            int sum = 0;
            double ret = FactorialLn(n);
            for(var i = 0; i < ni.Length; i++) {
                ni[i].ShouldBePositiveOrZero(string.Format("ni[{0}]", i));

                ret -= FactorialLn(ni[i]);
                sum += ni[i];
            }

            // Before returning, check that the sum of all elements was equal to n.
            Guard.Assert(sum != n, "sum[{0}] 과 n[{1}] 은 같은 값이어야 합니다.", sum, n);

            return Math.Floor(0.5 + Math.Exp(ret));
        }
    }
}