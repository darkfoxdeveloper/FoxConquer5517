// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Dynamic Map.cs
// File Created: 2015/08/01 15:59

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqDynamapMap : ClassMap<DbDynamicMap>
    {
        public CqDynamapMap()
        {
            Table(TableName.DYNAMAP);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.Description).Column("describe_text").Not.Nullable();
            Map(x => x.MapDoc).Column("mapdoc").Not.Nullable();
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.OwnerId).Column("owner_id").Not.Nullable();
            Map(x => x.Mapgroup).Column("mapgroup").Not.Nullable();
            Map(x => x.Idxserver).Column("idxserver").Not.Nullable();
            Map(x => x.Weather).Column("weather").Not.Nullable();
            Map(x => x.Bgmusic).Column("bgmusic").Not.Nullable();
            Map(x => x.BgmusicShow).Column("bgmusic_show").Not.Nullable();
            Map(x => x.Portal0X).Column("portal0_x").Not.Nullable();
            Map(x => x.Portal0Y).Column("portal0_y").Not.Nullable();
            Map(x => x.RebornMapid).Column("reborn_mapid").Not.Nullable();
            Map(x => x.RebornPortal).Column("reborn_portal").Not.Nullable();
            Map(x => x.ResLev).Column("res_lev").Not.Nullable();
            Map(x => x.OwnerType).Column("owner_type").Not.Nullable();
            Map(x => x.LinkMap).Column("link_map").Not.Nullable();
            Map(x => x.LinkX).Column("link_x").Not.Nullable();
            Map(x => x.LinkY).Column("link_y").Not.Nullable();
            Map(x => x.DelFlag).Column("del_flag").Not.Nullable();
            Map(x => x.Color).Column("color");
            Map(x => x.FileName).Column("file_name").Not.Nullable();
        }
    }
}