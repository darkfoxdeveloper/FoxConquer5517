// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Syndicate Advertising.cs
// Last Edit: 2017/01/27 18:17
// Created: 2017/01/27 18:14
namespace DB.Entities
{
    public class DbSyndicateAdvertising
    {
        public virtual uint Identity { get; set; }
        public virtual uint SyndicateIdentity { get; set; }
        public virtual string SyndicateName { get; set; }
        public virtual string Message { get; set; }
        public virtual uint Donation { get; set; }
        public virtual uint Timestamp { get; set; }
        public virtual uint RequiredLevel { get; set; }
        public virtual uint RequiredProfession { get; set; }
        public virtual uint RequiredMetempsychosis { get; set; }
        public virtual byte AutoRecruit { get; set; }
    }
}