using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerClassHierarchy {
    public class PersonalPlan : PlanBase {
        public virtual TimeSpan? ResourceTime { get; set; }
    }
}