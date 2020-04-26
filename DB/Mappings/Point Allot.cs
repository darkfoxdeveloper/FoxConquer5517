// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Point Allot.cs
// File Created: 2015/08/03 12:45

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqPointAllotMap : ClassMap<DbPointAllot>
    {
        public CqPointAllotMap()
        {
            Table(TableName.POINT_ALLOT);
            LazyLoad();
            Id(x => x.Identity).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.Profession).Column("profession").Not.Nullable();
            Map(x => x.Level).Column("level").Not.Nullable();
            Map(x => x.Strength).Column("force").Not.Nullable();
            Map(x => x.Agility).Column("Speed").Not.Nullable();
            Map(x => x.Vitality).Column("health").Not.Nullable();
            Map(x => x.Spirit).Column("soul").Not.Nullable();
        }
    }
}