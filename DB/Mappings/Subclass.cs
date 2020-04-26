// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Subclass.cs
// File Created: 2015/08/03 12:51

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqSubclassMap : ClassMap<DbSubclass>
    {
        public CqSubclassMap()
        {
            Table("cq_subclass");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Userid).Column("userid").Not.Nullable();
            Map(x => x.Class).Column("class").Not.Nullable();
            Map(x => x.Promotion).Column("promotion").Not.Nullable();
            Map(x => x.Level).Column("level").Not.Nullable();
        }
    }
}