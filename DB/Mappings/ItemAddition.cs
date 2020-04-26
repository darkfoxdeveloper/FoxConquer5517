// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Item Addition.cs
// File Created: 2015/08/03 12:28

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqItemAddition : ClassMap<DbItemAddition>
    {
        public CqItemAddition()
        {
            Table(TableName.ITEMADDITION);
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.TypeId).Column("typeid").Not.Nullable();
            Map(x => x.Level).Default("0").Column("level").Not.Nullable();
            Map(x => x.Life).Default("0").Column("life").Not.Nullable();
            Map(x => x.AttackMax).Column("attack_max").Default("0").Not.Nullable();
            Map(x => x.AttackMin).Column("attack_min").Default("0").Not.Nullable();
            Map(x => x.Defense).Column("defense").Default("0").Not.Nullable();
            Map(x => x.MagicAtk).Column("magic_atk").Default("0").Not.Nullable();
            Map(x => x.MagicDef).Column("magic_def").Default("0").Not.Nullable();
            Map(x => x.Dexterity).Column("dexterity").Default("0").Not.Nullable();
            Map(x => x.Dodge).Column("dodge").Default("0").Not.Nullable();
        }
    }
}