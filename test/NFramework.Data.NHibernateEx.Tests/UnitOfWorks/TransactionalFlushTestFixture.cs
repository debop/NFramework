
#if LOG4NET

using System;
using System.IO;
using NUnit.Framework;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;

namespace NSoft.NFramework.Data.NHibernateEx.UnitOfWorks
{
	[TestFixture]
	public class TransactionalFlushTestFixture : NHDatabaseTestFixtureBase // DatabaseTestFixtureBase
	{
		private const string TransactionLogName = "NHibernate.Transaction.AdoTransaction";

		protected override void OnTestFixtureSetUp()
		{
			//BasicConfigurator.Configure(new MemoryAppender());

			base.OnTestFixtureSetUp();
		}
		protected override void OnTestFixtureTearDown()
		{
			//LogManager.ResetConfiguration();
		}

		protected override string ContainerFilePath
		{
			get { return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"NH\Windsor.Hibernate.config")); }
		}

		[Test]
		public void Will_Flush_To_Database_Within_Transaction_And_Dispose_Of_Transation_AtEnd()
		{
			UnitOfWork.CurrentSession.Save(new SimpleObject());

			var logMessages = NSoft.NFramework.With.Log(TransactionLogName,
												delegate { UnitOfWork.Current.TransactionalFlush(); });

			bool hasCommitted = false;

			foreach(string msg in logMessages)
			{
				if(msg.ToLower().Contains("commit"))
				{
					hasCommitted = true;
					break;
				}
			}

			if(CurrentContext.DatabaseEngine != DatabaseEngine.SQLite)
				Assert.IsTrue(hasCommitted);
		}
		[Test]
		public void Will_Rollback_And_Dispose_Transaction_If_Flush_Throws()
		{
			// SQLite에서는 Length 제한이 안 먹네... 그래서 SQLite에서는 Rollback이 되지 않는다.
			//
			var obj = new SimpleObject { TwoCharactersMax = "This string is too big" };
			UnitOfWork.CurrentSession.Save(obj);

			var logMessages = NSoft.NFramework.With.Log(TransactionLogName, delegate
																	{
																		try
																		{
																			UnitOfWork.Current.TransactionalFlush();
																		}
																		catch { }
																	});

			bool hasRollback = false;
			foreach(string msg in logMessages)
			{
				if(msg.ToLower().Contains("rollback"))
				{
					hasRollback = true;
					break;
				}
			}

			if(CurrentContext.DatabaseEngine == DatabaseEngine.SQLite)
				Assert.IsFalse(hasRollback);
			else
				Assert.IsTrue(hasRollback);
		}
		[Test]
		public void Will_Not_Start_Transaction_If_Already_Started()
		{
			UnitOfWork.Current.BeginTransaction();

			var obj = new SimpleObject();
			obj.TwoCharactersMax = "This string is too big";
			UnitOfWork.CurrentSession.Save(obj);

			var logMessages = NSoft.NFramework.With.Log(TransactionLogName,
												delegate
												{
													try
													{
														UnitOfWork.Current.TransactionalFlush();
													}
													catch { }
												});

			//
			// 이미 trasaction이 시작되었으므로, 
			// TransactionalFlush() 하는 동안에는 begin transaction 이란 말이 들어가지 않는다.
			//
			Assert.IsFalse(logMessages.Contains("begin"));
		}
	}

	//[TestFixture]
	//public class TransactionalFlush_SQLite : TransactionalFlush
	//{
	//    protected override DatabaseEngine GetDatabaseEngine()
	//    {
	//        return DatabaseEngine.SQLite;
	//    }
	//}

	[TestFixture]
	public class TransactionalFlush_SQLServer : TransactionalFlush
	{
		protected override DatabaseEngine GetDatabaseEngine()
		{
			return DatabaseEngine.MsSql2005;
		}
	}
}
#endif