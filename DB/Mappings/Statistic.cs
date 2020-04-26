// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Statistic.cs
// File Created: 2015/08/03 12:48

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqStatisticMap : ClassMap<DbStatistic>
    {
        public CqStatisticMap()
        {
            Table("cq_statistic");
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.PlayerIdentity).Column("player_id").Not.Nullable().Default("0");
            Map(x => x.PlayerName).Column("player_name").Not.Nullable().Default("Undefined");
            Map(x => x.EventType).Column("event_type").Not.Nullable().Default("0");
            Map(x => x.DataType).Column("data_type").Not.Nullable().Default("0");
            Map(x => x.Data).Column("data").Not.Nullable().Default("0");
            Map(x => x.Timestamp).Column("timestamp").Not.Nullable().Default("0");
        }
    }
}