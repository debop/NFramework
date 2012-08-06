using System;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    [TestFixture]
    public class ReflectionToolFixture : AbstractFixture {
        private const string AssemblyName = "NSoft.NFramework";

        [Test]
        public void LoadAssemblyTest() {
            var asm = ReflectionTool.LoadAssembly(AssemblyName + ".dll");

            Assert.IsNotNull(asm);
            Assert.IsTrue(asm.FullName.StartsWith(AssemblyName));
        }

        [Test]
        public void Can_ObjectToString() {
            var asm = ReflectionTool.LoadAssembly(AssemblyName + ".dll");

            Assert.IsNotNull(asm);
            Console.WriteLine(asm.ObjectToString());
            Assert.IsTrue(asm.ObjectToString().Contains(AssemblyName));
        }
    }
}