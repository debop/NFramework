using System;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 변량(Data)의 Histogram을 만든다.
    /// </summary>
    /// <remarks>
    /// 주어진 변량(Data) 를 N-1 개의 bins로 나누어 빈도를 막대그래프로 나타내기 위하여, 
    /// [b0, b1) [b1, b2) ... [bn-2, bn-1] 의 막대로 구간(boundary)을 나누고, 빈도를 넣는다.
    /// </remarks>
    [Obsolete("Use NSoft.NFramework.Numerics.Statistics.Histogram instead.")]
    public sealed class Histogram_Old {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string FloatNumberFormat = "E4";

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="numBins">히스토그램의 막대 수</param>
        /// <param name="minValue">최소 값</param>
        /// <param name="maxValue">최대 값</param>
        public Histogram_Old(int numBins, double minValue, double maxValue) {
            if(IsDebugEnabled)
                log.Debug("Histogram을 생성합니다... numBins=[{0}], minValue=[{1}], maxValue=[{2}]", numBins, minValue, maxValue);

            numBins.ShouldBePositive("numBins");
            minValue.ShouldNotBeEquals(maxValue, "minValue");

            Counts = new int[numBins];

            if(minValue > maxValue)
                MathTool.Swap(ref minValue, ref maxValue);

            MakeBinBoundary(numBins, minValue, maxValue);
            CheckBinBoundary(BinBoundaries);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="BinBoundaries"></param>
        public Histogram_Old(double[] BinBoundaries) {
            BinBoundaries.ShouldNotBeEmpty("BinBoundaries");

            CheckBinBoundary(BinBoundaries);
            this.BinBoundaries = new double[BinBoundaries.Length];
            Array.Copy(BinBoundaries, this.BinBoundaries, BinBoundaries.Length);
            Counts = new int[BinBoundaries.Length - 1];
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="numBins">히스토그램의 막대 수</param>
        /// <param name="data">변량</param>
        public Histogram_Old(int numBins, double[] data) {
            data.ShouldNotBeEmpty("data");

            var sortedData = (double[])data.Clone();
            Array.Sort(sortedData);

            MakeBinBoundary(numBins, sortedData[0], sortedData[sortedData.Length - 1]);
            CheckBinBoundary(BinBoundaries);
            Counts = new int[BinBoundaries.Length - 1];
            AddSortedData(sortedData);
        }

        /// <summary>
        /// float 수형의 문자열 포맷
        /// </summary>
        public static string FloatFormat { get; set; }

        /// <summary>
        /// 히스토그램의 빈도를 나타내는 X축 위치 값들
        /// </summary>
        public double[] BinBoundaries { get; private set; }

        /// <summary>
        /// 각 히스토그램별 갯수
        /// </summary>
        public int[] Counts { get; private set; }

        /// <summary>
        /// 막대 갯수
        /// </summary>
        public int BinsCount {
            get { return Counts.Length; }
        }

        /// <summary>
        /// 최소 구간을 벗어난 변량의 수
        /// </summary>
        public int SmallerCount { get; private set; }

        /// <summary>
        /// 최대 구간을 벗어난 변량의 수
        /// </summary>
        public int LargerCount { get; private set; }

        /// <summary>
        /// Formats the contents of the Histogram into a string.
        /// </summary>
        /// <remarks>If the bin boundaries are b0, b1, b2,...,bn-1, and the 
        /// counts for these bins are c1, c2,...,cn, respectively,
        /// then the this method returns a string with the following 
        /// format:
        /// Number Smaller:   number SmallerCount
        /// [b0,b1)  :   c1
        /// [b1,b2)  :   c2
        /// [b2,b3)  :   c3
        /// .
        /// .
        /// .
        /// [bn-2,bn-1]: cn
        /// Number LargerCount : number LargerCount</remarks>
        /// <returns>Fomatted string.</returns>
        public override string ToString() {
            var buff = new StringBuilder();

            if(SmallerCount > 0)
                buff.AppendFormat("Number of SmallerCount: {0}", SmallerCount).AppendLine();

            for(int i = 0; i < Counts.Length; i++) {
                char closing = (i == Counts.Length - 1) ? ']' : ')';
                buff.AppendFormat(@"[{0}, {1}{2}: {3}",
                                  BinBoundaries[i].ToString(FloatNumberFormat),
                                  BinBoundaries[i + 1].ToString(FloatNumberFormat),
                                  closing,
                                  Counts[i]);
                buff.AppendLine();
            }

            if(LargerCount > 0)
                buff.AppendFormat(@"Number of LargerCount: {0}", LargerCount).AppendLine();

            return buff.ToString();
        }

        /// <summary>
        /// Formats the contents of the Histogram into a simple acsii stem-leaf
        /// diagram.
        /// </summary>
        /// <remarks>If the bin boundaries are b0, b1, b2,...,bn-1, and the 
        /// counts for these bins are c1, c2,...,cn, respectively,
        /// then the this method returns a string with the following 
        /// format:
        /// Number SmallerCount:   ***number SmallerCount
        /// [b0,b1):     *****c1
        /// [b1,b2):     **********c2
        /// [b2,b3):     ***************c3
        /// .
        /// .
        /// .
        /// [bn-2,bn-1]: *****cn
        /// Number LargerCount : *****number LargerCount.
        /// Where the number of '*'s is for a particular bin is equal to
        /// the count for that bin minus one.</remarks>
        /// <returns>Fomatted string.</returns>
        public string StemLeaf(int maxMark) {
            var buff = new StringBuilder();

            if(SmallerCount > 0)
                buff.AppendFormat("Number of SmallerCount: {0}", SmallerCount).AppendLine();

            var sortedCount = (int[])Counts.Clone();
            Array.Sort(sortedCount);

            double scale;
            if(maxMark > 0)
                scale = Math.Max(maxMark, 10) / (double)sortedCount[Counts.Length - 1];
            else
                scale = 1.0;

            for(int i = 0; i < Counts.Length; i++) {
                char closing = (i == Counts.Length - 1) ? ']' : ')';
                buff.AppendFormat("[{0}, {1}{2}: {3}:{4}",
                                  BinBoundaries[i].ToString(FloatNumberFormat),
                                  BinBoundaries[i + 1].ToString(FloatNumberFormat),
                                  closing,
                                  "*".Replicate((int)(Counts[i] * scale)),
                                  Counts[i]);
                buff.AppendLine();
            }

            if(LargerCount > 0)
                buff.AppendFormat("Number of LargerCount: {0}", LargerCount).AppendLine();

            return buff.ToString();
        }

        /// <summary>
        /// 지정된 구간의 갯수
        /// </summary>
        /// <param name="binNumber"></param>
        /// <returns></returns>
        public int Count(int binNumber) {
            return Counts[binNumber];
        }

        /// <summary>
        /// Data 추가
        /// </summary>
        /// <param name="d"></param>
        public void AddData(double d) {
            if(d < BinBoundaries[0])
                SmallerCount++;
            else if(d > BinBoundaries[BinBoundaries.Length - 1])
                LargerCount++;
            else {
                int n = 1;
                while(n < BinBoundaries.Length && d >= BinBoundaries[n])
                    n++;

                if(n == BinBoundaries.Length)
                    ++Counts[Counts.Length - 1];
                else
                    ++Counts[n - 1];
            }
        }

        /// <summary>
        /// Data 추가
        /// </summary>
        /// <param name="data"></param>
        public void AddData(double[] data) {
            var sortedData = (double[])data.Clone();
            Array.Sort(sortedData);
            AddSortedData(sortedData);
        }

        private void AddSortedData(double[] data) {
            var i = 0;

            while(data[i] < BinBoundaries[0]) {
                i++;
                SmallerCount++;
            }

            for(int n = 0; n < Counts.Length; n++) {
                if(i >= data.Length)
                    break;

                for(;;) {
                    if(i >= data.Length)
                        break;

                    if((n == Counts.Length - 1 && data[i] <= BinBoundaries[n + 1]) ||
                       data[i] < BinBoundaries[n + 1]) {
                        ++Counts[n];
                        ++i;
                    }
                    else {
                        break;
                    }
                }
            }

            if(i < data.Length) {
                LargerCount += data.Length - i;
            }
        }

        /// <summary>
        /// 히스토그램 결과를 모두 삭제한다.
        /// </summary>
        public void Reset() {
            SmallerCount = 0;
            LargerCount = 0;
            for(int i = 0; i < Counts.Length; i++)
                Counts[i] = 0;
        }

        private void MakeBinBoundary(int numBins, double minValue, double maxValue) {
            BinBoundaries = new double[numBins + 1];
            BinBoundaries[0] = minValue;
            BinBoundaries[BinBoundaries.Length - 1] = maxValue;

            double binSize = (maxValue - minValue) / numBins;

            for(int i = 1; i < BinBoundaries.Length - 1; i++)
                BinBoundaries[i] = BinBoundaries[i - 1] + binSize;
        }

        private static void CheckBinBoundary(double[] boundaries) {
            /*
			for (int i = 0; i < boundaries.Length - 1; i++)
				if (boundaries[i] >= boundaries[i + 1])
					throw new ArgumentException(
						string.Format("Bin Boundary {0} is >= Bin Boundary {1}", boundaries[i], boundaries[i + 1]));
		
			 */
        }
    }
}