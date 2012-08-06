
#if NH_CACHE
using System.Collections.Generic;
using NUnit.Framework;
using NHibernate.Cache;
using NHibernate.Caches.Velocity;

namespace NSoft.NFramework.Data.NHibernateEx.Caches
{
	/// <summary>
	/// Velocity의 ClusterConfig.xml에 Cache (name="nhibernate") 를 추가해야 한다. 
	/// Velocity Admin tool에서 "create cache nhibernate" 를 수행하면 된다.
	/// NHibernate.Caches.Velocity.VelocityProvider가 nhibernate cache를 이용하기 때문이다.
	/// </summary>
	[TestFixture]
	public class VelocityProviderFixture
	{
		private ICacheProvider provider;
		private Dictionary<string, string> props;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			props = new Dictionary<string, string>();
			provider = new VelocityProvider();
			provider.Start(props);
		}

		[TestFixtureTearDown]
		public void Stop()
		{
			provider.Stop();
		}

		[Test]
		public void BuildCacheFromConfig()
		{
			ICache cache = provider.BuildCache("foo", null);
			Assert.IsNotNull(cache, "pre-configured cache not found.");
		}

		[Test]
		public void BuildCacheNullNull()
		{
			ICache cache = provider.BuildCache(null, null);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void BuildCacheStringICollection()
		{
			ICache cache = provider.BuildCache("another_region", props);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void BuildCacheStringNull()
		{
			ICache cache = provider.BuildCache("a_region", null);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void TestNextTimestamp()
		{
			long ts = provider.NextTimestamp();
			Assert.IsNotNull(ts, "no timestamp returned");
		}
	}
}
#endif