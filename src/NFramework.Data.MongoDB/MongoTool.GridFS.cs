using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.GridFS;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.MongoDB {
    public static partial class MongoTool {
        internal static ICompressor DefaultFileCompressor = new SharpGZipCompressor();

        private static BinarySerializer<CompressedFileItem> _fileSerializer;

        internal static BinarySerializer<CompressedFileItem> FileSerializer {
            get {
                if(_fileSerializer == null)
                    lock(_syncLock)
                        if(_fileSerializer == null) {
                            var serializer = new BinarySerializer<CompressedFileItem>();
                            Thread.MemoryBarrier();
                            _fileSerializer = serializer;
                        }

                return _fileSerializer;
            }
        }

        public static MongoGridFSFileInfo UploadFileWithCompress(this IMongoRepository repository, byte[] data, string remoteFilename) {
            return UploadFileWithCompress(repository, DefaultFileCompressor, data, remoteFilename);
        }

        public static MongoGridFSFileInfo UploadFileWithCompress(this IMongoRepository repository, ICompressor compressor, byte[] data,
                                                                 string remoteFilename) {
            compressor.ShouldNotBeNull("compressor");

            var entry = new CompressedFileItem(compressor.GetType(), compressor.Compress(data));

            using(var stream = new MemoryStream(FileSerializer.Serialize(entry))) {
                return repository.UploadFile(remoteFilename, stream);
            }
        }

        public static Task<MongoGridFSFileInfo> UploadFileWithCompressTask(this IMongoRepository repository, byte[] data,
                                                                           string remoteFilename) {
            return UploadFileWithCompressTask(repository, DefaultFileCompressor, data, remoteFilename);
        }

        public static Task<MongoGridFSFileInfo> UploadFileWithCompressTask(this IMongoRepository repository, ICompressor compressor,
                                                                           byte[] data, string remoteFilename) {
            return Task.Factory.StartNew(() => UploadFileWithCompress(repository, compressor, data, remoteFilename),
                                         TaskCreationOptions.PreferFairness);
        }

        [Serializable]
        internal class CompressedFileItem {
            private static readonly byte[] EmptyData = new byte[0];

            public CompressedFileItem(Type compressorType, byte[] compressedData) {
                CompressorType = compressorType;
                CompressedData = compressedData;
            }

            public Type CompressorType { get; protected set; }

            public byte[] CompressedData { get; protected set; }

            public byte[] GetDecompressData() {
                if(CompressedData == null)
                    return EmptyData;

                if(CompressorType == null)
                    return EmptyData;

                try {
                    var compressor = ActivatorTool.CreateInstance(CompressorType) as ICompressor;

                    if(compressor == null)
                        return EmptyData;

                    return compressor.Decompress(CompressedData);
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled)
                        log.ErrorException("압축하여 저장한 정보를 압축 해제하는데 실패했습니다", ex);
                }
                return EmptyData;
            }
        }
    }
}