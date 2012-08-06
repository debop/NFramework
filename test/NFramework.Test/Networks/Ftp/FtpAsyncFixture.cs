using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Parallelism.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Networks.Ftp {
    [TestFixture]
    public class FtpAsyncFixture : AbstractFtpFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected override void OnFixtureTearDown() {
            Task.WaitAll(Dirs.Select(dir => Client.DeleteDirectoryTask(dir, true)).ToArray());

            ClearFilesAsync(Client, RemoteFileDir);
            Client.DeleteDirectoryTask(RemoteFileDir).IgnoreExceptions().Wait();

            ClearFilesAsync(Client, MovedDirectoryName);
            Client.DeleteDirectoryTask(MovedDirectoryName).IgnoreExceptions().Wait();

            base.OnFixtureTearDown();
        }

        [Test]
        public void ClearTestDirectoryTest() {
            foreach(var dir in Dirs)
                Client.DeleteDirectoryTask(dir, true).Wait();

            Dirs.All(dir => Client.DirectoryExistsTask(dir).Result == false).Should().Be.True();
        }

        [Test]
        public void CreateDirectoryTest() {
            ClearTestDirectoryTest();

            foreach(var dir in Dirs)
                Client.CreateDirectoryTask(dir).Result.Should().Be.True();

            Dirs.All(dir => Client.DirectoryExistsTask(dir).Result).Should().Be.True();

            Client.ListDirectoryDetailTask().Result.RunEach(path => Console.WriteLine(path.FullName));
        }

        [Test]
        public void DeleteDirectory() {
            foreach(var dir in Dirs)
                Client.CreateDirectoryTask(dir).Wait();

            foreach(var dir in Dirs)
                Client.DeleteDirectoryTask(dir, true).Wait();

            Dirs.All(dir => Client.DirectoryExistsTask(dir).Result == false).Should().Be.True();
        }

        [Test]
        public void ListDirectoryTest() {
            foreach(var dir in Dirs)
                Client.CreateDirectoryTask(dir).Wait();

            var dirs = Client.ListDirectoryTask().Result;

            foreach(string dir in dirs)
                Console.WriteLine("Ftp Directory Name: " + dir);

            dirs = Client.ListDirectoryTask("/").Result;

            Client.KeepAlive = false;

            foreach(var dir in dirs)
                Console.WriteLine("Ftp Directory Name: " + dir);

            Client.KeepAlive = true;
        }

        [Test]
        public void ListDirectoryDetailTest() {
            CreateDirectoryTest();

            foreach(var dir in Dirs) {
                var dirInfo = Client.ListDirectoryDetailTask(dir).Result;

                Assert.IsNotNull(dirInfo);
                dirInfo.GetDirectories().Count.Should().Be.GreaterThanOrEqualTo(0);
                dirInfo.GetFiles().Count.Should().Be(0);
            }
        }

        [Test]
        public void DeleteDirectoryTest() {
            foreach(string dir in Dirs)
                Client.DeleteDirectoryTask(dir, true).Result.Should().Be.True();
        }

        [Test]
        public void ClearFilesTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            Client.CreateDirectoryTask(remoteDir).Wait();

            ClearFilesAsync(Client, remoteDir);
            Client.ListDirectoryDetailTask(remoteDir).Result.GetFiles().Count().Should().Be(0);

            UploadFilesAsync(Client, remoteDir);
            Client.ListDirectoryDetailTask(remoteDir).Result.GetFiles().Count().Should().Be(FileCount);

            // Thread.Sleep(100);

            // ClearFilesAsync(Client, remoteDir);
            // Client.ListDirectoryDetailTask(remoteDir).Result.GetFiles().Count().Should().Be(0);
        }

        [Test]
        public void FileTransferTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            Client.CreateDirectoryTask(remoteDir).Wait();

            ClearFilesAsync(Client, remoteDir);
            Client.ListDirectoryDetailTask(remoteDir).Result.GetFiles().Count().Should().Be(0);

            UploadFilesAsync(Client, remoteDir);
            Client.ListDirectoryDetailTask(remoteDir).Result.GetFiles().Count().Should().Be(FileCount);

            Thread.Sleep(100);

            DownloadFilesAsync(Client, remoteDir);
            Client.ListDirectoryDetailTask(remoteDir).Result.GetFiles().Count().Should().Be(FileCount);
        }

        [Test]
        public void FileSizeTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            ClearFilesAsync(Client, remoteDir);
            UploadFilesAsync(Client, remoteDir);

            for(int i = 0; i < FileCount; i++) {
                Client.GetFileSizeTask(RemoteFileHeader + i).Result.Should().Be.GreaterThan(0);
            }
        }

        /// <summary>
        /// //! NET 4.0에서는 Rename시에는 파일명만 전달해야 함  (버그임) - 이전 버전에서는 문제 없음
        /// http://stackoverflow.com/questions/4159903/problem-renaming-file-on-ftp-server-in-net-framework-4-0-only/5897531#5897531
        /// </summary>
        [Test]
        public void RenameTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            ClearFilesAsync(Client, remoteDir);
            UploadFilesAsync(Client, remoteDir);

            for(var i = 0; i < FileCount; i++) {
                var srcFile = RemoteFileHeader + i;
                var destFile = FileTool.ExtractFileName(srcFile + ".renamed");

                //if(Client.FileExistsTask(srcFile).Result)
                Client.RenameFileTask(srcFile, destFile).Result.Should().Be.True();
            }

            DisplayEntries(remoteDir, FtpEntryKind.File);

            ClearFilesAsync(Client, remoteDir);
        }

        /// <summary>
        /// //! NET 4.0에서는 Move시에는 대상파일은 원본파일의 상대경로 해야 함. (버그임) - 이전 버전에서는 문제 없음
        /// http://stackoverflow.com/questions/4159903/problem-renaming-file-on-ftp-server-in-net-framework-4-0-only/5897531#5897531
        /// </summary>
        [Test]
        public void MoveTest() {
            var remoteDir = Client.ExtractPath(RemoteFileHeader);

            ClearFilesAsync(Client, remoteDir);
            UploadFilesAsync(Client, remoteDir);

            Client.CreateDirectoryTask(MovedDirectoryName).Wait();
            ClearFilesAsync(Client, MovedDirectoryName);

            for(int i = 0; i < FileCount; i++) {
                var srcFile = RemoteFileHeader + i;
                var destFile = ".." + MovedDirectoryName + "/File.Data." + i;

                if(Client.FileExistsTask(srcFile).Result)
                    Client.RenameFileTask(srcFile, destFile).Result.Should().Be.True();
            }

            DisplayEntries(remoteDir, FtpEntryKind.File);

            ClearFilesAsync(Client, MovedDirectoryName);
            ClearFilesAsync(Client, remoteDir);
        }

        [Test]
        public void DeleteTest() {
            UploadFilesAsync(Client, RemoteFileDir);

            for(var i = 0; i < FileCount; i++) {
                Client.FileExistsTask(RemoteFileHeader + i).Result.Should().Be.True();
            }

            ClearFilesAsync(Client, RemoteFileDir);

            for(var i = 0; i < FileCount; i++) {
                Client.FileExistsTask(RemoteFileHeader + i).Result.Should().Be.False();
            }
        }

        private void ClearFilesAsync(FtpClient ftpClient, string remoteDir) {
            if(ftpClient.DirectoryExistsTask(remoteDir).Result == false)
                return;

            if(IsDebugEnabled)
                log.Debug("원격 디렉토리의 모든 파일을 삭제합니다!!! remote directory=[{0}]", remoteDir);

            var tasks =
                ftpClient
                    .ListDirectoryDetailTask(remoteDir).Result
                    .GetFiles()
                    .Select(file => ftpClient.DeleteFileTask(file.FullName))
                    .ToArray();

            Task.WaitAll(tasks);

            Thread.Sleep(10);
        }

        private void UploadFilesAsync(FtpClient ftpClient, string remoteDir) {
            if(ftpClient.DirectoryExistsTask(remoteDir).Result == false)
                ftpClient.CreateDirectoryTask(remoteDir).Wait();

            var fis = new FileInfo[FileCount];

            for(int i = 0; i < FileCount; i++) {
                string file = LocalFileHeader + i;

                if(File.Exists(file) == false)
                    fis[i] = CreateTestFile(LocalFileHeader + i, BufferSize * 1024 * (i + 1));
                else
                    fis[i] = new FileInfo(file);
            }

            var tasks = new List<Task<bool>>();

            for(int i = 0; i < FileCount; i++) {
                tasks.Add(ftpClient.UploadTask(fis[i], RemoteFileHeader + i));
            }
            Task.WaitAll(tasks.ToArray());
            tasks.All(t => t.Result).Should().Be.True();

            for(int i = 0; i < FileCount; i++)
                fis[i].Delete();

            Thread.Sleep(100);
        }

        private void DownloadFilesAsync(FtpClient ftpClient, string remoteDir) {
            var fis = new FileInfo[FileCount];
            for(var i = 0; i < FileCount; i++) {
                fis[i] = new FileInfo(LocalFileHeader + i + ".download");
            }

            var tasks = new List<Task<bool>>();
            for(int i = 0; i < FileCount; i++) {
                tasks.Add(ftpClient.DownloadTask(RemoteFileHeader + i, fis[i]));
            }

            Task.WaitAll(tasks.ToArray());
            tasks.All(t => t.Result).Should().Be.True();

            for(var i = 0; i < FileCount; i++) {
                fis[i].Delete();
            }
        }
    }
}