using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NSoft.NFramework.Parallelism.Partitioners {
    public static class PartitionerTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 특정 범위를 분할 할 때 기본 분할 갯수 (시스템의 Process 수와 같다)
        /// </summary>
        public static int DefaultPartitionCount = Environment.ProcessorCount;

        /// <summary>
        /// NET-4.0 에서만 지원하는 범위로 분할하는 분할자를 구현하였음. NET-3.5 환경에서만 사용하세요
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <returns></returns>
        public static OrderablePartitioner<Tuple<int, int>> CreateRangePartition(int fromInclusive, int toExclusive) {
            if(IsDebugEnabled)
                log.Debug("범위 [{0}] ~ [{1}] 를 정렬된 Partitioner로 빌드합니다.", fromInclusive, toExclusive);

            var rangeSize = (toExclusive - fromInclusive + 1) / DefaultPartitionCount + 1;

            var ranges = new List<Tuple<int, int>>();

            if(DefaultPartitionCount == 1 || rangeSize <= DefaultPartitionCount) {
                ranges.Add(new Tuple<int, int>(fromInclusive, toExclusive));
            }
            else {
                for(var i = fromInclusive; i < toExclusive; i += rangeSize) {
                    ranges.Add(new Tuple<int, int>(i, Math.Min(i + rangeSize, toExclusive)));
                }
            }
            return Partitioner.Create(ranges);
        }

        /// <summary>
        /// NET-4.0에서만 지원하는 범위로 분할하는 분할자를 구현하였음. NET-3.5 환경에서만 사용하세요
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <returns></returns>
        public static OrderablePartitioner<Tuple<long, long>> CreateRangePartition(long fromInclusive, long toExclusive) {
            if(IsDebugEnabled)
                log.Debug("범위 [{0}] ~ [{1}] 를 정렬된 Partitioner로 빌드합니다.", fromInclusive, toExclusive);

            var rangeSize = (toExclusive - fromInclusive + 1) / Environment.ProcessorCount + 1;

            var ranges = new List<Tuple<long, long>>();

            if(DefaultPartitionCount == 1 || rangeSize <= DefaultPartitionCount) {
                ranges.Add(new Tuple<long, long>(fromInclusive, toExclusive));
            }
            else {
                for(var i = fromInclusive; i < toExclusive; i += rangeSize) {
                    ranges.Add(new Tuple<long, long>(i, Math.Min(i + rangeSize, toExclusive)));
                }
            }
            return Partitioner.Create(ranges);
        }
    }
}