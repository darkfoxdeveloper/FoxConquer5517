// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Action.cs
// File Created: 2015/08/01 15:56

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqActionMap : ClassMap<DbGameAction>
    {
        public CqActionMap()
        {
            Table(TableName.ACTION);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.IdNext).Column("id_next").Not.Nullable();
            Map(x => x.IdNextfail).Column("id_nextfail").Not.Nullable();
            Map(x => x.Type).Column("type").Not.Nullable();
            Map(x => x.Data).Column("data").Not.Nullable();
            Map(x => x.Param).Column("param");
        }
    }
}