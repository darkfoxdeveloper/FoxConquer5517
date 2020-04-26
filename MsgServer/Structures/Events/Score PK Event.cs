// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Score PK Event.cs
// Last Edit: 2016/12/06 20:46
// Created: 2016/12/06 20:45

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.Events
{
    public sealed class ScorePkEvent
    {
        private readonly List<UserScore> _getUserScores;
        private Map m_pMap;
        private uint m_dwMapIdentity;
        private EventState m_pState;
        private TimeOut m_pAutoPoint = new TimeOut(15);
        private bool m_bIsComplete = false;

        private readonly ushort[] m_pTpX = { 66, 127, 161, 123, 127, 79, 37, 50 };
        private readonly ushort[] m_pTpY = { 12, 69, 138, 183, 150, 118, 76, 43 };

        public ScorePkEvent()
        {
            _getUserScores = new List<UserScore>();
        }

        public bool Create(uint idMap)
        {
            if (!ServerKernel.Maps.TryGetValue(idMap, out m_pMap))
            {
                ServerKernel.Log.SaveLog(string.Format("Could not load mapid:{0} to event", idMap), true, LogType.WARNING);
                return false;
            }
            m_dwMapIdentity = idMap;
            m_bIsComplete = true;
            m_pState = EventState.IDLE;
            return true;
        }

        public bool IsReady { get { return m_bIsComplete; } }

        public void OnTimer()
        {
            switch (m_pState)
            {
                case EventState.IDLE:
                    OnIdle();
                    break;
                case EventState.STARTING:
                    OnStartup();
                    break;
                case EventState.RUNNING:
                    OnProgress();
                    break;
                case EventState.FINISHING:
                    OnFinish();
                    break;
                case EventState.ENDED:
                    OnFinished();
                    break;
            }
        }

        public List<UserScore> GetUserScores
        {
            get { return new List<UserScore>(_getUserScores.AsReadOnly()); }
        }

        public Map Map
        {
            get { return m_pMap; }
        }

        public uint MapIdentity
        {
            get { return m_dwMapIdentity; }
        }

        public bool IsParticipating(uint idMember)
        {
            return m_pMap.Players.ContainsKey(idMember);
        }

        public bool IsInMatch(uint idMember)
        {
            bool hasPoints = false;
            foreach (var plr in _getUserScores)
                if (plr.Identity == idMember)
                    hasPoints = true;
            return m_pMap.Players.ContainsKey(idMember) && hasPoints;
        }

        public bool AlterPoints(uint idMember, long lPoint)
        {
            for (int i = _getUserScores.Count - 1; i >= 0; i--)
            {
                if (_getUserScores[i].Identity == idMember)
                {
                    if (_getUserScores[i].Points + lPoint < 0)
                        _getUserScores[i].Points = 0;
                    else
                        _getUserScores[i].Points += lPoint;
                }
            }

            m_pMap.SendMessageToMap("Ranking", ChatTone.EVENT_RANKING);
            int pos = 1;
            foreach (var part in _getUserScores.OrderByDescending(x => x.Points))
            {
                if (part.Name.Contains("[GM]") || part.Name.Contains("[PM]"))
                    continue;
                if (pos > 8)
                    break;
                m_pMap.SendMessageToMap(string.Format("{0} - {1} - {2}", pos++, part.Name, part.Points), ChatTone.EVENT_RANKING_NEXT);
            }
            foreach (var part in _getUserScores)
            {
                Client client;
                if (ServerKernel.Players.TryGetValue(part.Identity, out client)
                    && client.Character.MapIdentity == m_pMap.Identity)
                {
                    client.SendMessage(string.Format("Your points: {0}", part.Points), ChatTone.EVENT_RANKING_NEXT);
                }
            }
            return true;
        }

        public int GetRank(uint idMember)
        {
            int pos = 1;
            foreach (var pls in _getUserScores.OrderByDescending(x => x.Points))
            {
                if (pls.Name.Contains("[GM]") || pls.Name.Contains("[PM]"))
                    continue;
                if (pls.Identity == idMember)
                    return pos;
                pos++;
            }
            return 999;
        }

        public void OnIdle()
        {
            DateTime now = DateTime.Now;

            if (m_pState == EventState.IDLE
                && now.Minute >= 29
                && now.Minute < 40)
            {
                foreach (var plr in m_pMap.Players.Values)
                {
                    plr.ChangeMap(430, 380, 1002);
                }

                ServerKernel.SendMessageToAll("Score PK Tournament will start in 1 minute! Hurry up to join the event.", ChatTone.TALK);

                m_pState = EventState.STARTING;
            }
        }

        public void OnStartup()
        {
            DateTime now = DateTime.Now;

            if (now.Minute == 30
                && m_pState == EventState.STARTING)
            {
                _getUserScores.Clear();

                Map m_pPresentation;
                if (ServerKernel.Maps.TryGetValue(7504, out m_pPresentation))
                {
                    Random rand = new Random();
                    foreach (var plr in m_pPresentation.Players.Values.Where(x => x.IsAlive))
                    {
                        plr.ChangeMap(m_pTpX[rand.Next() % 8], m_pTpY[rand.Next() % 8], ServerKernel.SCORE_PK_MAPID);
                    }
                }

                m_pAutoPoint.Startup(15);

                m_pState = EventState.RUNNING;
            }
        }

        public void OnProgress()
        {
            DateTime now = DateTime.Now;

            foreach (var plr in m_pMap.Players.Values)
            {
                try
                {
                    if (!IsInMatch(plr.Identity))
                    {
                        UserScore score = new UserScore(plr);
                        _getUserScores.Add(score);
                    }
                }
                catch
                {
                    ServerKernel.Log.SaveLog(string.Format("Could not add {0}:{1} to Score PK Tournament", plr.Identity, plr.Name), true, LogType.EXCEPTION);
                }
            }

            if ((now.Minute >= 30 && now.Minute < 40) && m_pState == EventState.RUNNING && m_pAutoPoint.ToNextTime())
            {
                foreach (var plr in _getUserScores)
                {
                    Client pClient;
                    if (!ServerKernel.Players.TryGetValue(plr.Identity, out pClient) || !pClient.Character.IsAlive
                        || m_dwMapIdentity != pClient.Character.MapIdentity)
                        continue;

                    if (DuplicatedAddress(pClient))
                    {
                        pClient.Character.ChangeMap(1002, 430, 378);
                        pClient.SendMessage("You can't stand with multiple accounts inside of this event.");
                    }

                    AlterPoints(plr.Identity, 1);
                }
            }
            else if (now.Minute == 40 && m_pState == EventState.RUNNING)
            {
                m_pState = EventState.FINISHING;
            }
        }

        public void OnFinish()
        {
            ushort[] m_startX = { 430, 423, 439, 428, 452, 464, 439 };
            ushort[] m_startY = { 378, 394, 384, 365, 365, 378, 396 };

            foreach (var usr in _getUserScores.OrderByDescending(x => x.Points))
            {
                Client client;
                if (!ServerKernel.Players.TryGetValue(usr.Identity, out client) || client.Character == null)
                    continue;

                Character plr = client.Character;

                plr.ChangeMap(m_startX[ThreadSafeRandom.RandGet() % 7], m_startY[ThreadSafeRandom.RandGet() % 7], 1002);

                switch (GetRank(plr.Identity))
                {
                    case 1:
                        plr.AwardMoney(10000000);
                        plr.AwardEmoney(21500);
                        ServerKernel.SendMessageToAll(
                            string.Format(
                                "Congratulations! {0} has won the Score PK Tournament and awarded 10,000,000 silvers and 21,500 CPs.",
                                plr.Name), ChatTone.TALK);
                        break;
                    case 2:
                        plr.AwardMoney(7500000);
                        plr.AwardEmoney(10750);
                        ServerKernel.SendMessageToAll(
                            string.Format(
                                "{0} has got second place in the Score PK Tournament and awarded 7,500,000 silvers and 10,750 CPs.",
                                plr.Name), ChatTone.TALK);
                        break;
                    case 3:
                        plr.AwardMoney(5000000);
                        plr.AwardEmoney(6450);
                        ServerKernel.SendMessageToAll(
                            string.Format(
                                "{0} has got third place in the Score PK Tournament and awarded 5,000,000 silvers and 6,450 CPs.",
                                plr.Name), ChatTone.TALK);
                        break;
                    default:
                        plr.AwardMoney(3000000);
                        plr.AwardEmoney(4300);
                        plr.Send(
                            "You awarded 3,000,000 silvers and 4,300 CPs for participating the Score PK Tournament.");
                        break;
                }
            }

            m_pState = EventState.ENDED;
        }

        public void OnFinished()
        {

            m_pAutoPoint.Clear();
            m_pState = EventState.IDLE;
        }

        public bool DuplicatedAddress(Client pClient)
        {
            return m_pMap.Players.Values.Count(x => x.Identity != pClient.Identity && x.Owner.IpAddress == pClient.IpAddress) > 0;
        }

        public void GetRebornPos(out Point pos)
        {
            pos = new Point();
            pos.X = m_pTpX[ThreadSafeRandom.RandGet(0, m_pTpX.Length)%m_pTpX.Length];
            pos.Y = m_pTpY[ThreadSafeRandom.RandGet(0, m_pTpY.Length)%m_pTpY.Length];
        }

        public EventState State { get { return m_pState; } }
    }
}