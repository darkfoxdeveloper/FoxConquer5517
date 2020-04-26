// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Syndicate Totem.cs
// File Created: 2015/08/03 12:54

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqSyntotemMap : ClassMap<DbSyntotem>
    {
        public CqSyntotemMap()
        {
            Table("cq_syntotem");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Synid).Column("synid").Not.Nullable();
            Map(x => x.Userid).Column("userid").Not.Nullable();
            Map(x => x.Itemid).Column("itemid").Not.Nullable();
            Map(x => x.Username).Column("name").Not.Nullable();
        }
    }
}