using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Data.DevartOracle.SetUp {
    [TestFixture]
    public class IocSetupFixture : IoCSetupBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestCase("DefaultAdoRepository")]
        [TestCase("OracleRepository.Default")]
        [TestCase("OracleRepository.NSoft")]
        public void ResolveAdoRepository(string repositoryId) {
            var repository = IoC.Resolve<IAdoRepository>(repositoryId);

            Assert.IsNotNull(repository);
            Assert.IsNotNull(repository.QueryProvider);
            var queries = repository.QueryProvider.GetQueries();
            Assert.Greater(queries.Count, 0);

            if(IsDebugEnabled)
                log.Debug("Repository Id=[{0}], Type=[{1}]", repositoryId, repository.GetType().FullName);

            if(IsDebugEnabled) {
                log.Debug("Queries:");
                queries.RunEach(de => log.Debug("{0}=[{1}]", de.Key, de.Value));
            }
        }
    }
}