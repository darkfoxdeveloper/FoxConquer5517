// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: Administrador
// File Created by: Felipe Vieira Vendramini
// zfserver - DB - Npc.cs
// File Created: 2015/08/03 12:43

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class CqNpcMap : ClassMap<DbNpc>
    {
        public CqNpcMap()
        {
            Table(TableName.NPC);
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("id");
            Map(x => x.Ownerid).Column("ownerid");
            Map(x => x.Playerid).Column("playerid");
            Map(x => x.Name).Column("name").Not.Nullable();
            Map(x => x.Type).Column("type");
            Map(x => x.Lookface).Column("lookface");
            Map(x => x.Idxserver).Column("idxserver");
            Map(x => x.Mapid).Column("mapid");
            Map(x => x.Cellx).Column("cellx");
            Map(x => x.Celly).Column("celly");
            Map(x => x.Task0).Column("task0");
            Map(x => x.Task1).Column("task1");
            Map(x => x.Task2).Column("task2");
            Map(x => x.Task3).Column("task3");
            Map(x => x.Task4).Column("task4");
            Map(x => x.Task5).Column("task5");
            Map(x => x.Task6).Column("task6");
            Map(x => x.Task7).Column("task7");
            Map(x => x.Data0).Column("data0").Not.Nullable();
            Map(x => x.Data1).Column("data1").Not.Nullable();
            Map(x => x.Data2).Column("data2").Not.Nullable();
            Map(x => x.Data3).Column("data3").Not.Nullable();
            Map(x => x.Datastr).Column("datastr");
            Map(x => x.Linkid).Column("linkid").Not.Nullable();
            Map(x => x.Life).Column("life").Not.Nullable();
            Map(x => x.Maxlife).Column("maxlife").Not.Nullable();
            Map(x => x.Base).Column("base").Not.Nullable();
            Map(x => x.Sort).Column("sort").Not.Nullable();
            Map(x => x.Itemid).Column("itemid");
        }
    }
}