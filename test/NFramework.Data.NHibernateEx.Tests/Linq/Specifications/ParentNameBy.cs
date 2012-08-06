using System;
using System.Linq.Expressions;
using LinqSpecs;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.Linq.Specifications {
    public class ParentNameBy : Specification<Parent> {
        public ParentNameBy(string name) {
            Name = name;
        }

        public string Name { get; private set; }

        public override Expression<Func<Parent, bool>> IsSatisfiedBy() {
            return p => p.Name == Name;
        }
    }
}