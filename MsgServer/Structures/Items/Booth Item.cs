// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Booth Item.cs
// Last Edit: 2016/12/06 21:38
// Created: 2016/12/06 21:38

namespace MsgServer.Structures.Items
{
    public class BoothItem
    {
        private Item m_pItem;
        private uint m_dwValue;
        private bool m_bSilver;

        public bool Create(Item item, uint dwMoney, bool bSilver)
        {
            m_pItem = item;
            m_dwValue = dwMoney;
            m_bSilver = bSilver;

            return m_dwValue > 0;
        }

        public Item Item { get { return m_pItem; } }
        public uint Value { get { return m_dwValue; } }
        public bool IsSilver { get { return m_bSilver; } }
    }
}