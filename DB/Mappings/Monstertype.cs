// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Monstertype.cs
// File Created: 2015/08/03 12:42

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqMonstertypeMap : ClassMap<DbMonstertype>
    {
        public CqMonstertypeMap()
        {
            Table("cq_monstertype");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.Lookface).Column("lookface").Not.Nullable();
            Map(x => x.Life).Column("life").Not.Nullable();
            Map(x => x.Mana).Column("mana").Not.Nullable();
            Map(x => x.AttackMax).Column("attack_max").Not.Nullable();
            Map(x => x.AttackMin).Column("attack_min").Not.Nullable();
            Map(x => x.Defence).Column("defence").Not.Nullable();
            Map(x => x.Dexterity).Column("dexterity").Not.Nullable();
            Map(x => x.Dodge).Column("dodge").Not.Nullable();
            Map(x => x.HelmetType).Column("helmet_type").Not.Nullable();
            Map(x => x.ArmorType).Column("armor_type").Not.Nullable();
            Map(x => x.WeaponrType).Column("weaponr_type").Not.Nullable();
            Map(x => x.WeaponlType).Column("weaponl_type").Not.Nullable();
            Map(x => x.AttackRange).Column("attack_range").Not.Nullable();
            Map(x => x.ViewRange).Column("view_range").Not.Nullable();
            Map(x => x.EscapeLife).Column("escape_life").Not.Nullable();
            Map(x => x.AttackSpeed).Column("attack_speed").Not.Nullable();
            Map(x => x.MoveSpeed).Column("move_speed").Not.Nullable();
            Map(x => x.Level).Column("level").Not.Nullable();
            Map(x => x.AttackUser).Column("attack_user").Not.Nullable();
            Map(x => x.DropMoney).Column("drop_money").Not.Nullable();
            Map(x => x.DropItemtype).Column("drop_itemtype").Not.Nullable();
            Map(x => x.SizeAdd).Column("size_add").Not.Nullable();
            Map(x => x.Action).Column("action").Not.Nullable();
            Map(x => x.RunSpeed).Column("run_speed").Not.Nullable();
            Map(x => x.DropArmet).Column("drop_armet").Not.Nullable();
            Map(x => x.DropNecklace).Column("drop_necklace").Not.Nullable();
            Map(x => x.DropArmor).Column("drop_armor").Not.Nullable();
            Map(x => x.DropRing).Column("drop_ring").Not.Nullable();
            Map(x => x.DropWeapon).Column("drop_weapon").Not.Nullable();
            Map(x => x.DropShield).Column("drop_shield").Not.Nullable();
            Map(x => x.DropShoes).Column("drop_shoes").Not.Nullable();
            Map(x => x.DropHp).Column("drop_hp").Not.Nullable();
            Map(x => x.DropMp).Column("drop_mp").Not.Nullable();
            Map(x => x.MagicType).Column("magic_type").Not.Nullable();
            Map(x => x.MagicDef).Column("magic_def").Not.Nullable();
            Map(x => x.MagicHitrate).Column("magic_hitrate").Not.Nullable();
            Map(x => x.AiType).Column("ai_type").Not.Nullable();
            Map(x => x.Defence2).Column("defence2").Not.Nullable();
            Map(x => x.StcType).Column("stc_type").Not.Nullable();
            Map(x => x.AntiMonster).Column("anti_monster").Not.Nullable();
            Map(x => x.ExtraBattlelev).Column("extra_battlelev").Not.Nullable();
            Map(x => x.ExtraExp).Column("extra_exp").Not.Nullable();
            Map(x => x.ExtraDamage).Column("extra_damage").Not.Nullable();
            Map(x => x.WaterAtk).Column("water_atk").Not.Nullable();
            Map(x => x.FireAtk).Column("fire_atk").Not.Nullable();
            Map(x => x.EarthAtk).Column("earth_atk").Not.Nullable();
            Map(x => x.WoodAtk).Column("wood_atk").Not.Nullable();
            Map(x => x.MetalAtk).Column("metal_atk").Not.Nullable();
            Map(x => x.WaterDef).Column("water_def").Not.Nullable();
            Map(x => x.FireDef).Column("fire_def").Not.Nullable();
            Map(x => x.EarthDef).Column("earth_def").Not.Nullable();
            Map(x => x.WoodDef).Column("wood_def").Not.Nullable();
            Map(x => x.MetalDef).Column("metal_def").Not.Nullable();
            Map(x => x.Boss).Column("boss").Not.Nullable();
        }
    }
}