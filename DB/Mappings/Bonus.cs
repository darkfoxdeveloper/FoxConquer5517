// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Bonus.cs
// File Created: 2015/08/01 15:58

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqBonusMap : ClassMap<DbGameBonus>
    {
        public CqBonusMap()
        {
            Table(TableName.BONUS);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.Action).Column("action").Not.Nullable();
            Map(x => x.AccountIdentity).Column("id_account").Not.Nullable();
            Map(x => x.Flag).Column("flag").Not.Nullable();
            Map(x => x.ReferenceCode).Column("ref_id").Not.Nullable();
            Map(x => x.Time).Column("time").Not.Nullable();
        }
    }
}