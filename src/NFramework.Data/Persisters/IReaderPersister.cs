using System.Data;

namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// <see cref="IDataReader"/>의 한 레코드를 읽어 TPersistent 형식의 object를 빌드합니다.
    /// </summary>
    /// <typeparam name="TPersistent">Type of persistent object</typeparam>
    public interface IReaderPersister<TPersistent> : IAdoPersister<IDataReader, TPersistent> {}
}