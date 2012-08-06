
#if NH_CACHE
using System;
using System.Collections.Generic;
using System.Data.Caching;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Caches
{
	/// <summary>
	/// Velocity를 직접 Test 한다. (app.config에서 velocity 관련 설정만 제대로 해주면 된다.)
	/// </summary>
	[TestFixture]
	public class VelocityClientFixture
	{
		private CacheFactory _cacheFactory;
		private Cache _defaultCache;

		private string myObjectForCaching = "This is my Object";
		private CacheItemVersion itemVersion;
		private string item;

		private string myRegion = "MyRegion";
		private string myLoadTestRegion = "LetsLoadTheRegion";

		private readonly Tag[] myTagIsNull;
		private CacheItemVersion thisisNull;

		private CacheItem cacheItem1, cacheItem2;
		private CacheItemVersion cacheItemVersion;

		[TestFixtureSetUp]
		public void ClassSetUp()
		{
			_cacheFactory = new CacheFactory();
			Console.WriteLine("CacheFactory initialization - PASS");

			if ((_defaultCache = _cacheFactory.GetCache("default")) != null)
				Console.WriteLine("PASS->GetCache-Received reference to 'default' Named Cache.");
			else
				Console.WriteLine("**FAIL->GetCache-did not receive reference to 'default' named cache.");
		}

		[TestFixtureTearDown]
		public void ClassCleanUp()
		{
			if (_cacheFactory != null)
				_cacheFactory.Close();
		}

		[Test]
		public void CreateRegion()
		{
			Console.WriteLine();
			Console.WriteLine("Create Region for general use in default cache");

			try
			{
				bool regionCreated = _defaultCache.CreateRegion(myRegion, false);
				Console.WriteLine("Create Region Created : " + regionCreated);

				_defaultCache.ClearRegion(myRegion);
			}
			catch (Exception ex)
			{
				Console.WriteLine("**FAIL--->CreateRegion-Probably failing since we are creating a region that");
				Console.WriteLine("          already exists. Removing and re-Creating the region");
				Console.Write("**FAIL--->Distributed Cache Generated Exception:");
				Console.WriteLine(ex.Message);
				Console.WriteLine();
				Console.WriteLine("Recovering from above failure");
				if (_defaultCache.RemoveRegion(myRegion))
					Console.WriteLine("PASS--->RemoveRegion " + myRegion);
				else
					Console.WriteLine("**FAIL--->RemoveRegion " + myRegion + ". Region never existed. Some other exception");

				if (_defaultCache.CreateRegion(myRegion, false))
					Console.WriteLine("PASS--->CreateRegion " + myRegion);
				else
					Console.WriteLine("**FAIL--->CreateRegion " + myRegion);
			}
			Console.WriteLine();
		}

		[Test]
		public void CreateLoadTestRegion()
		{
			Console.WriteLine("Creating Region for the load test in the default cache");

			try
			{
				if (_defaultCache.CreateRegion(myLoadTestRegion, false))
					Console.WriteLine("PASS--->CreateRegion " + myLoadTestRegion);
				else
					Console.WriteLine("**FAIL-->CreateRegion " + myLoadTestRegion);

				_defaultCache.ClearRegion(myLoadTestRegion);
			}
			catch (CacheException ex)
			{
				Console.WriteLine("**FAIL--->CreateRegion-This is probably failing since you are creating a region");
				Console.WriteLine("          that already exists in the cache");
				Console.Write("**FAIL--->Distributed Cache Generated Exception:");
				Console.WriteLine(ex.Message);
				Console.WriteLine();
				Console.WriteLine("Recovering from above failure");
				if (_defaultCache.RemoveRegion(myLoadTestRegion))
					Console.WriteLine("PASS--->RemoveRegion " + myLoadTestRegion);
				else
					Console.WriteLine("**FAIL--->RemoveRegion " + myLoadTestRegion +
					                  ". Region never existed. Some other exception");

				if (_defaultCache.CreateRegion(myLoadTestRegion, false))
					Console.WriteLine("PASS--->CreateRegion " + myLoadTestRegion);
				else
					Console.WriteLine("**FAIL--->CreateRegion " + myLoadTestRegion);
			}
		}

		[Test]
		public void Add_Get_Simple()
		{
			//
			// TESTING SIMPLE Add/Get on default cache
			//
			// no regions
			//
			// Need to catch exception here to allow the program to run continously
			// Try this out
			// - Comment the try catch on this block of code
			// - Run this program twice within 10 mins
			// - Result, Add will throw a exception
			// - Run this program after 10 mins
			// - Result, Program will run ok
			// Try this out
			// - Put a VS-BreakPoint at the Get("KetToMyString") call
			// - Run the sample test after 10 mins
			// - Get will fail
			Console.WriteLine("----------------------");
			Console.WriteLine("Testing Simple Add/Get");
			Console.WriteLine("Cache       = default");
			Console.WriteLine("Region      = <none>");
			Console.WriteLine("Tags        = <none>");
			Console.WriteLine("Version     = <none>");
			try
			{
				if ((itemVersion = _defaultCache.Add("KeyToMyString", myObjectForCaching)) != null)
					Console.WriteLine("PASS--->Add-Object Added to Cache, HashCode: " + itemVersion.GetHashCode());
				else
					Console.WriteLine("**FAIL--->Add-Object did not add to cache - FAIL");

				if ((item = (string) _defaultCache.Get("KeyToMyString")) != null)
					Console.WriteLine("PASS--->Get-Object Get from cache");
				else
					Console.WriteLine("**FAIL--->Get-Object did not Get from cache");

				if ((item = (string) _defaultCache.Get("InCorrectKeySpecified")) == null)
					Console.WriteLine("PASS--->Get-Object did not Get, since invalid key specified");
				else
					Console.WriteLine("**FAIL--->Get-Object Get from cache, unexpected result");
			}
			catch (CacheException ex)
			{
				Console.WriteLine("**FAIL--->Add-Get-This is failing probably because you are running this");
				Console.WriteLine("          sample test within 10mins (default timeout) in clusterconfig.xml");
				Console.WriteLine("          To get this working, in the admin tool");
				Console.WriteLine("          - stop cluster");
				Console.WriteLine("          - delete cache default");
				Console.WriteLine("          - create cache default");
				Console.WriteLine("          - start cluster");
				Console.Write("**FAIL--->Distributed Cache Generated Exception:");
				Console.WriteLine(ex.Message);
			}
		}

		[Test]
		public void Add_Get_Using_Region()
		{
			CreateRegion();

			string myRegion = "MyRegion";
			//
			// TESTING SIMPLE Add/Get on default cache USING Region
			//
			// without Tags
			// without version
			//
			Console.WriteLine("----------------------");
			Console.WriteLine("Testing Simple Add/Get");
			Console.WriteLine("Cache       = default");
			Console.WriteLine("Region      = " + myRegion);
			Console.WriteLine("Tags        = <none>");
			Console.WriteLine("Version     = <none>");

			Tag[] myTagIsNull = null;
			CacheItemVersion thisisNull = null;
			try
			{
				// Initialize the object with a Add
				if ((itemVersion = _defaultCache.Add(myRegion, "KeyToMyString", myObjectForCaching, myTagIsNull)) != null)
					Console.WriteLine("PASS----->Add-Object Added to Cache, " + itemVersion);
				else
					Console.WriteLine("**FAIL----->Add-Object did not add to cache");

				// Do a Simple Get using version API
				if ((item = (string) _defaultCache.Get(myRegion, "KeyToMyString", ref thisisNull)) != null)
					Console.WriteLine("PASS----->Get-Object Get from cache");
				else
					Console.WriteLine("**FAIL----->Get-Object did not Get from cache");

				// Do a Simple Get with invalid key
				if ((item = (string) _defaultCache.Get("InvalidKey")) != null)
					Console.WriteLine("**FAIL----->Get-Object returned from Cache, should not since key is invalid");
				else
					Console.WriteLine("PASS----->Get-Object did not Get from cache. Expected since key is invalid");
			}
			catch (CacheException ex)
			{
				Console.Write("**FAIL----->Add-Get-Distributed Cache Generated Exception:");
				Console.WriteLine(ex.Message);
				// Will never get this error since we are Removing existing regions and creating new ones
			}
		}

		[Test]
		public void Add_GetAndLock_Using_Region()
		{
			CreateRegion();

			//
			// TESTING SIMPLE Add/GetAndLock using Region
			//
			// without Tags
			// without version
			//
			// Try this out
			// - Put a BreakPoint on the first Get, and hold execution for then 5 seconds. 
			//   It will return the object. Since the lock will expire.
			//   Additionally : Study behaviour of Put and PutAndLock
			// - Put a BreakPoint on the second GetAndLock, and hold the execution for 5 seconds.
			//   It will lock the object for 10 seconds. Since the first lock has expired.
			//   Additionally : Study behaviour of Put and PutAndLock
			Console.WriteLine("-----------------------------");
			Console.WriteLine("Testing Simple Add/Get/GetAndLock/GetIfVersionMismatch/Put/PutAndLock");
			Console.WriteLine("Cache       = default");
			Console.WriteLine("Region      = " + myRegion);
			Console.WriteLine("Tags        = <none>");
			Console.WriteLine("Version     = <none>");

			CacheItemVersion myVersionBeforeChange = null, myVersionAfterChange = null, myVersionChangedOnceMore = null;
			LockHandle lockHandle;
			string myKey = "KeyToMyStringTryingLock";

			try
			{
				// Initialize the object with a Add
				if ((itemVersion = _defaultCache.Add(myRegion, myKey, myObjectForCaching, myTagIsNull)) != null)
					Console.WriteLine("PASS----->Add-Object Added to Cache");
				else
					Console.WriteLine("**FAIL----->Add-Object did not add to cache");

				// Do a Simple Get, lock the object for 5 seconds
				if ((item = (string) _defaultCache.GetAndLock(myRegion, myKey,
				                                              new TimeSpan(0, 0, 5), out lockHandle)) != null)
					Console.WriteLine("PASS----->GetAndLock-Object Get from cache");
				else
					Console.WriteLine("**FAIL----->GetAndLock-Object did not Get from cache");

				// Do a optimistic Get
				if ((item = (string) _defaultCache.Get(myRegion, myKey, ref myVersionBeforeChange)) != null)
				{
					Console.WriteLine("PASS----->Get-Object returned. Get will always pass. Will not wait");
					Console.WriteLine("          on a updating object. Current Version will be returned.");
				}
				else
					Console.WriteLine("**FAIL----->Get-Object did not return.");

				try
				{
					// Do a one more Simple Get, and attempt lock the object for 10 seconds
					if ((item = (string) _defaultCache.GetAndLock(myRegion, myKey,
					                                              new TimeSpan(0, 0, 10), out lockHandle)) != null)
						Console.WriteLine("**FAIL----->GetAndLock-Object Get from cache");
					else
						// Since a exception will catch it, this will never return null
						Console.WriteLine("PASS----->GetAndLock-Object did not Get from cache");
				}
				catch (CacheException ex)
				{
					Console.WriteLine("PASS----->GetAndLock hit a exception, becuase object is already locked");
					Console.Write("PASS----->GetAndLock-Distributed Cache Generated Exception:");
					Console.WriteLine(ex.Message);
				}

				if ((item = (string) _defaultCache.GetIfVersionMismatch(myRegion, myKey, ref myVersionBeforeChange)) != null)
				{
					Console.WriteLine("**FAIL----->GetIfVersionMismatch-Object changed. Should not return as object has");
					Console.WriteLine("            not been changed");
				}
				else
					Console.WriteLine("PASS----->GetIfVersionMismatch-Object has not changed. Hence did not return.");

				// Now update the object with a Put                
				if ((myVersionAfterChange = _defaultCache.Put(myRegion, myKey,
				                                              myObjectForCaching + "Put1", myTagIsNull, thisisNull)) != null)
				{
					Console.WriteLine("PASS----->Put1-null-version-Object changed. Put will pass even if object");
					Console.WriteLine("          is locked. Object will also be unlocked.");
					myObjectForCaching += "Put1";
				}
				else
					Console.WriteLine("PASS----->Put1-null-version-Object did not change.");

				// Object with older version changed
				if ((item = (string) _defaultCache.GetIfVersionMismatch(myRegion, myKey, ref myVersionBeforeChange)) != null)
					Console.WriteLine("PASS----->GetIfVersionMismatch-Object has been changed.");
				else
				{
					Console.WriteLine("**FAIL----->GetIfVersionMismatch-Object did not return. Put ");
					Console.WriteLine("            did modify the object. Should return.");
				}

				// Object with newer version after Put
				if ((item = (string) _defaultCache.GetIfVersionMismatch(myRegion, myKey, ref myVersionAfterChange)) != null)
				{
					Console.WriteLine("**FAIL----->GetIfVersionMismatch-Object with newer version not changed.");
					Console.WriteLine("            Should not return.");
				}
				else
					Console.WriteLine("PASS----->GetIfVersionMismatch-Object with newer version not changed");

				if ((myVersionChangedOnceMore = _defaultCache.Put(myRegion, myKey,
				                                                  myObjectForCaching + "Put2", myTagIsNull, myVersionBeforeChange)) !=
				    null)
				{
					Console.WriteLine("PASS----->Put2-version from Put1-Object changed.");
					myObjectForCaching += "Put2";
				}
				else
					Console.WriteLine("**FAIL----->Put2-version from Put1-Object did not change.");

				try
				{
					// Try the above PutAndUnlock                 
					if ((myVersionChangedOnceMore = _defaultCache.PutAndUnlock(myRegion, myKey,
					                                                           myObjectForCaching + "Put3", lockHandle, myTagIsNull)) !=
					    null)
					{
						Console.WriteLine("PASS----->PutAndUnlock-Object updated and unlocked.");
						myObjectForCaching += "Put3";
					}
					else
						Console.WriteLine("**FAIL----->PutAndUnlock-Object should have updated and unlocked.");
				}
				catch (CacheException ex)
				{
					Console.WriteLine("PASS----->PutAndUnlock-Expected exception since object is already unlocked.");
					Console.Write("PASS---->PutAndUnlock-Distributed Cache Generated Exception:");
					Console.WriteLine(ex.Message);
				}
				// Unlock Object
				try
				{
					if (_defaultCache.Unlock(myRegion, myKey, lockHandle))
						Console.WriteLine("PASS----->Unlock-Object unlocked");
					else
						Console.WriteLine("**FAIL----->Object could not be unlocked");
				}
				catch (CacheException ex)
				{
					Console.WriteLine("PASS----->Unlock-Expected exception since object is already unlocked.");
					Console.Write("PASS----->Unlock-Distributed Cache Generated Exception:");
					Console.WriteLine(ex.Message);
				}
				// Final Test the state of object should be "This is my Object.Put1Put2"
				if ((item = (string) _defaultCache.Get(myRegion, myKey, ref myVersionChangedOnceMore)) ==
				    myObjectForCaching)
					Console.WriteLine("PASS----->Get-Object retrived from cache.");
				else
					Console.WriteLine("**FAIL----->Get-Object was not retrived from cache");
			}
			catch (CacheException ex)
			{
				Console.Write(
					"**FAIL---->Add-Get-GetAndLock-GetIfVersionMismatch-Put-PutAndUnlock-Distributed Cache Generated Exception:");
				Console.WriteLine(ex.Message);
			}
		}

		[Test]
		public void Add_Get_On_Region_With_Version()
		{
			//
			// TESTING SIMPLE Add/Get ON REGION with Version
			//
			// without Tags
			// Try this
			// - Put a BreakPoint on the second Put and wait for 5 seconds before releaseing.

			Console.WriteLine("-----------------------------------");
			Console.WriteLine("Testing Simple Add/GetCacheItem/Put");
			Console.WriteLine("Cache       = default");
			Console.WriteLine("Region      = " + myRegion);
			Console.WriteLine("Tags        = <none>");
			Console.WriteLine("Version     = yes");

			try
			{
				if ((itemVersion = _defaultCache.Add(myRegion, "KeyToMyStringWithVersion", myObjectForCaching, myTagIsNull)) != null)
					Console.WriteLine("PASS----->Add-Object Added to Cache, " + itemVersion);
				else
					Console.WriteLine("**FAIL----->Add-Object did not add to cache");

				if ((cacheItem1 = _defaultCache.GetCacheItem(myRegion, "KeyToMyStringWithVersion")) != null)
					Console.WriteLine("PASS----->GetCacheItem-Object Get from cache");
				else
					Console.WriteLine("**FAIL----->GetCacheItem-Object did not Get from cache");

				if ((cacheItem2 = _defaultCache.GetCacheItem(myRegion, "KeyToMyStringWithVersion")) != null)
					Console.WriteLine("PASS----->GetCacheItem-Object Get from cache");
				else
					Console.WriteLine("**FAIL----->GetCacheItem-Object did not Get from cache");

				if ((cacheItemVersion = _defaultCache.Put(myRegion, "KeyToMyStringWithVersion",
				                                          cacheItem1.Value, myTagIsNull, cacheItem1.Version)) != null)
					Console.WriteLine("PASS----->Put-Object updated successfully");
				else
					Console.WriteLine("**FAIL----->Put-Object did not update successfully");

				if ((cacheItemVersion = _defaultCache.Put(myRegion, "KeyToMyStringWithVersion",
				                                          cacheItem2.Value, myTagIsNull, cacheItem2.Version)) != null)
					Console.WriteLine("**FAIL----->Put-Object update. Optimistic lock did not work");
				else // this will throw a exception, so the else will not run if the object is locked.
					Console.WriteLine("PASS----->Put-Object did not update. Optimistic lock worked");
			}
			catch (CacheException ex)
			{
				Console.WriteLine("PASS----->Add-GetCacheItem-Put-Expected behaviour since object is locked");
				Console.WriteLine("          for 5 seconds");
				Console.Write("PASS----->Distributed Cache Generated Exception:");
				Console.WriteLine(ex.Message);
			}
		}

		[Test]
		public void Add_Get_With_Tags()
		{
			// Testing simple Add/Get on a Region with Tags
			// without Version
			// Each object will have a unique key
			// Each object can have multiple tags, hence the tag[]
			// Multiple Objects can have the same tag
			Console.WriteLine("----------------------");
			Console.WriteLine("Testing Simple Add/GetByTag");
			Console.WriteLine("Cache       = default");
			Console.WriteLine("Region      = " + myRegion);
			Console.WriteLine("Tags        = yes");
			Console.WriteLine("Version     = <none>");

			const int totalTags = 5;
			Tag[] allMyTags = new System.Data.Caching.Tag[totalTags]
			                  {
			                  	new Tag("Tag1"), new Tag("Tag2"),
			                  	new Tag("Tag3"), new Tag("Tag4"),
			                  	new Tag("Tag5")
			                  };
			List<KeyValuePair<string, object>> getByTagReturnKeyValuePair;
			int totalObjects = 10;
			try
			{
				for (int objectid = 0; objectid < totalObjects; objectid++)
				{
					if ((itemVersion = _defaultCache.Add(myRegion, "MyKey" + objectid,
					                                     myObjectForCaching, allMyTags)) != null)
						Console.WriteLine("PASS----->Add-Object " +
						                  "MyKey" + objectid +
						                  " added to Cache, with all tags");
					else
						Console.WriteLine("**FAIL----->Add-Object did not add to cache");
				}
				for (int objectid = 0; objectid < totalObjects; objectid++)
					for (int tagid = 0; tagid < totalTags; tagid++)
						if ((getByTagReturnKeyValuePair = _defaultCache.GetByTag(myRegion, allMyTags[tagid])) != null)
							Console.WriteLine("PASS----->GetByTag-Object " +
							                  getByTagReturnKeyValuePair.ElementAt(tagid).Key +
							                  " get from cache. Using Tag " + tagid);
						else
							Console.WriteLine("**FAIL----->GetByTag-Object did not Get from cache");
			}
			catch (CacheException ex)
			{
				Console.WriteLine("**FAIL----->Add-GetByTag-This is failing probably because you are running this sample test");
				Console.WriteLine("          within 10mins (default timeout)");
				Console.Write("**FAIL----->Distributed Cache Generated Exception:");
				Console.WriteLine(ex.Message);
			}
		}

		[Test]
		public void Load_Large_Data()
		{
			CreateLoadTestRegion();

			// Simple Load testing
			// Object size every line starts with 100 bytes and grows "exponentially".
			// Try this out
			// - Increase the size of data and test behaviour
			// - Increase number of iterateMax
			// - If the app crashs with "out of memory"
			//   * Cache lotOfData.Length.ToString()
			//   * Behaviour noticed, application will still crash
			//   * Learning : Get the application to work before using DistributedCache
			// Behaviour noticed
			// - Regardless of a add to cache, test app run's out of memory
			// - Memory increases exponentially
			int iterateMax = 10;
			string lotOfData =
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
				"0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

			Console.WriteLine("----------------------");
			Console.WriteLine("Testing Simple Add/Get");
			Console.WriteLine("Cache       = default");
			Console.WriteLine("Region      = " + myLoadTestRegion);
			Console.WriteLine("Tags        = <none>");
			Console.WriteLine("Version     = <none>");
			long totalSizeAdded = 0;
			long iterate;
			for (iterate = 0; iterate < iterateMax; iterate++)
			{
				try
				{
					// Lets know how much we want to add to cache before the add
					Console.Write(lotOfData.Length);
					if ((itemVersion = _defaultCache.Add(myLoadTestRegion, iterate.ToString(), lotOfData, null)) != null)
						Console.Write(" PASS----->Add" + iterate + " ");
					else
						Console.WriteLine("**FAIL----->Add-Object did not add to cache - FAIL");

					if ((item = (string) _defaultCache.Get(myLoadTestRegion, iterate.ToString(), ref thisisNull)) != null)
						Console.WriteLine(item.Length + " PASS-->Get" + iterate + " ");
					else
						Console.WriteLine("**FAIL----->Get-Object did not Get from cache");
					totalSizeAdded += lotOfData.Length;
					lotOfData += lotOfData;
				}
				catch (CacheException ex)
				{
					Console.WriteLine("**FAIL----->Add-Get-This is failing probably because you are running this sample test");
					Console.WriteLine("         within 10mins (default timeout)");
					Console.Write("**FAIL----->Distributed Cache Generated Exception:");
					Console.WriteLine(ex.Message);
					// Will never get this error since I am now calling RemoveRegion at the start of the sample test
				}
			}
			Console.WriteLine("Total Size added " + totalSizeAdded);
		}
	}
}
#endif