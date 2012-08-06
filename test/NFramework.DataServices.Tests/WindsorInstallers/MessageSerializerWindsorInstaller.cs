using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.Json;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.DataServices.WebHost.WindsorInstallers {
    public class MessageSerializerWindsorInstaller : IWindsorInstaller {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(CompressSerializer<>))
                    .Named("MessageSerializer.Northwind")
                    .DependsOn(Dependency.OnComponent("serializer", typeof(JsonByteSerializer<>)),
                               Dependency.OnComponent("compressor", typeof(SharpBZip2Compressor))),
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(EncryptSerializer<>))
                    .Named("MessageSerializer.Pubs")
                    .DependsOn(Dependency.OnComponent("serializer", "CompressSerializer.Pubs"),
                               Dependency.OnComponent("encryptor", typeof(AriaSymmetricEncryptor))),
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(CompressSerializer<>))
                    .Named("CompressSerializer.Pubs")
                    .DependsOn(Dependency.OnComponent("serializer", typeof(BsonSerializer<>)),
                               Dependency.OnComponent("compressor", typeof(SharpBZip2Compressor)))
                );
        }
    }
}