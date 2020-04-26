using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class MentorTypeMapping : ClassMap<DbMentorType>
    {
        public MentorTypeMapping()
        {
            Table(TableName.TUTOR_TYPE);
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Not.Nullable().Column("id");
            Map(x => x.UserMinLevel).Not.Nullable().Default("0").Column("User_lev_min");
            Map(x => x.UserMaxLevel).Not.Nullable().Default("0").Column("User_lev_max");
            Map(x => x.StudentNum).Not.Nullable().Default("0").Column("Student_num");
            Map(x => x.BattleLevelShare).Not.Nullable().Default("0").Column("Battle_lev_share");
        }
    }
}