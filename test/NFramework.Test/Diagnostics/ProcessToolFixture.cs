using NUnit.Framework;

namespace NSoft.NFramework.Diagnostics {
    [TestFixture]
    public class ProcessToolFixture : AbstractFixture {
        /// <summary>
        /// 실행중인 모든 프로세스에서 사용하지 않는 메모리를 OS에 반환하도록 합니다.
        /// </summary>
        [Test]
        public void TrimAllProcessMemoryTest() {
            ProcessTool.TrimAllProcessMemory();
        }
    }
}