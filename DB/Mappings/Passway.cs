// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Passway.cs
// File Created: 2015/08/03 12:44

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqPasswayMap : ClassMap<DbPassway>
    {
        public CqPasswayMap()
        {
            Table(TableName.PASSWAY);
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.MapId).Not.Nullable().Column("mapid");
            Map(x => x.MapIndex).Column("passway_idx").Not.Nullable();
            Map(x => x.TargetMapId).Column("passway_mapid").Not.Nullable();
            Map(x => x.TargetPortal).Column("passway_mapportal").Not.Nullable();
        }
    }
}
