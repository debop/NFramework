using System;
using System.IO;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Compressions {
    [Serializable]
    public class CompressAdapter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly object _syncLock = new object();

        private readonly Func<ICompressor> @_compressorFactory;
        private ICompressor _compressor;

        public CompressAdapter() : this(() => new SharpGZipCompressor()) {}
        public CompressAdapter(ICompressor compressor) : this(() => compressor ?? new SharpGZipCompressor()) {}

        public CompressAdapter(Func<ICompressor> @compressorFactory) {
            @compressorFactory.ShouldNotBeNull("compressorFactory");
            @_compressorFactory = @compressorFactory;
        }

        /// <summary>
        /// 사용할 Compressor
        /// </summary>
        public ICompressor BaseCompressor {
            get {
                if(_compressor == null)
                    lock(_syncLock)
                        if(_compressor == null) {
                            var compressor = @_compressorFactory();
                            System.Threading.Thread.MemoryBarrier();
                            _compressor = compressor;
                        }

                return _compressor;
            }
        }

        /// <summary>
        /// 바이트 배열 데이터를 압축합니다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual byte[] Compress(byte[] input) {
            return BaseCompressor.Compress(input);
        }

        /// <summary>
        /// 바이트 배열 데이터를 복원합니다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual byte[] Decompress(byte[] input) {
            return BaseCompressor.Decompress(input);
        }

        /// <summary>
        /// 지정된 스트림을 압축합니다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual byte[] Compress(Stream input) {
            input.ShouldNotBeNull("input");
            return Compress(input.ToBytes());
        }

        /// <summary>
        /// 지정한 데이터를 압축하여, <paramref name="destinationStream"/> 에 저장합니다.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="destinationStream"></param>
        public virtual void Decompress(byte[] input, Stream destinationStream) {
            input.ShouldNotBeNull("input");
            destinationStream.ShouldNotBeNull("destinationStream");

            var decompressedBytes = Decompress(input);
            destinationStream.Write(decompressedBytes, 0, decompressedBytes.Length);
        }

        /// <summary>
        /// 원본 스트림을 압축하여, 대상 스트림에 씁니다.
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="destinationStream"></param>
        public virtual void CompressAsync(Stream sourceStream, Stream destinationStream) {
            sourceStream.ShouldNotBeNull("sourceStream");
            destinationStream.ShouldNotBeNull("destinationStream");

            var compressedBytes = Compress(sourceStream.ToBytes());
            destinationStream.WriteAsync(compressedBytes, 0, compressedBytes.Length);
        }

        /// <summary>
        /// 원본 스트림을 압축 해제하여, 대상 스트림에 씁니다.
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="targetStream"></param>
        public virtual void DecompressAsync(Stream sourceStream, Stream targetStream) {
            sourceStream.ShouldNotBeNull("sourceStream");
            targetStream.ShouldNotBeNull("destinationStream");

            var decompresedBytes = Decompress(sourceStream.ToBytes());
            targetStream.WriteAsync(decompresedBytes, 0, decompresedBytes.Length);
            targetStream.SetStreamPosition();
        }
    }
}