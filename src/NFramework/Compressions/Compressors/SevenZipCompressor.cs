using System;
using System.IO;
using NSoft.NFramework.Compressions.SevenZip;

namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// 7Zip Algorithm을 이용한 Compressor (참고: http://www.7-zip.org/sdk.html)
    /// </summary>
    public class SevenZipCompressor : AbstractCompressor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 데이타를 압축한다.
        /// </summary>
        /// <param name="input">압축할 Data</param>
        /// <returns>압축된 Data</returns>
        public override byte[] Compress(byte[] input) {
            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.CompressStartMsg);

            // check input data
            if(input.IsZeroLength()) {
                if(log.IsWarnEnabled)
                    log.Warn(CompressorTool.SR.InvalidInputDataMsg);

                return new byte[0];
            }

            byte[] output;
            using(var outStream = new MemoryStream())
            using(var inStream = new MemoryStream(input)) {
                var encoder = new NSoft.NFramework.Compressions.SevenZip.Compression.LZMA.LzmaEncoder();
                encoder.SetCoderProperties(propIDs, properties);
                encoder.WriteCoderProperties(outStream);
                long size = inStream.Length;
                for(int i = 0; i < 8; i++)
                    outStream.WriteByte((byte)(size >> (8 * i)));

                encoder.Code(inStream, outStream, -1, -1, null);

                output = outStream.ToArray();
            }

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.CompressResultMsg, input.Length, output.Length, output.Length / (double)input.Length);

            return output ?? new byte[0];
        }

        /// <summary>
        /// 압축된 데이타를 복원한다.
        /// </summary>
        /// <param name="input">복원할 Data</param>
        /// <returns>복원된 Data</returns>
        public override byte[] Decompress(byte[] input) {
            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressStartMsg);

            // check input data
            if(input.IsZeroLength()) {
                if(log.IsWarnEnabled)
                    log.Warn(CompressorTool.SR.InvalidInputDataMsg);

                return new byte[0];
            }

            byte[] properties2 = new byte[5];

            byte[] output;

            using(var outStream = new MemoryStream())
            using(var inStream = new MemoryStream(input)) {
                var decoder = new NSoft.NFramework.Compressions.SevenZip.Compression.LZMA.LzmaDecoder();

                inStream.Seek(0, SeekOrigin.Begin);

                if(inStream.Read(properties2, 0, 5) != 5)
                    throw new InvalidOperationException("input 7Zip.LZMA is too short.");

                long outSize = 0;
                for(int i = 0; i < 8; i++) {
                    int v = inStream.ReadByte();
                    if(v < 0)
                        throw new InvalidOperationException("Can't Read 1");

                    outSize |= ((long)(byte)v) << (8 * i);
                }

                decoder.SetDecoderProperties(properties2);

                long compressedSize = inStream.Length - inStream.Position;
                decoder.Code(inStream, outStream, compressedSize, outSize, null);

                output = outStream.ToArray();
            }

            if(IsDebugEnabled)
                log.Debug(CompressorTool.SR.DecompressResultMsg, input.Length, output.Length, output.Length / (double)input.Length);

            return output ?? new byte[0];
        }

        #region << FIELDS >>

        private const int dictionary = 1 << 23;

        // static Int32 posStateBits = 2;
        // static  Int32 litContextBits = 3; // for normal files
        // UInt32 litContextBits = 0; // for 32-bit data
        // static  Int32 litPosBits = 0;
        // UInt32 litPosBits = 2; // for 32-bit data
        // static   Int32 algorithm = 2;
        // static    Int32 numFastBytes = 128;

#pragma warning disable 649
        private static bool eos;
#pragma warning restore 649

        private static readonly CoderPropID[] propIDs =
            {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            };

        // these are the default properties, keeping it simple for now:
        private static readonly object[] properties =
            {
                (dictionary),
                (2),
                (3),
                (0),
                (2),
                (128),
                "bt4",
                eos
            };

        #endregion
    }
}