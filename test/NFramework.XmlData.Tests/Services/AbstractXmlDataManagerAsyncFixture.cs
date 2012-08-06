using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData.Services {
    public abstract class AbstractXmlDataManagerAsyncFixture : AbstractXmlDataManagerFixture {
        protected override bool IsParallel {
            get { return true; }
        }

        protected override XdsRequestDocument CreateRequestDocument() {
            return new XdsRequestDocument
                   {
                       Transaction = false,
                       IsParallelToolecute = IsParallel
                   };
        }
    }
}