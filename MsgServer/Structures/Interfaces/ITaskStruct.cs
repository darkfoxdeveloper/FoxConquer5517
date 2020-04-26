// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - ITaskStruct.cs
// Last Edit: 2016/12/06 14:11
// Created: 2016/12/06 14:11

namespace MsgServer.Structures.Interfaces
{
    public struct TaskStruct
    {
        public uint Id;
        public uint IdNext, IdNextfail, Money, Profession, Sex, Team, Metempsychosis;
        public ushort Query, ClientActive;
        public int? MinPk, MaxPk;
        public short Marriage;
        public string Itemname1, Itemname2;
    }
}