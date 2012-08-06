using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Repositories {
    /// <summary>
    /// SearchFilter를 이용하여, Detached Criteria를 쉽게 만들고, 검색을 쉽게 할 수 있다.
    /// </summary>
    [TestFixture]
    public class SearchFilterTextFixture : NHRepositoryTestFixtureBase {
        [Test]
        public void SearchParentByName() {
            var searchFilter = new ParentSearchFilter();

            searchFilter.Name = "A";
            // searchFilter.Name = "Parent";

            var dc = Repository<Parent>.CreateDetachedCriteria();
            searchFilter.Apply(dc);

            var parents = Repository<Parent>.FindAll(dc);

            foreach(var parent in parents)
                Console.WriteLine(parent.ToString(true));
        }
    }

    /// <summary>
    /// http://ayende.com/Blog/archive/2008/10/16/making-the-complex-trivial-rich-domain-querying.aspx
    /// 에 DetachedCriteria 만들기 어려울 때 SearchFilter를 이용해서 작업 할 수 있도록 한다.... 이게 좋을까???
    /// 
    /// 상세 검색을 많이 사용하는 놈은 사용하는게 좋을 듯하다. (여러가지 검색 옵션이 있을 경우 if 문을 많이 사용하는데...
    /// </summary>
    public abstract class AbstractSearchFilter {
        protected IDictionary<string, Action<DetachedCriteria>> _actions = new Dictionary<string, Action<DetachedCriteria>>();

        public void Apply(DetachedCriteria criteria) {
            foreach(var action in _actions.Values)
                action(criteria);
        }

        public virtual void Clear() {
            _actions.Clear();
        }
    }

    public class ParentSearchFilter : AbstractSearchFilter {
        private string _name;

        public string Name {
            get { return _name; }
            set {
                _name = value;

                if(_actions.ContainsKey("Name"))
                    _actions.Remove("Name");

                _actions.Add("Name", dc => {
                                         if(_name.IsNotWhiteSpace())
                                             dc.Add(Restrictions.Like("Name", _name, MatchMode.Start));
                                     });
            }
        }
    }
}