// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Syndicate Score.cs
// Last Edit: 2016/12/05 11:07
// Created: 2016/12/05 11:06

using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Society;

namespace MsgServer.Structures
{
    public class SynScore : IScore
    {
        private readonly ushort m_dwIdentity = 0;
        private readonly string m_szName = "Undefined";

        public SynScore(Syndicate syn)
        {
            m_dwIdentity = (ushort)syn.Identity;
            m_szName = syn.Name;
        }

        public ushort Identity { get { return m_dwIdentity; } }
        public string Name { get { return m_szName; } }
        public uint Score { get; set; }
    }
}