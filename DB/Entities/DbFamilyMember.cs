// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - DbFamilyMember.cs
// Last Edit: 2016/12/05 06:28
// Created: 2016/12/05 06:27
namespace DB.Entities
{
    public class DbFamilyMember
    {
        public virtual uint Identity { get; set; }
        public virtual uint FamilyIdentity { get; set; }
        public virtual uint Money { get; set; }
        public virtual byte Position { get; set; }
        public virtual uint JoinDate { get; set; }
    }
}