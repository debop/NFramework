using System;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// 파일 정보를 표현합니다.
    /// </summary>
    [Serializable]
    public class FileItem : ValueObjectBase, IEquatable<FileItem>
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public FileItem() { }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="clientFileName">클라이언트 파일명</param>
        /// <param name="serverFileName">서버에 저장된 파일명</param>
        public FileItem(string clientFileName, string serverFileName = null)
        {
            ClientFileName = clientFileName;
            ServerFileName = serverFileName;
        }

        /// <summary>
        /// 클라이언트 파일명
        /// </summary>
        public string ClientFileName { get; set; }

        /// <summary>
        /// 서버 저장 파일명
        /// </summary>
        public string ServerFileName { get; set; }

        /// <summary>
        /// MIME content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 파일 사이즈 (byte)
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        /// 파일명
        /// </summary>
        /// <returns>파일명</returns>
        public string GetName()
        {
            return ClientFileName.ExtractFileName();
        }

        /// <summary>
        /// 파일 확장자명
        /// </summary>
        /// <returns>확장자</returns>
        public string GetExtension()
        {
            return ClientFileName.ExtractFileExt();
        }

        public bool Equals(FileItem other)
        {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return HashTool.Compute(ClientFileName, ServerFileName);
        }

        public override string ToString()
        {
            return string.Format("FileItem# ClientFileName=[{0}], ServerFileName=[{1}], ContentType=[{2}]",
                                 ClientFileName, ServerFileName, ContentType);
        }
    }
}