// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Friend Player.cs
// File Created: 2015/08/03 12:26

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqFriendMap : ClassMap<DbFriend>
    {
        public CqFriendMap()
        {
            Table("cq_friend");
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.UserIdentity).Column("userid").Not.Nullable();
            Map(x => x.Friend).Column("friend").Not.Nullable();
            Map(x => x.FriendName).Column("friendname").Not.Nullable();
        }
    }
}