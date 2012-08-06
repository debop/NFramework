using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Serializations;
using NUnit.Framework;

namespace NSoft.NFramework.DataServices.Adapters {
    [TestFixture]
    public class BsonDataServiceAdapterFixture : AbstractDataServiceAdapterFixture {
        public override ISerializer<RequestMessage> GetRequestSerializer() {
            return SerializerTool.CreateSerializer<RequestMessage>(SerializationOptions.Bson);
        }

        public override ISerializer<ResponseMessage> GetResponseSerializer() {
            return SerializerTool.CreateSerializer<ResponseMessage>(SerializationOptions.Bson);
        }
    }
}