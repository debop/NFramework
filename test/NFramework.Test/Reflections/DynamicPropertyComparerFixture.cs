using System.Collections.Generic;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    [TestFixture]
    public class DynamicPropertyComparerFixture {
        [Test]
        [Ignore("NET 4에서는 보안문제로 사용할 수 없다.")]
        public void CompareTest() {
            const int numWidgets = 100;
            var widgets = new List<Widget>(numWidgets);

            for(var i = 0; i < numWidgets; i++)
                widgets.Add(new Widget(i, "Widget " + i.ToString()));

            widgets.Sort(new DynamicPropertyComparer<Widget>("Name"));

            //foreach(var widget in widgets)
            //    Console.WriteLine(widget);
        }
    }

    public class Widget {
        public int Id { get; set; }
        public string Name { get; set; }

        public Widget(int id, string name) {
            Id = id;
            Name = name;
        }

        public override string ToString() {
            return string.Format("Widget# Id={0}, Name={1}", Id, Name);
        }
    }
}