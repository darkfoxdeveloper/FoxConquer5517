// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Rebirth.cs
// File Created: 2015/08/03 12:46

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqRebirthMap : ClassMap<DbCqRebirth>
    {
        public CqRebirthMap()
        {
            Table("cq_rebirth");
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.NeedProfession).Column("need_prof").Not.Nullable();
            Map(x => x.NewProfession).Column("new_prof").Not.Nullable();
            Map(x => x.NeedLevel).Column("need_level").Not.Nullable();
            Map(x => x.NewLevel).Column("new_level").Not.Nullable();
            Map(x => x.Metempsychosis).Column("metempsychosis").Not.Nullable();
        }
    }
}