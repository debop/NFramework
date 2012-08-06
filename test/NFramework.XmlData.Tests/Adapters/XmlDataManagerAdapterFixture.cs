using System;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.Tools;
using NSoft.NFramework.XmlData.Messages;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.XmlData.Adapters {
    [TestFixture]
    public class XmlDataManagerAdapterFixture : AbstractXmlDataFixture {
        #region << IXmlDataManagerAdapter Factory >>

        public virtual IXmlDataManagerAdapter GetXmlDataManagerAdapter() {
            return new XmlDataManagerAdapter(GetXmlDataManager())
                   {
                       RequestSerializer = GetRequestSerializer(),
                       ResponseSerializer = GetResponseSerializer()
                   };
        }

        public virtual IXmlDataManagerAdapter GetXmlDataManagerAdapter(Func<IXmlDataManagerAdapter, IXmlDataManagerAdapter> adapterFunc) {
            var adapter = new XmlDataManagerAdapter();

            if(adapterFunc != null)
                return adapterFunc(adapter);

            return adapter;
        }

        public virtual IXmlDataManager GetXmlDataManager() {
            return XmlDataTool.ResolveXmlDataManager();
        }

        public virtual ISerializer<XdsRequestDocument> GetRequestSerializer() {
            return XmlDataTool.ResolveRequestSerializer();
        }

        public virtual ISerializer<XdsResponseDocument> GetResponseSerializer() {
            return XmlDataTool.ResolveResponseSerializer();
        }

        public XdsRequestDocument BuildRequest() {
            var requestDoc = CreateRequestDocument();

            requestDoc.AddQuery("SELECT * FROM [Order Details]", XmlDataResponseKind.DataSet, 10, 2);
            requestDoc.AddQuery("SELECT * FROM [Order Details]", XmlDataResponseKind.DataSet, 10, 3);
            requestDoc.AddQuery("SELECT * FROM [Order Details]", XmlDataResponseKind.DataSet, 10, 4);
            requestDoc.AddQuery("SELECT * FROM Orders", XmlDataResponseKind.DataSet, 20, 2);

            return requestDoc;
        }

        protected virtual XdsRequestDocument CreateRequestDocument() {
            return new XdsRequestDocument
                   {
                       Transaction = false,
                       IsParallelToolecute = true
                   };
        }

        #endregion

        public void VerifyXdsResponseDocument(XdsResponseDocument responseMsg) {
            responseMsg.Should().Not.Be.Null();
            responseMsg.HasError.Should().Be.False();
        }

        [Test]
        public void CreateAdapterTest() {
            GetXmlDataManagerAdapter().Should().Not.Be.Null();

            GetXmlDataManagerAdapter(null).Should().Not.Be.Null();

            var adapter = GetXmlDataManagerAdapter(a => {
                                                       a.XmlDataManager = GetXmlDataManager();
                                                       return a;
                                                   });

            adapter.XmlDataManager.Should().Not.Be.Null();
        }

        [Test]
        public void ExecutePlainBytes() {
            var adapter = GetXmlDataManagerAdapter();
            var requestMsg = BuildRequest();

            var requestData = adapter.RequestSerializer.Serialize(requestMsg);
            requestData.Should().Not.Be.Null();

            var responseData = adapter.Execute(requestData);
            responseData.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseData);

            VerifyXdsResponseDocument(responseMsg);
        }

        [Test]
        public void ExecutePlainText() {
            var adapter = GetXmlDataManagerAdapter();
            var requestMsg = BuildRequest();

            var requestData = adapter.RequestSerializer.Serialize(requestMsg);
            requestData.Should().Not.Be.Null();

            var responseData = adapter.Execute(requestData.Base64Encode()).Base64Decode();
            responseData.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseData);
            VerifyXdsResponseDocument(responseMsg);
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

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new CompressSerializer<XdsRequestDocument>(GetRequestSerializer(), compressor);
                                                       ba.ResponseSerializer =
                                                           new CompressSerializer<XdsResponseDocument>(GetResponseSerializer(),
                                                                                                       compressor);
                                                       return ba;
                                                   });

            var requestMsg = BuildRequest();

            var requestData = adapter.RequestSerializer.Serialize(requestMsg);
            requestData.Should().Not.Be.Null();

            var responseData = adapter.Execute(requestData);
            responseData.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseData);

            VerifyXdsResponseDocument(responseMsg);
        }

        [TestCase(typeof(GZipCompressor))]
        //[TestCase(typeof(DeflateCompressor))]
        //[TestCase(typeof(SharpGZipCompressor))]
        //[TestCase(typeof(SharpBZip2Compressor))]
        //[TestCase(typeof(SevenZipCompressor))]
        //[TestCase(typeof(IonicBZip2Compressor))]
        //[TestCase(typeof(IonicDeflateCompressor))]
        //[TestCase(typeof(IonicGZipCompressor))]
        //[TestCase(typeof(IonicZlibCompressor))]
        public void ExecuteCompressedText(Type compressorType) {
            var compressor = (ICompressor)ActivatorTool.CreateInstance(compressorType);

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new CompressSerializer<XdsRequestDocument>(GetRequestSerializer(), compressor);
                                                       ba.ResponseSerializer =
                                                           new CompressSerializer<XdsResponseDocument>(GetResponseSerializer(),
                                                                                                       compressor);
                                                       return ba;
                                                   });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);
            requestBytes.Should().Not.Be.Null();

            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyXdsResponseDocument(responseMsg);
        }

        [TestCase(typeof(AriaSymmetricEncryptor))]
        [TestCase(typeof(RC2SymmetricEncryptor))]
        [TestCase(typeof(TripleDESSymmetricEncryptor))]
        public void ExecuteEncryptedBytes(Type encryptorType) {
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new EncryptSerializer<XdsRequestDocument>(GetRequestSerializer(), encryptor);
                                                       ba.ResponseSerializer =
                                                           new EncryptSerializer<XdsResponseDocument>(GetResponseSerializer(), encryptor);
                                                       return ba;
                                                   });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);
            requestBytes.Should().Not.Be.Null();

            var responseBytes = adapter.Execute(requestBytes);
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyXdsResponseDocument(responseMsg);
        }

        [TestCase(typeof(AriaSymmetricEncryptor))]
        [TestCase(typeof(RC2SymmetricEncryptor))]
        [TestCase(typeof(TripleDESSymmetricEncryptor))]
        public void ExecuteEncryptedText(Type encryptorType) {
            var encryptor = (ISymmetricEncryptor)ActivatorTool.CreateInstance(encryptorType);

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new EncryptSerializer<XdsRequestDocument>(GetRequestSerializer(), encryptor);
                                                       ba.ResponseSerializer =
                                                           new EncryptSerializer<XdsResponseDocument>(GetResponseSerializer(), encryptor);
                                                       return ba;
                                                   });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);

            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyXdsResponseDocument(responseMsg);
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

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new CompressSerializer<XdsRequestDocument>(
                                                               new EncryptSerializer<XdsRequestDocument>(GetRequestSerializer(),
                                                                                                         encryptor), compressor);
                                                       ba.ResponseSerializer =
                                                           new CompressSerializer<XdsResponseDocument>(
                                                               new EncryptSerializer<XdsResponseDocument>(GetResponseSerializer(),
                                                                                                          encryptor), compressor);
                                                       return ba;
                                                   });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);
            requestBytes.Should().Not.Be.Null();

            var responseBytes = adapter.Execute(requestBytes);
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyXdsResponseDocument(responseMsg);
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

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new CompressSerializer<XdsRequestDocument>(
                                                               new EncryptSerializer<XdsRequestDocument>(GetRequestSerializer(),
                                                                                                         encryptor), compressor);
                                                       ba.ResponseSerializer =
                                                           new CompressSerializer<XdsResponseDocument>(
                                                               new EncryptSerializer<XdsResponseDocument>(GetResponseSerializer(),
                                                                                                          encryptor), compressor);
                                                       return ba;
                                                   });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);


            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyXdsResponseDocument(responseMsg);
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

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new EncryptSerializer<XdsRequestDocument>(
                                                               new CompressSerializer<XdsRequestDocument>(GetRequestSerializer(),
                                                                                                          compressor), encryptor);
                                                       ba.ResponseSerializer =
                                                           new EncryptSerializer<XdsResponseDocument>(
                                                               new CompressSerializer<XdsResponseDocument>(GetResponseSerializer(),
                                                                                                           compressor), encryptor);
                                                       return ba;
                                                   });


            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);
            requestBytes.Should().Not.Be.Null();

            var responseBytes = adapter.Execute(requestBytes);
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyXdsResponseDocument(responseMsg);
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

            var adapter = GetXmlDataManagerAdapter(ba => {
                                                       ba.XmlDataManager = GetXmlDataManager();
                                                       ba.RequestSerializer =
                                                           new EncryptSerializer<XdsRequestDocument>(
                                                               new CompressSerializer<XdsRequestDocument>(GetRequestSerializer(),
                                                                                                          compressor), encryptor);
                                                       ba.ResponseSerializer =
                                                           new EncryptSerializer<XdsResponseDocument>(
                                                               new CompressSerializer<XdsResponseDocument>(GetResponseSerializer(),
                                                                                                           compressor), encryptor);
                                                       return ba;
                                                   });

            var requestMsg = BuildRequest();

            var requestBytes = adapter.RequestSerializer.Serialize(requestMsg);

            var responseBytes = adapter.Execute(requestBytes.Base64Encode()).Base64Decode();
            responseBytes.Should().Not.Be.Null();

            var responseMsg = adapter.ResponseSerializer.Deserialize(responseBytes);

            VerifyXdsResponseDocument(responseMsg);
        }
    }
}