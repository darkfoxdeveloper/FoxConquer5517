// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Broadcast Queue.cs
// File Created: 2015/08/01 15:55

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class AdQueueMap : ClassMap<DbBroadcastQueue>
    {
        public AdQueueMap()
        {
            Table(TableName.AD_QUEUE);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.NextIdentity).Column("idnext").Not.Nullable();
            Map(x => x.UserIdentity).Column("user_id").Not.Nullable();
            Map(x => x.UserName).Column("user_name").Not.Nullable();
            Map(x => x.Addition).Column("addition").Not.Nullable();
            Map(x => x.Message).Column("words").Not.Nullable();
        }
    }
}