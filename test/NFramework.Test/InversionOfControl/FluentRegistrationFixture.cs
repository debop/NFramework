using System;
using System.Linq;
using Castle.Core;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.DynamicProxy;
using NSoft.NFramework.Json;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.InversionOfControl {
    /// <summary>
    /// Castle.Windsor의 Installer 를 이용한 컴포넌트 등록을 테스트합니다.
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("IoC")]
    [TestFixture]
    public class FluentRegistrationFixture : AbstractFixture {
        private IWindsorContainer _container;

        protected override void OnSetUp() {
            base.OnSetUp();
            _container = new WindsorContainer();
        }

        protected override void OnTearDown() {
            if(_container != null)
                _container.Dispose();

            _container = null;

            base.OnTearDown();
        }

        #region << One-By-One Registration >>

        [Test]
        public void Component_AllTypes_BasedOn() {
            // Assembly 중에 ICompressor 를 정의한 Assembly 에서 ICompressor 를 구한한 모든 클래스를 등록합니다.
            _container.Register(AllTypes
                                    .FromAssemblyContaining<ICompressor>()
                                    .BasedOn<ICompressor>()
                                    .WithServiceBase());

            var compressors = _container.ResolveAll<ICompressor>();
            compressors.Should().Not.Be.Null();
            compressors.Length.Should().Be.GreaterThan(0);
        }

        // BUG : CompressSerializer.Generic 를 Resolve 하지 못한다.

        /// <summary>
        /// For/ImplementedBy/Named 쌍이 가장 기본적인 컴포넌트 정의 방식이다. (Named 안해도 된다)
        /// </summary>
        [Test]
        public void Component_For_ImplementedBy() {
            // 특정 Service를 구현한 Class를 지정하여 등록합니다. 
            _container.Register(
                Component
                    .For<ICompressor>()
                    .ImplementedBy<SharpBZip2Compressor>()
                    .Named("SharpBZip2")
                    .LifeStyle.Singleton,
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(CompressSerializer<>))
                    .Named("CompressSerializer.Generic")
                    .DependsOn(Dependency.OnComponent("Compressor", "SharpBZip2"))
                    .LifeStyle.PerThread);

            _container.ResolveAll<ICompressor>().Count().Should().Be(1);
            _container.Resolve<ISerializer<UserInfo>>("CompressSerializer.Generic").Should().Not.Be.Null();
        }

        // BUG : CompressSerializer Resolve에 실패 

        /// <summary>
        /// For/ImplementedBy/Named 쌍이 가장 기본적인 컴포넌트 정의 방식이다. (Named 안해도 된다)
        /// </summary>
        [Test]
        public void Component_For_ImplementedBy_GenericType() {
            _container.Register(Component
                                    .For(typeof(ISerializer<>))
                                    .ImplementedBy(typeof(CompressSerializer<>))
                                    .Named("CompressSerializer")
                                    .LifeStyle.PerThread,
                                Component
                                    .For(typeof(ISerializer<>))
                                    .ImplementedBy(typeof(EncryptSerializer<>))
                                    .Named("EncryptSerializer")
                                    .LifeStyle.PerThread);

            _container.ResolveAll<ISerializer<UserInfo>>().Length.Should().Be(2);
            _container.Resolve<ISerializer<UserInfo>>("CompressSerializer").Should().Not.Be.Null();
        }

        [Test]
        public void Register_Instance() {
            // 인스턴스를 직접 등록하면, Lifestyle 지정은 무시됩니다. 또한 예외가 발생할 소지가 큽니다.
            // 아래의 Component Factory delegate 를 활용하는 것이 좋습니다.

            var compressor = new SharpBZip2Compressor();
            _container.Register(Component
                                    .For<ICompressor>()
                                    .Instance(compressor));

            _container.Resolve<ICompressor>().Should().Be.InstanceOf<SharpBZip2Compressor>();
        }

        /// <summary>
        /// UsingFactoryMethod 를 이용하여 특정 컴포넌트의 인스턴스 생성을 Factory 를 사용하도록 했다.
        /// </summary>
        [Test]
        public void Register_By_ComponentFactory() {
            // 특정 컴포넌트의 인스턴스 생성을 Factory 를 사용하도록 했다.

            With.TryAction(() => _container.AddFacility<TypedFactoryFacility>());

            _container
                .Register(Component
                              .For<ICompressor>()
                              .UsingFactoryMethod(() => new SharpBZip2Compressor()));

            _container.Resolve<ICompressor>().Should().Be.InstanceOf<SharpBZip2Compressor>();

            _container
                .Register(Component
                              .For<ISerializer>()
                              .UsingFactoryMethod(() => new JsonByteSerializer()));

            _container.Resolve<ISerializer>().Should().Be.InstanceOf<JsonByteSerializer>();
        }

        /// <summary>
        /// OnCreate 시에 컴포넌트의 속성을 지정할 수 있습니다.
        /// </summary>
        [Test]
        public void Register_And_Setting_Property_By_OnCreate() {
            Func<DateTime> @getDate = () => DateTime.Today;
            _container
                .Register(Component
                              .For<ITimeBlock>()
                              .ImplementedBy<TimeBlock>()
                              .OnCreate((kernel, instance) => {
                                            instance.Start = @getDate();
                                            instance.Duration = TimeSpan.FromDays(1);
                                        })
                              .LifeStyle.Transient);

            var block = _container.Resolve<ITimeBlock>();
            var today = @getDate();
            block.Start.Should().Be(today);
            block.Duration.Should().Be(TimeSpan.FromDays(1));
            block.Should().Be(new TimeBlock(today, TimeSpan.FromDays(1)));
        }

        /// <summary>
        /// 다른 컴포넌트의 생성자의 인자 또는 속성으로 컴포넌트를 주입하는 Dependency Injection 을 수행합니다.
        /// </summary>
        [Test]
        public void Supplying_The_Component_For_A_Dependency_To_Use() {
            // new CompressorSerializer<T>( new JsonByteSerializer<T>(), new SharpBZip2Compressor() );

            _container.Register(
                Component.For<ICompressor>()
                    .ImplementedBy<SharpBZip2Compressor>(),
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(JsonByteSerializer<>)),
                // 위의 두 컴포넌트를 속성으로 Dependency Injection 을 수행합니다.
                Component
                    .For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(CompressSerializer<>))
                    .Named("CompressSerializer")
                    .DependsOn(Dependency.OnComponent("serializer", typeof(JsonByteSerializer<>)),
                               Dependency.OnComponent("compressor", typeof(SharpBZip2Compressor)))
                );

            var component = _container.Resolve<ISerializer<UserInfo>>("CompressSerializer");

            component.Should().Not.Be.Null();
            component.Should().Be.InstanceOf<CompressSerializer<UserInfo>>();

            var compressSerializer = (CompressSerializer<UserInfo>)component;
            compressSerializer.Serializer.Should().Not.Be.Null();
            compressSerializer.Serializer.Should().Be.InstanceOf<JsonByteSerializer<UserInfo>>();

            compressSerializer.Compressor.Should().Not.Be.Null();
            compressSerializer.Compressor.Should().Be.InstanceOf<SharpBZip2Compressor>();
        }

        /// <summary>
        /// 여러 서비스 Interface 에 대응하는 하나의 구현체를 등록합니다.
        /// </summary>
        [Test]
        public void Register_Component_With_Multiple_Services() {
            _container.Register(
                Component
                    .For<ITimePeriod, ITimeRange>()
                    .ImplementedBy<TimeRange>());

            _container.Resolve<ITimePeriod>().Should().Be.InstanceOf<TimeRange>();
            _container.Resolve<ITimeRange>().Should().Be.InstanceOf<TimeRange>();
        }

        [Test]
        public void Serivce_Forwarding() {
            _container.Register(
                Component.For<ITimeRange>()
                    .Forward<ITimePeriod, IComparable>()
                    .ImplementedBy<TimeRange>());

            _container.Resolve<ITimePeriod>().Should().Be.InstanceOf<TimeRange>();
            _container.Resolve<IComparable>().Should().Be.InstanceOf<TimeRange>();
            _container.Resolve<ITimeRange>().Should().Be.InstanceOf<TimeRange>();
        }

        #endregion

        #region << Conditional Registration >>

        [Test]
        public void Conditional_Registration_Unless() {
            // 등록되지 않은 경우에만 등록합니다.


            _container.Register(
                Component.For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(JsonByteSerializer<>))
                    .OnlyNewServices());
            //.Unless(Component.ServiceAlreadyRegistered));

            _container.Register(
                Component.For(typeof(ISerializer<>))
                    .ImplementedBy(typeof(CompressJsonSerializer<>))
                    .OnlyNewServices());
            //.Unless(Component.ServiceAlreadyRegistered));

            //! Open Generic Type을 등록해 놓고, Resolve 시에 Close Generic Type 을 Resolve 해도 잘 되네^^

            var serializer = _container.Resolve<ISerializer<UserInfo>>();
            serializer.Should().Not.Be.Null();
            serializer.Should().Be.InstanceOf<JsonByteSerializer<UserInfo>>();

            var serializer2 = _container.Resolve<ISerializer<UserInfo.AddressInfo>>();
            serializer2.Should().Not.Be.Null();
            serializer2.Should().Be.InstanceOf<JsonByteSerializer<UserInfo.AddressInfo>>();
        }

        [Test]
        public void Conditional_Registration_Unless_On_Multiple_Types() {
            // 특정 수형(ITimePeriod)을 구현한 클래스 중에 TimePeriodBase로 Casting 할 수 없는 수형만 등록합니다.

            _container.Register(
                AllTypes
                    .FromAssemblyContaining<ITimePeriod>()
                    .BasedOn(typeof(ITimePeriod))
                    .WithServiceBase()
                    .Unless(t => typeof(TimePeriodBase).IsAssignableFrom(t)));

            var components = _container.ResolveAll<ITimePeriod>();
            components.Length.Should().Be.GreaterThan(0);
            components.RunEach(c => Console.WriteLine(c.GetType().FullName));
        }

        [Test]
        public void Conditional_Registration_If_On_Multiple_Types() {
            // 특정 수형(ITimePeriod)을 구현한 클래스 중에 TimePeriodBase로 Casting 할 수 있는 수형만 등록합니다.

            _container.Register(
                AllTypes
                    .FromAssemblyContaining<ITimePeriod>()
                    .BasedOn(typeof(ITimePeriod))
                    .WithServiceBase()
                    .If(t => typeof(TimePeriodBase).IsAssignableFrom(t) && t.HasDefaultConstructor()));

            var components = _container.ResolveAll<ITimePeriod>();
            components.Length.Should().Be.GreaterThan(0);
            components.RunEach(c => Console.WriteLine(c.GetType().FullName));
        }

        [Test]
        public void Conditional_Registration_If_On_Multiple_Types2() {
            // 특정 수형(ITimePeriod)을 구현한 클래스 중에 수형중에 "Block" 이란 명칭이 들어간 수형만 등록합니다. (여기서는 TimeBlock 하나만!!!)

            _container.Register(
                AllTypes
                    .FromAssemblyContaining<ITimePeriod>()
                    .BasedOn(typeof(ITimePeriod))
                    .WithServiceBase()
                    .If(t => t.FullName.Contains("Block") && t.HasDefaultConstructor()));

            var components = _container.ResolveAll<ITimePeriod>();
            components.Length.Should().Be.GreaterThan(0);
            components.RunEach(c => Console.WriteLine(c.GetType().FullName));
        }

        #endregion

        #region << Registering components by conventions >>

        //  이 부분은 Castle.Windsor 3.0 이상에서 제공되는 Classes 를 기반으로 하고 있다.

        [Test]
        public void Register_All_Class_InSameNamespaceAs() {
            _container.Register(AllTypes
                                    .FromAssemblyContaining<ICompressor>()
                                    .BasedOn<ICompressor>()
                                    .WithServiceBase());

            _container.ResolveAll<ICompressor>().Length.Should().Be.GreaterThan(0);
        }

        #endregion

        #region << Registering Interceptors and ProxyOptions >>

        [Test]
        public void Regiser_Interceptors() {
            // 특정 인터페이스에 대해 

            _container.Register(
                Component
                    .For(typeof(ISerializer<>))
                    .Interceptors(InterceptorReference.ForType<NotifyPropertyChangedInterceptor>(),
                                  InterceptorReference.ForType<EditableObjectInterceptor>())
                    .Last,
                Component
                    .For<ISerializer>()
                    .Interceptors(InterceptorReference.ForType<EditableObjectInterceptor>())
                    .Last,
                Component.For<NotifyPropertyChangedInterceptor>(),
                Component.For<EditableObjectInterceptor>());

            var serializer = _container.Resolve(typeof(ISerializer<UserInfo>));
            serializer.Should().Not.Be.Null();

            // interceptor 가 씌워진 Proxy인지 판단합니다.
            serializer.IsDynamicProxy().Should().Be.True();
        }

        [Test]
        public void Register_Interceptor_Proxy_MixIn() {
            _container.Register(Component.For<ICompressor>()
                                    .ImplementedBy<SharpBZip2Compressor>()
                                    .Proxy.MixIns(new EditableObjectInterceptor()));

            var component = _container.Resolve<ICompressor>();
            component.Should().Not.Be.Null();
            component.IsDynamicProxy().Should().Be.True();
            Console.WriteLine(component.GetType().FullName);
        }

        #endregion

        #region << Fluent Registration API Extensions >>

        /// <summary>
        /// 등록된 컴포넌트의 Activator 즉 생성자를 지정할 수 있습니다.
        /// http://docs.castleproject.org/Windsor.Fluent-Registration-API-Extensions.ashx
        /// </summary>
        [Test]
        [Ignore("Activator 제작이 상당히 복잡하다.")]
        public void Extensions_Custom_Activiator() {
            _container.Register(Component.For<ICompressor>()
                                    .ImplementedBy<SharpBZip2Compressor>()
                                    .Named("Compressor.SharpBZip2"));
            // .Activator<CompressorActivator>());

            _container.Resolve<ICompressor>("Compressor.SharpBZip2").Should().Not.Be.Null();
        }

        [Test]
        [Ignore("뭔지 모르겠음")]
        public void Extensions_Configuration() {
            // http://docs.castleproject.org/Windsor.Fluent-Registration-API-Extensions.ashx

            _container.Register(
                Component.For<UserInfo>()
                    .Configuration(
                        Child.ForName("HomeAddr").Eq(Attrib.ForName("Street").Eq("집"),
                                                     Attrib.ForName("Phone").Eq("444-4444")),
                        Child.ForName("OfficeAddr").Eq(Attrib.ForName("Street").Eq("사무실"),
                                                       Attrib.ForName("Phone").Eq("555-5555"))));

            var userInfo = _container.Resolve<UserInfo>();
            userInfo.Should().Not.Be.Null();
            userInfo.HomeAddr.Street.Should().Be("집");
            userInfo.OfficeAddr.Street.Should().Be("사무실");
        }

        [Test]
        [Ignore("뭔지 모르겠음")]
        public void Extensions_Extended_Properties() {
            // http://docs.castleproject.org/Windsor.Fluent-Registration-API-Extensions.ashx

            _container.Register(
                Component.For<UserInfo>()
                    .ExtendedProperties(
                        Property.ForKey("HomeAddr").Eq(new UserInfo.AddressInfo { Street = "집" }),
                        Property.ForKey("OfficeAddr").Eq(new UserInfo.AddressInfo { Street = "사무실" })));


            var userInfo = _container.Resolve<UserInfo>();
            userInfo.Should().Not.Be.Null();
            userInfo.HomeAddr.Street.Should().Be("집");
            userInfo.OfficeAddr.Street.Should().Be("사무실");
        }

        #endregion
    }
}