// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Business.cs
// File Created: 2015/08/01 15:59

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqBusinessMap : ClassMap<DbBusiness>
    {
        public CqBusinessMap()
        {
            Table(TableName.BUSINESS);
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Userid).Column("userid").Not.Nullable();
            Map(x => x.Business).Column("business").Not.Nullable();
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.Date).Column("date").Not.Nullable();
        }
    }
}