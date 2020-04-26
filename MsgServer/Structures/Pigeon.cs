// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Pigeon.cs
// Last Edit: 2016/12/06 20:38
// Created: 2016/12/06 20:37

using System.Collections.Generic;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public class Pigeon : IOnTimer
    {
        private TimeOut m_pNext = new TimeOut(60);
        private BroadcastPastRepository m_pAdLogRepo = new BroadcastPastRepository();
        private BroadcastQueueRepository m_pAdQueueRepo = new BroadcastQueueRepository();

        private uint m_dwNext = 1;
        private List<Broadcast> m_lLast = new List<Broadcast>();
        private List<Broadcast> m_lNext = new List<Broadcast>();
        private Broadcast m_pActive = null;

        private const int _PIGEON_PRICE = 100;
        private const int _PIGEON_ADDITION = 100;
        private const int _PIGEON_TOP_ADDITION = 1500;

        /// <summary>
        /// I think broadcasts wont hurt to use locks :)
        /// </summary>
        private object m_pLock = new object();

        public Pigeon()
        {
            m_pNext.Startup(60);

            // if server restarted or whatever, it will fetch the latest and order by addition
            var possibleNext = m_pAdQueueRepo.FetchAll();
            foreach (var bc in possibleNext.OrderByDescending(x => x.Addition).ThenBy(x => x.Identity))
            {
                m_lNext.Add(new Broadcast
                {
                    Identity = m_dwNext++,
                    OwnerIdentity = bc.UserIdentity,
                    OwnerName = bc.UserName,
                    Message = bc.Message,
                    Addition = bc.Addition
                });
                m_pAdQueueRepo.Delete(bc);
            }
        }

        // only for GMs
        public bool Push(string szMessage)
        {
            var newBc = new Broadcast
            {
                Identity = m_dwNext++,
                OwnerIdentity = 0,
                OwnerName = "[GM]",
                Message = szMessage
            };

            lock (m_pLock)
                m_lNext.Insert(0, newBc);
            return true;
        }

        public bool Push(Character pRole, string szMessage, bool bShowError)
        {
            if (szMessage.Length > 80)
            {
                if (bShowError)
                    pRole.Send(ServerString.STR_PIGEON_SEND_ERR_STRING_TOOLONG);
                return false;
            }

            if (szMessage.Length <= 0)
            {
                if (bShowError)
                    pRole.Send(ServerString.STR_PIGEON_SEND_ERR_EMPTYSTRING);
                return false;
            }

            if (UserNextMessagesCount(pRole.Identity) >= 5)
            {
                if (bShowError)
                    pRole.Send(ServerString.STR_PIGEON_SEND_OVER_5_PIECES);
                return false;
            }

            if (pRole.Emoney < _PIGEON_PRICE)
            {
                if (bShowError)
                    pRole.Send(ServerString.STR_PIGEON_URGENT_ERR_NOEMONEY);
                return false;
            }

            if (!pRole.ReduceMoney(_PIGEON_PRICE))
            {
                if (bShowError)
                    pRole.Send(ServerString.STR_PIGEON_URGENT_ERR_NOEMONEY);
                return false;
            }

            var newBc = new Broadcast
            {
                Identity = m_dwNext++,
                Message = szMessage,
                OwnerIdentity = pRole.Identity,
                OwnerName = pRole.Name
            };

            lock (m_pLock)
                m_lNext.Add(newBc);

            //m_pAdQueueRepo.SaveOrUpdate(new DbBroadcastQueue
            //{
            //    NextIdentity = m_dwNext++,
            //    UserIdentity = pRole.Identity,
            //    Message = szMessage,
            //    UserName = pRole.Name,
            //    Time = (uint) UnixTimestamp.Timestamp()
            //});

            pRole.Send(ServerString.STR_PIGEON_SEND_PIGEON_PRODUCE_PROMPT);
            return true;
        }

        private int UserNextMessagesCount(uint idRole)
        {
            lock (m_pLock)
                return m_lNext.Count(x => x.OwnerIdentity == idRole);
        }

        public void Addition(Character pSender, uint idMsg, uint dwAmount)
        {
            Broadcast bc = null;
            int position = 0;

            lock (m_pLock)
            {
                for (int i = 0; i < m_lNext.Count; i++)
                {
                    position = i;
                    var broc = m_lNext[i];

                    if (broc.OwnerIdentity != pSender.Identity || idMsg != broc.Identity) continue;
                    bc = broc;
                    break;
                }

                if (bc == null)
                {
                    pSender.Send(ServerString.STR_PIGEON_ADDITION_UNEXIST);
                    return;
                }

                int newPos = 0;
                switch (dwAmount)
                {
                    case _PIGEON_ADDITION:
                        if (!pSender.ReduceEmoney(_PIGEON_ADDITION))
                        {
                            pSender.Send(ServerString.STR_PIGEON_URGENT_ERR_NOEMONEY);
                            return;
                        }
                        newPos = position - 5;
                        if (newPos < 0)
                            newPos = 0;
                        break;
                    case _PIGEON_TOP_ADDITION:
                        if (!pSender.ReduceEmoney(_PIGEON_TOP_ADDITION))
                        {
                            pSender.Send(ServerString.STR_PIGEON_URGENT_ERR_NOEMONEY);
                            return;
                        }
                        newPos = 0;
                        break;
                }
                bc.Addition += (ushort)dwAmount;

                m_lNext.RemoveAt(position);
                m_lNext.Insert(newPos, bc);
            }

            pSender.Send(ServerString.STR_PIGEON_SEND_PIGEON_PRODUCE_PROMPT);
        }

        public void OnTimer()
        {
            if (m_lNext.Count <= 0)
                return;

            if (!m_pNext.IsTimeOut() && m_lLast.Count > 0)
                return;

            m_pNext.Update();
            var bc = m_lNext[0];
            m_pActive = bc;
            lock (m_pLock)
            {
                m_lLast.Add(bc);
                m_lNext.RemoveAt(0);
            }

            m_pAdLogRepo.SaveOrUpdate(new DbBroadcastLog
            {
                Addition = bc.Addition,
                Message = bc.Message,
                Time = (uint)UnixTimestamp.Timestamp(),
                UserIdentity = bc.OwnerIdentity,
                UserName = bc.OwnerName
            });

            ServerKernel.SendMessageToAll(new MsgTalk(bc.Message, ChatTone.BROADCAST) { Sender = bc.OwnerName });
        }

        public void Request5Last(Character pRole, MsgPigeon pMsg)
        {
            var pNewMsg = new MsgPigeonQuery { Param = pMsg.DwParam };
            ushort wTempPos = 0;
            lock (m_pLock)
            {
                foreach (var bc in m_lNext)
                {
                    if (pNewMsg.Total >= 5)
                        break;
                    pNewMsg.AddBroadcast(bc.Identity, wTempPos++, bc.OwnerIdentity, bc.OwnerName, bc.Addition,
                        bc.Message);
                }
            }
            pRole.Owner.Send(pNewMsg);
        }

        public void RequestNextPage(Character pRole, MsgPigeon pMsg)
        {
            var pNewMsg = new MsgPigeonQuery { Param = 0 };
            ushort wTempPos = 0;
            lock (m_pLock)
            {
                foreach (var bc in m_lNext)
                {
                    if (pNewMsg.Total >= 8)
                    {
                        pRole.Owner.Send(pNewMsg);
                        pNewMsg = new MsgPigeonQuery { Param = 0 };
                    }
                    pNewMsg.AddBroadcast(bc.Identity, wTempPos++, bc.OwnerIdentity, bc.OwnerName, bc.Addition,
                        bc.Message);
                }
            }
            pRole.Owner.Send(pNewMsg);
        }

        public void SendLastMessage(Character pRole)
        {
            if (m_lLast.Count > 0)
            {
                lock (m_pLock)
                {
                    pRole.Send(new MsgTalk(m_pActive.Message, ChatTone.BROADCAST) { Sender = m_pActive.OwnerName });
                }
            }
        }

        public void SaveUnsent()
        {
            foreach (var msg in m_lNext)
            {
                m_pAdQueueRepo.SaveOrUpdate(new DbBroadcastQueue
                {
                    NextIdentity = m_dwNext++,
                    UserIdentity = msg.OwnerIdentity,
                    Message = msg.Message,
                    UserName = msg.OwnerName,
                    Time = msg.Addition
                });
            }
        }
    }

    public sealed class Broadcast
    {
        private string m_szMessage;

        public uint Identity;
        public string Message
        {
            get { return m_szMessage; }
            set { m_szMessage = value.Length > 80 ? value.Substring(0, 80) : value; }
        }

        public uint OwnerIdentity;
        public string OwnerName;
        public ushort Addition = 0;
    }
}