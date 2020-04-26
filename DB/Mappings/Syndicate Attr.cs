// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Syndicate Attr.cs
// File Created: 2015/08/03 12:53

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqSynattrMap : ClassMap<DbCqSynattr>
    {
        public CqSynattrMap()
        {
            Table("cq_synattr");
            LazyLoad();
            Id(x => x.Id).Column("id").Not.Nullable();
            Map(x => x.SynId).Column("syn_id").Not.Nullable();
            Map(x => x.Rank).Column("rank").Not.Nullable();
            Map(x => x.Proffer).Column("proffer").Not.Nullable();
            Map(x => x.ProfferTotal).Column("proffer_total").Not.Nullable();
            Map(x => x.Emoney).Column("emoney").Not.Nullable();
            Map(x => x.EmoneyTotal).Column("emoney_total").Not.Nullable();
            Map(x => x.Pk).Column("pk").Not.Nullable();
            Map(x => x.PkTotal).Column("pk_total").Not.Nullable();
            Map(x => x.Guide).Column("guide").Not.Nullable();
            Map(x => x.GuideTotal).Column("guide_total").Not.Nullable();
            Map(x => x.Exploit).Column("exploit").Not.Nullable();
            Map(x => x.Arsenal).Column("arsenal").Not.Nullable();
            Map(x => x.Expiration).Column("expiration").Not.Nullable();
            Map(x => x.JoinDate).Column("join_date").Not.Nullable();
        }
    }
}