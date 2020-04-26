using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class MentorMap : ClassMap<DbMentor>
    {
        public MentorMap()
        {
            Table(TableName.TUTOR);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.GuideIdentity).Not.Nullable().Default("0").Column("tutor_id");
            Map(x => x.GuideName).Not.Nullable().Default("").Column("tutor_name");
            Map(x => x.StudentIdentity).Not.Nullable().Default("0").Column("Student_id");
            Map(x => x.StudentName).Not.Nullable().Default("").Column("Student_name");
            Map(x => x.BetrayalFlag).Not.Nullable().Default("0").Column("Betrayal_flag");
            Map(x => x.Date).Not.Nullable().Default("0").Column("Date");
        }
    }
}