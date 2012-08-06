
#if USE_AOP

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.ByteCode.Castle;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;
using Environment = NHibernate.Cfg.Environment;

namespace NSoft.NFramework.Data.NHibernateEx.AOP
{
	[TestFixture]
	public class CustomProxyFixture : NHRepositoryTestFixtureBase
	{
		protected override void OnTestFixtureSetUp()
		{
			// Nothing to do. // 초기화를 SetUp 시에 한다.			
		}

		// 모든 테스트 함수가 같은 초기환경에서 테스트가 이루어지도록 OnSetUp/OnTearDown에서 초기화/정리를 수행하였다.
		//
		protected override void OnSetUp()
		{
			base.OnTestFixtureSetUp();
			base.CreateObjectsInDB();
			UnitOfWork.CurrentSession.Clear();
		}

		protected override IDictionary<string, string> GetNHibernateProperties()
		{
			var properties = base.GetNHibernateProperties();
			properties[Environment.ProxyFactoryFactoryClass] = typeof(DataBindingProxyFactoryFactory).AssemblyQualifiedName;
			return properties;
		}

		[Test]
		public void CanImplementNotifyPropertyChanged()
		{
			// var parent = (Parent)UnitOfWork.CurrentSession.Load(typeof(Parent), parentsInDB[0].Id);
			var parent = Repository<Parent>.Load(parentsInDB[0].Id);
			// var parent = Repository<Parent>.Get(parentsInDB[0].Id);  // Get은 Lazy로 가져오지 않는다 흑흑
			var propertyChanged = (INotifyPropertyChanged)parent;

			string propChanged = null;
			propertyChanged.PropertyChanged += (s, e) => propChanged = e.PropertyName;

			parent.Name = "foo";
			Assert.AreEqual("Name", propChanged);
		}
	}

	[TestFixture]
	public class CustomProxyFixture_SQLServer : CustomProxyFixture
	{
		protected override DatabaseEngine GetDatabaseEngine()
		{
			return DatabaseEngine.MsSql2005;
		}
	}

	/// <summary>
	/// DataBindingProxyFactory를 생성해주는 Factory
	/// </summary>
	public class DataBindingProxyFactoryFactory : IProxyFactoryFactory
	{
#region Implementation of IProxyFactoryFactory

		/// <summary>
		/// Build a proxy factory specifically for handling runtime
		///             lazy loading. 
		/// </summary>
		/// <returns>
		/// The lazy-load proxy factory. 
		/// </returns>
		public IProxyFactory BuildProxyFactory()
		{
			return new DataBindingProxyFactory();
		}

		public IProxyValidator ProxyValidator
		{
			get { return new DynProxyTypeValidator(); }
		}

#endregion
	}

	/// <summary>
	/// Proxy object에 DataBindingInterceptor를 적용해서 반환하도록 한다.
	/// </summary>
	public class DataBindingProxyFactory : ProxyFactory
	{
		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				log.Info("Get Intercepted Proxy object. id={0}", id);

				LazyInitializer initializer =
					new DataBindingInterceptor(EntityName, PersistentClass, id,
											   GetIdentifierMethod, SetIdentifierMethod,
											   ComponentIdType, session);

				object generatedProxy;
				var list = new List<Type>(Interfaces);
				list.Add(typeof(INotifyPropertyChanged));
				var interfaces = list.ToArray();
				//var list = new ArrayList(Interfaces);
				//list.Add(typeof(INotifyPropertyChanged));
				//var interfaces = (System.Type[])list.ToArray(typeof(System.Type));

				if(IsClassProxy)
					generatedProxy = DefaultProxyGenerator.CreateClassProxy(PersistentClass,
																			interfaces,
																			ProxyGenerationOptions.Default,
																			initializer);
				else
					generatedProxy = DefaultProxyGenerator.CreateInterfaceProxyWithoutTarget(interfaces[0], interfaces, initializer);

				initializer._constructed = true;
				return (INHibernateProxy)generatedProxy;
			}
			catch(Exception ex)
			{
				log.ErrorException("Creating a proxy instance is failed. id=" + id, ex);
				throw new HibernateException("Creating a proxy instance is failed. id=" + id, ex);
			}
		}
	}

	/// <summary>
	/// 일반적인 Entity class에 INotifyPropertyChanged 를 적용하는 Interceptor이다.
	/// </summary>
	public class DataBindingInterceptor : LazyInitializer
	{
#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

#endregion

		private PropertyChangedEventHandler subscribers = delegate { };

		public DataBindingInterceptor(string entityName, Type persistentClass, object id,
									  MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
									  IAbstractComponentType componentIdType, ISessionImplementor session)
			: base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session) { }


		public override void Intercept(Castle.Core.Interceptor.IInvocation invocation)
		{
			if(invocation.Method.DeclaringType == typeof(INotifyPropertyChanged))
			{
				var propertyChangedEventHandler = (PropertyChangedEventHandler)invocation.GetArgumentValue(0);
				if(invocation.Method.Name.StartsWith("add_"))
				{
					subscribers += propertyChangedEventHandler;
				}
				else
				{
					subscribers -= propertyChangedEventHandler;
				}
				return;
			}
			base.Intercept(invocation);
			var result = invocation.ReturnValue;

			// 속성값이 변경되었을 때 PropertyChanged event를 실행시킨다.
			if(invocation.Method.Name.StartsWith("set_"))
			{
				if(IsDebugEnabled)
					log.Debug("Raise PropertyChanged event. PropertyName=" + invocation.Method.Name);

				subscribers(this, new PropertyChangedEventArgs(invocation.Method.Name.Substring(4)));
			}

			invocation.ReturnValue = result;
		}
	}
}
#endif