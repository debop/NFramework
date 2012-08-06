using System;
using System.Linq;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.DataServices.Clients;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.DataServices.Adapters {
    public abstract class AbstractDataServiceAdapterFixture : AbstractServicesFixture {
        #region << IDataServiceAdapter Factory >>

        public virtual IDataServiceAdapter GetDataServiceAdapter() {
            return new DataServiceAdapter(GetDataService())
                   {
                       RequestSerializer = GetRequestSerializer(),
                       ResponseSerializer = GetResponseSerializer()
                   };
        }

        public virtual IDataServiceAdapter GetDataServiceAdapter(Func<IDataServiceAdapter, IDataServiceAdapter> adapterFunc) {
            var adapter = new DataServiceAdapter();

            if(adapterFunc != null)
                return adapterFunc(adapter);

            return adapter;
        }

        public virtual IDataService GetDataService() {
            return DataServiceTool.ResolveDataService();
        }

        public virtual ISerializer<RequestMessage> GetRequestSerializer() {
            return SerializerTool.CreateSerializer<RequestMessage>(SerializationOptions.Binary);
        }

        public virtual ISerializer<ResponseMessage> GetResponseSerializer() {
            return SerializerTool.CreateSerializer<ResponseMessage>(SerializationOptions.Binary);
        }

        #endregion

        public void VerifyResponseMessage(ResponseMessage responseMsg) {
            responseMsg.Should().Not.Be.Null();
            responseMsg.HasError.Should().Be.False();

            responseMsg.CreatedUtcTime.HasValue.Should().Be.True();
            responseMsg.Items.All(item => item.ExecutionTime.HasValue).Should("ExecutionTime.HasValue").Be.True();
        }

        public RequestMessage BuildRequest() {
            var requestMsg = CreateRequestMessage();

            requestMsg.AddItem(SQL_ORDER_DETAILS, ResponseFormatKind.ResultSet, 10, 10);
            requestMsg.AddItem(SQL_ORDERS, ResponseFormatKind.ResultSet, 20, 10);
            requestMsg.AddItem(SQL_CUSTOMERS, ResponseFormatKind.ResultSet, 30, 10);

            return requestMsg;
        }

        [Test]
        public void CreateAdapterTest() {
            GetDataServiceAdapter().Should().Not.Be.Null();

            GetDataServiceAdapter(null).Should().Not.Be.Null();

            var adapter = GetDataServiceAdapter(a => {
                                                    a.DataService = GetDataService();
                                                    return a;
                                                });

            adapter.DataService.Should().Not.Be.Null();
        }

        [Test]
        public void ExecutePlainBytes() {
            var adapter = GetDataServiceAdapter(null);
            var requestMsg = BuildRequest();

            var requestData = adapter.RequestSerializer.Serialize(requestMsg);
            requestData.Should().Not.Be.Null();

            var responseData = adapter.Execute(requestData);
            responseData.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseData);

            VerifyResponseMessage(responseMsg);
        }

        [Test]
        public void ExecutePlainText() {
            var adapter = GetDataServiceAdapter(null);
            var requestMsg = BuildRequest();

            var requestData = adapter.RequestSerializer.Serialize(requestMsg).Base64Encode();
            requestData.Should().Not.Be.NullOrEmpty();

            var responseData = adapter.Execute(requestData);
            responseData.Should().Not.Be.NullOrEmpty();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseData.Base64Decode());
            VerifyResponseMessage(responseMsg);
        }

        [TestCase(typeof(DeflateCompressor))]
        [TestCase(typeof(GZipCompressor))]
        [TestCase(typeof(SharpGZipCompressor))]
        [TestCase(typeof(SharpBZip2Compressor))]
        [TestCase(typeof(SevenZipCompressor))]
        [TestCase(typeof(IonicBZip2Compressor))]
        [TestCase(typeof(IonicDeflateCompressor))]
        [TestCase(typeof(IonicGZipCompressor))]
        [TestCase(typeof(IonicZlibCompressor))]
        public void ExecuteCompressedBytes(Type compressorType) {
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer = new CompressSerializer<RequestMessage>(
                                                        GetRequestSerializer(), compressor);
                                                    ba.ResponseSerializer =
                                                        new CompressSerializer<ResponseMessage>(GetResponseSerializer(), compressor);
                                                    return ba;
                                                });

            var requestMsg = BuildRequest();

            var requestData = adapter.RequestSerializer.Serialize(requestMsg);
            requestData.Should().Not.Be.Null();

            var responseData = adapter.Execute(requestData);
            responseData.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseData);

            VerifyResponseMessage(responseMsg);
        }

        [TestCase(typeof(DeflateCompressor))]
        [TestCase(typeof(GZipCompressor))]
        [TestCase(typeof(SharpGZipCompressor))]
        [TestCase(typeof(SharpBZip2Compressor))]
        [TestCase(typeof(SevenZipCompressor))]
        [TestCase(typeof(IonicBZip2Compressor))]
        [TestCase(typeof(IonicDeflateCompressor))]
        [TestCase(typeof(IonicGZipCompressor))]
        [TestCase(typeof(IonicZlibCompressor))]
        public void ExecuteCompressedText(Type compressorType) {
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer = new CompressSerializer<RequestMessage>(
                                                        GetRequestSerializer(), compressor);
                                                    ba.ResponseSerializer =
                                                        new CompressSerializer<ResponseMessage>(GetResponseSerializer(), compressor);
                                                    return ba;
                                                });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);

            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyResponseMessage(responseMsg);
        }

        [TestCase(typeof(AriaSymmetricEncryptor))]
        [TestCase(typeof(RC2SymmetricEncryptor))]
        [TestCase(typeof(TripleDESSymmetricEncryptor))]
        public void ExecuteEncryptedBytes(Type encryptorType) {
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer = new EncryptSerializer<RequestMessage>(
                                                        GetRequestSerializer(), encryptor);
                                                    ba.ResponseSerializer =
                                                        new EncryptSerializer<ResponseMessage>(GetResponseSerializer(), encryptor);
                                                    return ba;
                                                });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);
            requestBytes.Should().Not.Be.Null();

            var responseBytes = adapter.Execute(requestBytes);
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyResponseMessage(responseMsg);
        }

        [TestCase(typeof(AriaSymmetricEncryptor))]
        [TestCase(typeof(RC2SymmetricEncryptor))]
        [TestCase(typeof(TripleDESSymmetricEncryptor))]
        public void ExecuteEncryptedText(Type encryptorType) {
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer = new EncryptSerializer<RequestMessage>(
                                                        GetRequestSerializer(), encryptor);
                                                    ba.ResponseSerializer =
                                                        new EncryptSerializer<ResponseMessage>(GetResponseSerializer(), encryptor);
                                                    return ba;
                                                });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);

            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyResponseMessage(responseMsg);
        }

        //
        //! NOTE: SharpGZip 압축 후 Aria 암호화를 하면 예외가 발생하고, 밴다로 Aria 암호화 후 SharpGZip 압축을 하면 예외가 발생하지 않는다.
        //! NOTE: 앞으로는 항상 암호화 후에 압축을 하는 방식을 택하도록 한다.
        //

        [Test, Combinatorial]
        public void ExecuteCompressedEncryptedBytes([Values(typeof(GZipCompressor),
                                                        //typeof(SharpGZipCompressor),
                                                        typeof(SharpBZip2Compressor),
                                                        typeof(DeflateCompressor),
                                                        typeof(SevenZipCompressor),
                                                        typeof(IonicBZip2Compressor),
                                                        typeof(IonicDeflateCompressor),
                                                        typeof(IonicGZipCompressor),
                                                        typeof(IonicZlibCompressor))] Type compressorType,
                                                    [Values(typeof(AriaSymmetricEncryptor),
                                                        typeof(RC2SymmetricEncryptor),
                                                        typeof(RijndaelSymmetricEncryptor),
                                                        typeof(TripleDESSymmetricEncryptor))] Type encryptorType) {
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer =
                                                        new CompressSerializer<RequestMessage>(
                                                            new EncryptSerializer<RequestMessage>(GetRequestSerializer(), encryptor),
                                                            compressor);
                                                    ba.ResponseSerializer =
                                                        new CompressSerializer<ResponseMessage>(
                                                            new EncryptSerializer<ResponseMessage>(GetResponseSerializer(), encryptor),
                                                            compressor);
                                                    return ba;
                                                });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);
            requestBytes.Should().Not.Be.Null();

            var responseBytes = adapter.Execute(requestBytes);
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyResponseMessage(responseMsg);
        }

        [Test, Combinatorial]
        public void ExecuteCompressedEncryptedText([Values(typeof(GZipCompressor),
                                                       //typeof(SharpGZipCompressor),
                                                       typeof(SharpBZip2Compressor),
                                                       typeof(DeflateCompressor),
                                                       typeof(SevenZipCompressor),
                                                       typeof(IonicBZip2Compressor),
                                                       typeof(IonicDeflateCompressor),
                                                       typeof(IonicGZipCompressor),
                                                       typeof(IonicZlibCompressor))] Type compressorType,
                                                   [Values(typeof(AriaSymmetricEncryptor),
                                                       typeof(RC2SymmetricEncryptor),
                                                       typeof(RijndaelSymmetricEncryptor),
                                                       typeof(TripleDESSymmetricEncryptor))] Type encryptorType) {
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer =
                                                        new CompressSerializer<RequestMessage>(
                                                            new EncryptSerializer<RequestMessage>(GetRequestSerializer(), encryptor),
                                                            compressor);
                                                    ba.ResponseSerializer =
                                                        new CompressSerializer<ResponseMessage>(
                                                            new EncryptSerializer<ResponseMessage>(GetResponseSerializer(), encryptor),
                                                            compressor);
                                                    return ba;
                                                });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);


            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyResponseMessage(responseMsg);
        }

        [Test, Combinatorial]
        public void ExecuteEncryptedCompressedBytes([Values(typeof(GZipCompressor),
                                                        //typeof(SharpGZipCompressor),
                                                        typeof(SharpBZip2Compressor),
                                                        typeof(DeflateCompressor),
                                                        typeof(SevenZipCompressor),
                                                        typeof(IonicBZip2Compressor),
                                                        typeof(IonicDeflateCompressor),
                                                        typeof(IonicGZipCompressor),
                                                        typeof(IonicZlibCompressor))] Type compressorType,
                                                    [Values(typeof(AriaSymmetricEncryptor),
                                                        typeof(RC2SymmetricEncryptor),
                                                        typeof(RijndaelSymmetricEncryptor),
                                                        typeof(TripleDESSymmetricEncryptor))] Type encryptorType) {
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer =
                                                        new EncryptSerializer<RequestMessage>(
                                                            new CompressSerializer<RequestMessage>(GetRequestSerializer(), compressor),
                                                            encryptor);
                                                    ba.ResponseSerializer =
                                                        new EncryptSerializer<ResponseMessage>(
                                                            new CompressSerializer<ResponseMessage>(GetResponseSerializer(), compressor),
                                                            encryptor);
                                                    return ba;
                                                });


            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);
            requestBytes.Should().Not.Be.Null();

            var responseBytes = adapter.Execute(requestBytes);
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyResponseMessage(responseMsg);
        }

        [Test, Combinatorial]
        public void ExecuteEncryptedCompressedText([Values(typeof(GZipCompressor),
                                                       //typeof(SharpGZipCompressor),
                                                       typeof(SharpBZip2Compressor),
                                                       typeof(DeflateCompressor),
                                                       typeof(SevenZipCompressor),
                                                       typeof(IonicBZip2Compressor),
                                                       typeof(IonicDeflateCompressor),
                                                       typeof(IonicGZipCompressor),
                                                       typeof(IonicZlibCompressor))] Type compressorType,
                                                   [Values(typeof(AriaSymmetricEncryptor),
                                                       typeof(RC2SymmetricEncryptor),
                                                       typeof(RijndaelSymmetricEncryptor),
                                                       typeof(TripleDESSymmetricEncryptor))] Type encryptorType) {
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetDataServiceAdapter(ba => {
                                                    ba.DataService = GetDataService();
                                                    ba.RequestSerializer =
                                                        new EncryptSerializer<RequestMessage>(
                                                            new CompressSerializer<RequestMessage>(GetRequestSerializer(), compressor),
                                                            encryptor);
                                                    ba.ResponseSerializer =
                                                        new EncryptSerializer<ResponseMessage>(
                                                            new CompressSerializer<ResponseMessage>(GetResponseSerializer(), compressor),
                                                            encryptor);
                                                    return ba;
                                                });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);

            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyResponseMessage(responseMsg);
        }
    }
}