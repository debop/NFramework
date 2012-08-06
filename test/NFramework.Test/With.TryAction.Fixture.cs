using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework {
    public class WithTryActionFixture : AbstractFixture {
        [Test]
        public void TryAction_Test() {
            var isCalled = false;
            var exceptionRaised = false;

            With.TryAction(() => {
                               isCalled = true;
                               throw new InvalidOperationException("Test");
                           },
                           ex => {
                               if(ex is InvalidOperationException)
                                   exceptionRaised = true;
                           },
                           () => {
                               isCalled.Should().Be.True();
                               exceptionRaised.Should().Be.True();
                           });
        }

        [Test]
        public void TryFunction_Test() {
            var isCalled = false;
            var exceptionRaised = false;

            var result =
                With.TryFunction<int>(() => {
                                          isCalled = true;
                                          throw new InvalidOperationException("Test");
                                      },
                                      null,
                                      ex => {
                                          if(ex is InvalidOperationException)
                                              exceptionRaised = true;
                                      },
                                      () => {
                                          isCalled.Should().Be.True();
                                          exceptionRaised.Should().Be.True();
                                      });
        }

        [Test]
        public void TryActionAsync_Test() {
            var isCalled = false;
            var exceptionRaised = false;

            With.TryActionAsync(() => {
                                    Task.Factory.StartNew(() => { isCalled = true; }).Wait();
                                    throw new AggregateException("AggEx", new InvalidOperationException("Test"));
                                },
                                age => age.Handle(ex => {
                                                      if(ex is InvalidOperationException)
                                                          exceptionRaised = true;

                                                      return true;
                                                  }),
                                () => {
                                    Assert.IsTrue(isCalled);
                                    Assert.IsTrue(exceptionRaised);
                                });
        }

        [Test]
        public void TryFunctionAsync_Test() {
            var isCalled = false;
            var exceptionRaised = false;

            var result =
                With.TryFunctionAsync<int>(() => {
                                               Task.Factory.StartNew(() => { isCalled = true; })
                                                   .Wait();
                                               throw new AggregateException("AggEx", new InvalidOperationException("Test"));
                                           },
                                           null,
                                           age => age.Handle(ex => {
                                                                 if(ex is InvalidOperationException)
                                                                     exceptionRaised = true;

                                                                 return true;
                                                             }),
                                           () => {
                                               Assert.IsTrue(isCalled);
                                               Assert.IsTrue(exceptionRaised);
                                           });
        }
    }
}