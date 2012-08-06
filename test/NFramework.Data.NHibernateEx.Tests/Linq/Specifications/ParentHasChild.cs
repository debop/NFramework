using System;
using System.Linq;
using System.Linq.Expressions;
using LinqSpecs;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.Linq.Specifications {
    public class ParentHasChild : Specification<Parent> {
        public ParentHasChild(string childName) {
            ChildName = childName;
        }

        public string ChildName { get; private set; }

        public override Expression<Func<Parent, bool>> IsSatisfiedBy() {
            return p => p.Children.Any(c => c.Name == ChildName);
        }
    }
}