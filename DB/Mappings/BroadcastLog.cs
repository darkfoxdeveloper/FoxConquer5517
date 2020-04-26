// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Broadcast Log.cs
// File Created: 2015/08/01 15:54

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class AdLogMap : ClassMap<DbBroadcastLog>
    {
        public AdLogMap()
        {
            Table(TableName.AD_LOG);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.UserIdentity).Column("user_id").Not.Nullable();
            Map(x => x.UserName).Column("user_name").Not.Nullable();
            Map(x => x.Time).Column("time").Not.Nullable();
            Map(x => x.Addition).Column("addition").Not.Nullable();
            Map(x => x.Message).Column("words").Not.Nullable();
        }
    }
}