// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Event Flag.cs
// Last Edit: 2016/12/15 17:51
// Created: 2016/12/15 09:54

using Core.Common.Enums;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.World;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Entities
{
    public sealed class EventFlag : IScreenObject
    {
        private MsgPlayer m_pPacket;

        public EventFlag(MsgPlayer pMsg, Map pMap)
        {
            m_pPacket = pMsg;
            Map = pMap;
            MapX = pMsg.MapX;
            MapY = pMsg.MapY;
        }

        public bool Grab(Character pUser)
        {
            try
            {
                pUser.AttachStatus(pUser, FlagInt.CTF_FLAG, 0, ALIVE_SECONDS, 0, 0);
                MsgWarFlag pMsg = new MsgWarFlag
                {
                    Type = WarFlagType.GRAB_FLAG_EFFECT,
                    Identity = pUser.Identity
                };
                pUser.Send(pMsg);
                pMsg = new MsgWarFlag();
                pMsg.Type = (WarFlagType) 8;
                pMsg.Identity = (uint) ALIVE_SECONDS;
                pUser.Send(pMsg);
                Map.RemoveNpc(this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public uint Identity { get { return m_pPacket.Identity; } }
        public string Name { get; set; }

        public Map Map { get; set; }
        public uint MapIdentity { get; set; }
        public ushort MapX { get; set; }
        public ushort MapY { get; set; }

        public short Elevation { get { return (short) (Map == null ? 999 : Map[MapX, MapY].Elevation); } }

        public IScreenObject FindAroundRole(uint idRole)
        {
            if (Map == null) return null;
            return Map.FindAroundRole(this, idRole);
        }

        public void SendSpawnTo(Character pObj)
        {
            pObj.Send(m_pPacket);
        }

        public const int ALIVE_SECONDS = 60;
    }
}