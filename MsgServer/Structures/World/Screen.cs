// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Screen.cs
// Last Edit: 2016/12/15 11:04
// Created: 2016/11/23 10:26

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.World
{
    /// <summary>
    /// This class encapsulates the client's screen system. It handles screen objects that the player can currently
    /// see in the client window as they enter, move, and leave the screen. It controls the distribution of packets
    /// to the other players in the screen and adding new objects as the character (the actor) moves. 
    /// </summary>
    public sealed class Screen
    {
        // Ownership variable declaration
        private Character m_pOwner;

        private ConcurrentDictionary<uint, IScreenObject> m_pPlayers = new ConcurrentDictionary<uint, IScreenObject>();
        private ConcurrentDictionary<uint, IScreenObject> m_pRoles = new ConcurrentDictionary<uint, IScreenObject>();
        private ConcurrentDictionary<uint, IScreenObject> m_pObjects = new ConcurrentDictionary<uint, IScreenObject>();

        /// <summary>
        /// This class encapsulates the client's screen system. It handles screen objects that the player can currently
        /// see in the client window as they enter, move, and leave the screen. It controls the distribution of packets
        /// to the other players in the screen and adding new objects as the character (the actor) moves. 
        /// </summary>
        /// <param name="pOwner">The owner of the screen.</param>
        public Screen(Character pOwner)
        {
            m_pOwner = pOwner;
        }

        /// <summary>
        /// This method adds the screen object specified in the parameter arguments to the owner's screen. If the 
        /// object already exists in the screen, it will not be added and this method will return false. If the
        /// screen object is being added, and the object is of type character, then the owner will be added to the
        /// observer's screen as well. 
        /// </summary>
        /// <param name="pObj">The screen object being added to the owner's screen.</param>
        public bool Add(IScreenObject pObj)
        {
            if (pObj is Character)
                return m_pPlayers.TryAdd(pObj.Identity, pObj) && (pObj as Character).Screen.Add(m_pOwner);
            if (pObj is IRole)
            {
                if (pObj is Monster)
                    (pObj as Monster).TargetCount += 1;
                return m_pRoles.TryAdd(pObj.Identity, pObj);
            }
            return m_pObjects.TryAdd(pObj.Identity, pObj);
        }

        /// <summary>
        /// This method checks if the screen object specified in the parameter arguments is inserted into the owner
        /// screen.
        /// </summary>
        /// <param name="dwIdentity">The unique identifier of the object.</param>
        public bool Contains(uint dwIdentity)
        {
            return m_pPlayers.ContainsKey(dwIdentity) || m_pObjects.ContainsKey(dwIdentity);
        }

        /// <summary>
        /// This method deletes a screen object from the owner's screen. It uses the entity removal subtype from
        /// the general action packet to forcefully remove the entity from the owner's screen. It returns false if
        /// the character was never in the owner's screen to begin with.
        /// </summary>
        /// <param name="dwIdentity">The identity of the screen object.</param>
        public bool Delete(uint dwIdentity)
        {
            IScreenObject pObj;
            if (m_pPlayers.TryRemove(dwIdentity, out pObj)
                || m_pRoles.TryRemove(dwIdentity, out pObj)
                || m_pObjects.TryRemove(dwIdentity, out pObj))
            {
                if (pObj is Monster)
                {
                    (pObj as Monster).TargetCount -= 1;
                }
                m_pOwner.Send(new MsgAction(dwIdentity, 0, 0, GeneralActionType.REMOVE_ENTITY));
                m_pOwner.Send(new MsgAction(dwIdentity, 0, 0, GeneralActionType.REMOVE_ENTITY));
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method removes a screen object from the owner's screen without using force. It will not remove the
        /// spawn. This method is used for characters who are actively removing themselves out of the screen.
        /// </summary>
        /// <param name="dwIdentity">The identity of the actor.</param>
        public bool Remove(uint dwIdentity)
        {
            IScreenObject pObj;
            if (m_pPlayers.TryRemove(dwIdentity, out pObj)
                || m_pRoles.TryRemove(dwIdentity, out pObj)
                || m_pObjects.TryRemove(dwIdentity, out pObj))
            {
                if (pObj is Monster)
                {
                    (pObj as Monster).TargetCount -= 1;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method removes the owner from all observers. It makes use of the delete method (general action 
        /// subtype packet) to forcefully remove the owner from each screen within the owner's screen distance.
        /// </summary>
        public void RemoveFromObservers()
        {
            foreach (var obs in m_pPlayers.Values)
            {
                var pPlayer = obs as Character;
                if (pPlayer == null) continue;
                pPlayer.Screen.Delete(m_pOwner.Identity);
                Remove(obs.Identity);
            }
            foreach (var mob in m_pObjects.Values)
                if (mob is Monster && (mob as Monster).TargetCount > 0)
                {
                    (mob as Monster).TargetCount -= 1;
                }
        }

        /// <summary>
        /// This method removes an object from the owner and observers screen. It makes use of the delete method
        /// to forcefully remove the owner from each screen within the owner's screen distance.
        /// </summary>
        /// <param name="dwIdentity">The idendity of the object.</param>
        public void RemoveFromObservers(uint dwIdentity)
        {
            foreach (var obs in m_pPlayers.Values)
            {
                if (!(obs is Character))
                    continue;
                Character plr = obs as Character;
                if (plr.Screen == null)
                    continue;
                plr.Screen.Delete(dwIdentity);
            }
        }

        /// <summary>
        /// This method removes the owner from all observers. It makes use of the delete method (general action 
        /// subtype packet) to forcefully remove the owner from each screen within the owner's screen distance.
        /// It then respawns the character in the observers' screens.
        /// </summary>
        public void RefreshSpawnForObservers()
        {
            foreach (var observer in m_pPlayers.Values.Where(x => x is Character).Cast<Character>())
            {
                observer.Screen.Delete(m_pOwner.Identity);
                Remove(observer.Identity);
                Add(observer);
                m_pOwner.ExchangeSpawnPackets(observer);
            }
        }

        public void LoadSurroundings()
        {
            #region Spawn Players
            foreach (var pPlayer in m_pOwner.Map.Players.Values.Where(x => x.Identity != m_pOwner.Identity))
            {
                if (Calculations.InScreen(pPlayer.MapX, pPlayer.MapY, m_pOwner.MapX, m_pOwner.MapY))
                    if (Add(pPlayer))
                    {
                        if (!m_pOwner.IsWatcher)
                        {
                            m_pOwner.ExchangeSpawnPackets(pPlayer);
                        }
                        else
                        {
                            pPlayer.SendSpawnTo(m_pOwner);
                        }
                    }
            }
            #endregion
            #region Spawn Objects (NPCs, Items)

            foreach (var pObj in m_pOwner.Map.GameObjects.Values)
            {
                if (Calculations.InScreen(pObj.MapX, pObj.MapY, m_pOwner.MapX, m_pOwner.MapY) && Add(pObj))
                {
                    pObj.SendSpawnTo(m_pOwner);
                }
            }

            #endregion
        }

        public void SendMovement(byte[] pMsg)
        {
            // For each possible observer on the map
            foreach (var pPlayer in m_pOwner.Map.Players.Values.Where(x => x.Identity != m_pOwner.Identity))
            {
                // If the character is in screen, make sure it's in the owner's screen
                if (Calculations.InScreen(pPlayer.MapX, pPlayer.MapY, m_pOwner.MapX, m_pOwner.MapY))
                {
                    // Check if the user is already added to the owner's screen
                    if (Add(pPlayer)) 
                        m_pOwner.ExchangeSpawnPackets(pPlayer);
                    // if it already is, then we send the movement packet
                    if (!m_pOwner.Invisible && !m_pOwner.IsWatcher)
                        pPlayer.Send(pMsg);
                }
                // Else, remove the entity from the screens and send the last packet
                else if (pPlayer.Screen.Remove(m_pOwner.Identity) || m_pOwner.Screen.Remove(pPlayer.Identity))
                {
                    if (!pPlayer.Invisible && !pPlayer.IsWatcher)
                        pPlayer.Send(pMsg);

                    //if (pPlayer.FetchTradeRequest(m_pOwner.Identity))
                    //    pPlayer.ClearTradeRequest();
                    //if (m_pOwner.FetchTradeRequest(pPlayer.Identity))
                    //    m_pOwner.ClearTradeRequest();
                    //if (pPlayer.Trade != null)
                        //pPlayer.Trade.CloseWindow();
                }
            }

            foreach (var pObj in m_pOwner.Map.GameObjects.Values)
            {
                if (Calculations.InScreen(pObj.MapX, pObj.MapY, m_pOwner.MapX, m_pOwner.MapY))
                {
                    if (Add(pObj))
                        pObj.SendSpawnTo(m_pOwner);
                }
                else
                {
                    if (pObj is Monster)
                    {
                        (pObj as Monster).TargetCount -= 1;
                    }

                    Remove(pObj.Identity);
                }
            }
        }

        /// <summary>
        /// This method clears the owner's screen trackers. It does not remove the objects from the screen. The 
        /// delete method is required to delete screen objects.
        /// </summary>
        public void Clear()
        {
            m_pPlayers = new ConcurrentDictionary<uint, IScreenObject>();
            m_pRoles = new ConcurrentDictionary<uint, IScreenObject>();
            m_pObjects = new ConcurrentDictionary<uint, IScreenObject>();
        }

        public void Send(string szMsg, bool bSelf, ChatTone pTone = ChatTone.TOP_LEFT)
        {
            var pMsg = new MsgTalk(szMsg, pTone);
            if (bSelf) m_pOwner.Send(pMsg);
            foreach (var pRole in m_pPlayers.Values.Cast<Character>()) pRole.Send(pMsg);
        }

        public void Send(byte[] pMsg, bool bSelf)
        {
            if (m_pOwner != null && bSelf) m_pOwner.Send(pMsg);
            foreach (var role in m_pPlayers.Values.Cast<Character>()) role.Send(pMsg);
        }

        /// <summary>
        /// This method will check if the screen of the player has any item on that coordinate.
        /// TODO improve
        /// </summary>
        /// <param name="x">The required x coordinate</param>
        /// <param name="y">The required y coordinate</param>
        /// <returns></returns>
        public bool HasItem(ushort x, ushort y)
        {
            return m_pObjects.Values.FirstOrDefault(item => item is MapItem && item.MapX == x && item.MapY == y) != null;
        }

        public List<IRole> GetAroundPlayers
        {
            get
            {
                return
                    m_pPlayers.Values.Where(x => x is IRole && Calculations.InScreen(m_pOwner.MapX, m_pOwner.MapY, x.MapX, x.MapY)).Cast<IRole>().ToList();
            }
        }

        public List<IRole> GetAroundRoles
        {
            get
            {
                List<IRole> roles = new List<IRole>();
                foreach (var plr in m_pPlayers.Values)
                    roles.Add(plr as IRole);
                foreach (var ai in m_pRoles.Values)
                    roles.Add(ai as IRole);
                return roles;
            }
        }

        public List<IScreenObject> GetTraps
        {
            get { return m_pObjects.Values.Where(x => x.Identity > IdentityRange.TRAPID_FIRST && x.Identity < IdentityRange.TRAPID_LAST).ToList(); }
        }

        public bool ContainsTrap()
        {
            return m_pObjects.Values.Any(pObj => pObj.Identity > IdentityRange.TRAPID_FIRST && pObj.Identity < IdentityRange.TRAPID_LAST);
        }
    }
}