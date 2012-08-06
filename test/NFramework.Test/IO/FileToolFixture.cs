using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.IO {
    [TestFixture]
    public class FileToolFixture : AbstractFixture {
        [Test]
        public void GetPhysicalPath() {
            var virtualPaths = new[] { "~/Hbm", ".\\Hbm", "Hbm", "./Hbm" };
            var physicalPaths = new string[virtualPaths.Length];

            for(int i = 0; i < virtualPaths.Length; i++) {
                physicalPaths[i] = FileTool.GetPhysicalPath(virtualPaths[i]);

                Console.WriteLine("{0} => {1}", virtualPaths[i], physicalPaths[i]);
                if(i > 0)
                    Assert.AreEqual(physicalPaths[i], physicalPaths[i - 1]);
            }
        }

        [Test]
        public void GetPathTest() {
            Console.WriteLine("Temp FilePath: {0}", FileTool.GetTempPath());
            Console.WriteLine("CurrentPath: {0}", FileTool.GetCurrentPath());
            Console.WriteLine("SystemPath: {0}", FileTool.GetSystemPath());
            Console.WriteLine("WindowsPath: {0}", FileTool.GetWindowsPath());
            Console.WriteLine("ProgramFilesPath: {0}", FileTool.GetProgramFilesPath());
        }

        [Test]
        public void CreateTempFileTest() {
            string tempFile = FileTool.GetTempFileName("Rw_");

            tempFile.CreateEmptyFile();
            Assert.IsTrue(tempFile.FileExists());

            tempFile.DeleteFile();
            Assert.IsFalse(tempFile.FileExists());
        }

        [Test]
        public void GetFileVersionTest() {
            var assemPath = Assembly.GetExecutingAssembly().Location;
            var assemFile = assemPath.ExtractFileName();

            Assert.IsTrue(assemPath.FileExists());
            Assert.IsNotEmpty(assemFile);

            Console.WriteLine("FileVersion:" + assemPath.GetFileVersion());
            Console.WriteLine("CreationTime:" + assemPath.GetFileCreateTime());
            Console.WriteLine("LastAccessTime:" + assemPath.GetFileLastWriteTime());
        }

        [Test]
        public void TestOfExtractFileInfo() {
            string assemPath = Assembly.GetExecutingAssembly().Location;
            string assemFile = assemPath.ExtractFileName();

            Assert.IsTrue(assemPath.FileExists());
            Assert.IsNotEmpty(assemFile);

            Console.WriteLine("");
            Console.WriteLine("FilePath : " + assemPath);
            Console.WriteLine("ExtractFilePath : " + assemPath.ExtractFilePath());
            Console.WriteLine("ExtractFileName : " + assemPath.ExtractFileName());
            Console.WriteLine("ExtractFileExt : " + assemPath.ExtractFileExt());
        }

        [Test]
        public void TestOfValidPath() {
            const string path = @"c:\temp\abc<>def|{}|gh.txt";
            string validPath = path.GetValidPath("_");

            Assert.IsTrue(validPath.Contains("_"));
            Console.WriteLine("Input FilePath={0}\tValidFilePathh={1}", path, validPath);
        }

        [Test]
        public void GetDirectorySizeTest() {
            string tempPath = FileTool.GetTempPath();
            long size = tempPath.GetDirectorySize(true);
            Console.Write(size.ToString("#,##0") + " bytes");
        }

        [Test]
        public void CopyAndMoveDirectory() {
            string srcPath = FileTool.GetTempPath();
            const string copyPath = @"C:\Temp\Test of CopyDirectory";
            const string movePath = @"C:\Temp\Test of MoveDirectory";

            srcPath.CopyDirectory(copyPath, true);
            Console.WriteLine("CopyDirectory(\"{0}\", \"{1}\", true) was done.", srcPath, copyPath);

            movePath.DeleteDirectory();

            copyPath.MoveDirectory(movePath);
            Console.WriteLine("MoveDirectory(\"{0}\", \"{1}\", true) was done.", copyPath, movePath);

            //
            // delete test directories
            //

            movePath.DeleteDirectory(true);
            Console.WriteLine("DeleteDirectory(\"{0}\") was done.", movePath);
        }

        [Test]
        public void SaveAndLoadTest() {
            const string s = "동해물과 백두산이 마르고 닳도록\r\n~~~~ Bravo my life ~~~";
            const string streamSave = @"C:\Temp\StreamSave.txt";

            const string textFile = @"C:\Temp\Text.txt";
            const string utf8File = @"C:\Temp\TextUTF8.txt";
            const string euckrFile = @"C:\Temp\TextEucKr.txt";

            FileTool.Save(streamSave, s.ToStream(), true);
            Console.WriteLine("FileTool.Save([{0}], StringTool.ToStream([{1}]), true)", streamSave, s);

            FileTool.Save(textFile, s, true);
            FileTool.Save(utf8File, s, true, Encoding.UTF8);
            FileTool.Save(euckrFile, s, true, Encoding.GetEncoding("euc-kr"));

            Console.WriteLine("FileTool.ToString({0}) = {1}", textFile, FileTool.ToString(textFile));
            Console.WriteLine("FileTool.ToString({0}) = {1}", utf8File, FileTool.ToString(utf8File));
            Console.WriteLine("FileTool.ToString({0}) = {1}", euckrFile, FileTool.ToString(euckrFile));

            textFile.DeleteFile();
            utf8File.DeleteFile();
            euckrFile.DeleteFile();
        }

        [Test]
        public void SearchTest() {
            var dirs = FileTool.GetDirectories(FileTool.GetTempPath(), "*");
            Console.Write("Dir: " + dirs.Length + "; ");

            var files = FileTool.GetFiles(FileTool.GetTempPath(), "*");
            Console.Write("File: " + files.Length + "; ");
        }

        [Test]
        public void ParsePathTest() {
            var path = Assembly.GetExecutingAssembly().Location;

            string root, dirName, fileName, ext;
            FileTool.ParsePath(path, out root, out dirName, out fileName, out ext);
            Console.WriteLine("remoteDir={0}", path);
            Console.WriteLine("root={0}, dirName={1}, filename={2}, ext={3}", root, dirName, fileName, ext);

            int index = fileName.LastIndexOf('.');
            if(index > 0)
                fileName = fileName.Substring(0, index);
            Console.WriteLine("remoteFile only = {0}", fileName);
        }

        [Test]
        public void ToBase64Test() {
            var di = new DirectoryInfo(FileTool.GetWindowsPath());
            var fileInfos = di.GetFiles("*.ini");

            if(fileInfos.Any()) {
                string srcFile = fileInfos[0].FullName;
                string destFile = Path.Combine(FileTool.GetWindowsPath(), "tested_file.txt");

                string base64String = srcFile.Base64Encode();
                Console.WriteLine("base64String : " + base64String);
                var buffer = base64String.Base64Decode();
                FileTool.Save(destFile, buffer, true);
                destFile.DeleteFile(true);
            }
        }

        [Test]
        public void GetMimeTypeTest() {
            Assert.AreEqual("text/html", "string.html".GetMime());
            Assert.AreEqual("text/xml", "data.xml".GetMime());
            Assert.AreEqual("image/jpeg", "image.jpg".GetMime());
            Assert.AreEqual("application/x-shockwave-flash", "line.swf".GetMime());

            Assert.AreEqual("application/octet-stream", "file.abcdefg".GetMime());
        }
    }
}