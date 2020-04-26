// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Magictype.cs
// File Created: 2015/08/03 12:35

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqMagictypeMap : ClassMap<DbMagictype>
    {
        public CqMagictypeMap()
        {
            Table("cq_magictype");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.Sort).Column("sort").Not.Nullable();
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.Crime).Column("crime").Not.Nullable();
            Map(x => x.Ground).Column("ground").Not.Nullable();
            Map(x => x.Multi).Column("multi").Not.Nullable();
            Map(x => x.Target).Column("target").Not.Nullable();
            Map(x => x.Level).Column("level").Not.Nullable();
            Map(x => x.UseMp).Column("use_mp").Not.Nullable();
            Map(x => x.Power).Column("power").Not.Nullable();
            Map(x => x.IntoneSpeed).Column("intone_speed").Not.Nullable();
            Map(x => x.Percent).Column("percent").Not.Nullable();
            Map(x => x.StepSecs).Column("step_secs").Not.Nullable();
            Map(x => x.Range).Column("range").Not.Nullable();
            Map(x => x.Distance).Column("distance").Not.Nullable();
            Map(x => x.Status).Column("status").Not.Nullable();
            Map(x => x.NeedProf).Column("need_prof").Not.Nullable();
            Map(x => x.NeedExp).Column("need_exp").Not.Nullable();
            Map(x => x.NeedLevel).Column("need_level").Not.Nullable();
            Map(x => x.UseXp).Column("use_xp").Not.Nullable();
            Map(x => x.WeaponSubtype).Column("weapon_subtype").Not.Nullable();
            Map(x => x.ActiveTimes).Column("active_times").Not.Nullable();
            Map(x => x.AutoActive).Column("auto_active").Not.Nullable();
            Map(x => x.FloorAttr).Column("floor_attr").Not.Nullable();
            Map(x => x.AutoLearn).Column("auto_learn").Not.Nullable();
            Map(x => x.LearnLevel).Column("learn_level").Not.Nullable();
            Map(x => x.DropWeapon).Column("drop_weapon").Not.Nullable();
            Map(x => x.UseEp).Column("use_ep").Not.Nullable();
            Map(x => x.WeaponHit).Column("weapon_hit").Not.Nullable();
            Map(x => x.UseItem).Column("use_item").Not.Nullable();
            Map(x => x.NextMagic).Column("next_magic").Not.Nullable();
            Map(x => x.DelayMs).Column("delay_ms").Not.Nullable();
            Map(x => x.UseItemNum).Column("use_item_num").Not.Nullable();
            Map(x => x.WeaponSubtypeNum).Column("weapon_subtype_num").Not.Nullable();
            Map(x => x.ElementType).Column("element_type").Not.Nullable();
            Map(x => x.ElementPower).Column("element_power").Not.Nullable();
        }
    }
}