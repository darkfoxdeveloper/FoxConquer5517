// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Portal.cs
// File Created: 2015/08/03 12:46

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqPortalMap : ClassMap<DbPortal>
    {
        public CqPortalMap()
        {
            Table("cq_portal");
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.MapId).Column("mapid").Not.Nullable();
            Map(x => x.PortalIndex).Column("portal_idx").Not.Nullable();
            Map(x => x.PortalX).Column("portal_x").Not.Nullable();
            Map(x => x.PortalY).Column("portal_y").Not.Nullable();
        }
    }
}