// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Region.cs
// File Created: 2015/08/03 12:48

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqRegionMap : ClassMap<DbRegion>
    {
        public CqRegionMap()
        {
            Table("cq_region");
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.MapIdentity).Column("mapid").Not.Nullable();
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.BoundX).Column("bound_x").Not.Nullable();
            Map(x => x.BoundY).Column("bound_y").Not.Nullable();
            Map(x => x.BoundCX).Column("bound_cx").Not.Nullable();
            Map(x => x.BoundCY).Column("bound_cy").Not.Nullable();
            Map(x => x.DataString).Column("datastr").Not.Nullable();
            Map(x => x.Data0).Column("data0").Not.Nullable();
            Map(x => x.Data1).Column("data1").Not.Nullable();
            Map(x => x.Data2).Column("data2").Not.Nullable();
            Map(x => x.Data3).Column("data3").Not.Nullable();
            Map(x => x.ActionId).Column("actionid").Not.Nullable();
        }
    }
}