using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices {
    /// <summary>
    /// Data 처리를 수행하는 Command를 나타내는 인터페이스입니다.
    /// </summary>
    public interface IDataCommand {
        /// <summary>
        /// Command를 수행하고, 결과를 XML 문자열로 반환합니다.
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="requestItem">요청정보</param>
        /// <returns>Data 처리 결과의 XML 문자열</returns>
        string Execute(IAdoRepository repository, RequestItem requestItem);

        /// <summary>
        /// Name Mapper
        /// </summary>
        INameMapper NameMapper { get; set; }
    }
}