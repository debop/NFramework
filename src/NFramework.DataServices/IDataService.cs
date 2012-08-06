using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices {
    /// <summary>
    /// JSON Data Service의 인터페이스
    /// </summary>
    public interface IDataService {
        /// <summary>
        /// Data 처리 시에 사용할 <see cref="IAdoRepository"/> 인스턴스
        /// </summary>
        IAdoRepository AdoRepository { get; set; }

        /// <summary>
        /// Database 컬럼명을 Class 의 속성명으로 매핑해주는 Mapper입니다.
        /// </summary>
        /// <seealso cref="CapitalizeNameMapper"/>
        /// <seealso cref="TrimNameMapper"/>
        INameMapper NameMapper { get; set; }

        /// <summary>
        /// DATA 처리를 위한 요청정보를 처리해서, 응답정보를 빌드해서 반환합니다.
        /// </summary>
        /// <param name="requestMessage">요청 메시지</param>
        /// <returns>응답 메시지</returns>
        ResponseMessage Execute(RequestMessage requestMessage);
    }
}