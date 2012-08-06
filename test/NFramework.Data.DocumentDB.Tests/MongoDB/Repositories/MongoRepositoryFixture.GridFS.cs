using System;
using System.IO;
using System.Linq;
using System.Threading;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.Repositories {
    [TestFixture]
    public class MongoRepositoryFixture_GridFS : MongoFixtureBase {
        private readonly object _syncLock = new object();

        private MongoRepositoryImpl _repository;

        public MongoRepositoryImpl Repository {
            get {
                if(_repository == null)
                    lock(_syncLock)
                        if(_repository == null) {
                            var repository = new MongoRepositoryImpl(DefaultConnectionString);
                            Thread.MemoryBarrier();
                            _repository = repository;
                        }
                return _repository;
            }
        }

        public const string NationalSong = "동해물과 백두산이 ";

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            Repository.DeleteAllFile();
        }

        protected override void OnTestFixtureTearDown() {
            base.OnTestFixtureTearDown();

            Repository.DeleteAllFile();
        }

        private static Stream CreateFileStream() {
            return NationalSong.Replicate(100).ToStream();
        }

        private MongoGridFSFileInfo UploadTestFile() {
            var filename = "filename_" + Guid.NewGuid().ToString();

            using(var stream = CreateFileStream()) {
                return Repository.UploadFile(filename, stream);
            }
        }

        [Test]
        public void DownloadFileByNameTest() {
            var fi = UploadTestFile();

            using(var stream = new MemoryStream()) {
                Repository.DownloadFile(stream, fi.Name);

                stream.SetStreamPosition();
                stream.ToText().Should().Contain(NationalSong);
            }
        }

        [Test]
        public void DownloadFileByFileInfoTest() {
            var fi = UploadTestFile();
            using(var stream = new MemoryStream()) {
                Repository.DownloadFile(stream, fi);

                stream.SetStreamPosition();
                stream.ToText().Should().Contain(NationalSong);
            }
        }

        [Test]
        public void DownloadFileWithQueryTest() {
            Repository.DeleteAllFile();

            var fi = UploadTestFile();
            using(var stream = new MemoryStream()) {
                Repository.DownloadFile(stream, Query.EQ("filename", fi.Name));

                stream.SetStreamPosition();
                stream.ToText().Should().Contain(NationalSong);
            }

            using(var stream = new MemoryStream()) {
                Repository.DownloadFile(stream, Query.GT("length", 0));

                stream.SetStreamPosition();
                stream.ToText().Should().Contain(NationalSong);
            }
        }

        [Test]
        public void OpenFileReadTest() {
            var fi = UploadTestFile();

            using(var gridFSStream = Repository.OpenFile(fi.Name)) {
                gridFSStream.ToText().Should().Contain(NationalSong);
            }
        }

        [Test]
        public void OpenFileTextTest() {
            var fi = UploadTestFile();

            using(var reader = Repository.OpenFileText(fi.Name)) {
                reader.ReadToEnd().Should().Contain(NationalSong);
            }
        }

        [Test]
        public void UploadFileTest() {
            var fi = UploadTestFile();
            fi.Should().Not.Be.Null();

            Repository.FileExists(fi.Name).Should().Be.True();
            Repository.FileExists(Query.EQ("filename", fi.Name)).Should().Be.True();
            Repository.FileExists(Query.GT("length", 0)).Should().Be.True();
        }

        [Test]
        public void UploadFileWithCreateOptionsTest() {
            var fi = UploadTestFile();
            fi.Should().Not.Be.Null();

            Repository.FileExists(fi.Name).Should().Be.True();
            Repository.FileExists(Query.EQ("filename", fi.Name)).Should().Be.True();
            Repository.FileExists(Query.GT("length", 0)).Should().Be.True();
        }

        [Test]
        public void FileExistsTest() {
            var fi = UploadTestFile();

            Repository.FileExists(fi.Name).Should().Be.True();
            Repository.FileExists(Query.EQ("filename", fi.Name)).Should().Be.True();
            Repository.FileExists(Query.GT("length", 0)).Should().Be.True();
        }

        [Test]
        public void FindOneFileByNameTest() {
            var fi = UploadTestFile();

            var foundfile = Repository.FindOneFile(fi.Name, -1);

            foundfile.Should().Not.Be.Null();
            foundfile.Should().Be(fi);
        }

        [Test]
        public void FindOneFileByIdTest() {
            var fi = UploadTestFile();

            var foundfile = Repository.FindOneFileById(fi.Id);

            foundfile.Should().Not.Be.Null();
            foundfile.Should().Be(fi);
        }

        [Test]
        public void FindFileByNameTest() {
            Repository.DeleteAllFile();

            var fi = UploadTestFile();

            var files = Repository.FindFile(fi.Name).ToList();

            files.Count.Should().Be(1);
            files[0].Should().Be(fi);
        }

        [Test]
        public void FindFileWithQueryTest() {
            Repository.DeleteAllFile();

            var fi = UploadTestFile();

            var files = Repository.FindFile(Query.EQ("filename", fi.Name)).ToList();

            files.Count.Should().Be(1);
            files[0].Should().Be(fi);

            files = Repository.FindFile(Query.GT("length", 0)).ToList();

            files.Count.Should().Be(1);
            files[0].Should().Be(fi);
        }

        [Test]
        public void DeleteFileTest() {
            Repository.DeleteAllFile();

            var fi = UploadTestFile();
            fi.Should().Not.Be.Null();

            Repository.DeleteFile(fi.Name);
            Repository.FileExists(fi.Name).Should().Be.False();
        }

        [Test]
        public void DeleteAllFileTest() {
            for(int i = 0; i < 100; i++)
                UploadTestFile();

            Repository.DeleteAllFile();

            Repository.FindAllFile().Count().Should().Be(0);
        }
    }
}