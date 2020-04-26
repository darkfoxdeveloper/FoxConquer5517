using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class FlowerMap : ClassMap<DbFlower>
    {
        public FlowerMap()
        {
            Table(TableName.FLOWER);
            LazyLoad();
            Id(x => x.PlayerIdentity).Column("player_id").Not.Nullable();
            Map(x => x.PlayerName).Column("player_name").Not.Nullable().Default("");
            Map(x => x.RedRoses).Column("flower_r").Not.Nullable().Default("0");
            Map(x => x.WhiteRoses).Column("flower_w").Not.Nullable().Default("0");
            Map(x => x.Orchids).Column("flower_lily").Not.Nullable().Default("0");
            Map(x => x.Tulips).Column("flower_tulip").Not.Nullable().Default("0");
        }
    }
}