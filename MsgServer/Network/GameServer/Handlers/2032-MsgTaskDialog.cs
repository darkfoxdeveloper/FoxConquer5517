﻿// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2032 - MsgTaskDialog.cs
// Last Edit: 2016/12/27 15:29
// Created: 2016/12/07 00:27

using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Networking.Packets;
using System.Linq;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleTaskDialog(Character pUser, MsgTaskDialog pMsg)
        {
            if (!pUser.IsAlive)
            {
                pUser.Send("You are dead.");
                return;
            }

            byte controlId = pMsg.OptionId;

            if (pUser.Map.GameObjects.TryGetValue(pMsg.TaskId, out IScreenObject interactedNpc) || ServerKernel.Maps[5000].GameObjects.TryGetValue(pMsg.TaskId, out interactedNpc))
            {
                pUser.InteractingNpc = interactedNpc;
            }
            if (pUser.InteractingNpc != null)
            {
                #region Direct NPC Dialogs
                if (pUser.InteractingNpc.Identity >= 60001 && pUser.InteractingNpc.Identity <= 70000 || pUser.InteractingNpc.Identity == 1209)
                {
                    TQDialog dialog = new TQDialog(pUser);
                    dialog.SetAvatar(0);
                    switch (pUser.InteractingNpc.Identity)
                    {
                        case 1209:
                            {
                                dialog.SetAvatar(82);
                                switch (controlId)
                                {
                                    case 0:
                                        {
                                            dialog.AddText("Welcome to FoxConquer! This is a beta server when it is no longer beta you will continue to keep all the inventory and the level. Enjoy this server and report any bug you find :) ");
                                            dialog.AddText("You receive 100 CPs, 50,000 silvers and 200 Study Points for staying 30 minutes online! ");
                                            dialog.AddText("Can buy some required items for promotion here only talk me.");
                                            dialog.AddOption("Buy Promotion Items", 1);
                                            dialog.AddOption("Thanks", 255);
                                            dialog.Show();
                                            break;
                                        }
                                    case 1:
                                        {
                                            dialog.AddText("Have this items for promotion you can buy.");
                                            dialog.AddOption("Buy Emerald [250 CPs]", 2);
                                            dialog.AddOption("Buy MoonBox [500 CPs]", 3);
                                            dialog.AddOption("Buy 20 EuxeniteOre [2000 CPs]", 4);
                                            dialog.AddOption("No thanks", 255);
                                            dialog.Show();
                                            break;
                                        }
                                    case 2: // Emerald
                                        {
                                            if (pUser.ReduceEmoney(250))
                                            {
                                                if (pUser.Inventory.Create(1080001))
                                                {
                                                    dialog.AddText("Here is your item!");
                                                    dialog.AddOption("Thanks", 255);
                                                }
                                                else
                                                {
                                                    dialog.AddText("Not have space in your inventory");
                                                    dialog.AddOption("Ok", 255);
                                                }
                                            }
                                            else
                                            {
                                                dialog.AddText("You not have 250 CPs.");
                                                dialog.AddOption("Oh sorry", 255);
                                            }
                                            dialog.Show();
                                            break;
                                        }
                                    case 3: // MoonBox
                                        {
                                            if (pUser.ReduceEmoney(500))
                                            {
                                                if (pUser.Inventory.Create(721080))
                                                {
                                                    dialog.AddText("Here is your item!");
                                                    dialog.AddOption("Thanks", 255);
                                                }
                                                else
                                                {
                                                    dialog.AddText("Not have space in your inventory");
                                                    dialog.AddOption("Ok", 255);
                                                }
                                            }
                                            else
                                            {
                                                dialog.AddText("You not have 500 CPs.");
                                                dialog.AddOption("Oh sorry", 255);
                                            }
                                            dialog.Show();
                                            break;
                                        }
                                    case 4: // EuxeniteOre
                                        {
                                            if (pUser.ReduceEmoney(2000))
                                            {
                                                if (pUser.Inventory.Create(1072031))
                                                {
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    pUser.Inventory.Create(1072031);
                                                    dialog.AddText("Here is your items!");
                                                    dialog.AddOption("Thanks", 255);
                                                }
                                                else
                                                {
                                                    dialog.AddText("Not have space in your inventory");
                                                    dialog.AddOption("Ok", 255);
                                                }
                                            }
                                            else
                                            {
                                                dialog.AddText("You not have 2000 CPs.");
                                                dialog.AddOption("Oh sorry", 255);
                                            }
                                            dialog.Show();
                                            break;
                                        }
                                }
                                break;
                            }
                        case 60001:
                            {
                                dialog.SetAvatar(50);
                                switch (controlId)
                                {
                                    case 0:
                                        {
                                            dialog.AddText("Hello " + pUser.Name + ", I need help with some monsters in twin city. You can help me?");
                                            dialog.AddOption("Oh yes of course.", 1);
                                            dialog.AddOption("Sorry...", 255);
                                            dialog.Show();
                                            break;
                                        }
                                    case 1:
                                        {
                                            if (QuestJarManager.QuestsFinished(pUser).Count() >= 3) // Already completed 3 or more quests
                                            {
                                                dialog.AddText("Sorry but cannot help me more today. Come back tomorrow!");
                                                dialog.AddOption("Oh...", 255);
                                            }
                                            else
                                            {
                                                dialog.AddText("Choose type of monster you can help me.");
                                                dialog.AddOption("Pheasant (Lv1)", 2);
                                                dialog.AddOption("Turtledove (Lv7)", 3);
                                                dialog.AddOption("Robin (Lv12)", 4);
                                                dialog.AddOption("Apparition (Lv17)", 5);
                                                dialog.AddOption("In other time...", 255);
                                            }
                                            dialog.Show();
                                            break;
                                        }
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                        {
                                            ushort monsterType = System.Convert.ToUInt16(controlId);
                                            monsterType--;
                                            ushort requiredKills = 30;
                                            if (monsterType > 1)
                                            {
                                                requiredKills = 100;
                                            }

                                            if (pUser.Level <= 20)
                                            {
                                                QuestJar quest = QuestJarManager.CurrentQuest(pUser);
                                                Item cloudSaintsJar = pUser.Inventory.GetByType(SpecialItem.CLOUDSAINTS_JAIR);
                                                if (quest == null)
                                                {
                                                    if (cloudSaintsJar == null)
                                                    {
                                                        if (pUser.Inventory.CreateJar(monsterType, requiredKills))
                                                        {
                                                            QuestJarManager.NewQuest(pUser, monsterType);
                                                            dialog.AddText("Here is your CloudSaint's Jar for follow the quest!");
                                                            dialog.AddOption("Oh Thanks", 255);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (quest.Finished)
                                                    {
                                                        dialog.AddText("You cannot help more today. Thanks for your work!");
                                                        dialog.AddOption("I come tomorrow.", 255);
                                                    }
                                                    else
                                                    {
                                                        if (quest.IsFinished(true))
                                                        {
                                                            dialog.AddText("You finished. Thanks for you help.");
                                                            dialog.AddOption("Thank you!", 255);
                                                        }
                                                        else
                                                        {
                                                            dialog.AddText("You have killed " + quest.Kills + "/" + quest.RequiredKills + " " + quest.Monster.Name);
                                                            if (cloudSaintsJar == null)
                                                            {
                                                                dialog.AddOption("I Lost the CloudSaint'sJar", 6);
                                                            }
                                                            dialog.AddOption("Oh Thanks", 255);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                dialog.AddText("You can help my brother in Phoenix City. This monsters is for level 20 or less only.");
                                                dialog.AddOption("Oh ok...", 255);
                                            }
                                            dialog.Show();
                                            break;
                                        }
                                    case 6:
                                        {
                                            QuestJar quest = QuestJarManager.CurrentQuest(pUser);

                                            pUser.Inventory.CreateJar((ushort)quest.Monster.Id, (ushort)quest.RequiredKills);
                                            dialog.AddText("Here is your CloudSaint's Jar for follow the quest!");
                                            dialog.AddOption("Oh Thanks", 255);
                                            dialog.Show();
                                            break;
                                        }
                                }
                                break;
                            }
                        default:
                            {
                                dialog.SetAvatar(1);
                                switch (controlId)
                                {
                                    case 0:
                                        {
                                            dialog.AddText("Sorry but this npc not have dialog");
                                            dialog.AddOption(":(", 255);
                                            dialog.Show();
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
                #endregion
                #region Database NPC Dialogs
                else
                {
                    switch (pMsg.InteractType)
                    {
                        // The message box does have the options OK and Cancel
                        case MsgTaskDialog.MESSAGE_BOX:
                            {
                                if (pUser.CaptchaBox != null)
                                {
                                    if (controlId == 0)
                                    {
                                        pUser.CaptchaBox.OnCancel(pUser);
                                    }
                                    else
                                    {
                                        pUser.CaptchaBox.OnOk(pUser);
                                    }
                                    pUser.CaptchaBox = null;
                                }
                                else if (pUser.RequestBox != null)
                                {
                                    if (controlId == 0) // cancel
                                    {
                                        pUser.RequestBox.OnCancel(pUser);
                                    }
                                    else if (controlId == 255) // ok
                                    {
                                        pUser.RequestBox.OnOk(pUser);
                                    }
                                    pUser.RequestBox = null;
                                }
                                break;
                            }
                        // This is what we receive when we click on OK of a Input box on a NPC
                        // Or a option of the NPC.
                        case MsgTaskDialog.ANSWER:
                            {
                                if (pUser.NextActions.TryGetValue(pMsg.OptionId, out INextAction action))
                                {
                                    pUser.NextActions.Clear();
                                    pUser.GameAction.ProcessAction(action.Identity, pUser, pUser.InteractingNpc, pUser.TaskItem, action.IsInput ? pMsg.Text : null);
                                }
                                break;
                            }
                        case MsgTaskDialog.TEXT_INPUT:
                            {
                                if (pMsg.InteractType == 102)
                                {
                                    if (pUser.SyndicateIdentity <= 0)
                                        return;

                                    pUser.Syndicate.ExpelMember(pUser, pMsg.Text, true);
                                    return;
                                }

                                if (controlId == 255)
                                    break;

                                if (pUser.NextActions.TryGetValue(controlId, out INextAction action))
                                {
                                    pUser.NextActions.Clear();
                                    if (pUser.InteractingNpc != null &&
                                        (pUser.InteractingNpc.MapIdentity == 5000 ||
                                         Calculations.InScreen(pUser.MapX, pUser.MapY,
                                             pUser.InteractingNpc.MapX, pUser.InteractingNpc.MapY)))
                                    {
                                        if (pUser.InteractingNpc is GameNpc)
                                        {
                                            var pNpc = pUser.InteractingNpc as GameNpc;
                                            pUser.GameAction.ProcessAction(GetNextAction(pUser, action.Identity), pUser, pNpc, null,
                                                action.IsInput ? pMsg.Text : null);
                                        }
                                        else if (pUser.InteractingNpc is DynamicNpc)
                                        {
                                            var pNpc = pUser.InteractingNpc as DynamicNpc;
                                            pUser.GameAction.ProcessAction(GetNextAction(pUser, action.Identity), pUser, pNpc, null,
                                                action.IsInput ? pMsg.Text : null);
                                        }
                                    }
                                    else if (pUser.TaskItem != null)
                                    {
                                        pUser.GameAction.ProcessAction(GetNextAction(pUser, action.Identity), pUser, null, pUser.TaskItem,
                                            action.IsInput ? pMsg.Text : null);
                                        pUser.TaskItem = null;
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                switch (controlId)
                                {
                                    case 0:
                                        {
                                            pUser.NextActions.Clear();
                                            if (pUser.Map.GameObjects.TryGetValue(pMsg.TaskId, out IScreenObject pNpc)
                                                || ServerKernel.Maps[5000].GameObjects.TryGetValue(pMsg.TaskId, out pNpc))
                                            {
                                                pUser.GameAction.ProcessAction(GetActionIdentity(pUser, pNpc), pUser, pNpc, null,
                                                    "");
                                            }
                                        }
                                        break;
                                    /*case 12:
                                    {
                                        Dialog = new TQDialog(client);
                                        NpcSpawn npc;
                                        if (!client.Map.NPCs.TryGetValue(requestedId, out npc))
                                            break;
                                        ExecuteTask(client, npc);
                                        break;
                                    }*/
                                    case 255:
                                        {
                                            // Close Dialog
                                            pUser.TaskItem = null;
                                            pUser.InteractingNpc = null;
                                            pUser.NextActions.Clear();
                                            break;
                                        }
                                    default:
                                        {
                                            pUser.TaskItem = null;
                                            pUser.InteractingNpc = null;
                                            pUser.NextActions.Clear();
                                            ServerKernel.Log.SaveLog(string.Format("Npc interact type default [{0}] not handled.", controlId), false, LogType.WARNING);
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
                #endregion
            }
            else if (pUser.TaskItem != null)
            {
                switch (pMsg.InteractType)
                {
                    case MsgTaskDialog.ANSWER:
                        {
                            if (pUser.NextActions.TryGetValue(pMsg.OptionId, out INextAction action))
                            {
                                pUser.NextActions.Clear();
                                pUser.GameAction.ProcessAction(action.Identity, pUser, null, pUser.TaskItem, action.IsInput ? pMsg.Text : null);
                            }
                            break;
                        }
                    case 255:
                        {
                            // Close Dialog
                            pUser.TaskItem = null;
                            pUser.InteractingNpc = null;
                            pUser.NextActions.Clear();
                            break;
                        }
                    default:
                        {
                            pUser.TaskItem = null;
                            pUser.InteractingNpc = null;
                            pUser.NextActions.Clear();
                            ServerKernel.Log.SaveLog(string.Format("Npc interact type default [{0}] not handled for item.", controlId), false, LogType.WARNING);
                            break;
                        }

                }
            }
        }

        private static uint GetNextAction(Character pUser, uint idAction)
        {
            if (ServerKernel.GameTasks.TryGetValue(idAction, out TaskStruct pTask))
            {
                bool bItem1 = true,
                    bItem2 = true,
                    bMoney = true,
                    bProf = true,
                    bSex = true,
                    bMinPk = true,
                    bMaxPk = true,
                    bTeam = true,
                    bMetem = true,
                    bMarriage = true;
                if (pTask.Itemname1 != "")
                    bItem1 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname1) != null;
                if (pTask.Itemname2 != "")
                    bItem2 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname2) != null;
                if (pTask.Money > 0)
                    bMoney = pUser.Silver >= pTask.Money;
                if (pTask.Profession > 0)
                {
                    if (pTask.Profession > 9)
                        bProf = pUser.Profession / 10 == pTask.Profession;
                    else
                        bProf = pUser.Profession == pTask.Profession;
                }
                if (pTask.Sex > 0 && pTask.Sex < 999)
                    bSex = pUser.Gender == pTask.Sex;
                bMinPk = pUser.PkPoints >= pTask.MinPk;
                bMaxPk = pUser.PkPoints <= pTask.MaxPk;
                if (pTask.Team != 0 && pTask.Team != 999)
                    bTeam = pUser.Team != null;
                if (pTask.Metempsychosis > 0)
                    bMetem = pUser.Metempsychosis >= pTask.Metempsychosis;
                if (pTask.Marriage > 0)
                    bMarriage = pUser.Mate != "None";

                bool bRet = bItem1 && bItem2 && bMoney && bProf && bSex && bMinPk
                            && bMaxPk && bTeam && bMetem && bMarriage;

                if (bRet)
                {
                    idAction = pTask.IdNext;
                }
                if (!bRet && pTask.IdNextfail != 0)
                {
                    idAction = pTask.IdNextfail;
                }
            }
            return idAction;
        }

        private static uint GetActionIdentity(Character pUser, IScreenObject pNpc)
        {
            uint idAction = 0;
            if (pNpc is GameNpc)
            {
                GameNpc pGameNpc = pNpc as GameNpc;

                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0: idAction = pGameNpc.Task0; break;
                        case 1: idAction = pGameNpc.Task1; break;
                        case 2: idAction = pGameNpc.Task2; break;
                        case 3: idAction = pGameNpc.Task3; break;
                        case 4: idAction = pGameNpc.Task4; break;
                        case 5: idAction = pGameNpc.Task5; break;
                        case 6: idAction = pGameNpc.Task6; break;
                        case 7: idAction = pGameNpc.Task7; break;
                    }

                    if (ServerKernel.GameTasks.TryGetValue(idAction, out TaskStruct pTask))
                    {
                        bool bItem1 = true,
                            bItem2 = true,
                            bMoney = true,
                            bProf = true,
                            bSex = true,
                            bMinPk = true,
                            bMaxPk = true,
                            bTeam = true,
                            bMetem = true,
                            bMarriage = true;
                        if (pTask.Itemname1 != "")
                            bItem1 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname1) != null;
                        if (pTask.Itemname2 != "")
                            bItem2 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname2) != null;
                        if (pTask.Money > 0)
                            bMoney = pUser.Silver >= pTask.Money;
                        if (pTask.Profession > 0)
                        {
                            if (pTask.Profession > 9)
                                bProf = pUser.Profession / 10 == pTask.Profession;
                            else
                                bProf = pUser.Profession == pTask.Profession;
                        }
                        if (pTask.Sex > 0 && pTask.Sex < 999)
                            bSex = pUser.Gender == pTask.Sex;
                        bMinPk = pUser.PkPoints >= pTask.MinPk;
                        bMaxPk = pUser.PkPoints <= pTask.MaxPk;
                        if (pTask.Team != 0 && pTask.Team != 999)
                            bTeam = pUser.Team != null;
                        if (pTask.Metempsychosis > 0)
                            bMetem = pUser.Metempsychosis >= pTask.Metempsychosis;
                        if (pTask.Marriage > 0)
                            bMarriage = pUser.Mate != "None";

                        bool bRet = bItem1 && bItem2 && bMoney && bProf && bSex && bMinPk
                                    && bMaxPk && bTeam && bMetem && bMarriage;

                        if (bRet)
                        {
                            idAction = pTask.IdNext;
                            break;
                        }
                        if (!bRet && pTask.IdNextfail != 0)
                        {
                            idAction = pTask.IdNextfail;
                            break;
                        }
                    }
                }
            }
            else if (pNpc is DynamicNpc)
            {
                DynamicNpc pGameNpc = pNpc as DynamicNpc;

                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0: idAction = pGameNpc.Task0; break;
                        case 1: idAction = pGameNpc.Task1; break;
                        case 2: idAction = pGameNpc.Task2; break;
                        case 3: idAction = pGameNpc.Task3; break;
                        case 4: idAction = pGameNpc.Task4; break;
                        case 5: idAction = pGameNpc.Task5; break;
                        case 6: idAction = pGameNpc.Task6; break;
                        case 7: idAction = pGameNpc.Task7; break;
                    }

                    if (ServerKernel.GameTasks.TryGetValue(idAction, out TaskStruct pTask))
                    {
                        bool bItem1 = true,
                            bItem2 = true,
                            bMoney = true,
                            bProf = true,
                            bSex = true,
                            bMinPk = true,
                            bMaxPk = true,
                            bTeam = true,
                            bMetem = true,
                            bMarriage = true;
                        if (pTask.Itemname1 != "")
                            bItem1 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname1) != null;
                        if (pTask.Itemname2 != "")
                            bItem2 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname2) != null;
                        if (pTask.Money > 0)
                            bMoney = pUser.Silver >= pTask.Money;
                        if (pTask.Profession > 0)
                        {
                            if (pTask.Profession > 9)
                                bProf = pUser.Profession / 10 == pTask.Profession;
                            else
                                bProf = pUser.Profession == pTask.Profession;
                        }
                        if (pTask.Sex > 0 && pTask.Sex < 999)
                            bSex = pUser.Gender == pTask.Sex;
                        bMinPk = pUser.PkPoints >= pTask.MinPk;
                        bMaxPk = pUser.PkPoints <= pTask.MaxPk;
                        if (pTask.Team != 0 && pTask.Team != 999)
                            bTeam = pUser.Team != null;
                        if (pTask.Metempsychosis > 0)
                            bMetem = pUser.Metempsychosis >= pTask.Metempsychosis;
                        if (pTask.Marriage > 0)
                            bMarriage = pUser.Mate != "None";

                        bool bRet = bItem1 && bItem2 && bMoney && bProf && bSex && bMinPk
                                    && bMaxPk && bTeam && bMetem && bMarriage;

                        if (bRet)
                        {
                            idAction = pTask.IdNext;
                            break;
                        }
                        if (!bRet && pTask.IdNextfail != 0)
                        {
                            idAction = pTask.IdNextfail;
                            break;
                        }
                    }
                }
            }
            return idAction;
        }
    }
}