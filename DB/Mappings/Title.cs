// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Title.cs
// File Created: 2015/08/03 12:56

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqTitleMap : ClassMap<DbTitle>
    {
        public CqTitleMap()
        {
            Table("cq_title");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Userid).Column("userid").Not.Nullable();
            Map(x => x.Title).Column("title").Not.Nullable();
            Map(x => x.Timestamp).Column("timestamp").Not.Nullable();
        }
    }
}