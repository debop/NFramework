using System;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 순열에서 조합을 구한다.
        /// </summary>
        /// <param name="n">모집단 갯수</param>
        /// <param name="k">선택할 갯수</param>
        /// <returns>조합</returns>
        [Obsolete("Use Combinations(int,int) instead.")]
        public static int Combination(this int n, int k) {
            if(k == 0 || n == k) return 1;

            return Combination(n - 1, k - 1) + Combination(n - 1, k);
        }

        /// <summary>
        /// 순열에서 조합을 구한다.
        /// </summary>
        /// <param name="n">모집단 갯수</param>
        /// <param name="k">선택할 갯수, k가 17보다 크면 예외를 발생시킨다.</param>
        /// <returns>조합 수</returns>
        [Obsolete("Use Combinations(int,int) instead.")]
        public static long LongCombination(this int n, int k) {
            var a = new long[17];

            if(n - k < k)
                k = n - k;

            if(k == 0) return 1L;
            if(k == 1) return n;

            if(k > 17)
                throw new InvalidOperationException("k is two large number.");

            for(var i = 1; i < k; i++)
                a[i] = (i + 2);

            for(var i = 3; i <= n - k + 1; i++) {
                a[0] = i;
                for(var j = 1; j < k; j++)
                    a[j] += a[j - 1];
            }

            return a[k - 1];
        }

        /// <summary>
        /// Counts the number of possible variations without repetition.
        /// The order matters and each object can be chosen only once.
        /// </summary>
        /// <param name="n">Number of elements in the set.</param>
        /// <param name="k">Number of elements to choose from the set. Each element is chosen at most once.</param>
        /// <returns>Maximum number of distinct variations.</returns>
        public static double Variations(int n, int k) {
            if(k < 0 || n < 0 || k > n) {
                return 0;
            }

            return Math.Floor(
                0.5 + Math.Exp(
                    SpecialFunctions.FactorialLn(n)
                    - SpecialFunctions.FactorialLn(n - k)));
        }

        /// <summary>
        /// Counts the number of possible variations with repetition.
        /// The order matters and each object can be chosen more than once.
        /// </summary>
        /// <param name="n">Number of elements in the set.</param>
        /// <param name="k">Number of elements to choose from the set. Each element is chosen 0, 1 or multiple times.</param>
        /// <returns>Maximum number of distinct variations with repetition.</returns>
        public static double VariationsWithRepetition(int n, int k) {
            if(k < 0 || n < 0) {
                return 0;
            }

            return Math.Pow(n, k);
        }

        /// <summary>
        /// Counts the number of possible combinations without repetition.
        /// The order does not matter and each object can be chosen only once.
        /// </summary>
        /// <param name="n">Number of elements in the set.</param>
        /// <param name="k">Number of elements to choose from the set. Each element is chosen at most once.</param>
        /// <returns>Maximum number of combinations.</returns>
        public static double Combinations(int n, int k) {
            return SpecialFunctions.Binomial(n, k);
        }

        /// <summary>
        /// Counts the number of possible combinations with repetition.
        /// The order does not matter and an object can be chosen more than once.
        /// </summary>
        /// <param name="n">Number of elements in the set.</param>
        /// <param name="k">Number of elements to choose from the set. Each element is chosen 0, 1 or multiple times.</param>
        /// <returns>Maximum number of combinations with repetition.</returns>
        public static double CombinationsWithRepetition(int n, int k) {
            if(k < 0 || n < 0 || (n == 0 && k > 0)) {
                return 0;
            }

            if(n == 0 && k == 0) {
                return 1;
            }

            return Math.Floor(
                0.5 + Math.Exp(
                    SpecialFunctions.FactorialLn(n + k - 1)
                    - SpecialFunctions.FactorialLn(k)
                    - SpecialFunctions.FactorialLn(n - 1)));
        }

        /// <summary>
        /// Counts the number of possible permutations (without repetition). 
        /// </summary>
        /// <param name="n">Number of (distinguishable) elements in the set.</param>
        /// <returns>Maximum number of permutations without repetition.</returns>
        public static double Permutations(int n) {
            return SpecialFunctions.Factorial(n);
        }
    }
}