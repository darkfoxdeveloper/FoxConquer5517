// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IScore.cs
// Last Edit: 2016/12/05 11:06
// Created: 2016/12/05 11:06

namespace MsgServer.Structures.Interfaces
{
    public interface IScore
    {
        ushort Identity { get; }
        string Name { get; }
        uint Score { get; set; }
    }
}