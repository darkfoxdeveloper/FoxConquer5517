// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 2080 - MsgChangeName.cs
// Last Edit: 2017/02/04 16:59
// Created: 2017/02/04 14:46

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
        private static ushort MAX_PER_YEAR = 3;
        private static uint YEAR_INTERVAL = 60*60*24*365;

        public static void HandleChangeName(Character pUser, MsgChangeName pMsg)
        {
            NameChangeLogRepo repo = new NameChangeLogRepo();
            var nameChanges = repo.FetchByUser(pUser.Identity);

            bool waitingChange = false;
            ushort usDone = 0;
            if (nameChanges != null)
            {
                int now = UnixTimestamp.Timestamp();
                foreach (var name in nameChanges)
                {
                    if (now < name.Timestamp + YEAR_INTERVAL)
                        usDone += 1;
                    if (name.Changed == 0)
                        waitingChange = true;
                }
            }

            switch (pMsg.Mode)
            {
                case ChangeNameMode.REQUEST_INFO:
                {
                    pMsg.Param1 = usDone;
                    pMsg.Param2 = MAX_PER_YEAR;
                    pUser.Send(pMsg);
                    break;
                }
                case ChangeNameMode.CHANGE_NAME:
                {
                    if (waitingChange)
                    {
                        pUser.Send("You are waiting for your name to change. Please wait until next maintenance and try again.", ChatTone.TALK);
                        return;
                    }

                    if (usDone >= MAX_PER_YEAR)
                    {
                        pUser.Send("You cannot change your name anymore.");
                        return;
                    }

                    string szName = pMsg.Name;

                    if (!CheckName(szName))
                    {
                        pMsg.Mode = ChangeNameMode.CHANGE_NAME_ERROR;
                        pUser.Send(pMsg);
                        pUser.Send("The name you input have invalid characters.");
                        return;
                    }

                    if (!pUser.ReduceEmoney(430000))
                    {
                        pUser.Send(ServerString.STR_NOT_ENOUGH_EMONEY);
                        return;
                    }

                    szName = szName.Replace(' ', '~');
                    DbNameChangeLog pName = new DbNameChangeLog
                    {
                        UserIdentity = pUser.Identity,
                        OldName = pUser.Name,
                        NewName = szName,
                        Timestamp = (uint) UnixTimestamp.Timestamp()
                    };
                    if (!repo.SaveOrUpdate(pName))
                    {
                        pUser.AwardEmoney(430000);
                        pUser.Send("Error while trying to save your new name. You will receive your 430,000 CPs back.");
                        return;
                    }
                    ServerKernel.SendMessageToAll(string.Format("{0} has changed his/her name to {1}. The new name will take effect after the next maintenance.", pUser.Name, szName), ChatTone.TALK);
                    pMsg.Mode = ChangeNameMode.CHANGED_SUCCESSFULY;
                    pUser.Send(pMsg);
                    break;
                }
                default:
                {
                    ServerKernel.Log.SaveLog("unhandled type " + pMsg.Mode, true, "name_change", LogType.ERROR);
                    break;
                }
            }
        }
    }
}