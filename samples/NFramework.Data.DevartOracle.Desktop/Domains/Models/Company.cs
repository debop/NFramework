﻿using System;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.DevartOracle.Desktop.Domains.Models {
    /// <summary>
    /// class 의 lazy=false 이면 virtual이 아니더라도 괜찮다.
    /// </summary>
    public class Company : DataEntityBase<Int64>, ICodeEntity, IUpdateTimestampedEntity {
        public Company() {}

        public Company(string code) {
            code.ShouldNotBeEmpty("code");

            Code = code;
        }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Code);
        }
    }
}