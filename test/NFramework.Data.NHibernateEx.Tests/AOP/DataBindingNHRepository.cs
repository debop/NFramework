
#if USE_DECORATOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.AOP
{
	[TestFixture]
	public class DataBindingRepositoryFixture : NHRepositoryTestFixtureBase
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

		[Test]
		public void CanGetINotifyPropertyChangedProxy()
		{
			var repository = new DataBindingNHRepository<Parent>(new NHRepository<Parent>());
			var parent = repository.Get(parentsInDB[0].Id);

			var propertyChanged = (INotifyPropertyChanged) parent;

			string propChanged = null;
			propertyChanged.PropertyChanged += (s, e) => propChanged = e.PropertyName;

			parent.Name = "foo";
			Assert.AreEqual("Name", propChanged);
			// Assert.AreEqual(parentsInDB[0].Id, parent.Id);
		}
	}

	/// <summary>
	/// Entity에 INotifyPropertyChanged 를 제공하는 Proxy로 변형해서 제공한다.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataBindingNHRepository<T> : NHRepositoryDecorator<T> where T : class, IDataObject
	{
		private static ProxyGenerator _generator = new ProxyGenerator();

		public DataBindingNHRepository(INHRepository<T> inner) : base(inner) {}

		private static IInterceptor DataBindingInterceptor
		{
			get { return new NotifyPropertyInterceptor(); }
		}

		private static TProxy CreateInterfaceProxyWithTarget<TProxy>(TProxy target)
		{
			var list = new List<Type>(typeof(TProxy).GetInterfaces());

			if(list.Contains(typeof(INotifyPropertyChanged)) == false)
				list.Add(typeof(INotifyPropertyChanged));

			var interfaces = list.ToArray();

			// 여기서 문제다. Entity의 대표 Interface를 지정하던가. instance를 class proxy에 지정할 수 있으면 좋으련만...
			// return _generator.CreateInterfaceProxyWithTarget(typeof(IParent), interfaces, target, ProxyGenerationOptions.Default, DataBindingInterceptor);
			var proxy = (TProxy) _generator.CreateClassProxy(typeof(TProxy), interfaces, ProxyGenerationOptions.Default, DataBindingInterceptor);
			return proxy;
		}

		public override T Get(object id)
		{
			var entity = Wrapper.Get(id);
			return CreateInterfaceProxyWithTarget(entity);
		}
	}

	public class NotifyPropertyInterceptor : IInterceptor
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		private PropertyChangedEventHandler subscribers = delegate { };

		public virtual void Intercept(IInvocation invocation)
		{
			if(invocation.Method.DeclaringType == typeof(INotifyPropertyChanged))
			{
				var propertyChangedEventHandler = (PropertyChangedEventHandler) invocation.GetArgumentValue(0);
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

			invocation.Proceed();

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