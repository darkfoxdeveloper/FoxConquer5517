// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Syndicate.cs
// Last Edit: 2016/12/02 22:15
// Created: 2016/11/23 08:02
namespace DB.Entities
{
    public class DbSyndicate
    {
        public virtual ushort Identity { get; set; }
        public virtual string Name { get; set; }
        public virtual string Announce { get; set; }
        public virtual uint AnnounceDate { get; set; }
        public virtual uint LeaderIdentity { get; set; }
        public virtual string LeaderName { get; set; }
        public virtual ulong Money { get; set; }
        public virtual uint EMoney { get; set; }

        /// <summary>
        /// Tells the server if the guild has already been deleted.
        /// </summary>
        public virtual byte DelFlag { get; set; }

        public virtual uint Amount { get; set; }
        public virtual byte TotemHead { get; set; }
        public virtual byte TotemNeck { get; set; }
        public virtual byte TotemRing { get; set; }
        public virtual byte TotemWeapon { get; set; }
        public virtual byte TotemArmor { get; set; }
        public virtual byte TotemBoots { get; set; }
        public virtual byte TotemFan { get; set; }
        public virtual byte TotemTower { get; set; }

        /// <summary>
        /// The date of the last totem opened. (yyyymmdd)
        /// </summary>
        public virtual uint LastTotem { get; set; }

        public virtual byte ReqLevel { get; set; }
        public virtual byte ReqClass { get; set; }
        public virtual byte ReqMetempsychosis { get; set; }

        public virtual uint CreationDate { get; set; }

        public virtual long MoneyPrize { get; set; }  // 20160202
        public virtual uint EmoneyPrize { get; set; } // 20160202
    }
}