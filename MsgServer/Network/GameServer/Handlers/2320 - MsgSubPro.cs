// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2320 - MsgSubPro.cs
// Last Edit: 2016/11/24 10:51
// Created: 2016/11/24 10:51

using System.Collections.Generic;
using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void SubclassLogin(Character pUser)
        {
            IList<DbSubclass> _sub = Database.SubclassRepository.GetAllSubclasses(pUser.Identity);
            if (_sub != null)
            {
                foreach (DbSubclass sub in _sub)
                {
                    ISubclass iclass = new ISubclass
                    {
                        Database = sub,
                        Class = (SubClasses)sub.Class,
                        Level = sub.Level,
                        Promotion = sub.Promotion
                    };

                    if (!pUser.SubClass.Add(iclass))
                        ServerKernel.Log.SaveLog(string.Format("Could not load [{0}] from [{1}]", iclass.Class, pUser.Identity), false, "subclass");
                }
                pUser.SubClass.LearnAll();
                pUser.SubClass.SendAll();
            }

            if (pUser.SubClass.Professions.ContainsKey(pUser.ActiveSubclass))
            {
                pUser.SubClass.Active(pUser.ActiveSubclass);
            }
        }

        public static void HandleSubPro(Character pUser, MsgSubPro pMsg)
        {
            if (pUser.SubClass == null)
                return;
            switch (pMsg.Action)
            {
                case SubClassActions.SWITCH:
                    {
                        pUser.SubClass.Active(pMsg.Subclass);
                        break;
                    }
                case SubClassActions.REQUEST_UPLEV:
                    {
                        pUser.SubClass.Uplev(pMsg.Subclass);
                        break;
                    }
                case SubClassActions.MARTIAL_PROMOTED:
                    {
                        pUser.SubClass.Promote(pMsg.Subclass);
                        break;
                    }
                case SubClassActions.INFO:
                    {
                        pUser.SubClass.SendAll();
                        break;
                    }
                default:
                    ServerKernel.Log.SaveLog(string.Format("Not handled [2320:{0}]", pMsg.Action), true, "sys");
                    break;
            }
        }
    }
}