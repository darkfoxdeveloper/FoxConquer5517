// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1130 - MsgTitle.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/24 09:50

using System.Collections.Generic;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void LoadTitles(Character pUser)
        {
            CqTitleRepository tempRepo = new CqTitleRepository();
            IList<DbTitle> titles = tempRepo.GetUserTitle(pUser.Identity);
            if (titles != null)
            {
                int nNow = UnixTimestamp.Timestamp();
                foreach (DbTitle title in titles)
                {
                    if (title.Timestamp < nNow)
                    {
                        tempRepo.Delete(title);
                        continue;
                    }

                    pUser.Titles.TryAdd(title.Title, title);
                }
            }

            if (pUser.Titles.Count > 0)
            {
                MsgTitle pTitle = new MsgTitle
                {
                    Identity = pUser.Identity,
                    Action = TitleAction.QUERY_TITLE
                };
                foreach (var title in pUser.Titles.Values)
                    pTitle.Append(title.Title);
                pUser.Send(pTitle);
            }

            if (pUser.Titles.ContainsKey(pUser.Title))
            {
                MsgTitle pTitle = new MsgTitle
                {
                    Identity = pUser.Identity,
                    Action = TitleAction.ADD_TITLE,
                    SelectedTitle = (UserTitle)pUser.Title
                };
                pUser.Send(pTitle);
            }
            else
            {
                pUser.Title = 0;
            }
        }

        public static void HandleTitlePacket(Character pUser, MsgTitle pMsg)
        {
            switch (pMsg.Action)
            {
                #region Query Title
                case TitleAction.QUERY_TITLE:
                    {
                        if (pUser.Titles.Count > 0)
                        {
                            foreach (var title in pUser.Titles.Values)
                                pMsg.Append(title.Title);
                            pUser.Send(pMsg);
                        }
                        break;
                    }
                #endregion
                #region Select Title
                case TitleAction.SELECT_TITLE:
                    {
                        if (pMsg.SelectedTitle == UserTitle.NONE)
                        {
                            pUser.Title = 0;
                            pUser.Send(pMsg);
                        }
                        else if (pUser.Titles.ContainsKey((byte)pMsg.SelectedTitle))
                        {
                            pUser.Title = (byte)pMsg.SelectedTitle;
                            pUser.Send(pMsg);
                        }
                        else
                        {
                            return;
                        }
                        pUser.Screen.RefreshSpawnForObservers();
                        break;
                    }
                #endregion
            }
        }
    }
}