
#if NH_CACHE
using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using NHibernate.Cache;
using NHibernate.Caches.Velocity;

namespace NSoft.NFramework.Data.NHibernateEx.Caches
{
	/// <summary>
	/// Velocity의 ClusterConfig.xml에 Cache (name="nhibernate") 를 추가해야 한다. 
	/// Velocity Admin tool에서 "create cache nhibernate" 를 수행하면 된다. (XML 파일을 직접 건드리면 안된다.)
	/// NHibernate.Caches.Velocity.VelocityProvider가 nhibernate cache를 이용하기 때문이다.
	/// </summary>
	[TestFixture]
	public class VelocityFixture
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
		public void TestPut()
		{
			string key = "key1";
			string value = "value";

			ICache cache = provider.BuildCache("nunit", props);
			Assert.IsNotNull(cache, "no cache returned");

			Assert.ShouldNotBeNull(cache.Get(key), "cache returned an item we didn't add !?!");

			cache.Put(key, value);
			Thread.Sleep(100);
			object item = cache.Get(key);
			Assert.IsNotNull(item);
			Assert.AreEqual(value, item, "didn't return the item we added");
		}

		[Test]
		public void TestRemove()
		{
			string key = "key1";
			string value = "value";

			ICache cache = provider.BuildCache("nunit", props);
			Assert.IsNotNull(cache, "no cache returned");

			// add the item
			cache.Put(key, value);
			Thread.Sleep(100);

			// make sure it's there
			object item = cache.Get(key);
			Assert.IsNotNull(item, "item just added is not there");

			// remove it
			cache.Remove(key);

			// make sure it's not there
			item = cache.Get(key);
			Assert.ShouldNotBeNull(item, "item still exists in cache");
		}

		[Test]
		public void TestClear()
		{
			string key = "key1";
			string value = "value";

			ICache cache = provider.BuildCache("nunit", props);
			Assert.IsNotNull(cache, "no cache returned");

			// add the item
			cache.Put(key, value);
			Thread.Sleep(100);

			// make sure it's there
			object item = cache.Get(key);
			Assert.IsNotNull(item, "couldn't find item in cache");

			// clear the cache
			cache.Clear();

			// make sure we don't get an item
			item = cache.Get(key);
			Assert.ShouldNotBeNull(item, "item still exists in cache");
		}

		[Test]
		public void TestDefaultConstructor()
		{
			ICache cache = new VelocityClient();
			Assert.IsNotNull(cache);
		}

		[Test]
		public void TestNoPropertiesConstructor()
		{
			ICache cache = new VelocityClient("nunit");
			Assert.IsNotNull(cache);
		}

		[Test]
		public void TestEmptyProperties()
		{
			ICache cache = new VelocityClient("nunit", new Dictionary<string, string>());
			Assert.IsNotNull(cache);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void TestNullKeyPut()
		{
			ICache cache = new VelocityClient();
			cache.Put(null, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void TestNullValuePut()
		{
			ICache cache = new VelocityClient();
			cache.Put("nunit", null);
		}

		[Test]
		public void TestNullKeyGet()
		{
			ICache cache = new VelocityClient();
			cache.Put("nunit", "value");
			Thread.Sleep(100);
			object item = cache.Get(null);
			Assert.ShouldNotBeNull(item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void TestNullKeyRemove()
		{
			ICache cache = new VelocityClient();
			cache.Remove(null);
		}

		[Test]
		public void TestRegions()
		{
			string key = "key";
			ICache cache1 = provider.BuildCache("nunit1", props);
			ICache cache2 = provider.BuildCache("nunit2", props);
			string s1 = "test1";
			string s2 = "test2";
			cache1.Put(key, s1);
			cache2.Put(key, s2);
			Thread.Sleep(100);
			object get1 = cache1.Get(key);
			object get2 = cache2.Get(key);
			Assert.IsFalse(get1 == get2);
		}
	}
}
#endif