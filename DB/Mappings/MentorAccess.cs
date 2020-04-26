using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class MentorAccess : ClassMap<DbMentorAccess>
    {
        public MentorAccess()
        {
            Table(TableName.TUTOR_ACCESS);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Column("id");
            Map(x => x.GuideIdentity).Not.Nullable().Default("0").Column("tutor_id");
            Map(x => x.Experience).Not.Nullable().Default("0").Column("Exp");
            Map(x => x.Blessing).Not.Nullable().Default("0").Column("God_time");
            Map(x => x.Composition).Not.Nullable().Default("0").Column("Addlevel");
        }
    }
}