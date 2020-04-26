using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class PkRewardMapping : ClassMap<DbPkReward>
    {
        public PkRewardMapping()
        {
            Table(TableName.PK_BONUS);
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.HunterIdentity).Column("Hunter").Not.Nullable().Default("0");
            Map(x => x.HunterName).Column("Hunter_name").Not.Nullable().Default("");
            Map(x => x.TargetIdentity).Column("Target").Not.Nullable().Default("0");
            Map(x => x.TargetName).Column("Target_name").Not.Nullable().Default("");
            Map(x => x.Bonus).Column("Bonus").Not.Nullable().Default("0");
            Map(x => x.BonusType).Column("B_type").Not.Nullable().Default("0");
        }
    }
}