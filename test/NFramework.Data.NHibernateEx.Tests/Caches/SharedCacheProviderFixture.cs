//! NSoft.NFramework.Caching.SharedCache 로 이동했음



#if NH_CACHE
using System.Collections.Generic;
using NUnit.Framework;
using NHibernate.Cache;
using NHibernate.Caches.SharedCache;

namespace NSoft.NFramework.Data.NHibernateEx.Caches
{
	/// <summary>
	/// IndeXus.NET Shared Cache Provider를 테스트 합니다. app.config에서 환경설정을 맞춰줘야 합니다.
	/// </summary>
	/// <seealso cref="http://nhcontrib.svn.sourceforge.net/svnroot/nhcontrib/trunk/src"/>
	/// <remarks>
	/// 소스 SVN : http://nhcontrib.svn.sourceforge.net/svnroot/nhcontrib/trunk/src
	/// 다운로드 위치 : http://sourceforge.net/project/showfiles.php?group_id=216446&package_id=286204
	/// </remarks>
	[TestFixture]
	public class SharedCacheProviderFixture
	{
		private ICacheProvider provider;
		private Dictionary<string, string> props;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			// XmlConfigurator.Configure();
			props = new Dictionary<string, string>();
			provider = new SharedCacheProvider();
			provider.Start(props);
		}

		[TestFixtureTearDown]
		public void Stop()
		{
			provider.Stop();
		}

		[Test]
		public void TestBuildCacheFromConfig()
		{
			ICache cache = provider.BuildCache("foo", null);
			Assert.IsNotNull(cache, "pre-configured cache not found");
		}

		[Test]
		public void TestBuildCacheNullNull()
		{
			ICache cache = provider.BuildCache(null, null);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void TestBuildCacheStringICollection()
		{
			ICache cache = provider.BuildCache("another_region", props);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void TestBuildCacheStringNull()
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