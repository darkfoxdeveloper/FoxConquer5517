using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class MonsterMagicMap : ClassMap<DbMonsterMagic>
    {
        public MonsterMagicMap()
        {
            Table(TableName.MONSTER_MAGIC);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Column("id");
            Map(x => x.OwnerIdentity).Not.Nullable().Default("0").Column("monster_id");
            Map(x => x.MagicIdentity).Not.Nullable().Default("0").Column("magic_id");
            Map(x => x.MagicLevel).Not.Nullable().Default("0").Column("magic_level");
            Map(x => x.Chance).Not.Nullable().Default("10000").Column("chance");
        }
    }
}