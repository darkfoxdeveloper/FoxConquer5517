// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Itemtype.cs
// File Created: 2015/08/03 12:31

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqItemtypeMap : ClassMap<DbItemtype>
    {
        public CqItemtypeMap()
        {
            Table(TableName.ITEMTYPE);
            LazyLoad();
            Id(x => x.Type).GeneratedBy.Identity().Column("id");
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.ReqProfession).Column("req_profession").Not.Nullable();
            Map(x => x.ReqWeaponskill).Column("req_weaponskill").Not.Nullable();
            Map(x => x.ReqLevel).Column("req_level").Not.Nullable();
            Map(x => x.ReqSex).Column("req_sex").Not.Nullable();
            Map(x => x.ReqForce).Column("req_force").Not.Nullable();
            Map(x => x.ReqSpeed).Column("req_speed").Not.Nullable();
            Map(x => x.ReqHealth).Column("req_health").Not.Nullable();
            Map(x => x.ReqSoul).Column("req_soul").Not.Nullable();
            Map(x => x.Monopoly).Column("monopoly").Not.Nullable();
            Map(x => x.Weight).Column("weight").Not.Nullable();
            Map(x => x.Price).Column("price").Not.Nullable();
            Map(x => x.IdAction).Column("id_action").Not.Nullable();
            Map(x => x.AttackMax).Column("attack_max").Not.Nullable();
            Map(x => x.AttackMin).Column("attack_min").Not.Nullable();
            Map(x => x.Defense).Column("defense").Not.Nullable();
            Map(x => x.Dexterity).Column("dexterity").Not.Nullable();
            Map(x => x.Dodge).Column("dodge").Not.Nullable();
            Map(x => x.Life).Column("life").Not.Nullable();
            Map(x => x.Mana).Column("mana").Not.Nullable();
            Map(x => x.Amount).Column("amount").Not.Nullable();
            Map(x => x.AmountLimit).Column("amount_limit").Not.Nullable();
            Map(x => x.RequireWeaponType).Column("weapon_req").Not.Nullable();
            Map(x => x.Ident).Column("ident").Not.Nullable();
            Map(x => x.Gem1).Column("gem1").Not.Nullable();
            Map(x => x.Gem2).Column("gem2").Not.Nullable();
            Map(x => x.Magic1).Column("magic1").Not.Nullable();
            Map(x => x.Magic2).Column("magic2").Not.Nullable();
            Map(x => x.Magic3).Column("magic3").Not.Nullable();
            Map(x => x.MagicAtk).Column("magic_atk").Not.Nullable();
            Map(x => x.MagicDef).Column("magic_def").Not.Nullable();
            Map(x => x.AtkRange).Column("atk_range").Not.Nullable();
            Map(x => x.AtkSpeed).Column("atk_speed").Not.Nullable();
            Map(x => x.FrayMode).Column("fray_mode").Not.Nullable();
            Map(x => x.RepairMode).Column("repair_mode").Not.Nullable();
            Map(x => x.TypeMask).Column("type_mask").Not.Nullable();
            Map(x => x.EmoneyPrice).Column("emoney_price").Not.Nullable();
            Map(x => x.BoundEmoneyPrice).Column("emoney2_price").Not.Nullable();
            Map(x => x.CritStrike).Column("crit_strike").Not.Nullable();
            Map(x => x.SkillCritStrike).Column("skill_crit_strike").Not.Nullable();
            Map(x => x.Immunity).Column("immunity").Not.Nullable();
            Map(x => x.Penetration).Column("penetration").Not.Nullable();
            Map(x => x.Block).Column("block").Not.Nullable();
            Map(x => x.Breakthrough).Column("breakthrough").Not.Nullable();
            Map(x => x.Counteraction).Column("counteraction").Not.Nullable();
            Map(x => x.Detoxication).Column("detoxication").Not.Nullable();
            Map(x => x.StackLimit).Column("stack_limit").Not.Nullable();
            Map(x => x.ResistMetal).Column("resist_metal").Not.Nullable();
            Map(x => x.ResistWood).Column("resist_wood").Not.Nullable();
            Map(x => x.ResistWater).Column("resist_water").Not.Nullable();
            Map(x => x.ResistFire).Column("resist_fire").Not.Nullable();
            Map(x => x.ResistEarth).Column("resist_earth").Not.Nullable();
            Map(x => x.Phase).Column("phase").Not.Nullable();
            Map(x => x.MeteorAmount).Column("meteor_num").Not.Nullable();
            Map(x => x.HonorPrice).Column("honor_price").Not.Nullable();
            Map(x => x.LifeTime).Column("life_time").Not.Nullable();
        }
    }
}