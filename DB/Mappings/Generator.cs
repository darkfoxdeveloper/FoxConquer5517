using FluentNHibernate.Mapping;
using DB.Entities;

namespace DB.Mappings
{
    public class CqGeneratorMap : ClassMap<DbGenerator>
    {
        public CqGeneratorMap()
        {
            Table(TableName.GENERATOR);
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Mapid).Column("mapid").Not.Nullable();
            Map(x => x.BoundX).Column("bound_x").Not.Nullable();
            Map(x => x.BoundY).Column("bound_y").Not.Nullable();
            Map(x => x.BoundCx).Column("bound_cx").Not.Nullable();
            Map(x => x.BoundCy).Column("bound_cy").Not.Nullable();
            Map(x => x.MaxNpc).Column("max_npc").Not.Nullable();
            Map(x => x.RestSecs).Column("rest_secs").Not.Nullable();
            Map(x => x.MaxPerGen).Column("max_per_gen").Not.Nullable();
            Map(x => x.Npctype).Column("npctype").Not.Nullable();
            Map(x => x.TimerBegin).Column("timer_begin").Not.Nullable();
            Map(x => x.TimerEnd).Column("timer_end").Not.Nullable();
            Map(x => x.BornX).Column("born_x").Not.Nullable();
            Map(x => x.BornY).Column("born_y").Not.Nullable();
        }
    }
}