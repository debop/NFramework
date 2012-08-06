using System;
using System.Web.UI;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// <see cref="PageStatePersister"/>를 IoC를 통해 다양하게 제공하기 위해 Service Type으로 제공하기 위해 인터페이스를 정의합니다.
    /// </summary>
    public interface IServerPageStatePersister {
        /// <summary>
        /// Persister Type Name  
        /// </summary>
        string PersisterName { get; }

        /// <summary>
        /// ViewState를 압축 저장하기 위한 Compressor. Compressor가 지정되지 않았으면, 기본적으로 <see cref="SharpGZipCompressor"/>을 사용합니다.
        /// </summary>
        ICompressor Compressor { get; set; }

        /// <summary>
        /// 최소 압축 크기
        /// </summary>
        int CompressThreshold { get; set; }

        /// <summary>
        /// ViewState 유효 기간
        /// </summary>
        TimeSpan Expiration { get; set; }

        /// <summary>
        /// 저장된 Page의 ViewState 정보를 Load합니다.
        /// </summary>
        void Load();

        /// <summary>
        /// Page의 ViewState 정보를 원하는 장소(Cache, 파일, DB 등)에 저장합니다.
        /// </summary>
        void Save();
    }
}