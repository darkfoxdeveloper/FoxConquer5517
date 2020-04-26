// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - DB - Carry.cs
// Last Edit: 2017/02/06 08:02
// Created: 2017/02/06 08:02

namespace DB.Entities
{
    public class DbCarry
    {
        public virtual uint Identity { get; set; }
        public virtual uint ItemIdentity { get; set; }
        public virtual string Name { get; set; }
        public virtual uint RecordMapId { get; set; }
        public virtual ushort RecordMapX { get; set; }
        public virtual ushort RecordMapY { get; set; }
    }
}