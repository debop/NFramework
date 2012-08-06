using System.Data;

namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// DataTable의 한 레코드를 TPersistent 형식의 object로 빌드한다.
    /// </summary>
    /// <typeparam name="TPersistent">Type of persistent object</typeparam>
    public interface IRowPersister<TPersistent> : IAdoPersister<DataRow, TPersistent> {}
}