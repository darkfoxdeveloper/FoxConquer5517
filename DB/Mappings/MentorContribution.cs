using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class MentorContributionMap : ClassMap<MentorContribution>
    {
        public MentorContributionMap()
        {
            Table(TableName.TUTOR_CONTRIBUTION);
            LazyLoad();
            Id(x => x.Identity).Column("id").Not.Nullable().GeneratedBy.Identity();
            Map(x => x.TutorIdentity).Column("tutor_id").Not.Nullable().Default("0");
            Map(x => x.StudentIdentity).Column("Student_id").Not.Nullable().Default("0");
            Map(x => x.StudentName).Column("Student_name").Not.Nullable().Default("0");
            Map(x => x.GodTime).Column("God_time").Not.Nullable().Default("0");
            Map(x => x.PlusStone).Column("Exp").Not.Nullable().Default("0");
            Map(x => x.Experience).Column("Addlevel").Not.Nullable().Default("0");
        }
    }
}