// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - User Score.cs
// Last Edit: 2016/12/06 20:46
// Created: 2016/12/06 20:46

using MsgServer.Structures.Interfaces;

namespace MsgServer.Structures
{
    public class UserScore
    {
        private uint m_dwIdentity;
        private string m_szName;
        private long m_lPoints;

        public UserScore(IRole pOwner)
        {
            m_dwIdentity = pOwner.Identity;
            m_szName = pOwner.Name;
        }

        public uint Identity { get { return m_dwIdentity; } }
        public string Name { get { return m_szName; } }
        public long Points { get { return m_lPoints; } set { m_lPoints = value; } }
    }
}