using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CoreTaskMap : ClassMapping<CoreTask> {
        public CoreTaskMap() {
            Table("CoreTask");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(task => task.Id, c => {
                                    c.Column("TaskId");
                                    c.Generator(Generators.Identity);
                                    c.Type(new Int32Type());
                                });

            Property(task => task.Name, c => {
                                            c.NotNullable(true);
                                            c.Length(256);
                                        });

            Property(task => task.Planned);

            Property(task => task.Notes, c => {
                                             c.NotNullable(false);
                                             c.Length(9999);
                                         });

            Property(task => task.Estimation);

            Property(task => task.CreatedAt, c => c.Index("IX_Task_UserId_CreatedAt"));

            Property(task => task.DueAt, c => c.Index("IX_CoreTask_Completed"));
            Property(task => task.CompletedAt, c => c.Index("IX_CoreTask_Completed"));
            Property(task => task.Archieved, c => c.Index("IX_CoreTask_Completed"));

            ManyToOne(task => task.User, c => {
                                             c.Column("UserId");
                                             c.NotNullable(true);
                                             c.ForeignKey("FK_Task_User");
                                             c.Index("IX_Task_UserId_CreatedAt");
                                         });
        }
    }
}