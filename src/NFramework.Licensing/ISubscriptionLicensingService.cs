using System.ServiceModel;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// Subscription Server의 Service Contract
    /// </summary>
    [ServiceContract]
    public interface ISubscriptionLicensingService {
        /// <summary>
        /// Issues a leased license
        /// </summary>
        /// <param name="previousLicense"></param>
        /// <returns></returns>
        [OperationContract]
        string LeaseLicense(string previousLicense);
    }
}