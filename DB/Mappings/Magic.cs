// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Magic.cs
// File Created: 2015/08/03 12:33

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqMagicMap : ClassMap<DbMagic>
    {
        public CqMagicMap()
        {
            Table(TableName.MAGIC);
            LazyLoad();
            Id(x => x.Id).Column("id").GeneratedBy.Identity().Not.Nullable();
            Map(x => x.OwnerId).Column("ownerid").Not.Nullable();
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.Level).Column("level").Not.Nullable();
            Map(x => x.Experience).Column("exp").Not.Nullable();
            Map(x => x.Unlearn).Column("unlearn").Not.Nullable();
            Map(x => x.OldLevel).Column("old_level").Not.Nullable();
        }
    }
}