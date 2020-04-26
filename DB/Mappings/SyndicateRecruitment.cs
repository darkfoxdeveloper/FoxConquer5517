// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Syndicate Recruitment.cs
// Last Edit: 2017/01/27 18:18
// Created: 2017/01/27 18:18

using DB.Entities;
using FluentNHibernate.Mapping;

namespace DB.Mappings
{
    public class SyndicateRecruitmentMap : ClassMap<DbSyndicateAdvertising>
    {
        public SyndicateRecruitmentMap()
        {
            Table(TableName.SYN_ADVERTISE);
            LazyLoad();
            Id(x => x.Identity).GeneratedBy.Identity().Not.Nullable().Default("0").Column("id");
            Map(x => x.SyndicateIdentity).Column("syn_id").Default("0").Not.Nullable();
            Map(x => x.SyndicateName).Column("syn_name").Default("NONE").Not.Nullable();
            Map(x => x.Message).Column("message").Default("Join my guild please.").Not.Nullable();
            Map(x => x.Donation).Column("amount").Default("0").Not.Nullable();
            Map(x => x.Timestamp).Column("add_date").Default("0").Not.Nullable();
            Map(x => x.RequiredLevel).Column("req_lev").Default("1").Not.Nullable();
            Map(x => x.RequiredMetempsychosis).Column("req_pro").Default("0").Not.Nullable();
            Map(x => x.RequiredProfession).Column("req_metempsychosis").Default("0").Not.Nullable();
            Map(x => x.AutoRecruit).Column("auto").Default("1").Not.Nullable();
        }
    }
}