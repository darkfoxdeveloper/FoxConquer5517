// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Totem.cs
// Last Edit: 2016/11/25 01:44
// Created: 2016/11/25 00:09

using System;
using System.Linq;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.Society
{
    public sealed class Totem
    {
        private Item m_pItem;
        private DbSyntotem m_pTotem;

        public Totem(Item pItem, Character pOwner)
        {
            m_pTotem = new DbSyntotem
            {
                Itemid = pItem.Identity, 
                Synid = pOwner.SyndicateIdentity,
                Userid = pOwner.Identity,
                Username = pOwner.Name
            };

            if (m_pTotem.Synid <= 0)
                throw new Exception("Synid cannot be zero.");

            m_pItem = pItem;
        }

        public Totem(DbSyntotem dbTotem, Item pItem)
        {
            m_pItem = pItem;
            m_pTotem = dbTotem;
        }

        public Client Owner
        {
            get { return ServerKernel.Players.Values.FirstOrDefault(x => x.Identity == m_pTotem.Userid); }
        }

        public uint Identity
        {
            get { return m_pItem.Identity; }
        }

        public string Name
        {
            get { return m_pItem.Itemtype.Name; }
        }

        public uint PlayerIdentity
        {
            get { return m_pTotem.Userid; }
        }

        public string PlayerName
        {
            get { return m_pTotem.Username; }
        }

        public Item Item
        {
            get { return m_pItem; }
        }

        public uint Itemtype
        {
            get { return m_pItem.Type; }
        }

        public uint Donation()
        {
            uint Return = 0;
            var id = (int)(m_pItem.Type % 10);

            switch (id)
            {
                case 8: Return = 1000; break;
                case 9: Return = 16660; break;
                default: return 0;
            }

            if (m_pItem.SocketOne > 0 && m_pItem.SocketTwo == 0)
                Return += 33330;
            if (m_pItem.SocketOne > 0 && m_pItem.SocketTwo > 0)
                Return += 133330;

            switch (m_pItem.Plus)
            {
                case 1: Return += 90; break;
                case 2: Return += 490; break;
                case 3: Return += 1350; break;
                case 4: Return += 4070; break;
                case 5: Return += 12340; break;
                case 6: Return += 37030; break;
                case 7: Return += 111110; break;
                case 8: Return += 333330; break;
                case 9: Return += 1000000; break;
                case 10: Return += 1033330; break;
                case 11: Return += 1101230; break;
                case 12: Return += 1212340; break;
            }

            return Return;
        }

        public void Remove()
        {
            m_pItem.Inscribed = false;
            if (Owner != null && m_pItem.Position < ItemPosition.CROP)
                Owner.Send(m_pItem.InformationPacket(true));
        }

        public bool Save()
        {
            return Database.TotemPoleRepository.SaveOrUpdate(m_pTotem);
        }

        public bool Delete()
        {
            return Database.TotemPoleRepository.Delete(m_pTotem);
        }
    }
}