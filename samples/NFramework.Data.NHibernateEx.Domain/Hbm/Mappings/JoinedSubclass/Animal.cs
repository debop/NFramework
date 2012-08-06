using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    //! Hint: joined-subclass 에 대한 예제입니다.

    public class Animal : DataEntityBase<int> {
        public virtual string Description { get; set; }
        public virtual double BodyWeight { get; set; }
        public virtual Animal Mother { get; set; }
        public virtual Animal Father { get; set; }
        public virtual IList<Animal> Children { get; set; }
        public virtual string SerialNumber { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(SerialNumber);
        }
    }

    /// <summary>
    /// 파충류
    /// </summary>
    public abstract class Reptile : Animal {
        public virtual double BodyTemperature { get; set; }
    }

    public class Lizard : Reptile {}

    /// <summary>
    /// 포유류
    /// </summary>
    public abstract class Mammal : Animal {
        public virtual bool Pregnant { get; set; }
        public virtual DateTime? BirthDate { get; set; }
    }

    public class Dog : Mammal {}

    public class Cat : Mammal {}
}