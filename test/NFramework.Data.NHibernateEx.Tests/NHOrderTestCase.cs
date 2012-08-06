using System;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx {
    [TestFixture]
    public class NHOrderTestCase {
        [Test]
        public void Generic_NHOrder_ParseOrder() {
            var expression = DynamicExpression.ParseLambda<User, object>("Name");
            var order = new NHOrder<User>(expression);
            Console.WriteLine(order);
        }

        [Test]
        public void OrderByPropertyName() {
            var expected = new NHOrder<User>(u => u.Name);
            var actual = new NHOrder<User>("Name");

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}