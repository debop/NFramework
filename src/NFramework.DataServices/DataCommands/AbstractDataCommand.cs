using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.DataServices.DataCommands {
    /// <summary>
    /// Command 패턴을 이용하여, Data 처리를 수행합니다. Data 처리를 위한 Command 의 가장 추상적인 클래스입니다.
    /// </summary>
    public abstract class AbstractDataCommand : IDataCommand {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// WorkRepository
        /// </summary>
        protected IAdoRepository Repository { get; set; }

        /// <summary>
        /// Command를 수행하고, 결과를 XML 문자열로 반환합니다.
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="requestItem">요청 정보</param>
        /// <returns>Data 처리 결과의 XML 문자열</returns>
        public abstract string Execute(IAdoRepository repository, RequestItem requestItem);

        /// <summary>
        /// 실행에 필요한 인자정보를  <see cref="IAdoParameter"/> 형식으로 만들어 배열로 제공합니다.
        /// </summary>
        /// <param name="requestParameters">요청 정보의 인자들</param>
        /// <returns></returns>
        protected virtual IAdoParameter[] GetParameters(IEnumerable<RequestParameter> requestParameters) {
            return requestParameters.Select(p => new AdoParameter(p.Name, p.Value)).ToArray();
        }

        private INameMapper _nameMapper;

        /// <summary>
        /// Name Mapper	( BpaContext 로 이동 시켜야 함 )
        /// </summary>
        public INameMapper NameMapper {
            get { return _nameMapper ?? (_nameMapper = IoC.TryResolve<INameMapper, CapitalizeNameMapper>()); }
            set { _nameMapper = value; }
        }
    }
}