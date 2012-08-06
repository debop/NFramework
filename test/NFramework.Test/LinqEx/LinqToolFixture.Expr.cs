using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.LinqEx {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class LinqToolFixture_Expr {
        [Test]
        public void Can_Retrieve_MebmerName_From_Expression() {
            Expression<Func<TimeSpan, object>> expr = (ts) => ts.TotalMilliseconds;

            var propertyName = LinqTool.FindMemberName(expr.Body);

            propertyName.Should().Be("TotalMilliseconds");
        }

        [Test]
        public void Can_Retrieve_PropertyName_From_Expression() {
            Expression<Func<TimeSpan, object>> expr = (ts) => ts.TotalMilliseconds;

            var propertyName = LinqTool.FindPropertyName(expr.Body);

            propertyName.Should().Be("TotalMilliseconds");
        }
    }
}