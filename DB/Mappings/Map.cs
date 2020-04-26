// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Map.cs
// File Created: 2015/08/03 12:41

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    // lol, dat name tho
    public class CqMapMap : ClassMap<DbMap>
    {
        public CqMapMap()
        {
            Table(TableName.MAP);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Column("id");
            Map(x => x.Name).Not.Nullable().Column("name").Default("");
            Map(x => x.Description).Column("describe_text").Default("by Felipe Vieira");
            Map(x => x.MapDoc).Column("mapdoc");
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.OwnerId).Column("owner_id");
            Map(x => x.MapGroup).Column("mapgroup");
            Map(x => x.IdXServer).Column("idxserver");
            Map(x => x.PortalX).Column("portal0_x");
            Map(x => x.PortalY).Column("portal0_y");
            Map(x => x.RebornMap).Column("reborn_map");
            Map(x => x.RebornPortal).Column("reborn_portal");
            Map(x => x.ResLevel).Column("res_lev");
            Map(x => x.Path).Column("file_name").Not.Nullable();
        }
    }
}