using System.Threading.Tasks;
using System.Web;
using NSoft.NFramework.FusionCharts.WebHost.Domain.Services;
using NSoft.NFramework.Web.HttpApplications;

namespace NSoft.NFramework.FusionCharts.WebHost {
    public class FusionChartHttpApplication : WindsorAsyncHttpApplication {
        /// <summary>
        /// Application_Start 시에 실행할 비동기 작업의 본체입니다.
        /// </summary>
        protected override void ApplicationStartAfter(HttpContext context) {
            base.ApplicationStartAfter(context);

            // 이 함수 자체가 비동기로 실행되는데, 거기다가 병렬로 미리 Factory 관련 Data를 로드합니다.
            //
            var masters = FactoryRepository.FindAllFactoryMaster();

            Parallel.ForEach(masters,
                             master => FactoryRepository.FindAllFactoryOutputByFactoryId(master.Id));
        }
    }
}