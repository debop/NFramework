using System;
using NUnit.Framework;

namespace NSoft.NFramework.Samples {
    [TestFixture]
    public class EnumerationFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void EnumToStringTest() {
            var shape = Shape.Octagon | Shape.Circle;

            log.Info("shape = Shape.Octagon | Shape.Circle");
            log.Info("shape.ToString()      = {0,10}              ", shape.ToString());
            log.Info("shape.ToString(\"G\") = {0,10} : General    ", shape.ToString("G"));
            log.Info("shape.ToString(\"D\") = {0,10} : Decimal    ", shape.ToString("D"));
            log.Info("shape.ToString(\"F\") = {0,10} : Flag       ", shape.ToString("F"));
            log.Info("shape.ToString(\"X\") = {0,10} : Hexadecimal", shape.ToString("X"));
        }

        [Flags]
        internal enum Shape {
            Square = 1,
            Circle = 2,
            Cylinder = 4,
            Octagon = 8
        }
    }
}