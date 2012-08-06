using System;
using System.IO;
using System.Linq;
using NSoft.NFramework.IO;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Networks.Ftp {
    [TestFixture]
    public class FtpClientFixture : AbstractFtpFixture {
        protected override void OnFixtureTearDown() {
            foreach(var dir in Dirs)
                Client.DeleteDirectory(dir, true);

            ClearFiles(Client, RemoteFileDir);
            Client.DeleteDirectory(RemoteFileDir);

            ClearFiles(Client, MovedDirectoryName);
            Client.DeleteDirectory(MovedDirectoryName);

            base.OnFixtureTearDown();
        }

        [Test]
        public void ClearTestDirectoryTest() {
            foreach(var dir in Dirs)
                Client.DeleteDirectory(dir, true);

            Dirs.All(dir => Client.DirectoryExists(dir) == false).Should().Be.True();
        }

        [Test]
        public void CreateDirectoryTest() {
            ClearTestDirectoryTest();

            foreach(var dir in Dirs)
                Client.CreateDirectory(dir).Should().Be.True();

            Dirs.All(dir => Client.DirectoryExists(dir)).Should().Be.True();

            Client.ListDirectoryDetail().RunEach(path => Console.WriteLine(path.FullName));
        }

        [Test]
        public void DeleteDirectory() {
            foreach(var dir in Dirs)
                Client.CreateDirectory(dir);

            foreach(var dir in Dirs)
                Client.DeleteDirectory(dir, true);

            Dirs.All(dir => Client.DirectoryExists(dir) == false).Should().Be.True();
        }

        [Test]
        public void ListDirectoryTest() {
            foreach(var dir in Dirs)
                Client.CreateDirectory(dir);

            var dirs = Client.ListDirectory();

            foreach(string dir in dirs)
                Console.WriteLine("Ftp Directory Name: " + dir);

            dirs = Client.ListDirectory("/");

            Client.KeepAlive = false;

            foreach(var dir in dirs)
                Console.WriteLine("Ftp Directory Name: " + dir);
        }

        [Test]
        public void ListDirectoryDetailTest() {
            CreateDirectoryTest();

            foreach(var dir in Dirs) {
                var dirInfo = Client.ListDirectoryDetail(dir);
                Assert.IsNotNull(dirInfo);
                dirInfo.GetDirectories().Count.Should().Be.GreaterThanOrEqualTo(0);
                dirInfo.GetFiles().Count.Should().Be(0);
            }
        }

        [Test]
        public void DeleteDirectoryTest() {
            foreach(string dir in Dirs)
                Client.DeleteDirectory(dir, true).Should().Be.True();
        }

        [Test]
        public void ClearFilesTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            Client.CreateDirectory(remoteDir);

            ClearFiles(Client, remoteDir);
            Client.ListDirectoryDetail(remoteDir).GetFiles().Count().Should().Be(0);

            UploadFiles(Client, remoteDir);
            Client.ListDirectoryDetail(remoteDir).GetFiles().Count().Should().Be(FileCount);

            ClearFiles(Client, remoteDir);
            Client.ListDirectoryDetail(remoteDir).GetFiles().Count().Should().Be(0);
        }

        [Test]
        public void FileTransferTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            Client.CreateDirectory(remoteDir);

            ClearFiles(Client, remoteDir);
            Client.ListDirectoryDetail(remoteDir).GetFiles().Count().Should().Be(0);

            UploadFiles(Client, remoteDir);
            Client.ListDirectoryDetail(remoteDir).GetFiles().Count().Should().Be(FileCount);

            DownloadFiles(Client, remoteDir);
        }

        /// <summary>
        /// //! NET 4.0에서는 Rename시에는 파일명만 전달해야 함  (버그임) - 이전 버전에서는 문제 없음
        /// http://stackoverflow.com/questions/4159903/problem-renaming-file-on-ftp-server-in-net-framework-4-0-only/5897531#5897531
        /// </summary>
        [Test]
        public void RenameTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            ClearFiles(Client, remoteDir);
            UploadFiles(Client, remoteDir);

            for(var i = 0; i < FileCount; i++) {
                var srcFile = RemoteFileHeader + i;
                var destFile = FileTool.ExtractFileName(srcFile + ".renamed");

                if(Client.FileExists(srcFile))
                    Client.RenameFile(srcFile, destFile).Should().Be.True();
            }

            DisplayEntries(remoteDir, FtpEntryKind.File);

            ClearFiles(Client, remoteDir);
        }

        /// <summary>
        /// //! NET 4.0에서는 Move시에는 대상파일은 원본파일의 상대경로 해야 함. (버그임) - 이전 버전에서는 문제 없음
        /// http://stackoverflow.com/questions/4159903/problem-renaming-file-on-ftp-server-in-net-framework-4-0-only/5897531#5897531
        /// </summary>
        [Test]
        public void MoveTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            ClearFiles(Client, remoteDir);
            UploadFiles(Client, remoteDir);

            Client.CreateDirectory(MovedDirectoryName);
            ClearFiles(Client, MovedDirectoryName);

            for(int i = 0; i < FileCount; i++) {
                var srcFile = RemoteFileHeader + i;
                var destFile = ".." + MovedDirectoryName + "/File.Data." + i;

                if(Client.FileExists(srcFile))
                    Client.RenameFile(srcFile, destFile).Should().Be.True();
            }

            DisplayEntries(remoteDir, FtpEntryKind.File);

            ClearFiles(Client, MovedDirectoryName);
            ClearFiles(Client, remoteDir);
        }

        [Test]
        public void DeleteFileTest() {
            UploadFiles(Client, RemoteFileDir);
            ClearFiles(Client, RemoteFileDir);
        }

        private void ClearFiles(FtpClient ftpClient, string remoteDir) {
            if(ftpClient.DirectoryExists(remoteDir) == false)
                return;

            foreach(var file in Client.ListDirectoryDetail(remoteDir).GetFiles()) {
                ftpClient.DeleteFile(file.FullName);
            }
        }

        private void UploadFiles(FtpClient ftpClient, string remoteDir) {
            if(!ftpClient.DirectoryExists(remoteDir))
                ftpClient.CreateDirectory(remoteDir);

            var fis = new FileInfo[FileCount];

            for(int i = 0; i < FileCount; i++) {
                string file = LocalFileHeader + i;

                if(!File.Exists(file))
                    fis[i] = CreateTestFile(LocalFileHeader + i, BufferSize * 1024 * (i + 1));
                else
                    fis[i] = new FileInfo(file);
            }

            for(int i = 0; i < FileCount; i++) {
                using(new OperationTimer("Upload File:" + fis[i].FullName))
                    ftpClient.Upload(fis[i], RemoteFileHeader + i).Should().Be.True();

                fis[i].Delete();
            }
        }

        private void DownloadFiles(FtpClient ftpClient, string remoteDir) {
            for(int i = 0; i < FileCount; i++) {
                var fi = new FileInfo(LocalFileHeader + i);

                using(new OperationTimer("Download File:" + fi.FullName))
                    ftpClient.Download(RemoteFileHeader + i, fi, true).Should().Be.True();
            }
        }
    }
}