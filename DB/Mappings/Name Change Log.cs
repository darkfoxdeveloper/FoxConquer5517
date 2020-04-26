// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Name Change Log.cs
// Last Edit: 2016/12/29 17:24
// Created: 2016/12/29 17:21

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public sealed class NameChangeLogMap : ClassMap<DbNameChangeLog>
    {
        public NameChangeLogMap()
        {
            Table(TableName.NAME_CHANGE_LOG);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Column("id");
            Map(x => x.UserIdentity).Not.Nullable().Default("0").Column("user_id");
            Map(x => x.OldName).Not.Nullable().Default("").Column("old_name");
            Map(x => x.NewName).Not.Nullable().Default("").Column("new_name");
            Map(x => x.Timestamp).Not.Nullable().Default("0").Column("timestamp");
            Map(x => x.Changed).Not.Nullable().Default("0").Column("changed");
        }
    }
}