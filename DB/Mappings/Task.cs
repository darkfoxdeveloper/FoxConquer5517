// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Task.cs
// File Created: 2015/08/03 12:56

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqTaskMap : ClassMap<DbTask>
    {
        public CqTaskMap()
        {
            Table("cq_task");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.IdNext).Column("id_next");
            Map(x => x.IdNextfail).Column("id_nextfail");
            Map(x => x.Itemname1).Column("itemname1");
            Map(x => x.Itemname2).Column("itemname2");
            Map(x => x.Money).Column("money");
            Map(x => x.Profession).Column("profession");
            Map(x => x.Sex).Column("sex");
            Map(x => x.MinPk).Column("min_pk");
            Map(x => x.MaxPk).Column("max_pk");
            Map(x => x.Team).Column("team");
            Map(x => x.Metempsychosis).Column("metempsychosis");
            Map(x => x.Query).Column("query").Not.Nullable();
            Map(x => x.Marriage).Column("marriage").Not.Nullable();
            Map(x => x.ClientActive).Column("client_active").Not.Nullable();
        }
    }
}