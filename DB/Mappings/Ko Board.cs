using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class SupermanMapping : ClassMap<DbSuperman>
    {
        public SupermanMapping()
        {
            Table(TableName.SUPERMAN);
            LazyLoad();
            Id(x => x.Identity).Column("id").Not.Nullable().Unique();
            Map(x => x.Amount).Column("number").Default("0").Not.Nullable();
            Map(x => x.Name).Column("name").Default("").Not.Nullable();
        }
    }
}