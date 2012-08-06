using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework {
    [TestFixture]
    public class DisposableActionFixture {
        [Test]
        public void DisposableActionWhichCalledAtDisposing() {
            TestTool
                .RunTasks(5,
                          () => {
                              bool calledAtDisposing = false;

                              using(new DisposableAction(() => { calledAtDisposing = true; })) {
                                  Thread.Sleep(50);
                                  Assert.IsFalse(calledAtDisposing);
                              }

                              Assert.IsTrue(calledAtDisposing);
                          });
        }

        [Test]
        public void DisposableActionGetsCorrectParameterFromCtor() {
            const int expected = 4543;

            TestTool
                .RunTasks(5,
                          () => {
                              int actual = 0;

                              // Disposing 되었을 때, actual 값이 변경됩니다.
                              var action = new DisposableAction<int>(delegate(int i) { actual = i; }, expected);

                              Assert.AreNotEqual(expected, actual);

                              action.Dispose();

                              Assert.AreEqual(expected, actual);
                          });
        }
    }
}