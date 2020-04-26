// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Magic Type Operation.cs
// Last Edit: 2016/12/15 09:46
// Created: 2016/11/29 16:14

using System.Collections.Generic;
using DB.Entities;

namespace MsgServer.Structures
{
    public sealed class MagicTypeOp
    {
        public MagicTypeOp(DbMagictypeop dbOp)
        {
            Magics = new List<ushort>();

            Identity = dbOp.Id;
            RebirthTime = dbOp.RebirthTime;
            ProfessionAgo = dbOp.ProfessionAgo;
            ProfessionNow = dbOp.ProfessionNow;
            Operation = dbOp.MagictypeOp;

            if (dbOp.Skill1 != 0)
                AppendMagic(dbOp.Skill1);
            if (dbOp.Skill2 != 0)
                AppendMagic(dbOp.Skill2);
            if (dbOp.Skill3 != 0)
                AppendMagic(dbOp.Skill3);
            if (dbOp.Skill4 != 0)
                AppendMagic(dbOp.Skill4);
            if (dbOp.Skill5 != 0)
                AppendMagic(dbOp.Skill5);
            if (dbOp.Skill6 != 0)
                AppendMagic(dbOp.Skill6);
            if (dbOp.Skill7 != 0)
                AppendMagic(dbOp.Skill7);
            if (dbOp.Skill8 != 0)
                AppendMagic(dbOp.Skill8);
            if (dbOp.Skill9 != 0)
                AppendMagic(dbOp.Skill9);
            if (dbOp.Skill10 != 0)
                AppendMagic(dbOp.Skill10);
            if (dbOp.Skill11 != 0)
                AppendMagic(dbOp.Skill11);
            if (dbOp.Skill12 != 0)
                AppendMagic(dbOp.Skill12);
            if (dbOp.Skill13 != 0)
                AppendMagic(dbOp.Skill13);
            if (dbOp.Skill14 != 0)
                AppendMagic(dbOp.Skill14);
            if (dbOp.Skill15 != 0)
                AppendMagic(dbOp.Skill15);
            if (dbOp.Skill16 != 0)
                AppendMagic(dbOp.Skill16);
            if (dbOp.Skill17 != 0)
                AppendMagic(dbOp.Skill17);
            if (dbOp.Skill18 != 0)
                AppendMagic(dbOp.Skill18);
            if (dbOp.Skill19 != 0)
                AppendMagic(dbOp.Skill19);
            if (dbOp.Skill20 != 0)
                AppendMagic(dbOp.Skill20);
            if (dbOp.Skill21 != 0)
                AppendMagic(dbOp.Skill21);
            if (dbOp.Skill22 != 0)
                AppendMagic(dbOp.Skill22);
            if (dbOp.Skill23 != 0)
                AppendMagic(dbOp.Skill23);
            if (dbOp.Skill24 != 0)
                AppendMagic(dbOp.Skill24);
            if (dbOp.Skill25 != 0)
                AppendMagic(dbOp.Skill25);
            if (dbOp.Skill26 != 0)
                AppendMagic(dbOp.Skill26);
            if (dbOp.Skill27 != 0)
                AppendMagic(dbOp.Skill27);
            if (dbOp.Skill28 != 0)
                AppendMagic(dbOp.Skill28);
            if (dbOp.Skill29 != 0)
                AppendMagic(dbOp.Skill29);
            if (dbOp.Skill30 != 0)
                AppendMagic(dbOp.Skill30);
            if (dbOp.Skill31 != 0)
                AppendMagic(dbOp.Skill31);
            if (dbOp.Skill32 != 0)
                AppendMagic(dbOp.Skill32);
            if (dbOp.Skill33 != 0)
                AppendMagic(dbOp.Skill33);
            if (dbOp.Skill34 != 0)
                AppendMagic(dbOp.Skill34);
            if (dbOp.Skill35 != 0)
                AppendMagic(dbOp.Skill35);
            if (dbOp.Skill36 != 0)
                AppendMagic(dbOp.Skill36);
            if (dbOp.Skill37 != 0)
                AppendMagic(dbOp.Skill37);
            if (dbOp.Skill38 != 0)
                AppendMagic(dbOp.Skill38);
            if (dbOp.Skill39 != 0)
                AppendMagic(dbOp.Skill39);
            if (dbOp.Skill40 != 0)
                AppendMagic(dbOp.Skill40);
            if (dbOp.Skill41 != 0)
                AppendMagic(dbOp.Skill41);
            if (dbOp.Skill42 != 0)
                AppendMagic(dbOp.Skill42);
            if (dbOp.Skill43 != 0)
                AppendMagic(dbOp.Skill43);
            if (dbOp.Skill44 != 0)
                AppendMagic(dbOp.Skill44);
            if (dbOp.Skill45 != 0)
                AppendMagic(dbOp.Skill45);
            if (dbOp.Skill46 != 0)
                AppendMagic(dbOp.Skill46);
            if (dbOp.Skill47 != 0)
                AppendMagic(dbOp.Skill47);
            if (dbOp.Skill48 != 0)
                AppendMagic(dbOp.Skill48);
            if (dbOp.Skill49 != 0)
                AppendMagic(dbOp.Skill49);
            if (dbOp.Skill50 != 0)
                AppendMagic(dbOp.Skill50);
            if (dbOp.Skill51 != 0)
                AppendMagic(dbOp.Skill51);
            if (dbOp.Skill52 != 0)
                AppendMagic(dbOp.Skill52);
            if (dbOp.Skill53 != 0)
                AppendMagic(dbOp.Skill53);
            if (dbOp.Skill54 != 0)
                AppendMagic(dbOp.Skill54);
            if (dbOp.Skill55 != 0)
                AppendMagic(dbOp.Skill55);
            if (dbOp.Skill56 != 0)
                AppendMagic(dbOp.Skill56);
            if (dbOp.Skill57 != 0)
                AppendMagic(dbOp.Skill57);
            if (dbOp.Skill58 != 0)
                AppendMagic(dbOp.Skill58);
            if (dbOp.Skill59 != 0)
                AppendMagic(dbOp.Skill59);
            if (dbOp.Skill60 != 0)
                AppendMagic(dbOp.Skill60);
        }

        public uint Identity;

        public List<ushort> Magics;

        public byte RebirthTime;

        public ushort ProfessionAgo;

        public ushort ProfessionNow;

        public byte Operation;

        public bool AppendMagic(ushort nMagicId)
        {
            if (Magics.Contains(nMagicId)) return false;
            Magics.Add(nMagicId);
            return true;
        }
    }
}