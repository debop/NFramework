using System;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Core {
    [TestFixture]
    public class MongoGridFSFixture : MongoFixtureBase {
        private const string UploadFileName = "HelloWorld.txt";
        private const string UploadFileName2 = "HelloWorld2.txt";

        private const string Content = "동해물과 백두산이 마르고 닳도록!!! Hello World.";

        private static readonly byte[] ContentBytes = Encoding.UTF8.GetBytes(Content);

        private MongoGridFS gridFS;

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            gridFS = Database.GridFS;
            gridFS.Chunks.RemoveAll();
            gridFS.Chunks.ResetIndexCache();
            gridFS.Files.RemoveAll();
        }

        [Test]
        public void ConstructorFeezesSettingsTest() {
            var settings = new MongoGridFSSettings();
            settings.IsFrozen.Should().Be.False();

            var gridFS = new MongoGridFS(Database, settings);
            gridFS.Settings.IsFrozen.Should().Be.True();
        }

        [Test]
        public void CopyTo() {
            gridFS.Delete(Query.Null);
            gridFS.Chunks.Count().Should().Be(0);
            gridFS.Files.Count().Should().Be(0);

            var uploadStream = new MemoryStream(ContentBytes);
            var createOptions = new MongoGridFSCreateOptions
                                {
                                    Aliases = new[] { "애국가", "HelloWorld" },
                                    ChunkSize = gridFS.Settings.ChunkSize,
                                    ContentType = "text/plain",
                                    Id = ObjectId.GenerateNewId(),
                                    Metadata = new BsonDocument { { "a", 1 }, { "b", 2 } },
                                    UploadDate = DateTime.UtcNow
                                };

            var fileInfo = gridFS.Upload(uploadStream, "HelloWorld.txt", createOptions);
            fileInfo.Should().Not.Be.Null();
            var copyInfo = fileInfo.CopyTo("HelloWorld2.txt");
            copyInfo.Should().Not.Be.Null();

            gridFS.Chunks.Count().Should().Be(2); // 하나의 파일 크기가 ChunkSize 보다 작으므로
            gridFS.Files.Count().Should().Be(2);
            copyInfo.Aliases.Should().Be.Null(); // Alias는 복사되지 않습니다.

            copyInfo.ChunkSize.Should().Be(fileInfo.ChunkSize);
            copyInfo.ContentType.Should().Be(fileInfo.ContentType);
            copyInfo.Id.Should().Not.Be(fileInfo.Id);
            copyInfo.Length.Should().Be(fileInfo.Length);
            copyInfo.MD5.Should().Be(fileInfo.MD5);
            Assert.AreEqual(fileInfo.Metadata, copyInfo.Metadata);
            copyInfo.Name.Should().Be("HelloWorld2.txt");
            copyInfo.UploadDate.Should().Be(fileInfo.UploadDate);
        }

        [Test]
        public void AppendTextTest() {
            Assert.IsFalse(gridFS.Exists("HelloWorld.txt"));
            using(var writer = gridFS.AppendText("HelloWorld.txt")) {
                Assert.IsFalse(writer.BaseStream.CanRead);
                Assert.IsTrue(writer.BaseStream.CanSeek);
                Assert.IsTrue(writer.BaseStream.CanWrite);
                writer.Write("동해물과");
            }

            Assert.IsTrue(gridFS.Exists("HelloWorld.txt"));
            using(var writer = gridFS.AppendText("HelloWorld.txt")) {
                writer.Write(" 백두산이");
            }
            var memoryStream = new MemoryStream();
            gridFS.Download(memoryStream, "HelloWorld.txt");
            var bytes = memoryStream.ToArray();

            // 2-byte 문자열임을 나타내는 헤더값
            StringTool.IsMultiByteString(bytes).Should().Be.True();

            //Assert.AreEqual(0xEF, bytes[0]); // the BOM
            //Assert.AreEqual(0xBB, bytes[1]);
            //Assert.AreEqual(0xBF, bytes[2]);

            // Header 값을 빼고, 한다.
            // var text = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            var text = memoryStream.ToText(Encoding.UTF8);

            Assert.AreEqual("동해물과 백두산이", text);
        }

        [Test]
        public void DeleteByFileIdTest() {
            gridFS.Delete(Query.Null);

            Assert.AreEqual(0, gridFS.Chunks.Count());
            Assert.AreEqual(0, gridFS.Files.Count());

            var fileInfo = UploadHelloWord();
            Assert.AreEqual(1, gridFS.Chunks.Count());
            Assert.AreEqual(1, gridFS.Files.Count());

            gridFS.DeleteById(fileInfo.Id);
            Assert.AreEqual(0, gridFS.Chunks.Count());
            Assert.AreEqual(0, gridFS.Files.Count());
        }

        [Test]
        public void DeleteByFileNameTest() {
            gridFS.Delete(Query.Null);
            Assert.AreEqual(0, gridFS.Chunks.Count());
            Assert.AreEqual(0, gridFS.Files.Count());

            UploadHelloWord();
            Assert.AreEqual(1, gridFS.Chunks.Count());
            Assert.AreEqual(1, gridFS.Files.Count());

            gridFS.Delete(UploadFileName);
            Assert.AreEqual(0, gridFS.Chunks.Count());
            Assert.AreEqual(0, gridFS.Files.Count());
        }

        [Test]
        public void DeleteAllTest() {
            gridFS.Delete(Query.Null);
            Assert.AreEqual(0, gridFS.Chunks.Count());
            Assert.AreEqual(0, gridFS.Files.Count());
        }

        [Test]
        public void DownloadTest() {
            gridFS.Delete(Query.Null);

            var fileInfo = UploadHelloWord();

            using(var downloadStream = new MemoryStream()) {
                gridFS.Download(downloadStream, fileInfo);
                var downloadedBytes = downloadStream.ToArray();
                var downloadedContents = downloadStream.ToText(Encoding.UTF8);
                Assert.AreEqual(Content, downloadedContents);
            }
        }

        [Test]
        public void DownloadTwoChunksTest() {
            gridFS.Delete(Query.Null);
            var contents = new string('x', 256 * 1024) + new string('y', 256 * 1024);
            var bytes = Encoding.UTF8.GetBytes(contents);
            using(var stream = new MemoryStream(bytes)) {
                var fileInfo = gridFS.Upload(stream, "TwoChunks.txt");
                Assert.AreEqual(2 * fileInfo.ChunkSize, fileInfo.Length);
                Assert.AreEqual(2, gridFS.Chunks.Count());
                Assert.AreEqual(1, gridFS.Files.Count());

                using(var downloadStream = new MemoryStream()) {
                    gridFS.Download(downloadStream, fileInfo);
                    var downloadedBytes = downloadStream.ToArray();
                    var downloadedContents = Encoding.UTF8.GetString(downloadedBytes);
                    Assert.AreEqual(contents, downloadedContents);
                }
            }
        }

        [Test]
        public void ExistsTest() {
            gridFS.Delete(Query.Null);
            Assert.IsFalse(gridFS.Exists(UploadFileName));

            var fileInfo = UploadHelloWord();

            fileInfo.Should().Not.Be.Null();
            gridFS.Exists(UploadFileName).Should().Be.True();
            gridFS.ExistsById(fileInfo.Id).Should().Be.True();

            gridFS.Exists(UploadFileName2).Should().Be.False();
        }

        [Test]
        public void FindAllTest() {
            gridFS.Delete(Query.Null);
            Assert.IsFalse(gridFS.Exists(UploadFileName));

            var fileInfo = UploadHelloWord();

            gridFS.FindAll().RunEach(fi => fi.Should().Be(fileInfo));

            //foreach(var foundInfo in gridFS.FindAll())
            //{
            //    Assert.AreEqual(fileInfo, foundInfo);
            //}
        }

        [Test]
        public void FindByNameTest() {
            gridFS.Delete(Query.Null);
            Assert.IsFalse(gridFS.Exists(UploadFileName));

            var fileInfo = UploadHelloWord();

            gridFS.Find(UploadFileName).RunEach(fi => fi.Should().Be(fileInfo));
        }

        [Test]
        public void FindOneByIdTest() {
            gridFS.Delete(Query.Null);
            Assert.IsFalse(gridFS.Exists(UploadFileName));

            var fileInfo = UploadHelloWord();
            var foundInfo = gridFS.FindOneById(fileInfo.Id);
            Assert.AreEqual(fileInfo, foundInfo);
        }

        [Test]
        public void FindOneByNameTest() {
            gridFS.Delete(Query.Null);
            Assert.IsFalse(gridFS.Exists(UploadFileName));

            var fileInfo = UploadHelloWord();
            var foundInfo = gridFS.FindOne(UploadFileName);
            Assert.AreEqual(fileInfo, foundInfo);
        }

        [Test]
        public void FindOneNewestTest() {
            gridFS.Delete(Query.Null);
            Assert.IsFalse(gridFS.Exists(UploadFileName));

            var fileInfo1 = UploadHelloWord();
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            var fileInfo2 = UploadHelloWord();
            var foundInfo = gridFS.FindOne(UploadFileName, -1);
            Assert.AreNotEqual(fileInfo1, foundInfo);
            Assert.AreEqual(fileInfo2, foundInfo);
        }

        [Test]
        public void FindOneOldestTest() {
            gridFS.Delete(Query.Null);
            Assert.IsFalse(gridFS.Exists(UploadFileName));

            var fileInfo1 = UploadHelloWord();
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            var fileInfo2 = UploadHelloWord();

            var foundInfo = gridFS.FindOne(UploadFileName, 1);
            Assert.AreEqual(fileInfo1, foundInfo);
            Assert.AreNotEqual(fileInfo2, foundInfo);

            foundInfo = gridFS.FindOne(UploadFileName, -1);
            Assert.AreNotEqual(fileInfo1, foundInfo);
            Assert.AreEqual(fileInfo2, foundInfo);
        }

        [Test]
        public void MoveToTest() {
            gridFS.Delete(Query.Null);
            Assert.AreEqual(0, gridFS.Chunks.Count());
            Assert.AreEqual(0, gridFS.Files.Count());

            using(var uploadStream = new MemoryStream(ContentBytes)) {
                var fileInfo = gridFS.Upload(uploadStream, UploadFileName);
                Assert.AreEqual(1, gridFS.Chunks.Count());
                Assert.AreEqual(1, gridFS.Files.Count());

                gridFS.MoveTo(UploadFileName, UploadFileName2);
                Assert.AreEqual(1, gridFS.Chunks.Count());
                Assert.AreEqual(1, gridFS.Files.Count());
                var movedInfo = gridFS.FindOne(UploadFileName2);
                Assert.AreEqual(UploadFileName2, movedInfo.Name);
                Assert.AreEqual(fileInfo.Id, movedInfo.Id);
            }
        }

        [Test]
        public void SetAliasesTest() {
            var fileInfo = UploadHelloWord();
            Assert.IsNull(fileInfo.Aliases);

            var aliases = new[] { "a", "b" };
            gridFS.SetAliases(fileInfo, aliases);
            fileInfo.Refresh();
            Assert.IsTrue(aliases.SequenceEqual(fileInfo.Aliases));

            gridFS.SetAliases(fileInfo, null);
            fileInfo.Refresh();
            Assert.IsNull(fileInfo.Aliases);
        }

        [Test]
        public void SetContentTypeTest() {
            var fileInfo = UploadHelloWord();
            Assert.IsNull(fileInfo.ContentType);

            gridFS.SetContentType(fileInfo, "text/plain");
            fileInfo.Refresh();
            Assert.AreEqual("text/plain", fileInfo.ContentType);

            gridFS.SetContentType(fileInfo, null);
            fileInfo.Refresh();
            Assert.IsNull(fileInfo.ContentType);
        }

        [Test]
        public void SetMetadataTest() {
            var fileInfo = UploadHelloWord();
            Assert.IsNull(fileInfo.Metadata);

            var metadata = new BsonDocument { { "a", 1 }, { "b", 2 } };
            gridFS.SetMetadata(fileInfo, metadata);
            fileInfo.Refresh();
            Assert.AreEqual(metadata, fileInfo.Metadata);

            gridFS.SetMetadata(fileInfo, null);
            fileInfo.Refresh();
            Assert.IsNull(fileInfo.Metadata);
        }

        [Test]
        public void UploadTest() {
            gridFS.Delete(Query.Null);
            Assert.AreEqual(0, gridFS.Chunks.Count());
            Assert.AreEqual(0, gridFS.Files.Count());

            using(var uploadStream = new MemoryStream(ContentBytes)) {
                var createOptions = new MongoGridFSCreateOptions
                                    {
                                        Aliases = new[] { UploadFileName, "HelloUniverse" },
                                        ChunkSize = gridFS.Settings.ChunkSize,
                                        ContentType = "text/plain",
                                        Id = ObjectId.GenerateNewId(),
                                        Metadata = new BsonDocument { { "a", 1 }, { "b", 2 } },
                                        UploadDate = DateTime.UtcNow
                                    };
                var fileInfo = gridFS.Upload(uploadStream, UploadFileName, createOptions);
                Assert.AreEqual(1, gridFS.Chunks.Count());
                Assert.AreEqual(1, gridFS.Files.Count());
                Assert.IsTrue(createOptions.Aliases.SequenceEqual(fileInfo.Aliases));
                Assert.AreEqual(createOptions.ChunkSize, fileInfo.ChunkSize);
                Assert.AreEqual(createOptions.ContentType, fileInfo.ContentType);
                Assert.AreEqual(createOptions.Id, fileInfo.Id);
                Assert.AreEqual(ContentBytes.Length, fileInfo.Length);
                Assert.IsTrue(!string.IsNullOrEmpty(fileInfo.MD5));
                Assert.AreEqual(createOptions.Metadata, fileInfo.Metadata);
                Assert.AreEqual(UploadFileName, fileInfo.Name);
                Assert.AreEqual(createOptions.UploadDate.ToMongoDateTime(), fileInfo.UploadDate);
            }
        }

        private MongoGridFSFileInfo UploadHelloWord() {
            using(var stream = new MemoryStream(ContentBytes)) {
                return gridFS.Upload(stream, UploadFileName);
            }
        }
    }
}