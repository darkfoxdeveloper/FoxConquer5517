// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Goods.cs
// File Created: 2015/08/03 12:28

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqGoodsMap : ClassMap<DbGoods>
    {
        public CqGoodsMap()
        {
            Table(TableName.GOODS);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Column("id");
            Map(x => x.OwnerIdentity).Column("ownerid");
            Map(x => x.Itemtype).Column("itemtype");
            Map(x => x.Moneytype).Column("moneytype");
            Map(x => x.Monopoly).Column("monopoly").Not.Nullable();
        }
    }
}