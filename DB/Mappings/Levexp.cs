// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Levexp.cs
// File Created: 2015/08/03 12:32

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqLevexpMap : ClassMap<DbLevexp>
    {
        public CqLevexpMap()
        {
            Table(TableName.LEVEXP);
            LazyLoad();
            Id(x => x.Level).GeneratedBy.Identity().Column("level");
            Map(x => x.Exp).Column("exp").Not.Nullable();
            Map(x => x.UpLevTime).Column("up_lev_time").Not.Nullable();
        }
    }
}