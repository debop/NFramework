using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Generates sequential GUIDs much in the same way that SQL Server 2005 does
    /// </summary>
    /// <remarks>
    /// links : http://codebetter.com/blogs/scott.bellware/archive/2006/12/27/156671.aspx
    /// </remarks>
    /// <example>
    /// <code>
    /// // generate sequential guid and confirm sequential generated guid.
    /// 
    /// Guid[] guids = new Guid[5];
    /// 
    /// for (int i = 0; i < guids.Length; i++)
    /// {
    /// 	guids[i] = GuidTool.NewSequentialGuid();
    /// 	Console.WriteLine("Guid{0} = {1}", i, guids[i]);
    /// 
    /// 	if(i > 0)
    /// 		if( guids[i].CompareTo(guids[i-1]) <= 0 ) 
    ///				Console.WriteLine("Error!!! not sequential guid.");
    /// }
    /// </code>
    /// </example>
    public static class GuidTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Guid.Comb 스타일의 Guid 값을 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static Guid NewComb() {
            var guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.UtcNow;

            // Get the days and milliseconds
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = now.TimeOfDay;

            // Convert to byte array
            // Note : SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        /// <summary>
        /// SQL Server 2005 처럼 순차적인 Guid를 생성한다.
        /// </summary>
        /// <returns>생성된 Guid 인스턴스</returns>
        public static Guid NewSequentialGuid() {
            const int RPC_S_OK = 0;
            Guid guid;

            var retVal = UuidCreateSequential(out guid);

            if(retVal != RPC_S_OK)
                throw new Win32Exception("Fail to create sequential uuid : " + retVal);

            if(IsDebugEnabled)
                log.Debug("New sequential uuid is created. uuid:" + guid);

            return guid;
        }

        /// <summary>
        /// SQL Server 처럼 순차적인 Guid 값을 지정된 갯수만큼 생성한다.
        /// </summary>
        /// <param name="count">생성할 순차 Guid 의 갯수</param>
        /// <returns>생성된 Guid의 배열</returns>
        public static Guid[] NewSequentialGuid(int count) {
            Guard.Assert(() => count >= 0);

            if(count == 0)
                return new Guid[0];

            return Enumerable.Range(0, count).Select(i => NewSequentialGuid()).ToArray();
        }

        /// <summary>
        /// SQL Server 처럼 순차적인 Guid 값을 지정된 갯수만큼 생성합니다.
        /// </summary>
        /// <param name="count">생성할 Guid 인스턴스 갯수</param>
        /// <returns>생성한 Guid 인스턴스의 컬렉션</returns>
        public static IEnumerable<Guid> AsEnumerable(int count) {
            Guard.Assert(() => count >= 0);

            if(count == 0)
                return Enumerable.Empty<Guid>();

            return Enumerable.Range(0, count).Select(i => NewSequentialGuid());
        }

        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid guid);
    }
}