// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Refinery.cs
// File Created: 2015/08/03 12:47

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqRefineryMap : ClassMap<DbRefinery>
    {
        public CqRefineryMap()
        {
            Table("cq_refinery");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.Itemtype).Column("itemtype").Not.Nullable();
            Map(x => x.Level).Column("level").Not.Nullable();
            Map(x => x.Power).Column("power").Not.Nullable();
        }
    }
}