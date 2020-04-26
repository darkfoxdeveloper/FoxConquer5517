// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Weapon Skill.cs
// File Created: 2015/08/03 12:58

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqWeaponSkillMap : ClassMap<DbWeaponSkill>
    {
        public CqWeaponSkillMap()
        {
            Table(TableName.WEAPON_SKILL);
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.Level).Column("level").Not.Nullable();
            Map(x => x.OldLevel).Column("old_level").Not.Nullable();
            Map(x => x.OwnerIdentity).Column("owner_id").Not.Nullable();
            Map(x => x.Experience).Column("exp").Not.Nullable();
            Map(x => x.Unlearn).Column("unlearn").Not.Nullable();
        }
    }
}