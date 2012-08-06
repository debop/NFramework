using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class UserDto {
        public virtual int Id { get; private set; }
        public virtual string Name { get; private set; }
        public virtual int InvalidLoginAttempts { get; set; }
        public virtual string RoleName { get; set; }
        public virtual UserDto2 Dto2 { get; set; }

        public UserDto(int id, string name) {
            Id = id;
            Name = name;
        }
    }
}