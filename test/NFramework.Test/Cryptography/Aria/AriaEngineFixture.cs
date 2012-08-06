using NUnit.Framework;

namespace NSoft.NFramework.Cryptography.Aria {
    [TestFixture]
    public class AriaEngineFixture {
/*
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		byte[] p = new byte[16];
		byte[] c = new byte[16];
		byte[] mk = new byte[32];

		private AriaEngine _aria;

		[SetUp]
		public void Setup()
		{
			ArrayTool.InitArray(mk);
			ArrayTool.InitArray(p);

			_aria = new AriaEngine();
		}

		[Test]
		public void DefaultConstructorTest()
		{
			var aria = new AriaEngine();
			Assert.IsNotNull(aria);
		}

		[Test]
		public void SetKeyTest()
		{
			_aria.SetKey(mk);
			_aria.SetupRoundKeys();
		}

		[Test]
		public void EmptyDataEncryptTest()
		{
			_aria.SetKey(mk);
			_aria.SetupRoundKeys();

			_aria.Encrypt(p, 0, c, 0);
			_aria.Decrypt(c, 0, p, 0);

			if(IsDebugEnabled)
			{
				log.Debug("p=[{0}], Length=[{1}]", p.BytesToString(EncryptionStringFormat.HexDecimal), p.Length);
				log.Debug("c=[{0}], Length=[{1}]", c.BytesToString(EncryptionStringFormat.HexDecimal), c.Length);
			}

			p.All(n => n == 0).Should().Be.True();
		}

		[Test]
		public void PerformaceTest()
		{
			const int TEST_NUM = 10000;

			for(var i = 0; i < 16; i++)
				mk[i] = (byte)i;

			_aria.Reset();
			_aria.KeySize = 128;
			_aria.SetKey(mk);

			// CPU 준비를 위해 미리 시도
			//
			for(var i = 0; i < 1000; i++)
				_aria.SetupEncRoundKeys();

			using(new OperationTimer("SetupEncRoundKeys"))
			{
				for(var i = 0; i < TEST_NUM; i++)
					_aria.SetupEncRoundKeys();
			}

			for(var i = 0; i < p.Length; i++)
				p[i] = (byte)((i << 4) ^ i);

			// CPU 준비를 위해 미리 시도
			//
			for(var i = 0; i < 1000; i++)
				_aria.Encrypt(p, 0, c, 0);

			using(new OperationTimer("Encrypt"))
			{
				for(var i = 0; i < TEST_NUM; i++)
					_aria.Encrypt(p, 0, c, 0);
			}

			// CPU 준비를 위해 미리 시도
			//
			for(var i = 0; i < 1000; i++)
				_aria.SetupDecRoundKeys();

			using(new OperationTimer("SetupDecRoundKeys"))
			{
				for(var i = 0; i < TEST_NUM; i++)
					_aria.SetupDecRoundKeys();
			}

			using(new OperationTimer("Decrypt"))
			{
				for(var i = 0; i < TEST_NUM; i++)
					_aria.Decrypt(c, 0, p, 0);
			}

			if(IsDebugEnabled)
			{
				log.Debug("p=[{0}], Length=[{1}]", p.BytesToString(EncryptionStringFormat.HexDecimal), p.Length);
				log.Debug("c=[{0}], Length=[{1}]", c.BytesToString(EncryptionStringFormat.HexDecimal), c.Length);
			}
		}
 */
    }
}