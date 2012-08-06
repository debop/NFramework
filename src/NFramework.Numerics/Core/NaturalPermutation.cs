using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 자연수 순열을 표현합니다.
    /// </summary>
    [Serializable]
    public class NaturalPermutation {
        private readonly int[] _indices;

        public NaturalPermutation(IEnumerable<int> indices) {
            Guard.Assert(IsProperPermutation(indices), "순열의 항목 값이 유효하지 않습니다");

            _indices = indices.ToArray();
        }

        /// <summary>
        /// Gets the number of elements this permutation is over.
        /// </summary>
        public int Dimension {
            get { return _indices.Length; }
        }

        /// <summary>
        /// Computes where <paramref name="idx"/> permutes too.
        /// </summary>
        /// <param name="idx">The index to permute from.</param>
        /// <returns>The index which is permuted to.</returns>
        public int this[int idx] {
            get { return _indices[idx]; }
        }

        /// <summary>
        /// Computes the inverse of the permutation.
        /// </summary>
        /// <returns>The inverse of the permutation.</returns>
        public NaturalPermutation Inverse() {
            var invIdx = new int[Dimension];
            for(int i = 0; i < invIdx.Length; i++) {
                invIdx[_indices[i]] = i;
            }

            return new NaturalPermutation(invIdx);
        }

        /// <summary>
        /// Construct an array from a sequence of inversions.
        /// </summary>
        /// <example>
        /// From wikipedia: the permutation 12043 has the inversions (0,2), (1,2) and (3,4). This would be
        /// encoded using the array [22244].
        /// </example>
        /// <param name="inv">The set of inversions to construct the permutation from.</param>
        /// <returns>A permutation generated from a sequence of inversions.</returns>
        public static NaturalPermutation FromInversions(int[] inv) {
            var idx = new int[inv.Length];
            for(int i = 0; i < inv.Length; i++) {
                idx[i] = i;
            }

            for(int i = inv.Length - 1; i >= 0; i--) {
                if(idx[i] != inv[i]) {
                    int t = idx[i];
                    idx[i] = idx[inv[i]];
                    idx[inv[i]] = t;
                }
            }

            return new NaturalPermutation(idx);
        }

        /// <summary>
        /// Construct a sequence of inversions from the permutation.
        /// </summary>
        /// <example>
        /// From wikipedia: the permutation 12043 has the inversions (0,2), (1,2) and (3,4). This would be
        /// encoded using the array [22244].
        /// </example>
        /// <returns>A sequence of inversions.</returns>
        public int[] ToInversions() {
            var idx = (int[])_indices.Clone();

            for(var i = 0; i < idx.Length; i++) {
                if(idx[i] != i) {
#if !SILVERLIGHT
                    int q = Array.FindIndex(idx, i + 1, x => x == i);
#else
					int q = -1;
					for(int j = i+1; j < Dimension; j++)
					{
						if(idx[j] == i)
						{
							q = j;
							break;
						}
					}
#endif
                    var t = idx[i];
                    idx[i] = q;
                    idx[q] = t;
                }
            }

            return idx;
        }

        private static bool IsProperPermutation(IEnumerable<int> indices) {
            indices.ShouldNotBeNull("indices");

            var length = indices.Count();
            var idxCheck = new bool[length];

            foreach(var item in indices) {
                if(item >= length || item < 0)
                    return false;

                idxCheck[item] = true;
            }

            return !indices.Any(x => idxCheck[x] == false);
        }
    }
}