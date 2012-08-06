using System;
using System.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Networks.Ftp {
    public abstract class AbstractFtpFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string FtpHost = "127.0.0.1:21";
        public const string FtpUsername = "anonymous";
        public const string FtpPassword = "debop@realweb21.com";

        //public const string FtpHost = "121.134.189.71:21";
        //public const string FtpUsername = "realweb";
        //public const string FtpPassword = "real21";

        public static readonly string[] Dirs = new[]
                                               {
                                                   "/Test 1 Step",
                                                   "/Test 1 Step/Test 2 Step",
                                                   "/한글 1 단계",
                                                   "/한글 1 단계/한글 2 단계"
                                               };

        public const string LocalFileHeader = @"C:\Temp\TestFile.Data.";
        public const string RemoteFileDir = "/파일_테스트";
        public const string RemoteFileHeader = RemoteFileDir + "/임시_파일.Data.";
        public const string MovedDirectoryName = "/Moved";

        public const int BufferSize = 50;
        public const int FileCount = 10;

        private readonly Lazy<FtpClient> lazyFtpClient = new Lazy<FtpClient>(() => new FtpClient(FtpHost, FtpUsername, FtpPassword),
                                                                             false);

        public FtpClient Client {
            get { return lazyFtpClient.Value; }
        }

        /// <summary>
        /// 테스트용 임시 파일을 생성한다.
        /// </summary>
        public FileInfo CreateTestFile(string filepath, long size) {
            var fi = new FileInfo(filepath);

            var buffer = new byte[BufferSize];

            ArrayTool.GetRandomBytes(buffer);

            using(var bs = new BufferedStream(fi.OpenWrite())) {
                long writeCount = 0;
                do {
                    bs.Write(buffer, 0, BufferSize);
                    writeCount += BufferSize;
                } while(size > writeCount);

                bs.Flush();
            }

            return fi;
        }

        public void DisplayEntries(string remotePath, FtpEntryKind entryKind) {
            var ftpDirInfo = Client.ListDirectoryDetail(remotePath);

            foreach(var ftpFI in ftpDirInfo) {
                if(ftpFI.FileKind == entryKind)
                    Console.WriteLine("FTP 서버 엔트리: {0}", ftpFI.FullName);
            }
        }
    }
}