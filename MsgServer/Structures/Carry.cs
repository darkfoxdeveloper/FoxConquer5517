// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - Carry.cs
// Last Edit: 2017/02/06 08:47
// Created: 2017/02/06 07:41

using System.Collections.Generic;
using System.Linq;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public sealed class Carry
    {
        public const int MAX_RECORDS = 10;
        public const ushort MAX_USAGE = 30;

        private Character m_pOwner;
        private Item m_pItem;
        private List<DbCarry> m_pLocations = new List<DbCarry>(10);

        public Carry(Character owner, Item item)
        {
            m_pOwner = owner;
            m_pItem = item;

            var list = Database.CarryRepository.FetchByItem(item.Identity);
            if (list != null)
                foreach (var obj in list)
                {
                    m_pLocations.Add(obj);
                }
        }

        public bool Usable => m_pItem.Durability > 0;

        public int Count => m_pLocations.Count;

        public bool AddLocation(uint idMap, ushort mapX, ushort mapY)
        {
            if (Count >= MAX_RECORDS)
                return false;
            DbCarry newCarry = new DbCarry
            {
                ItemIdentity = m_pItem.Identity,
                RecordMapId = idMap,
                RecordMapY = mapY,
                RecordMapX = mapX,
                Name = ""
                //Name = $"{m_pOwner.Map.Name}({mapX},{mapY})"
            };
            Database.CarryRepository.SaveOrUpdate(newCarry);
            m_pLocations.Add(newCarry);
            Send();
            return true;
        }

        public bool UpdateLocation(int idx)
        {
            if (idx >= m_pLocations.Count)
                return false;

            DbCarry carry;
            try
            {
                carry = m_pLocations[idx];
            }
            catch
            {
                return false;
            }
            carry.Name = "";
            carry.RecordMapId = m_pOwner.MapIdentity;
            carry.RecordMapX = m_pOwner.MapX;
            carry.RecordMapY = m_pOwner.MapY;
            Send();
            return Database.CarryRepository.SaveOrUpdate(carry);
        }

        public void SaveName(string name, int idx)
        {
            if (idx >= m_pLocations.Count)
                return;

            DbCarry carry;
            try
            {
                carry = m_pLocations[idx];
            }
            catch
            {
                return;
            }
            carry.Name = name;
            Send();
            Database.CarryRepository.SaveOrUpdate(carry);
        }

        public DbCarry Fetch(uint id)
        {
            if (id > m_pLocations.Count)
                return null;
            return m_pLocations[(int) id];
        }

        public void Send()
        {
            MsgSuperFlag pMsg = new MsgSuperFlag
            {
                Durability = m_pItem.Durability,
                ItemIdentity = m_pItem.Identity
            };
            for (int i = 0; i < m_pLocations.Count; i++)
                pMsg.AddLocation((uint) i, m_pLocations[i].RecordMapId, m_pLocations[i].RecordMapX,
                    m_pLocations[i].RecordMapY, m_pLocations[i].Name);
            m_pOwner.Send(pMsg);
        }
    }
}