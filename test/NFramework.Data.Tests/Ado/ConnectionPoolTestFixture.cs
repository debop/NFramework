using System;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Ado {
    [TestFixture]
    public class ConnectionPoolTestFixture {
        [Test]
        public void CanCloseDataReader() {
            try {
                using(var reader = AdoRepository.ExecuteReader("SELECT * FROM sysobjects")) {
                    // 예외를 일으켜본다. 그래도 Data Reader가 제대로 닫히는지 본다.
                    throw new InvalidOperationException("고의로 일으킨 예외입니다.");
                }
            }
            catch {
                // eat exception
            }
        }
    }
}