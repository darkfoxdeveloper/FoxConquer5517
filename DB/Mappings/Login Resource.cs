using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class LoginResourceMap : ClassMap<DbLoginRcd>
    {
        public LoginResourceMap()
        {
            Table("login_rcd");
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Column("id");
            Map(x => x.UserIdentity).Not.Nullable().Default("0").Column("account_id");
            Map(x => x.LoginTime).Not.Nullable().Default("0").Column("login_time");
            Map(x => x.MacAddress).Not.Nullable().Default("000000000000").Column("mac_adr");
            Map(x => x.IpAddress).Not.Nullable().Default("127.0.0.1").Column("ip_adr");
            Map(x => x.ResourceSource).Not.Nullable().Default("2").Column("res_src");
        }
    }
}