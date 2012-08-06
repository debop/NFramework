using System.Text;

namespace NSoft.NFramework.Tools {
    public static partial class StringTool {
        /// <summary>
        /// 두 문자열의 최대 공통 문자열 (Longest common substring) 을 찾습니다. 공통된 부분이 없다면 sequence는 빈 문자열을 가집니다.
        /// 참고: http://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Longest_common_substring
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
        public static int GetLongestCommonSubstring(this string str1, string str2, out string substring) {
            substring = string.Empty;

            if(str1.IsEmpty() || str2.IsEmpty())
                return 0;

            var num = new int[str1.Length,str2.Length];
            var maxLen = 0;
            var lastSubsBegin = 0;

            var sequenceBuilder = new StringBuilder();

            for(var i = 0; i < str1.Length; i++) {
                for(var j = 0; j < str2.Length; j++) {
                    if(str1[i] != str2[j]) {
                        num[i, j] = 0;
                        continue;
                    }

                    num[i, j] = (i == 0 || j == 0) ? 1 : num[i - 1, j - 1] + 1;

                    if(num[i, j] > maxLen) {
                        maxLen = num[i, j];
                        var thisSubsBegin = i - num[i, j] + 1;

                        if(lastSubsBegin == thisSubsBegin)
                            sequenceBuilder.Append(str1[i]);
                        else {
                            lastSubsBegin = thisSubsBegin;
                            sequenceBuilder.Length = 0; // clear it
                            sequenceBuilder.Append(str1.Substring(lastSubsBegin, (i + 1) - lastSubsBegin));
                        }
                    }
                }
            }

            substring = sequenceBuilder.ToString();
            return maxLen;
        }

        /// <summary>
        /// 두 문자열의 최대 공통 문자열 (Longest common substring) 을 찾습니다. 공통된 부분이 없다면 sequence는 빈 문자열을 가집니다.
        /// 참고: http://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Longest_common_substring
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static int GetLongestCommonSequence(this string str1, string str2, out string sequence) {
            int[,] table;
            var index = GetLongestCommonSequenceInternal(str1, str2, out table);

            sequence = ReadLongestCommonSequenceFromBacktrack(table, str1, str2, str1.Length - 1, str2.Length - 1).ToString();
            return index;
        }

        private static int GetLongestCommonSequenceInternal(string str1, string str2, out int[,] matrix) {
            matrix = null;

            if(string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2)) {
                return 0;
            }

            var table = new int[str1.Length + 1,str2.Length + 1];
            for(var i = 0; i < table.GetLength(0); i++) {
                table[i, 0] = 0;
            }
            for(var j = 0; j < table.GetLength(1); j++) {
                table[0, j] = 0;
            }

            for(var i = 1; i < table.GetLength(0); i++) {
                for(var j = 1; j < table.GetLength(1); j++) {
                    if(str1[i - 1] == str2[j - 1])
                        table[i, j] = table[i - 1, j - 1] + 1;
                    else {
                        if(table[i, j - 1] > table[i - 1, j])
                            table[i, j] = table[i, j - 1];
                        else
                            table[i, j] = table[i - 1, j];
                    }
                }
            }

            matrix = table;
            return table[str1.Length, str2.Length];
        }

        private static StringBuilder ReadLongestCommonSequenceFromBacktrack(int[,] backtrack, string str1, string str2, int position1,
                                                                            int position2) {
            if((position1 < 0) || (position2 < 0)) {
                return new StringBuilder();
            }
            if(str1[position1] == str2[position2]) {
                return
                    ReadLongestCommonSequenceFromBacktrack(backtrack, str1, str2, position1 - 1, position2 - 1).Append(str1[position1]);
            }
            if(backtrack[position1, position2 - 1] >= backtrack[position1 - 1, position2]) {
                return ReadLongestCommonSequenceFromBacktrack(backtrack, str1, str2, position1, position2 - 1);
            }
            return ReadLongestCommonSequenceFromBacktrack(backtrack, str1, str2, position1 - 1, position2);
        }
    }
}