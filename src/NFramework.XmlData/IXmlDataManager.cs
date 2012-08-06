using NSoft.NFramework.Data;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData {
    /// <summary>
    /// 요청문서를 받아 분석, 실행을 하고, 결과를 <see cref="XdsResponseDocument"/>를 빌드한다.
    /// </summary>
    /// <remarks>
    /// Client Application이 일반적인 SQL Query 문이나, Stored Procedure 호출을 ADO.NET을 통하지 않고, 
    /// WEB에서 수행할 수 있도록 요청정보를 분석하여 Database에 Query문을 수행하고, 결과를 XML Format으로 반환한다.
    /// </remarks>
    public interface IXmlDataManager {
        /// <summary>
        /// Data 처리 시에 사용할 <see cref="IAdoRepository"/> 인스턴스
        /// </summary>
        IAdoRepository Ado { get; set; }

        /// <summary>
        /// 요청정보를 실행하여 응답정보를 반환합니다.
        /// </summary>
        /// <param name="requestDocument">요청정보</param>
        /// <returns>요청 처리 응답정보</returns>
        XdsResponseDocument Execute(XdsRequestDocument requestDocument);
    }
}