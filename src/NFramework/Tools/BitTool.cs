using System;
using System.Collections;
using System.Text;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Bit 연산 Method 를 제공합니다.
    /// </summary>
    public static class BitTool {
        public const int ByteLength = 8;

        /// <summary>
        /// BitArray의 비트 값을 문자열로 표현합니다. 예:01010011
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static string ToText(this BitArray bits) {
            bits.ShouldNotBeNull("bits");
            var sb = new StringBuilder(bits.Length);

            for(var i = bits.Length - 1; i >= 0; i--) {
                bool bit = bits[i];
                sb.Append(Convert.ToInt32(bit));
            }

            return sb.ToString();
        }

        /// <summary>
        /// BitArray의 Bit 값을 ByteArray로 환산합니다. 이진법을 16진법으로 변경한다는 얘기입니다. (0011 은 0x03)
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this BitArray bits, int startIndex = 0, int count = Int32.MaxValue) {
            bits.ShouldNotBeNull("bits");

            if(count == Int32.MaxValue)
                count = bits.Length;

            int byteSize = count / ByteLength;

            if(count % ByteLength > 0)
                byteSize++;

            var bytes = new byte[byteSize];

            byte value = 0;
            byte significance = 1;

            int bytepos = 0;
            int bitpos = bytepos;

            while(bitpos - startIndex < count) {
                if(bits[bitpos])
                    value += significance;

                bitpos++;

                if(bitpos % ByteLength == 0) {
                    bytes[bytepos] = value;
                    bytepos++;
                    value = 0;
                    significance = 1;
                }
                else {
                    significance *= 2;
                }
            }

            return bytes;
        }

        /// <summary>
        /// value 의 특정 bit를 On 한다.
        /// </summary>
        /// <param name="value">대상 값</param>
        /// <param name="bitToOn">On 시킬 bit의 값</param>
        /// <returns>bit on value</returns>
        public static int BitOn(this int value, int bitToOn) {
            return (value | bitToOn);
        }

        /// <summary>
        /// value 의 특정 bit를 Off 한다.
        /// </summary>
        /// <param name="value">대상 값</param>
        /// <param name="bitToOff">On 시킬 bit의 값</param>
        /// <returns>bit off value</returns>
        public static int BitOff(this int value, int bitToOff) {
            return (value & ~bitToOff);
        }

        /// <summary>
        /// value의 특정 bit 를 Flip 시킨다.
        /// </summary>
        /// <param name="value">대상 값</param>
        /// <param name="bitToFlip">Flip 시킬 bit의 값</param>
        /// <returns>bit flip value</returns>
        public static int BitFlip(this int value, int bitToFlip) {
            return (value ^ bitToFlip);
        }
    }
}