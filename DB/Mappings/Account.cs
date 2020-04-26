// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Account.cs
// File Created: 2015/08/01 15:53

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class AccountMap : ClassMap<DbAccount>
    {
        public AccountMap()
        {
            Table(TableName.ACCOUNT_TABLE);
            Id(x => x.Identity).Column("id").Not.Nullable();
            Map(x => x.Username).Column("name").Not.Nullable();
            Map(x => x.Password).Column("password").Not.Nullable();
            Map(x => x.Vip).Column("vip").Not.Nullable().Default("0");
            Map(x => x.Lock).Column("lock").Not.Nullable().Default("0");
            Map(x => x.Type).Column("type").Not.Nullable().Default("2");
            Map(x => x.LastLogin).Column("last_login").Not.Nullable().Default("0");
            Map(x => x.MacAddress).Column("mac_addr").Not.Nullable().Default("000000000000");
            Map(x => x.LockExpire).Column("lock_expire").Not.Nullable().Default("0");
        }
    }
}