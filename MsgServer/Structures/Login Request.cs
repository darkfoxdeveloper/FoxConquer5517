// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Login Request.cs
// Last Edit: 2016/11/23 10:37
// Created: 2016/11/23 10:37

using ServerCore.Common;

namespace MsgServer.Structures
{
    public sealed class LoginRequest
    {
        public const int REQUEST_TIMEOUT = 60;

        private readonly TimeOut m_pTimeOut = new TimeOut(REQUEST_TIMEOUT);

        public LoginRequest(uint dwUsrId, uint dwHash, uint dwTime, string szAddress, byte pLevel, byte pType, string szMac)
        {
            AccountIdentity = dwUsrId;
            Hash = dwHash;
            RequestTime = dwTime;
            IpAddress = szAddress;
            VipLevel = pLevel;
            Authority = pType;

            m_pTimeOut.Update();
        }

        public uint AccountIdentity;
        public uint Hash;
        public uint RequestTime;
        public string IpAddress;
        public byte VipLevel;
        public byte Authority;
        public string MacAddress;

        public bool IsValid(uint dwUsrId, uint dwHash, string szIp)
        {
            return !m_pTimeOut.IsTimeOut() && AccountIdentity == dwUsrId && Hash == dwHash && IpAddress == szIp;
        }

        public bool IsExpired()
        {
            return m_pTimeOut.IsTimeOut();
        }
    }
}