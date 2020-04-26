// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Item Player.cs
// File Created: 2015/08/03 12:30

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqItemMap : ClassMap<DbItem>
    {
        public CqItemMap()
        {
            Table(TableName.ITEM);
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.OwnerId).Column("owner_id").Not.Nullable();
            Map(x => x.PlayerId).Column("player_id").Not.Nullable();
            Map(x => x.Amount).Column("amount").Not.Nullable();
            Map(x => x.AmountLimit).Column("amount_limit").Not.Nullable();
            Map(x => x.Ident).Column("ident").Not.Nullable();
            Map(x => x.Position).Column("position").Not.Nullable();
            Map(x => x.Gem1).Column("gem1").Not.Nullable();
            Map(x => x.Gem2).Column("gem2").Not.Nullable();
            Map(x => x.Magic1).Column("magic1").Not.Nullable();
            Map(x => x.Magic2).Column("magic2").Not.Nullable();
            Map(x => x.Magic3).Column("magic3").Not.Nullable();
            Map(x => x.Data).Column("data").Not.Nullable();
            Map(x => x.ReduceDmg).Column("reduce_dmg").Not.Nullable();
            Map(x => x.AddLife).Column("add_life").Not.Nullable();
            Map(x => x.AntiMonster).Column("anti_monster").Not.Nullable();
            Map(x => x.ChkSum).Column("chk_sum").Not.Nullable();
            Map(x => x.Plunder).Column("plunder").Not.Nullable();
            Map(x => x.RemainingTime).Column("remaining_time").Not.Nullable();
            Map(x => x.Specialflag).Column("SpecialFlag").Not.Nullable();
            Map(x => x.Color).Column("color").Not.Nullable();
            Map(x => x.AddlevelExp).Column("Addlevel_exp").Not.Nullable();
            Map(x => x.Monopoly).Column("monopoly").Not.Nullable();
            Map(x => x.Inscribed).Column("inscribed").Not.Nullable();
            Map(x => x.ArtifactType).Column("artifact_type").Not.Nullable();
            Map(x => x.ArtifactStart).Column("artifact_start").Not.Nullable();
            Map(x => x.ArtifactExpire).Column("artifact_expire").Not.Nullable();
            Map(x => x.ArtifactStabilization).Column("artifact_stabilization").Not.Nullable();
            Map(x => x.RefineryType).Column("refinery_type").Not.Nullable();
            Map(x => x.RefineryLevel).Column("refinery_level").Not.Nullable();
            Map(x => x.RefineryStart).Column("refinery_start").Not.Nullable();
            Map(x => x.RefineryExpire).Column("refinery_expire").Not.Nullable();
            Map(x => x.RefineryStabilization).Column("refinery_stabilization").Not.Nullable();
            Map(x => x.StackAmount).Column("stack_amount").Not.Nullable().Default("1");
        }
    }
}