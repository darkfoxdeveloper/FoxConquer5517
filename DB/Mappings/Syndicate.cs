// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Syndicate.cs
// File Created: 2015/08/03 12:53

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqSyndicateMap : ClassMap<DbSyndicate>
    {
        public CqSyndicateMap()
        {
            Table("cq_syndicate");
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.Announce).Column("announce").Not.Nullable();
            Map(x => x.AnnounceDate).Column("announce_date").Not.Nullable();
            Map(x => x.LeaderIdentity).Column("leader_id").Not.Nullable();
            Map(x => x.LeaderName).Column("leader_name").Not.Nullable();
            Map(x => x.Money).Column("money").Not.Nullable();
            Map(x => x.EMoney).Column("emoney").Not.Nullable();
            Map(x => x.DelFlag).Column("del_flag").Not.Nullable();
            Map(x => x.Amount).Column("amount").Not.Nullable();
            Map(x => x.TotemHead).Column("totem_head").Not.Nullable();
            Map(x => x.TotemNeck).Column("totem_neck").Not.Nullable();
            Map(x => x.TotemRing).Column("totem_ring").Not.Nullable();
            Map(x => x.TotemWeapon).Column("totem_weapon").Not.Nullable();
            Map(x => x.TotemArmor).Column("totem_armor").Not.Nullable();
            Map(x => x.TotemBoots).Column("totem_boots").Not.Nullable();
            Map(x => x.TotemFan).Column("totem_fan").Not.Nullable();
            Map(x => x.TotemTower).Column("totem_tower").Not.Nullable();
            Map(x => x.LastTotem).Column("last_totem").Not.Nullable();
            Map(x => x.ReqLevel).Column("req_lev").Not.Nullable();
            Map(x => x.ReqClass).Column("req_class").Not.Nullable();
            Map(x => x.ReqMetempsychosis).Column("req_metempsychosis").Not.Nullable();
            Map(x => x.CreationDate).Column("creation_date").Not.Nullable().Default("0");
            Map(x => x.MoneyPrize).Column("money_prize").Not.Nullable().Default("0");
            Map(x => x.EmoneyPrize).Column("emoney_prize").Not.Nullable().Default("0");
        }
    }
}