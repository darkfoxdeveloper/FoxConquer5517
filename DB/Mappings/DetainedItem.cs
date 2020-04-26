using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class DetainedItemMap : ClassMap<DbDetainedItem>
    {
        public DetainedItemMap()
        {
            Table(TableName.PK_ITEM);
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.ItemIdentity).Column("item").Not.Nullable().Default("0");
            Map(x => x.TargetIdentity).Column("target").Not.Nullable().Default("0");
            Map(x => x.TargetName).Column("target_name").Not.Nullable().Default("UnknownTarget");
            Map(x => x.HunterIdentity).Column("hunter").Not.Nullable().Default("0");
            Map(x => x.HunterName).Column("hunter_name").Not.Nullable().Default("UnknownHunter");
            Map(x => x.HuntTime).Column("manhunt_time").Not.Nullable().Default("0");
            Map(x => x.RedeemPrice).Column("bonus").Not.Nullable().Default("0");
        }
    }
}