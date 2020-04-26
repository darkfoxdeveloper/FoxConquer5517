// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Nobility Ranking.cs
// File Created: 2015/08/03 12:59

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class DynaRankRecMap : ClassMap<DbDynaRankRec>
    {
        public DynaRankRecMap()
        {
            Table("dyna_rank_rec");
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.RankType).Column("rank_type").Not.Nullable();
            Map(x => x.Value).Column("value").Not.Nullable();
            Map(x => x.ObjectId).Column("obj_id").Not.Nullable();
            Map(x => x.ObjectName).Column("obj_name").Nullable();
            Map(x => x.UserIdentity).Column("user_id").Not.Nullable();
            Map(x => x.Username).Column("user_Name").Nullable();
        }
    }
}