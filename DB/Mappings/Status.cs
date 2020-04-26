// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Status.cs
// File Created: 2015/08/03 12:49

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqStatusMap : ClassMap<DbStatus>
    {
        public CqStatusMap()
        {
            Table("cq_status");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.OwnerId).Column("owner_id").Not.Nullable();
            Map(x => x.Status).Column("status").Not.Nullable();
            Map(x => x.Power).Column("power").Not.Nullable();
            Map(x => x.Sort).Column("sort").Not.Nullable();
            Map(x => x.LeaveTimes).Column("leave_times").Not.Nullable();
            Map(x => x.RemainTime).Column("remain_time").Not.Nullable();
            Map(x => x.EndTime).Column("end_time").Not.Nullable();
            Map(x => x.IntervalTime).Column("interval_time").Not.Nullable();
        }
    }
}