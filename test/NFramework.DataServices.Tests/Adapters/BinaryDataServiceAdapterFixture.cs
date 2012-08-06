using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Serializations;
using NUnit.Framework;

namespace NSoft.NFramework.DataServices.Adapters {
    [TestFixture]
    public class BinaryDataServiceAdapterFixture : AbstractDataServiceAdapterFixture {
        public override ISerializer<RequestMessage> GetRequestSerializer() {
            return SerializerTool.CreateSerializer<RequestMessage>(SerializationOptions.Binary);
        }

        public override ISerializer<ResponseMessage> GetResponseSerializer() {
            return SerializerTool.CreateSerializer<ResponseMessage>(SerializationOptions.Binary);
        }
    }
}