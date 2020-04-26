// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Relationship.cs
// Last Edit: 2016/12/06 15:24
// Created: 2016/12/06 15:24

using System.Linq;
using MsgServer.Network;

namespace MsgServer.Structures
{
    public class Relationship
    {
        public uint Identity { get; set; }
        public string Name { get; set; }

        public bool IsOnline
        {
            get { return ServerKernel.Players.ContainsKey(Identity); }
        }

        public Client User
        {
            get { return ServerKernel.Players.Values.FirstOrDefault(x => x.Identity == Identity); }
        }

        public object Database { get; set; }
    }
}