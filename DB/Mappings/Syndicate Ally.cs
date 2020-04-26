// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Syndicate Ally.cs
// File Created: 2015/08/03 12:52

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqSynAllyMap : ClassMap<DbSynAlly>
    {
        public CqSynAllyMap()
        {
            Table("cq_syn_ally");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Synid).Column("synid").Not.Nullable();
            Map(x => x.Synname).Column("synname").Not.Nullable();
            Map(x => x.Allyid).Column("allyid").Not.Nullable();
            Map(x => x.Allyname).Column("allyname").Not.Nullable();
        }
    }
}