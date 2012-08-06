using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.Json;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.DataServices.Clients.WindsorInstallers {
    public class MessageSerializerWindsorInstaller : IWindsorInstaller {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(
                Component
                    .For<ISymmetricEncryptor>()
                    .ImplementedBy<AriaSymmetricEncryptor>(),
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(CompressSerializer<>))
                    .Named("MessageSerializer.Northwind")
                    .DependsOn(Dependency.OnComponent("serializer", typeof(JsonByteSerializer<>)),
                               Dependency.OnComponent("compressor", typeof(SharpBZip2Compressor))),
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(EncryptSerializer<>))
                    .Named("MessageSerializer.Pub")
                    .DependsOn(Dependency.OnComponent("serializer", "CompressSerializer.Pub"),
                               Dependency.OnComponent("encryptor", typeof(AriaSymmetricEncryptor))),
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(CompressSerializer<>))
                    .Named("CompressSerializer.Pub")
                    .DependsOn(Dependency.OnComponent("serializer", typeof(BsonSerializer<>)),
                               Dependency.OnComponent("compressor", typeof(SharpBZip2Compressor)))
                );
        }
    }
}