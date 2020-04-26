// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Banned Addresses.cs
// Last Edit: 2016/11/23 09:59
// Created: 2016/11/23 09:59

using System.Collections.Generic;
using ServerCore.Common;

namespace LoginServer.Structures
{
    public sealed class BannedAddress
    {
        private readonly string m_address;
        private readonly List<string> m_usernames;
        private uint m_banTime;

        private readonly TimeOut m_tBanTime;

        public BannedAddress(string address, List<string> usernames, uint banTime, int banSeconds)
        {
            m_address = address;
            m_usernames = usernames;
            m_banTime = banTime;

            m_tBanTime = new TimeOut(banSeconds);
            m_tBanTime.Update();
        }

        public string Address
        {
            get { return m_address; }
        }

        public List<string> Usernames
        {
            get { return m_usernames; }
        }

        public uint BanTime
        {
            get { return m_banTime; }
            set { m_banTime = value; }
        }

        public void SetBan(int nTime)
        {
            m_tBanTime.SetInterval(nTime);
            m_tBanTime.Update();
        }

        public void UnsetBan()
        {
            m_tBanTime.Clear();
        }

        public bool Banned
        {
            get { return !m_tBanTime.IsTimeOut(); }
        }
    }
}