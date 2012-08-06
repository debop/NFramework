using NSoft.NFramework.Data.OdpNet.EnterpriseLibrary;

namespace NSoft.NFramework.Data.OdpNet {
    public interface IOdpNetRepository : IAdoRepository {
        /// <summary>
        /// DAAB의 ODP.NET Oracle 용 Database 인스턴스
        /// </summary>
        new OdpNetDatabase Db { get; }
    }
}