using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class LotteryMapping : ClassMap<DbGameLottery>
    {
        public LotteryMapping()
        {
            Table(TableName.LOTTERY);
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.Type).Column("type").Not.Nullable().Default("1");
            Map(x => x.Rank).Column("rank").Not.Nullable().Default("8");
            Map(x => x.Chance).Column("chance").Not.Nullable().Default("100");
            Map(x => x.Itemname).Column("prize_name").Not.Nullable().Default("");
            Map(x => x.ItemIdentity).Column("prize_item").Not.Nullable().Default("0");
            Map(x => x.Color).Column("color").Not.Nullable().Default("1");
            Map(x => x.SocketNum).Column("hole_num").Not.Nullable().Default("0");
            Map(x => x.Plus).Column("addition_lev").Not.Nullable().Default("0");
        }
    }
}