using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using NUnit.Framework;

namespace NSoft.NFramework.Data.MongoDB.Core {
    [TestFixture]
    public class MongoGridFSStreamFixture : MongoFixtureBase {
        private const string FileName = "test";

        private MongoGridFS gridFS;

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            var settings = new MongoGridFSSettings
                           {
                               ChunkSize = 16,
                               SafeMode = SafeMode.True
                           };
            gridFS = Database.GetGridFS(settings);
        }

        [Test]
        public void CreateZeroLengthFileTest() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var fileInfo = gridFS.FindOne(FileName);
            Assert.IsNull(fileInfo);

            using(var stream = gridFS.Create(FileName)) {
                Assert.IsTrue(stream.CanRead);
                Assert.IsTrue(stream.CanSeek);
                Assert.IsFalse(stream.CanTimeout);
                Assert.IsTrue(stream.CanWrite);
                Assert.AreEqual(0, stream.Length);
                Assert.AreEqual(0, stream.Position);

                fileInfo = gridFS.FindOne(FileName);
                Assert.IsTrue(fileInfo.Exists);
                Assert.IsNull(fileInfo.Aliases);
                Assert.AreEqual(FileName, fileInfo.Name);
                Assert.AreEqual(gridFS.Settings.ChunkSize, fileInfo.ChunkSize);
                Assert.IsNull(fileInfo.ContentType);
                Assert.AreEqual(0, fileInfo.Length);
                Assert.IsNull(fileInfo.MD5);
                Assert.IsNull(fileInfo.Metadata);
            }

            fileInfo = gridFS.FindOne(FileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.AreEqual(0, fileInfo.Length);
            Assert.IsNotNull(fileInfo.MD5);
        }

        [Test]
        public void CreateOneByteFileTest() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var fileInfo = gridFS.FindOne(FileName);
            Assert.IsNull(fileInfo);

            using(var stream = gridFS.Create(FileName)) {
                stream.WriteByte(1);
            }

            fileInfo = gridFS.FindOne(FileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.AreEqual(1, fileInfo.Length);
            Assert.IsNotNull(fileInfo.MD5);

            using(var stream = gridFS.OpenRead(FileName)) {
                var b = stream.ReadByte();
                Assert.AreEqual(1, b);
                b = stream.ReadByte();
                Assert.AreEqual(-1, b); // EOF
            }
        }

        [Test]
        public void Create3ChunkFileTest() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var fileInfo = gridFS.FindOne(FileName);
            Assert.IsNull(fileInfo);

            using(var stream = gridFS.Create(FileName)) {
                fileInfo = gridFS.FindOne(FileName);
                var bytes = new byte[fileInfo.ChunkSize * 3];
                stream.Write(bytes, 0, bytes.Length);
            }

            fileInfo = gridFS.FindOne(FileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.AreEqual(fileInfo.ChunkSize * 3, fileInfo.Length);
            Assert.IsNotNull(fileInfo.MD5);

            using(var stream = gridFS.OpenRead(FileName)) {
                var bytes = new byte[fileInfo.ChunkSize * 3];
                var bytesRead = stream.Read(bytes, 0, fileInfo.ChunkSize * 3);
                Assert.AreEqual(bytesRead, fileInfo.ChunkSize * 3);
                Assert.IsTrue(bytes.All(b => b == 0));

                bytesRead = stream.Read(bytes, 0, 1);
                Assert.AreEqual(0, bytesRead); // EOF
            }
        }

        [Test]
        public void Create3ChunkFile1ByteAtATimeTest() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var fileInfo = gridFS.FindOne(FileName);
            Assert.IsNull(fileInfo);

            using(var stream = gridFS.Create(FileName)) {
                fileInfo = gridFS.FindOne(FileName);

                for(int i = 0; i < fileInfo.ChunkSize * 3; i++) {
                    stream.WriteByte((byte)i);
                }
            }

            fileInfo = gridFS.FindOne(FileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.AreEqual(fileInfo.ChunkSize * 3, fileInfo.Length);
            Assert.IsNotNull(fileInfo.MD5);

            using(var stream = gridFS.OpenRead(FileName)) {
                for(int i = 0; i < fileInfo.ChunkSize * 3; i++) {
                    var b = stream.ReadByte();
                    Assert.AreEqual((byte)i, b);
                }
                var eof = stream.ReadByte();
                Assert.AreEqual(-1, eof);
            }
        }

        [Test]
        public void Create3ChunkFile14BytesAtATimeTest() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var fileInfo = gridFS.FindOne(FileName);
            Assert.IsNull(fileInfo);

            using(var stream = gridFS.Create(FileName)) {
                fileInfo = gridFS.FindOne(FileName);

                var bytes = new byte[] { 1, 2, 3, 4 };
                for(int i = 0; i < fileInfo.ChunkSize * 3; i += 4) {
                    stream.Write(bytes, 0, 4);
                }
            }

            fileInfo = gridFS.FindOne(FileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.AreEqual(fileInfo.ChunkSize * 3, fileInfo.Length);
            Assert.IsNotNull(fileInfo.MD5);

            using(var stream = gridFS.OpenRead(FileName)) {
                var expected = new byte[] { 1, 2, 3, 4 };
                var bytes = new byte[4];
                for(int i = 0; i < fileInfo.ChunkSize * 3; i += 4) {
                    var bytesRead = stream.Read(bytes, 0, 4);
                    Assert.AreEqual(4, bytesRead);
                    Assert.IsTrue(expected.SequenceEqual(bytes));
                }
                var eof = stream.Read(bytes, 0, 1);
                Assert.AreEqual(0, eof);
            }
        }

        [Test]
        [Ignore("Id를 선지정해서 Create하면 예외가 발생합니다. Upload를 해야 합니다.")]
        public void OpenCreateWithIdTest() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var createOptions = new MongoGridFSCreateOptions
                                {
                                    Id = 1
                                };

            using(var stream = gridFS.Create(FileName, createOptions)) {
                var bytes = new byte[] { 1, 2, 3, 4 };
                stream.Write(bytes, 0, 4);
            }

            var fileInfo = gridFS.FindOneById(FileName);
            Assert.AreEqual(BsonInt32.One, fileInfo.Id);
        }

        [Test]
        public void OpenCreateWithMetadataTest() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var metadata = new BsonDocument("author", "John Doe");
            var createOptions = new MongoGridFSCreateOptions
                                {
                                    Metadata = metadata
                                };
            using(var stream = gridFS.Create(FileName, createOptions)) {
                var bytes = new byte[] { 1, 2, 3, 4 };
                stream.Write(bytes, 0, 4);
            }

            var fileInfo = gridFS.FindOne(FileName);
            Assert.AreEqual(metadata, fileInfo.Metadata);
        }

        [Test]
        public void UpdateMD5Test() {
            gridFS.Files.RemoveAll();
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();

            var fileInfo = gridFS.FindOne(FileName);
            Assert.IsNull(fileInfo);

            using(var stream = gridFS.Create(FileName)) {
                var bytes = new byte[] { 1, 2, 3, 4 };
                stream.Write(bytes, 0, 4);
                stream.UpdateMD5 = false;
            }

            fileInfo = gridFS.FindOne(FileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.AreEqual(4, fileInfo.Length);
            Assert.IsNull(fileInfo.MD5);

            using(var stream = gridFS.Open(FileName, FileMode.Append, FileAccess.Write)) {
                var bytes = new byte[] { 1, 2, 3, 4 };
                stream.Write(bytes, 0, 4);
            }

            fileInfo = gridFS.FindOne(FileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.AreEqual(8, fileInfo.Length);
            Assert.IsNotNull(fileInfo.MD5);
        }
    }
}