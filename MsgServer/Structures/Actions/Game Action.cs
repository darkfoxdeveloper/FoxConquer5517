// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Game Action.cs
// Last Edit: 2016/12/13 12:48
// Created: 2016/12/06 14:12

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Network.GameServer.Handlers;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Actions
{
    public sealed class GameAction
    {
        private Character m_pUser;
        private IScreenObject m_pRole;
        private Item m_pItem;

        private List<MsgTaskDialog> m_pReplies = new List<MsgTaskDialog>();

        public GameAction()
        {

        }

        public GameAction(IScreenObject pRole)
        {
            m_pRole = pRole;
            if (pRole is Character)
                m_pUser = pRole as Character;
        }

        public bool ProcessAction(uint idAction, Character pUser, IScreenObject pRole, Item pItem, string szAccept)
        {
            m_pReplies = new List<MsgTaskDialog>();
            if (idAction == 0)
                return false;

            m_pUser = pUser;
            m_pRole = pRole;
            m_pItem = pItem;

            if (m_pUser != null)
                m_pUser.InteractingNpc = m_pRole;

            try
            {
                const int maxAction = 32;
                int nActionCount = 0;

                while (idAction != 0)
                {
                    if (nActionCount > maxAction)
                    {
                        ServerKernel.Log.SaveLog("ERROR: deadloop idAction " + idAction);
                        return false;
                    }

                    ActionStruct pAction = null;
                    try
                    {
                        pAction = ServerKernel.GameActions[idAction];
                    }
                    catch
                    {
                        return false;
                    }
                    if (pAction == null)
                    {
                        ServerKernel.Log.SaveLog(string.Format("ERROR: game action {0} not found.", idAction));
                        return false;
                    }

                    string temp = pAction.Param;
                    pAction.Param = VariableReplace(pAction.Param);

                    if (szAccept != null && m_pUser != null)
                        m_pUser.TemporaryString = szAccept;

                    if (pUser != null && pUser.IsPm)
                    {
                        pUser.Send(string.Format("{0}: [{1},{2}]. type[{3}], data[{4}], param:[{5}].",
                            pAction.Id, pAction.IdNext, pAction.IdNextfail, pAction.Type, pAction.Data, pAction.Param),
                        ChatTone.TALK);
                    }

                    bool bRet = false;

                    var actionType = (TaskActionType)pAction.Type;
                    if (actionType > TaskActionType.ACTION_SYS_FIRST && actionType < TaskActionType.ACTION_SYS_LIMIT)
                        bRet = ProcessSystemAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_NPC_FIRST && actionType < TaskActionType.ACTION_NPC_LIMIT)
                        bRet = ProcessNpcAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_MAP_FIRST && actionType < TaskActionType.ACTION_MAP_LIMIT)
                        bRet = ProcessMapAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_ITEMONLY_FIRST && actionType < TaskActionType.ACTION_ITEM_LIMIT)
                        bRet = ProcessItemAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_SYN_FIRST && actionType < TaskActionType.ACTION_SYN_LIMIT)
                        bRet = ProcessSynAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_MST_FIRST && actionType < TaskActionType.ACTION_MST_LIMIT)
                        bRet = ProcessMonsterAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_FAMILY_FIRST && actionType < TaskActionType.ACTION_FAMILY_LIMIT)
                        bRet = ProcessFamilyAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_USER_FIRST && actionType < TaskActionType.ACTION_USER_LIMIT)
                        bRet = ProcessUserAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_EVENT_FIRST && actionType < TaskActionType.ACTION_EVENT_LIMIT)
                        bRet = ProcessEventAction(pAction, ref szAccept);
                    if (actionType > TaskActionType.ACTION_DETAIN_FIRST && actionType < TaskActionType.ACTION_DETAIN_LIMIT)
                        bRet = ProcessDetainAction(pAction, ref szAccept);

                    idAction = bRet ? pAction.IdNext : pAction.IdNextfail;

                    pAction.Param = temp;
                    if (nActionCount++ > maxAction)
                    {
                        ServerKernel.Log.SaveLog(string.Format("Error: too many game action, last action:{0}", idAction));
                        break;
                    }
                }
                return true;
            }
            catch (Exception kos)
            {
                ServerKernel.Log.SaveLog("ERROR: ACTION" + idAction + "\r\n" + kos, true, LogType.ERROR);
                return false;
            }
        }

        //=====================================================================================
        private bool ProcessSystemAction(ActionStruct action, ref string szAccept)
        {
            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_MENUTEXT: return MENUTEXT(action.Param);
                case TaskActionType.ACTION_MENULINK: return MENULINK(action.Param);
                case TaskActionType.ACTION_MENUEDIT: return MENUEDIT(action.Param);
                case TaskActionType.ACTION_MENUPIC: return MENUPIC(action.Param);
                case TaskActionType.ACTION_MENUCREATE: return MENUCREATE();
                case TaskActionType.ACTION_RAND: return SYSRAND(action.Param);
                case TaskActionType.ACTION_RANDACTION: return RANDACTION(action.Param, ref szAccept);
                case TaskActionType.ACTION_CHKTIME: return CHKTIME(action.Param, action.Data);
                case TaskActionType.ACTION_BROCASTMSG: return BROCASTMSG(action.Param, action.Data);
                case TaskActionType.ACTION_EXECUTEQUERY: return ACTION_EXECUTEQUERY(action);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessNpcAction(ActionStruct action, ref string szAccept)
        {
            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_NPC_ATTR: return NPCATTR(action.Param);
                case TaskActionType.ACTION_NPC_ERASE: return ACTIONERASENPC(action);
                case TaskActionType.ACTION_NPC_RESETSYNOWNER: return RESETSYNOWNER();
                case TaskActionType.ACTION_NPC_FIND_NEXT_TABLE: return ACTION_NPC_FIND_NEXT_TABLE(action);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessMapAction(ActionStruct action, ref string szAccept)
        {
            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_MAP_MOVENPC: return ACTIONMAPMOVENPC(action.Param, action.Data);
                case TaskActionType.ACTION_MAP_MAPUSER: return MAPUSER(action.Data, action.Param);
                case TaskActionType.ACTION_MAP_CHANGEWEATHER: return ACTIONMAPCHANGEWEATHER(action);
                case TaskActionType.ACTION_MAP_BROCASTMSG: return MAPBROADCAST(action.Data, action.Param);
                case TaskActionType.ACTION_MAP_DROPITEM: return DROPMAPITEM(action.Param);
                case TaskActionType.ACTION_MAP_SETSTATUS: return ACTIONMAPSETSTATUS(action.Param);
                case TaskActionType.ACTION_MAP_ATTRIB: return MAPATTRIBUTE(action.Param);
                case TaskActionType.ACTION_MAP_MAPEFFECT: return ACTIONMAPMAPEFFECT(action.Param);
                case TaskActionType.ACTION_MAP_FIREWORKS: return ACTIONMAPFIREWORKS();
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessItemAction(ActionStruct action, ref string szAccept)
        {
            if (m_pUser == null)
                return false;

            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_ITEM_REQUESTLAYNPC: return ACTION_ITEM_REQUESTLAYNPC(action);
                case TaskActionType.ACTION_ITEM_LAYNPC: return ACTION_ITEM_LAYNPC(action);
                case TaskActionType.ACTION_ITEM_DELTHIS: return ACTIONITEMDELTHIS();
                case TaskActionType.ACTION_ITEM_ADD: return USRITEMADD(action.Param, action.Data);
                case TaskActionType.ACTION_ITEM_DEL: return DELETEITEM(action);
                case TaskActionType.ACTION_ITEM_CHECK: return ITEMCHECK(action);
                case TaskActionType.ACTION_ITEM_HOLE: return ITEMHOLE(action);
                case TaskActionType.ACTION_ITEM_LEAVESPACE: return LEAVESPACE(action.Data);
                case TaskActionType.ACTION_ITEM_MULTIDEL: return ITEMMULTIDEL(action.Param, action.Data);
                case TaskActionType.ACTION_ITEM_MULTICHK: return ITEMMULTICHECK(action.Param, action.Data);
                case TaskActionType.ACTION_ITEM_UPEQUIPMENT: return ITEMUPEQUIPMENT(action.Param);
                case TaskActionType.ACTION_ITEM_EQUIPTEST: return EQUIPTEST(action.Param);
                case TaskActionType.ACTION_ITEM_EQUIPEXIST: return EQUIPEXIST(action.Data, action.Param);
                case TaskActionType.ACTION_ITEM_EQUIPCOLOR: return EQUIPCOLOR(action.Param);
                case TaskActionType.ACTION_ITEM_REMOVE_ANY: return ACTION_ITEM_REMOVE_ANY(action);
                case TaskActionType.ACTION_ITEM_CHECKRAND: return ACTION_ITEM_CHECKRAND(action);
                case TaskActionType.ACTION_ITEM_MODIFY: return ITEMMODIFY(action);
                case TaskActionType.ACTION_ITEM_JAR_CREATE: return ACTION_ITEM_JAR_CREATE(action);
                case TaskActionType.ACTION_ITEM_JAR_VERIFY: return ACTION_ITEM_JAR_VERIFY(action);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessSynAction(ActionStruct action, ref string szAccept)
        {
            if (m_pUser == null)
                return false;

            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_SYN_CREATE: return CREATESYNDICATE(action.Param, szAccept);
                case TaskActionType.ACTION_SYN_DESTROY: return ACTIONSYNDESTROY();
                case TaskActionType.ACTION_SYN_SET_ASSISTANT: return ACTIONSYNSETASSISTANT(szAccept);
                case TaskActionType.ACTION_SYN_CLEAR_RANK: return ACTIONSYNCLEARRANK(szAccept);
                case TaskActionType.ACTION_SYN_CHANGE_LEADER: return ACTIONSYNCHANGELEADER(szAccept);
                case TaskActionType.ACTION_SYN_ANTAGONIZE: return ACTIONSYNANTAGONIZE(szAccept);
                case TaskActionType.ACTION_SYN_CLEAR_ANTAGONIZE: return ACTIONSYNCLEARANTAGONIZE(szAccept);
                case TaskActionType.ACTION_SYN_ALLY: return ACTIONSYNALLY();
                case TaskActionType.ACTION_SYN_CLEAR_ALLY: return ACTIONSYNCLEARALLY(szAccept);
                case TaskActionType.ACTION_SYN_ATTR: return ACTIONSYNATTR(action);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessMonsterAction(ActionStruct action, ref string szAccept)
        {
            if (m_pRole == null || !(m_pRole is Monster))
                return false;

            Monster pMonster = m_pRole as Monster;

            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_MST_DROPITEM: return ACTIONMSGDROPITEM(action, pMonster);
                case TaskActionType.ACTION_MST_REFINERY: return ACTIONMSTREFINERY(action, pMonster);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessFamilyAction(ActionStruct action, ref string szAccept)
        {
            if (m_pUser == null)
                return false;

            switch ((TaskActionType) action.Type)
            {
                case TaskActionType.ACTION_FAMILY_CREATE: return ACTION_FAMILY_CREATE(action, szAccept);
                case TaskActionType.ACTION_FAMILY_DESTROY: return ACTION_FAMILY_DESTROY(action);
                case TaskActionType.ACTION_FAMILY_ATTR: return ACTION_FAMILY_ATTR(action);
                case TaskActionType.ACTION_FAMILY_UPLEV: return ACTION_FAMILY_UPLEV(action);
                case TaskActionType.ACTION_FAMILY_BPUPLEV: return ACTION_FAMILY_BPUPLEV(action);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessUserAction(ActionStruct action, ref string szAccept)
        {
            if (m_pUser == null)
                return false;

            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_USER_ATTR: return USERATTR(action.Param);
                case TaskActionType.ACTION_USER_FULL: return USERFILLATTR(action.Param);
                case TaskActionType.ACTION_USER_CHGMAP: return USERCHGMAP(action.Param);
                case TaskActionType.ACTION_USER_RECORDPOINT: return USERSAVELOCATION(action.Param);
                case TaskActionType.ACTION_USER_HAIR: return ACTIONUSERHAIR(action);
                case TaskActionType.ACTION_USER_CHGMAPRECORD: return USERCHGMAPRECORD();
                case TaskActionType.ACTION_USER_TRANSFORM: return USERTRANSFORM(action.Param);
                case TaskActionType.ACTION_USER_ISPURE: return ACTIONUSERISPURE();
                case TaskActionType.ACTION_USER_TALK: return USERTALK(action.Param, action.Data);
                case TaskActionType.ACTION_USER_MAGIC: return ACTIONUSERMAGIC(action.Param);
                case TaskActionType.ACTION_USER_WEAPONSKILL: return USERWEAPONSKILL(action.Param);
                case (TaskActionType) 1085:
                case TaskActionType.ACTION_USER_LOG: return ACTIONUSERLOG(action.Param);
                case TaskActionType.ACTION_USER_BONUS: return ACTIONUSERBONUS();
                case TaskActionType.ACTION_USER_DIVORCE: return ACTION_USER_DIVORCE();
                case TaskActionType.ACTION_USER_MARRIAGE: return ACTIONUSERMARRIAGE();
                case TaskActionType.ACTION_USER_SEX: return ACTIONUSERSEX();
                case TaskActionType.ACTION_USER_EFFECT: return ACTIONUSEREFFECT(action);
                case TaskActionType.ACTION_USER_MEDIAPLAY: return ACTIONUSERMEDIAPLAY(action);
                case TaskActionType.ACTION_USER_ADD_TITLE: return ACTIONADDUSERTITLE(action.Param);
                case TaskActionType.ACTION_USER_REMOVE_TITLE: return ACTIONREMOVEUSERTITLE(action.Data);
                case TaskActionType.ACTION_USER_REBIRTH: return ACTIONUSERREBIRTH(action);
                case TaskActionType.ACTION_USER_WEBPAGE: return ACTIONUSERWEBPAGE(action);
                case TaskActionType.ACTION_USER_BBS: return ACTIONUSERSENDBBS(action);
                case (TaskActionType)1043: return true;
                case TaskActionType.ACTION_USER_FIX_ATTR: return ACTIONUSERFIXATTR();
                case TaskActionType.ACTION_USER_OPEN_DIALOG: return USEROPENDIALOG(action.Data);
                case TaskActionType.ACTION_USER_CHGMAP_REBORN: return ACTION_USER_CHGMAP_REBORN(action);
                case TaskActionType.ACTION_USER_OPENINTERFACE: return OPENINTERFACE(action.Data);
                case TaskActionType.ACTION_USER_EXP_MULTIPLY: return USEREXPMULTIPLY(action.Param);
                case TaskActionType.ACTION_USER_WH_PASSWORD: return USERWHPASSWORD(action);
                case TaskActionType.ACTION_USER_SET_WH_PASSWORD: return ACTIONUSERSETWHPASSWORD();
                case TaskActionType.ACTION_USER_VAR_COMPARE: return USERVARCOMPARE(action.Param);
                case TaskActionType.ACTION_USER_VAR_DEFINE: return USERVARDEFINE(action.Param);
                case TaskActionType.ACTION_USER_VAR_CALC: return ACTIONUSERVARCALC(action.Param);
                case (TaskActionType)1065: return false;
                case TaskActionType.ACTION_USER_STC_COMPARE: return USERSTATISTICCOMPARE(action);
                case TaskActionType.ACTION_USER_STC_OPE: return USERSTATISTICOPE(action);
                case (TaskActionType)1075: return true;
                case TaskActionType.ACTION_USER_TASK_MANAGER: return ACTIONUSERTASKMANAGER(action);
                case TaskActionType.ACTION_USER_TASK_OPE: return ACTION_USER_TASK_OPE(action);
                case TaskActionType.ACTION_USER_ATTACH_STATUS: return USERATTACHSTATUS(action.Param);
                case TaskActionType.ACTION_USER_GOD_TIME: return USERADDGODTIME(action.Param);
                case TaskActionType.ACTION_USER_EXPBALL_EXP: return ACTION_USER_EXPBALL_EXP(action);
                case (TaskActionType)1095: return true;
                case TaskActionType.ACTION_USER_STATUS_CREATE: return ACTIONUSERSTATUSCREATE(action);
                case TaskActionType.ACTION_TEAM_BROADCAST: return ACTION_TEAM_BROADCAST(action);
                case TaskActionType.ACTION_TEAM_ATTR: return ACTIONTEAMATTR(action);
                case TaskActionType.ACTION_TEAM_LEAVESPACE: return ACTION_TEAM_LEAVESPACE(action);
                case TaskActionType.ACTION_TEAM_ITEM_ADD: return ACTION_TEAM_ITEM_ADD(action);
                case TaskActionType.ACTION_TEAM_ITEM_CHECK: return ACTION_TEAM_ITEM_CHECK(action);
                case TaskActionType.ACTION_TEAM_ITEM_DEL: return ACTION_TEAM_ITEM_DEL(action);
                case TaskActionType.ACTION_TEAM_CHGMAP: return ACTION_TEAM_CHGMAP(action);
                case TaskActionType.ACTION_TEAM_CHK_ISLEADER: return ACTION_TEAM_CHK_ISLEADER(action);
                case TaskActionType.ACTION_GENERAL_LOTTERY: return ACTION_GENERAL_LOTTERY(action);
                case TaskActionType.ACTION_GENERA_SUBCLASS_MANAGEMENT: return ACTION_GENERA_SUBCLASS_MANAGEMENT(action);
                case TaskActionType.ACTION_GENERAL_SKILL_LINE_ENABLED: return ACTION_GENERAL_SKILL_LINE_ENABLED();
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessEventAction(ActionStruct action, ref string szAccept)
        {
            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_EVENT_SETSTATUS: return ACTION_EVENT_SETSTATUS(action);
                case TaskActionType.ACTION_EVENT_DELNPC_GENID: return ACTION_EVENT_DELNPC_GENID(action);
                case TaskActionType.ACTION_EVENT_COMPARE: return ACTION_EVENT_COMPARE(action);
                case TaskActionType.ACTION_EVENT_COMPARE_UNSIGNED: return ACTION_EVENT_COMPARE_UNSIGNED(action);
                case TaskActionType.ACTION_EVENT_CREATEPET: return ACTION_EVENT_CREATEPET(action);
                case TaskActionType.ACTION_EVENT_CREATENEW_NPC: return EVENTCREATENPC(action.Param);
                case TaskActionType.ACTION_EVENT_COUNTMONSTER: return ACTION_EVENT_COUNTMONSTER(action);
                case TaskActionType.ACTION_EVENT_DELETEMONSTER: return ACTION_EVENT_DELETEMONSTER(action);
                case TaskActionType.ACTION_EVENT_BBS: return ACTION_EVENT_BBS(action);
                case TaskActionType.ACTION_EVENT_ERASE: return ACTIONEVENTERASE(action.Param);
                case TaskActionType.ACTION_EVENT_TELEPORT: return ACTION_EVENT_TELEPORT(action);
                case TaskActionType.ACTION_EVENT_MASSACTION: return ACTION_EVENT_MASSACTION(action);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        private bool ProcessDetainAction(ActionStruct action, ref string szAccept)
        {
            if (m_pUser == null) return false;

            switch ((TaskActionType)action.Type)
            {
                case TaskActionType.ACTION_DETAIN_DIALOG: return ACTION_DETAIN_INTERFACE(action);
                default: ServerKernel.Log.SaveLog("ERROR: System could not find type " + action.Type, false, "action", LogType.WARNING); return false;
            }
        }
        //=====================================================================================
        #region 100 System
        #region 101
        private bool MENUTEXT(string param)
        {
            if (m_pUser == null)
                return false;

            if (param.Length > 100)
            {
                if (param.Length > 1000)
                    param = param.Substring(0, 1000);
                int myLength = param.Length;
                while (myLength > 0)
                {
                    int lastIndex = 100;
                    if (myLength < 100)
                        lastIndex = myLength;
                    string txt = param.Substring(0, lastIndex);
                    param = param.Substring(lastIndex, myLength - lastIndex);
                    myLength -= lastIndex;
                    m_pReplies.Add(new MsgTaskDialog(MsgTaskDialog.DIALOG, txt));
                }
            }
            else
                m_pReplies.Add(new MsgTaskDialog(MsgTaskDialog.DIALOG, param));
            return true;
        }
        #endregion
        #region 102
        private bool MENULINK(string param)
        {
            if (m_pUser == null)
                return false;

            string[] Params = GetSafeParam(param);
            int Option = int.Parse(Params[1]);
            var task = (byte)(m_pUser.NextActions.Count + 1);
            m_pReplies.Add(new MsgTaskDialog(MsgTaskDialog.OPTION, Params[0]) { OptionId = task, TaskId = uint.Parse(Params[1]) });
            INextAction act;
            act.Task = task;
            act.Identity = (uint)Option;
            act.IsInput = false;
            m_pUser.NextActions.Add(act.Task, act);
            return true;
        }
        #endregion
        #region 103
        private bool MENUEDIT(string param)
        {
            if (m_pUser == null)
                return false;

            string[] Params = GetSafeParam(param);
            var task = (byte)(m_pUser.NextActions.Count + 1);

            m_pReplies.Add(new MsgTaskDialog(MsgTaskDialog.INPUT, Params[2])
            {
                InputMaxLength = ushort.Parse(Params[0]),
                OptionId = task
            });

            var act = new INextAction
            {
                Task = task,
                Identity = uint.Parse(Params[1]),
                IsInput = true
            };
            m_pUser.NextActions.Add(act.Task, act);
            return true;
        }
        #endregion
        #region 104
        private bool MENUPIC(string Param)
        {
            if (m_pUser == null)
                return false;

            m_pReplies.Add(new MsgTaskDialog
            {
                InteractType = MsgTaskDialog.AVATAR,
                InputMaxLength = ushort.Parse(Param.Split(' ')[2])
            });
            return true;
        }
        #endregion
        #region 120
        public bool MENUCREATE()
        {
            if (m_pUser == null)
                return false;

            foreach (MsgTaskDialog nr in m_pReplies)
                m_pUser.Send(nr);
            m_pUser.Send(new MsgTaskDialog { InteractType = 100, DontDisplay = false });
            m_pReplies.Clear();
            return true;
        }
        #endregion
        #region 121
        private bool SYSRAND(string param)
        {
            string[] Params = param.Split(' ');
            float Percent;
            int Val1 = int.Parse(Params[0]);
            int Val2 = int.Parse(Params[1]);
            if (Val1 > Val2)
                Percent = 100;
            else
                Percent = (float)Val1 / Val2;
            Percent *= 100;
            return Calculations.ChanceCalc(Percent);
        }
        #endregion
        #region 122
        private bool RANDACTION(string param, ref string szAccept)
        {
            string[] Params = param.Split(' ');
            int i = 0, nextaction;
            i += Params.Count();
            if (i <= 0)
                return false;
            i--;
            // nextaction = ServerKernel.Random.Next(0, i);
            nextaction = ThreadSafeRandom.RandGet(0, i);
            var npc = new MsgTaskDialog
            {
                InteractType = MsgTaskDialog.DIALOG
            };

            //new NpcRequest(uint.Parse(Params[nextaction]), _pUser);
            ProcessAction(uint.Parse(Params[nextaction]), m_pUser, m_pRole, m_pItem, szAccept);
            return true;
        }
        #endregion
        #region 123
        private bool CHKTIME(string param, uint data)
        {
            string[] Params = param.Split(' ');

            DateTime actual = DateTime.Now;
            int nCurDay = actual.Day;
            var nCurWeekDay = (int)actual.DayOfWeek;
            int nCurMonth = actual.Month;
            int nCurYear = actual.Year;
            int nCurHour = actual.Hour;
            int nCurMinute = actual.Minute;
            int nCurSecond = actual.Second;
            switch (data)
            {
                #region Complete date (yyyy-mm-dd hh:mm yyyy-mm-dd hh:mm)
                case 0:
                    {
                        if (Params.Length < 4)
                            return false;

                        string[] time0 = Params[1].Split(':');
                        string[] date0 = Params[0].Split('-');
                        string[] time1 = Params[3].Split(':');
                        string[] date1 = Params[2].Split('-');

                        var dTime0 = new DateTime(int.Parse(date0[0]), int.Parse(date0[1]),
                            int.Parse(date0[2]), int.Parse(time0[0]), int.Parse(time0[1]), 0);
                        var dTime1 = new DateTime(int.Parse(date1[0]), int.Parse(date1[1]),
                            int.Parse(date1[2]), int.Parse(time1[0]), int.Parse(time1[1]), 59);

                        int timestamp0 = UnixTimestamp.Timestamp(dTime0);
                        int timestamp1 = UnixTimestamp.Timestamp(dTime1);
                        int usertimestamp = UnixTimestamp.Timestamp();

                        return (timestamp0 <= usertimestamp && timestamp1 >= usertimestamp);
                    }
                #endregion
                #region On Year date (mm-dd hh:mm mm-dd hh:mm)
                case 1:
                    {
                        if (Params.Length < 4)
                            return false;

                        string[] time0 = Params[1].Split(':');
                        string[] date0 = Params[0].Split('-');
                        string[] time1 = Params[3].Split(':');
                        string[] date1 = Params[2].Split('-');

                        var dTime0 = new DateTime(DateTime.Now.Year, int.Parse(date0[1]),
                            int.Parse(date0[2]), int.Parse(time0[0]), int.Parse(time0[1]), 0);
                        var dTime1 = new DateTime(DateTime.Now.Year, int.Parse(date1[1]),
                            int.Parse(date1[2]), int.Parse(time1[0]), int.Parse(time1[1]), 59);

                        int timestamp0 = UnixTimestamp.MonthDayStamp(dTime0);
                        int timestamp1 = UnixTimestamp.MonthDayStamp(dTime1);
                        int usertimestamp = UnixTimestamp.MonthDayStamp();

                        return (timestamp0 <= usertimestamp && timestamp1 >= usertimestamp);
                    }
                #endregion
                #region Day of the month (dd hh:mm dd hh:mm)
                case 2:
                    {
                        if (Params.Length < 4)
                            return false;

                        string[] time0 = Params[1].Split(':');
                        string date0 = Params[0];
                        string[] time1 = Params[3].Split(':');
                        string date1 = Params[2];

                        var dTime0 = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                            int.Parse(date0), int.Parse(time0[0]), int.Parse(time0[1]), 0);
                        var dTime1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                            int.Parse(date1), int.Parse(time1[0]), int.Parse(time1[1]), 59);

                        int timestamp0 = UnixTimestamp.DayOfTheMonthStamp(dTime0);
                        int timestamp1 = UnixTimestamp.DayOfTheMonthStamp(dTime1);
                        int usertimestamp = UnixTimestamp.DayOfTheMonthStamp();

                        return (timestamp0 <= usertimestamp && timestamp1 >= usertimestamp);
                    }
                #endregion
                #region Day of the week (dw hh:mm dw hh:mm)
                case 3:
                    {
                        if (Params.Length < 4)
                            return false;

                        string[] time0 = Params[1].Split(':');
                        string[] time1 = Params[3].Split(':');

                        int nDay0 = int.Parse(Params[0]);
                        int nDay1 = int.Parse(Params[2]);
                        int nHour0 = int.Parse(time0[0]);
                        int nHour1 = int.Parse(time1[0]);
                        int nMinute0 = int.Parse(time0[1]);
                        int nMinute1 = int.Parse(time1[1]);

                        int timeNow = nCurWeekDay * 24 * 60 + nCurHour * 60 + nCurMinute;
                        int from = nDay0 * 24 * 60 + nHour0 * 60 + nMinute0;
                        int to = nDay1 * 24 * 60 + nHour1 * 60 + nMinute1;

                        return (timeNow >= from && timeNow <= to);
                    }
                #endregion
                #region Hour check (hh:mm hh:mm)
                case 4:
                    {
                        if (Params.Length < 2)
                            return false;

                        string[] time0 = Params[0].Split(':');
                        string[] time1 = Params[1].Split(':');

                        int nHour0 = int.Parse(time0[0]);
                        int nHour1 = int.Parse(time1[0]);
                        int nMinute0 = int.Parse(time0[1]);
                        int nMinute1 = int.Parse(time1[1]);

                        int timeNow = nCurHour * 60 + nCurMinute;
                        int from = nHour0 * 60 + nMinute0;
                        int to = nHour1 * 60 + nMinute1;

                        return (timeNow >= from && timeNow <= to);
                    }
                #endregion
                #region Minute check (mm mm)
                case 5:
                    {
                        if (Params.Length < 2)
                            return false;

                        return (nCurMinute >= int.Parse(Params[0]) && nCurMinute <= int.Parse(Params[1]));
                    }
                #endregion
            }
            return true;
        }
        #endregion
        #region 124
        private bool ACTIONPOSTCMD(string param, uint data)
        {
            if (m_pUser == null) return false;

            //var msg = new MsgAction(_pUser.Identity, data, _pUser.MapX, _pUser.MapY, GeneralActionType.)

            return true;
        }
        #endregion
        #region 125
        public bool BROCASTMSG(string param, uint data)
        {
            ServerKernel.SendMessageToAll(param, (ChatTone)data);
            return true;
        }
        #endregion
        #region 126
        private bool ACTIONMESSAGEBOX(string param)
        {
            if (m_pUser == null) return false;

            return true;
        }
        #endregion
        #region 127
        private bool ACTION_EXECUTEQUERY(ActionStruct action)
        {
            try
            {
                new QueryExecuter().Execute(action.Param);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not execute query ACTION_EXECUTEQUERY::" + action.Param, true, LogType.EXCEPTION);
            }
            return true;
        }
        #endregion
        #endregion
        #region 200 Npc
        #region 201
        private bool NPCATTR(string param)
        {
            string[] _params = GetSafeParam(param);
            if (_params.Count() < 3) return false;

            try
            {
                if ((m_pUser == null && _params.Count() <= 3)
                    || (m_pUser != null && m_pUser.InteractingNpc == null && _params.Count() <= 3))
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            DynamicNpc targetNpc;
            if (_params.Count() >= 4)
                targetNpc = ServerKernel.GetDynaNpcByIdentity(uint.Parse(_params[3]));
            else
                targetNpc = m_pUser.InteractingNpc as DynamicNpc;

            if (targetNpc == null)
                return false;

            string szAttr = _params[0];
            string szOpt = _params[1];
            string szData = _params[2];

            switch (szAttr)
            {
                #region Life
                case "life":
                    {
                        uint life = uint.Parse(szData);
                        switch (szOpt)
                        {
                            case "=":
                                targetNpc.SetAttribute(ClientUpdateType.HITPOINTS, life, false);
                                return true;
                            case "==":
                                return targetNpc.Life == life;
                            case "<":
                                return targetNpc.Life < life;
                            case ">=":
                                return targetNpc.Life >= life;
                            case "+=":
                                return targetNpc.AddAttribute(ClientUpdateType.HITPOINTS, int.Parse(szData), true);
                        }
                        break;
                    }
                #endregion
                case "data0":
                case "data1":
                case "data2":
                case "data3":
                    {
                        switch (szOpt)
                        {
                            case "=":
                                return targetNpc.SetAttribute(szAttr, int.Parse(szData));
                            case "+=":
                                return targetNpc.SetAttribute(szAttr, int.Parse(szData) + targetNpc.GetData(szAttr));
                            case "==":
                                return targetNpc.GetData(szAttr) == int.Parse(szData);
                            case "<":
                                return targetNpc.GetData(szAttr) < int.Parse(szData);
                            case "<=":
                                return targetNpc.GetData(szAttr) <= int.Parse(szData);
                            case ">":
                                return targetNpc.GetData(szAttr) > int.Parse(szData);
                            case ">=":
                                return targetNpc.GetData(szAttr) >= int.Parse(szData);
                        }
                        break;
                    }
                case "ownerid":
                    {
                        switch (szOpt)
                        {
                            case "=":
                                return targetNpc.SetOwnerIdentity(uint.Parse(szData));
                            case "==":
                                return targetNpc.OwnerIdentity == long.Parse(szData);
                        }
                        break;
                    }
                case "ownertype":
                    {
                        if (szOpt == "==")
                            return targetNpc.OwnerType == long.Parse(szData);
                        return false;
                    }
                case "lookface":
                    {
                        switch (szOpt)
                        {
                            case "==":
                                return targetNpc.Lookface == long.Parse(szData);
                            case "=":
                                return targetNpc.SetAttribute(ClientUpdateType.MESH, long.Parse(szData), true);
                        }
                        return false;
                    }
                //case "datastr":
                case "maxlife":
                    {
                        if (szOpt == "=")
                            return targetNpc.SetAttribute(ClientUpdateType.MAX_HITPOINTS, uint.Parse(szData), true);
                        break;
                    }
            }
            return false;
        }
        #endregion
        #region 205

        private bool ACTIONERASENPC(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.InteractingNpc == null)
                return false;

            var pNpc = m_pUser.InteractingNpc as DynamicNpc;
            if (pNpc == null)
                return false;

            //if (pNpc.IsDeleted())
            //    return false;

            uint nType = action.Data;
            if (nType == 0)
            {
                m_pUser.InteractingNpc = null;
                return pNpc.DelNpc();
            }

            foreach (var npc in m_pUser.Map.GameObjects.Values.Where(x => x is DynamicNpc && (x as DynamicNpc).Kind == nType))
            {
                var dynamicNpc = npc as DynamicNpc;
                if (dynamicNpc != null && !dynamicNpc.DelNpc())
                    return false;
            }
            return true;
        }
        #endregion
        #region 207
        private bool RESETSYNOWNER()
        {
            if (m_pRole == null) return false;

            var pNpc = m_pRole as DynamicNpc;
            if (pNpc == null) return false;

            if (!pNpc.Map.IsSynMap()) return false;

            // todo pNpc->SetSynOwnerID(pSynAtt->GetID(), false);		// true: with link map

            var score = pNpc.Scores.Values.OrderByDescending(x => x.Score).FirstOrDefault();
            if (score != null && !pNpc.IsCtfFlag())
            {
                ServerKernel.SendMessageToAll(string.Format("{0} has won.", score.Name), ChatTone.CENTER);

                Syndicate syn;
                if (ServerKernel.Syndicates.TryGetValue(score.Identity, out syn))
                {
                    pNpc.SetOwnerIdentity(syn.Identity);
                    pNpc.SendToRange();
                    pNpc.Scores.Clear();
                    pNpc.Map.OwnerIdentity = pNpc.OwnerIdentity;
                    pNpc.Map.Save();
                }
            } 
            else if (score != null && pNpc.IsCtfFlag())
            {
                string baseName = "Base";
                switch (pNpc.Identity % 10)
                {
                    case 1: baseName = "Bottom Base"; break;
                    case 2: baseName = "Middle Base"; break;
                    case 3: baseName = "Top Base"; break;
                }
                ServerKernel.SendMessageToAll(string.Format("{0} has dominated the {1}.", score.Name, baseName),
                    ChatTone.TOP_LEFT);

                uint oldOwner = pNpc.OwnerIdentity;
                Syndicate syn;
                if (ServerKernel.Syndicates.TryGetValue(score.Identity, out syn))
                {
                    pNpc.SetOwnerIdentity(syn.Identity);
                    pNpc.SendToRange();
                    pNpc.Scores.Clear();
                    pNpc.Save();
                    MsgWarFlag pMsg = new MsgWarFlag
                    {
                        Type = WarFlagType.WAR_BASE_DOMINATE,
                        Identity = pNpc.Identity%10
                    };
                    syn.Send(pMsg);
                    // deactivate the effect on the other syn
                    if (ServerKernel.Syndicates.TryGetValue(oldOwner, out syn))
                    {
                        pMsg.Identity = 0;
                        syn.Send(pMsg);
                    }
                }
            }

            foreach (var usr in pNpc.Map.Players.Values)
            {
                usr.SetAttackTarget(null);
            }

            if (pNpc.IsSynFlag() && !pNpc.IsCtfFlag())
            {
                foreach (var npc in pNpc.Map.GameObjects.Values)
                {
                    if (npc is GameNpc)
                    {
                        (npc as GameNpc).OwnerIdentity = pNpc.OwnerIdentity;
                    }
                    if (npc is DynamicNpc && npc != pNpc)
                    {
                        (npc as DynamicNpc).SetOwnerIdentity(pNpc.OwnerIdentity);
                        if (npc.Identity == 820)
                        {
                            DynamicNpc dynaNpc = npc as DynamicNpc;
                            dynaNpc.Data0 = (int) pNpc.OwnerIdentity;
                            dynaNpc.Data1 = 0;
                            dynaNpc.Data2 = 0;
                            dynaNpc.Data3 = 0;
                            dynaNpc.Save();
                        }
                    }
                }
            }

            return true;
        }
        #endregion
        #region 208
        private bool ACTION_NPC_FIND_NEXT_TABLE(ActionStruct action)
        {
            if (m_pRole == null) return false;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 4)
                return false;

            uint idNpc = uint.Parse(param[0]);
            uint idMap = uint.Parse(param[1]);
            ushort usMapX = ushort.Parse(param[2]);
            ushort usMapY = ushort.Parse(param[3]);

            DynamicNpc pNpc = ServerKernel.GetDynaNpcByIdentity(idNpc);
            if (pNpc == null) return false;

            pNpc.Data0 = (int)idMap;
            pNpc.Data1 = usMapX;
            pNpc.Data2 = usMapY;

            pNpc.Save();

            return true;
        }
        #endregion
        #endregion
        #region 300 Map
        #region 301 - Move NPC
        private bool ACTIONMAPMOVENPC(string szParam, uint dwData)
        {
            string[] param = GetSafeParam(szParam);
            if (param.Length < 3)
                return false;

            uint idMap = uint.Parse(param[0]);
            ushort nPosX = ushort.Parse(param[1]), nPosY = ushort.Parse(param[2]);

            if (idMap <= 0 || nPosX <= 0 || nPosY <= 0)
                return false;

            DynamicNpc pNpc = (from map in ServerKernel.Maps.Values select map.GameObjects.Values.FirstOrDefault(x => x is DynamicNpc && x.Identity == dwData) into obj where obj != null select obj as DynamicNpc).FirstOrDefault();

            if (pNpc == null)
                return false;

            if (!pNpc.IsDynaNpc())
                return false;

            return pNpc.ChangePos(idMap, nPosX, nPosY);
        }
        #endregion
        #region 302 - User Setting Check
        private bool MAPUSER(uint data, string str)
        {
            string[] _params = GetSafeParam(str);
            if (_params.Count() < 3) return false;

            int amount = 0;

            switch (_params[0])
            {
                case "map_user":
                    {
                        Map map;
                        if (!ServerKernel.Maps.TryGetValue(data, out map))
                            return false;
                        amount = map.Players.Count;
                        break;
                    }
                case "alive_user":
                    {
                        Map map;
                        if (!ServerKernel.Maps.TryGetValue(data, out map))
                            return false;
                        foreach (var usr in map.Players.Values.Where(x => x.IsAlive))
                            amount++;
                        break;
                    }
                default:
                    ServerKernel.Log.SaveLog(string.Format("ERROR: ACTION {0} invalid param"), false, LogType.ERROR);
                    break;
            }

            switch (_params[1])
            {
                case "==":
                    return amount == int.Parse(_params[2]);
                case "<=":
                    return amount <= int.Parse(_params[2]);
                case ">=":
                    return amount >= int.Parse(_params[2]);
            }

            return false;
        }
        #endregion
        #region 303 - Broadcast Message
        private bool MAPBROADCAST(uint data, string str)
        {
            if (str.Length > 127)
                return false;

            Map map;
            if (!ServerKernel.Maps.TryGetValue(data, out map))
                return false;

            map.SendMessageToMap(str, (ChatTone)data);

            return true;
        }
        #endregion
        #region 304 - Drop Item
        private bool DROPMAPITEM(string param)
        {
            string[] Params = GetSafeParam(param);
            if (Params.Length < 4)
                return false;

            uint idMap = uint.Parse(Params[0]);
            uint idItemtype = uint.Parse(Params[3]);
            ushort x = ushort.Parse(Params[1]);
            ushort y = ushort.Parse(Params[2]);

            Map map;
            if (!ServerKernel.Maps.TryGetValue(idMap, out map))
                return false;

            var gItem = new MapItem();
            if (!gItem.Create((uint)map.FloorItem, map, new Point(x, y), idItemtype, 0, 0, 0, 0))
                return false;
            map.AddItem(gItem);
            return true;
        }
        #endregion
        #region 305 - Set Status
        private bool ACTIONMAPSETSTATUS(string msg)
        {
            if (m_pUser == null)
                return false;

            string[] param = GetSafeParam(msg);
            if (param.Length < 3)
                return false;

            var idMap = uint.Parse(param[0]);
            var dwStatus = byte.Parse(param[1]);
            bool flag = param[2] != "0";

            Map temp;
            if (!ServerKernel.Maps.TryGetValue(idMap, out temp))
                return false;

            temp.SetStatus(dwStatus, flag);
            return true;
        }
        #endregion
        #region 306 - Map Attribute
        private bool MAPATTRIBUTE(string param)
        {
            string[] _params = GetSafeParam(param);
            if (_params.Count() < 3) return false;

            string szField = _params[0];
            string szOpt = _params[1];
            int data = int.Parse(_params[2]);
            uint idMap = 0;

            if (_params.Count() >= 4)
                idMap = uint.Parse(_params[3]);

            Map pTaskMap = null;
            if (idMap == 0)
                pTaskMap = m_pUser.Map;
            else
                pTaskMap = ServerKernel.Maps.Values.FirstOrDefault(x => x.Identity == idMap);

            if (pTaskMap == null)
                return false;

            switch (szField)
            {
                case "status":
                    {
                        switch (szOpt)
                        {
                            case "test":
                                return pTaskMap.IsWarTime();
                            case "set":
                                pTaskMap.SetStatus((byte)data, true);// |= (byte)data;
                                return true;
                            case "reset":
                                pTaskMap.SetStatus((byte)data, false);
                                break;
                        }
                        break;
                    }
                case "type":
                    {
                        switch (szOpt)
                        {
                            case "test":
                                return (pTaskMap.Type & data) != 0;
                        }
                        break;
                    }
            }

            return true;
        }
        #endregion
        #region 307 - Map Region Monster
        private bool ACTIONMAPREGIONMONSTER(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);
            if (param.Length < 8)
            {
                ServerKernel.Log.SaveLog(string.Format("ERROR: Invalid param amount on actionid: [{0}]", action.Id), false);
                return false;
            }

            string szOpt = param[6];
            uint idMap = uint.Parse(param[0]);
            uint idType = uint.Parse(param[5]);
            ushort nRegionX = ushort.Parse(param[1]),
                nRegionY = ushort.Parse(param[2]),
                nRegionCX = ushort.Parse(param[3]),
                nRegionCY = ushort.Parse(param[4]);
            int nData = int.Parse(param[7]);

            Map map = null;
            if (idMap == 0)
            {
                if (m_pUser == null)
                {
                    ServerKernel.Log.SaveLog(string.Format("ACTION {0}: Invalid map identity or _pUser class is set null"), false);
                    return false;
                }
                map = m_pUser.Map;
            }
            else
            {
                if (!ServerKernel.Maps.TryGetValue(idMap, out map))
                    return false;
            }

            int nCount = 0;

            foreach (var mst in map.GameObjects.Values.Where(x => x is Monster))
            {
                if (mst.MapX >= nRegionX && mst.MapX < nRegionX - nRegionCX
                    && mst.MapY >= nRegionY && mst.MapY < nRegionY - nRegionCY
                    /*todo check mst type*/)
                    nCount++;
            }

            if (szOpt == "==")
                return nCount == nData;
            if (szOpt == "<")
                return nCount < nData;
            return false;
        }
        #endregion
        #region 310 - Map Change Weather
        private bool ACTIONMAPCHANGEWEATHER(ActionStruct action)
        {
            if (m_pUser == null)
                return false;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 5) return false;

            int nType = int.Parse(param[0]), nIntensity = int.Parse(param[1]), nDir = int.Parse(param[2]);
            uint dwColor = uint.Parse(param[3]), dwKeepSecs = uint.Parse(param[4]);

            // todo
            ServerKernel.Log.SaveLog("ERROR: Missing SetNewWeather() for ACTION_MAP_CHANGEWEATHER on " + action.Id, false);

            return true;
        }
        #endregion
        #region 312 - Map Effect
        private bool ACTIONMAPMAPEFFECT(string msg)
        {
            string[] param = GetSafeParam(msg);

            if (param.Length < 4) return false;

            uint idMap = uint.Parse(param[0]);
            ushort posX = ushort.Parse(param[1]), posY = ushort.Parse(param[2]);
            string szEffect = param[3];

            if (idMap <= 0) return false;

            Map map;
            if (!ServerKernel.Maps.TryGetValue(idMap, out map))
                return false;

            var sPacket = new MsgName { Action = StringAction.MAP_EFFECT, PosX = posX, PosY = posY };
            map.SendToRange(sPacket, posX, posY);
            return true;
        }
        #endregion
        #region 314 - Fireworks
        private bool ACTIONMAPFIREWORKS()
        {
            if (m_pUser == null)
                return false;

            var msg = new MsgItem
            {
                Action = ItemAction.FIREWORKS,
                Identity = m_pUser.Identity
            };
            m_pUser.Screen.Send(msg, true);
            return true;
        }
        #endregion
        #endregion
        #region 400 Item Only
        #region 401 - Spawn Npc
        private bool ACTION_ITEM_REQUESTLAYNPC(ActionStruct action)
        {
            if (m_pUser == null) return false;

            string[] param = GetSafeParam(action.Param);

            if (param.Length < 4)
                return false;

            uint idNextTask = uint.Parse(param[0]);
            uint dwType = uint.Parse(param[1]);
            uint dwSort = uint.Parse(param[2]);
            uint dwLookface = uint.Parse(param[3]);
            uint dwRegion = 0;

            if (param.Length >= 5)
                dwRegion = uint.Parse(param[4]);

            if (idNextTask != 0)
            {
                m_pUser.LastUsedItem = idNextTask;
            }

            MsgNpc pNpc = new MsgNpc(dwRegion, dwLookface, NpcActionType.LAY_NPC, (ushort)dwType);
            m_pUser.Send(pNpc);
            return true;
        }
        #endregion
        #region 403 - Lay Npc
        private bool ACTION_ITEM_LAYNPC(ActionStruct action)
        {
            if (m_pUser.TemporaryString == string.Empty)
                return false;

            string[] pParam = GetSafeParam(m_pUser.TemporaryString);
            if (pParam.Length < 3) return false;

            ushort usMapX = ushort.Parse(pParam[0]), usMapY = ushort.Parse(pParam[1]);
            uint dwLookface = uint.Parse(pParam[2]);
            uint nRegionType = 0;
            uint dwPose = 0, dwFrame = 0;
            if (pParam.Length >= 4)
            {
                dwFrame = uint.Parse(pParam[3]);
                dwPose = uint.Parse(pParam[4]);
            }

            foreach (var obj in m_pUser.Map.GameObjects.Values) // check overlap npc
            {
                if (obj.MapX == usMapX && obj.MapX == usMapY)
                    return false;
            }

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 5) return false;

            string szName = param[0];
            ushort usType = ushort.Parse(param[1]);
            ushort usSort = ushort.Parse(param[2]);
            uint dwOwnerType = uint.Parse(param[4]);
            uint dwLife = 0;
            uint idBase = 0;
            uint idLink = 0;
            uint idTask0 = 0;
            uint idTask1 = 0;
            uint idTask2 = 0;
            uint idTask3 = 0;
            uint idTask4 = 0;
            uint idTask5 = 0;
            uint idTask6 = 0;
            uint idTask7 = 0;
            int idData0 = 0;
            int idData1 = 0;
            int idData2 = 0;
            int idData3 = 0;

            if (param.Length >= 6)
                dwLife = uint.Parse(param[5]);
            if (param.Length >= 7)
                nRegionType = uint.Parse(param[6]);
            if (param.Length >= 8)
                idBase = uint.Parse(param[7]);
            if (param.Length >= 9)
                idLink = uint.Parse(param[8]);
            if (param.Length >= 10)
                idTask0 = uint.Parse(param[9]);
            if (param.Length >= 11)
                idTask1 = uint.Parse(param[10]);
            if (param.Length >= 12)
                idTask2 = uint.Parse(param[11]);
            if (param.Length >= 13)
                idTask3 = uint.Parse(param[12]);
            if (param.Length >= 14)
                idTask4 = uint.Parse(param[13]);
            if (param.Length >= 15)
                idTask5 = uint.Parse(param[14]);
            if (param.Length >= 16)
                idTask6 = uint.Parse(param[15]);
            if (param.Length >= 17)
                idTask7 = uint.Parse(param[16]);
            if (param.Length >= 18)
                idData0 = int.Parse(param[17]);
            if (param.Length >= 19)
                idData1 = int.Parse(param[18]);
            if (param.Length >= 20)
                idData2 = int.Parse(param[19]);
            if (param.Length >= 21)
                idData3 = int.Parse(param[20]);

            if (m_pUser.Map.IsTeleportDisable() && usType == 15)
            {
                m_pUser.Send("You can't place this conductor here.");
                return false;
            }

            if (usType == 9)
            {
                szName = m_pUser.Name;
                dwLookface = m_pUser.Lookface % 10;
                idTask0 = m_pUser.Helmet;
                idTask1 = m_pUser.Armor;
                idTask2 = m_pUser.RightHand;
                idTask3 = m_pUser.LeftHand;
                idTask4 = dwFrame;
                idTask5 = dwPose;
                idTask6 = m_pUser.Lookface;
                idTask7 = ((uint)m_pUser.SyndicateRank << 16) + m_pUser.Hair;
            }
            else
            {

            }

            if (nRegionType > 0 && !m_pUser.Map.QueryRegion((int)nRegionType, usMapX, usMapY))
            {
                return false;
            }


            uint idOwner = 0;

            switch (dwOwnerType)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        idOwner = m_pUser.Identity;
                        break;
                    }
                case 2:
                    {
                        if (m_pUser.Syndicate == null) return false;
                        idOwner = m_pUser.Syndicate.Identity;
                        break;
                    }
            }

            DynamicNpc pNpc = null;// = ServerKernel.GetDynaNpcByIdentity(uint.Parse(param[8]));

            foreach (var map in ServerKernel.Maps.Values)
            {
                foreach (var npc in map.GameObjects.Values)
                {
                    if (npc is DynamicNpc)
                    {
                        DynamicNpc dynaNpc = npc as DynamicNpc;
                        uint nLook = dynaNpc.Lookface - (dynaNpc.Lookface % 10);
                        if (dynaNpc.Task0 == idTask0 && nLook == dwLookface - (dwLookface % 10))
                        {
                            pNpc = dynaNpc;
                            break;
                        }
                    }
                }
            }

            if (pNpc == null)
            {
                DbDynamicNPC dbDyna = new DbDynamicNPC
                {
                    Name = szName,
                    Ownerid = idOwner,
                    Ownertype = dwOwnerType,
                    Type = usType,
                    Sort = usSort,
                    Life = dwLife,
                    Maxlife = dwLife,
                    Base = idBase,
                    Linkid = idLink,
                    Task0 = idTask0,
                    Task1 = idTask1,
                    Task2 = idTask2,
                    Task3 = idTask3,
                    Task4 = idTask4,
                    Task5 = idTask5,
                    Task6 = idTask6,
                    Task7 = idTask7,
                    Data0 = idData0,
                    Data1 = idData1,
                    Data2 = idData2,
                    Data3 = idData3,
                    Datastr = "",
                    Defence = 0,
                    Cellx = usMapX,
                    Celly = usMapY,
                    Idxserver = 0,
                    Itemid = 0,
                    Lookface = 0,
                    MagicDef = 0,
                    Mapid = m_pUser.MapIdentity
                };

                bool save = Database.DynamicNpcRepository.SaveOrUpdate(dbDyna);
                pNpc = new DynamicNpc(dbDyna);
            }

            pNpc.Kind = usType;
            pNpc.OwnerIdentity = idOwner;
            pNpc.OwnerType = (byte)dwOwnerType;
            pNpc.Name = szName;
            pNpc.Lookface = dwLookface;
            pNpc.Sort = usSort;
            pNpc.Task0 = idTask0;
            pNpc.Task1 = idTask1;
            pNpc.Task2 = idTask2;
            pNpc.Task3 = idTask3;
            pNpc.Task4 = idTask4;
            pNpc.Task5 = idTask5;
            pNpc.Task6 = idTask6;
            pNpc.Task7 = idTask7;
            pNpc.Data0 = idData0;
            pNpc.Data1 = idData1;
            pNpc.Data2 = idData2;
            pNpc.Data3 = idData3;
            pNpc.MaxLife = dwLife;
            pNpc.MapX = usMapX;
            pNpc.MapY = usMapY;

            //ServerKernel.Maps[pNpc.MapIdentity].RemoveNpc(pNpc);
            pNpc.RemoveFromMap();
            pNpc.MapIdentity = m_pUser.MapIdentity;
            m_pUser.Map.AddDynaNpc(pNpc);
            pNpc.Save();
            m_pRole = pNpc;
            m_pUser.InteractingNpc = pNpc;
            m_pUser.TemporaryString = string.Empty;
            return true;
        }
        #endregion
        #region 498 - Del this
        private bool ACTIONITEMDELTHIS()
        {
            m_pUser.LastUsedItem = 0;
            if (m_pItem != null)
            {
                m_pUser.Inventory.Remove(m_pItem.Identity);
                m_pItem = null;
            }
            m_pUser.Send(ServerString.STR_USE_ITEM);
            return true;
        }
        #endregion
        #endregion
        #region 500 Item
        #region 501
        private bool USRITEMADD(string param, uint data)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            if (m_pUser.Inventory.RemainingSpace() <= 0)
                return false;

            DbItemtype itemtype;
            if (!ServerKernel.Itemtype.TryGetValue(data, out itemtype))
                return false;
            string[] Params = GetSafeParam(param);
            var newItem = new DbItem
            {
                AddLife = 0,
                AddlevelExp = 0,
                AntiMonster = 0,
                ArtifactExpire = 0,
                ArtifactType = 0,
                ChkSum = 0,
                Color = 3,
                Data = 0,
                Gem1 = 0,
                Gem2 = 0,
                Ident = 0,
                Magic1 = 0,
                Magic2 = 0,
                ReduceDmg = 0,
                Plunder = 0,
                Specialflag = 0,
                Inscribed = 0,
                StackAmount = 1,
                RefineryExpire = 0,
                RefineryLevel = 0,
                RefineryType = 0,
                Type = itemtype.Type,
                Position = 0,
                PlayerId = m_pUser.Identity,
                Monopoly = 0,
                Magic3 = itemtype.Magic3,
                Amount = itemtype.Amount,
                AmountLimit = itemtype.AmountLimit
            };
            for (int i = 0; i < Params.Length; i++)
            {
                uint value = uint.Parse(Params[i]);
                if (value <= 0) continue;

                switch (i)
                {
                    case 0:
                        newItem.Amount = (ushort)value;
                        break;
                    case 1:
                        newItem.AmountLimit = (ushort)value;
                        break;
                    case 2:
                        // Socket Progress
                        newItem.Data = value;
                        break;
                    case 3:
                        if (Enum.IsDefined(typeof(SocketGem), (byte)value))
                            newItem.Gem1 = (byte)value;
                        break;
                    case 4:
                        if (Enum.IsDefined(typeof(SocketGem), (byte)value))
                            newItem.Gem2 = (byte)value;
                        break;
                    case 5:
                        if (Enum.IsDefined(typeof(ItemEffect), (ushort)value))
                            newItem.Magic1 = (byte)value;
                        break;
                    case 6:
                        // magic2.. w/e
                        break;
                    case 7:
                        if (value > 0 && value < 256)
                            newItem.Magic3 = (byte)value;
                        break;
                    case 8:
                        if (value > 0
                            && value < 8)
                            newItem.ReduceDmg = (byte)value;
                        break;
                    case 9:
                        if (value > 0
                            && value < 256)
                            newItem.AddLife = (byte)value;
                        break;
                    case 10:
                        newItem.Plunder = value;
                        break;
                    case 11:
                        if (value == 0)
                            value = 3;
                        if (Enum.IsDefined(typeof(ItemColor), value))
                            newItem.Color = (byte)value;
                        break;
                    case 12:
                        if (value > 0 && value < 256)
                            newItem.Monopoly = (byte)value;
                        break;
                    case 13:
                    case 14:
                    case 15:
                        // R -> For Steeds only
                        // G -> For Steeds only
                        // B -> For Steeds only
                        // G == 8 R == 16
                        newItem.Data = (value | (uint.Parse(Params[14]) << 8) | (uint.Parse(Params[13]) << 16));
                        break;
                }
            }

            return m_pUser.Inventory.Create(newItem);
        }
        #endregion
        #region 502
        private bool DELETEITEM(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            if (action.Data != 0)
                return m_pUser.Inventory.Remove(action.Data, 1);
            if (action.Param != string.Empty)
            {
                Item item = m_pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == action.Param);
                if (item == null)
                    return false;
                return m_pUser.Inventory.Remove(item.Type, 1);
            }
            return false;
        }
        #endregion
        #region 503
        private bool ITEMCHECK(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            if (action.Data != 0)
                return m_pUser.Inventory.Contains(action.Data, 1);
            if (action.Param != string.Empty)
            {
                return m_pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == action.Param) != null;
            }
            return false;
        }
        #endregion
        #region 504
        private bool ITEMHOLE(ActionStruct act)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            string[] param = GetSafeParam(act.Param);
            if (param.Length < 2)
            {
                ServerKernel.Log.SaveLog(
                    string.Format("ACTION: invalid number of parameters (id:{0}, type{1}): ChkHole/MakeHole id", act.Id, act.Type), false, LogType.DEBUG);
                return false;
            }

            if (param[0].ToLower() == "chkhole")
            {
                Item item;
                if (!m_pUser.Equipment.Items.TryGetValue(ItemPosition.RIGHT_HAND, out item))
                {
                    m_pUser.Send(ServerString.STR_ITEM_INEXIST);
                    return false;
                }

                return param[1] == "1" ? item.SocketOne > SocketGem.NO_SOCKET : param[1] == "2" ? item.SocketTwo > SocketGem.NO_SOCKET : false;
            }
            else if (param[0].ToLower() == "makehole")
            {
                Item item;
                if (!m_pUser.Equipment.Items.TryGetValue(ItemPosition.RIGHT_HAND, out item))
                {
                    m_pUser.Send(ServerString.STR_ITEM_INEXIST);
                    return false;
                }

                if (param[1] == "1" && item.SocketOne <= SocketGem.NO_SOCKET)
                {
                    item.SocketOne = SocketGem.EMPTY_SOCKET;
                }
                else if (param[1] == "2" && item.SocketTwo <= SocketGem.NO_SOCKET)
                {
                    item.SocketTwo = SocketGem.EMPTY_SOCKET;
                }
                else
                {
                    return false;
                }
                m_pUser.Send(item.InformationPacket(true));
            }
            else
            {
                ServerKernel.Log.SaveLog(
                    string.Format("ACTION: invalid param data (id: {0})", act.Id), false, LogType.DEBUG);
                return false;
            }

            return true;
        }
        #endregion
        #region 506
        /// <summary>
        /// This method will delete items from the character inventory. When the data is 0,
        /// the param is the the range of items param: min max amount. When the data is the
        /// item identity, it should be a Meteor or DragonBall identity, then the param will
        /// be param: amount monopoly onlymonopoly.
        /// The onlymonopoly is 1 or 0, meaning true or false, if true, the server will look
        /// for the same monopoly listed. Monopoly for now is also true or false, 1 or 0.
        /// Meaning it asks if you want to take of only Bound items.
        /// </summary>
        /// <param name="param">The orders to the server.</param>
        /// <param name="data">DragonBall or Meteor identity.</param>
        /// <returns></returns>
        private bool ITEMMULTIDEL(string param, uint data)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            string[] Params = GetSafeParam(param);

            if (data == SpecialItem.TYPE_METEOR)
            {
                if (Params.Length < 1)
                    return m_pUser.Inventory.ReduceMeteors(1, true, false);

                uint amount0 = uint.Parse(Params[0]);
                if (Params.Length < 2)
                    return m_pUser.Inventory.ReduceMeteors(amount0, true, false);

                uint bound = uint.Parse(Params[1]);
                if (Params.Length < 3)
                    return m_pUser.Inventory.ReduceMeteors(amount0, bound > 0, false);

                uint onlybound = uint.Parse(Params[2]);
                return m_pUser.Inventory.ReduceMeteors(amount0, bound > 0, onlybound > 0);
            }

            if (data == SpecialItem.TYPE_DRAGONBALL)
            {
                if (Params.Length < 1)
                    return m_pUser.Inventory.ReduceDragonBalls(1, true, false);

                uint amount0 = uint.Parse(Params[0]);
                if (Params.Length < 2)
                    return m_pUser.Inventory.ReduceDragonBalls(amount0, true, false);

                uint bound = uint.Parse(Params[1]);
                if (Params.Length < 3)
                    return m_pUser.Inventory.ReduceDragonBalls(amount0, bound > 0, false);

                uint onlybound = uint.Parse(Params[2]);
                return m_pUser.Inventory.ReduceDragonBalls(amount0, bound > 0, onlybound > 0);
            }

            if (data > 0)
            {
                return false;
            }

            if (Params.Length < 3)
                return false;
            uint first = uint.Parse(Params[0]);
            uint last = uint.Parse(Params[1]);
            byte amount = byte.Parse(Params[2]);

            if (Params.Length < 4)
                return m_pUser.Inventory.DeleteMultiple(first, last, amount, true, false);

            bool bbound = uint.Parse(Params[3]) > 0;
            if (Params.Length < 5)
                return m_pUser.Inventory.DeleteMultiple(first, last, amount, bbound, false);

            bool bonlybound = uint.Parse(Params[4]) > 0;
            return m_pUser.Inventory.DeleteMultiple(first, last, amount, bbound, bonlybound);
        }
        #endregion
        #region 507
        private bool ITEMMULTICHECK(string param, uint data)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            string[] Params = GetSafeParam(param);

            if (data == SpecialItem.TYPE_METEOR)
            {
                if (Params.Length < 1)
                    return m_pUser.Inventory.MeteorAmount(true, false) >= 1;

                uint amount0 = uint.Parse(Params[0]);
                if (Params.Length < 2)
                    return m_pUser.Inventory.MeteorAmount(true, false) >= amount0;

                uint bound = uint.Parse(Params[1]);
                if (Params.Length < 3)
                    return m_pUser.Inventory.MeteorAmount(bound > 0, false) >= amount0;

                uint onlybound = uint.Parse(Params[2]);
                return m_pUser.Inventory.MeteorAmount(bound > 0, onlybound > 0) >= amount0;
            }

            if (data == SpecialItem.TYPE_DRAGONBALL)
            {
                if (Params.Length < 1)
                    return m_pUser.Inventory.DragonBallAmount(true, false) >= 1;

                uint amount0 = uint.Parse(Params[0]);
                if (Params.Length < 2)
                    return m_pUser.Inventory.DragonBallAmount(true, false) >= amount0;

                uint bound = uint.Parse(Params[1]);
                if (Params.Length < 3)
                    return m_pUser.Inventory.DragonBallAmount(bound > 0, false) >= amount0;

                uint onlybound = uint.Parse(Params[2]);
                return m_pUser.Inventory.DragonBallAmount(bound > 0, onlybound > 0) >= amount0;
            }

            if (data > 0)
                return false;

            if (Params.Length < 3)
                return false;
            uint first = uint.Parse(Params[0]);
            uint last = uint.Parse(Params[1]);
            byte amount = byte.Parse(Params[2]);
            if (Params.Length < 4)
                return m_pUser.Inventory.ContainsMultiple(first, last, amount);

            uint bbound = uint.Parse(Params[1]);
            if (Params.Length < 3)
                return m_pUser.Inventory.ContainsMultiple(first, last, amount, bbound > 0, false);

            uint bonlybound = uint.Parse(Params[2]);
            return m_pUser.Inventory.ContainsMultiple(first, last, amount, bbound > 0, bonlybound > 0);
        }
        #endregion
        #region 508
        private bool LEAVESPACE(uint data)
        {
            if (m_pUser == null)
                return false;

            return (m_pUser.Inventory.RemainingSpace() >= data);
        }
        #endregion
        #region 509
        private bool ITEMUPEQUIPMENT(string param)
        {
            if (m_pUser == null || m_pUser.Equipment == null)
                return false;

            string[] Params = GetSafeParam(param);
            if (Params.Length < 2)
                return false;
            string szCmd = Params[0];
            byte nPos = byte.Parse(Params[1]);
            switch (szCmd)
            {
                case "up_lev":
                    {
                        Item item;
                        if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition) nPos, out item))
                            return false;
                        return item.UpEquipmentLevel();
                    }
                case "recover_dur":
                    {
                        Item item;
                        if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition)nPos, out item))
                            return false;
                        var szPrice = (uint)item.GetRecoverDurCost();

                        m_pUser.ReduceMoney(szPrice);
                        return item.RecoverDurability();
                    }
                case "up_levultra":
                    {
                        Item item;
                        if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition)nPos, out item))
                            return false;
                        return item.UpUltraEquipmentLevel();
                    }
                case "up_quality":
                    {
                        Item item;
                        if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition)nPos, out item))
                            return false;
                        return item.UpItemQuality();
                    }
                default:
                    ServerKernel.Log.SaveLog(string.Format("ERROR: [509] [0] [{0}] not properly handled.", param), false);
                    return false;
            }
        }
        #endregion
        #region 510
        private bool EQUIPTEST(string param)
        {
            if (m_pUser == null || m_pUser.Equipment == null)
                return false;

            /* param: position type opt value (4 quality == 9) */
            string[] Params = GetSafeParam(param);
            if (Params.Length < 4)
                return false;

            byte nPosition = byte.Parse(Params[0]);
            string szCmd = Params[1];
            string szOpt = Params[2];
            int nData = int.Parse(Params[3]);

            Item item;
            if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition)nPosition, out item))
                return false;

            int nTestData = 0;
            switch (szCmd)
            {
                case "level":
                    nTestData = item.GetLevel();
                    break;
                case "quality":
                    nTestData = item.GetQuality();
                    break;
                case "durability":
                    if (nData == -1)
                        nData = item.MaximumDurability / 100;
                    nTestData = item.MaximumDurability / 100;
                    break;
                case "max_dur":
                    {
                        if (nData == -1)
                            nData = item.Itemtype.AmountLimit / 100;
                        // TODO Kylin Gem Support
                        nTestData = item.MaximumDurability / 100;
                        break;
                    }
                default:
                    ServerKernel.Log.SaveLog(string.Format("ACTION: EQUIPTEST error {0}", szCmd));
                    return false;
            }

            if (szOpt == "==")
                return nTestData == nData;
            if (szOpt == "<=")
                return nTestData <= nData;
            if (szOpt == ">=")
                return nTestData >= nData;
            return false;
        }
        #endregion
        #region 511
        private bool EQUIPEXIST(uint data, string param)
        {
            if (m_pUser == null)
                return false;

            string[] Params = GetSafeParam(param);
            if (param.Length >= 1 && m_pUser.Equipment != null && m_pUser.Equipment.Items.ContainsKey((ItemPosition) data))
                return (m_pUser.Equipment.Items[(ItemPosition)data].Type / 1000) == ushort.Parse(Params[0]);
            return m_pUser.Equipment == null || m_pUser.Equipment.Items.ContainsKey((ItemPosition) data);
        }
        #endregion
        #region 512
        private bool EQUIPCOLOR(string param)
        {
            if (m_pUser == null)
                return false;

            string[] Params = GetSafeParam(param);
            if (Params.Length < 2)
                return false;
            if (!Enum.IsDefined(typeof(ItemColor), byte.Parse(Params[1])))
                return false;

            Item item;
            if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition)byte.Parse(Params[0]), out item))
                return false;

            ItemPosition pos = Calculations.GetItemPosition(item.Type);
            if (pos != ItemPosition.ARMOR
                && pos != ItemPosition.HEADWEAR
                && (pos != ItemPosition.LEFT_HAND || item.GetSort() != ItemSort.ITEMSORT_WEAPON_SHIELD))
                return false;

            item.Color = (ItemColor)byte.Parse(Params[1]);
            item.Save();
            m_pUser.Send(item.InformationPacket(true));
            return true;
        }
        #endregion
        #region 513 - Remove Any

        private bool ACTION_ITEM_REMOVE_ANY(ActionStruct action)
        {
            uint idItem = action.Data;
            DbItemtype itemtype;
            if (!ServerKernel.Itemtype.TryGetValue(idItem, out itemtype))
                return false;

            var allItem = Database.Items.FetchAllByType(idItem);
            if (allItem == null) return true;

            foreach (var item in allItem)
            {
                Client owner;
                if (ServerKernel.Players.TryGetValue(item.PlayerId, out owner))
                {
                    bool stop = false;
                    if (owner.Character.Inventory.Remove(item.Id))
                        continue;
                    foreach (var equip in owner.Character.Equipment.Items.Values)
                    {
                        if (equip.Identity == item.Id)
                        {
                            owner.Character.Equipment.Remove(equip.Position, Equipment.ItemRemoveMethod.DELETE);
                            stop = true;
                            break;
                        }
                    }
                    if (stop) continue;
                    foreach (var wh in owner.Character.Warehouses.Values)
                    {
                        foreach (var hid in wh.Items.Values)
                        {
                            if (hid.Identity == item.Id)
                            {
                                wh.Remove(hid.Identity);
                                hid.Delete();
                                stop = true;
                                break;
                            }
                        }
                        if (stop) break;
                    }
                }
                else
                {
                    Database.Items.Delete(item);
                }
            }
            return true;
        }

        #endregion
        #region 516
        private bool ACTION_ITEM_CHECKRAND(ActionStruct action)
        {
            if (m_pUser.Equipment == null)
                return false;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 6)
                return false;

            byte initValue = byte.Parse(param[3]), endValue = byte.Parse(param[5]);

            List<ItemPosition> lPos = new List<ItemPosition>(15);

            byte pIdx = byte.Parse(param[1]);

            if (initValue == 0 && pIdx == 14)
                initValue = 1;

            for (ItemPosition i = ItemPosition.HEADWEAR; i < (ItemPosition.CROP) + 1; i++)
            {
                if (m_pUser.Equipment.Items.ContainsKey(i))
                {
                    if (pIdx == 14 && m_pUser.Equipment.Items[i].Position == ItemPosition.STEED)
                        continue;

                    switch (pIdx)
                    {
                        case 14:
                            if (m_pUser.Equipment.Items[i].ReduceDamage >= initValue
                                && m_pUser.Equipment.Items[i].ReduceDamage <= endValue)
                                continue;
                            break;
                    }

                    lPos.Add(i);
                }
            }

            byte pos = 0;

            if (lPos.Count > 0)
                pos = (byte) lPos[(ThreadSafeRandom.RandGet(lPos.Count)%lPos.Count)];

            if (pos == 0)
                return false;

            Item item;
            if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition) pos, out item))
                return false;

            byte pPos = byte.Parse(param[0]);
            string opt = param[2];

            switch (pIdx)
            {
                case 14: // bless
                    m_pUser.IterData7 = pos;
                    return true;
                default:
                    ServerKernel.Log.SaveLog("ACTION: 516:" + pIdx + " not handled id:" + action.Id);
                    break;
            }

            return false;
        }
        #endregion
        #region 517
        private bool ITEMMODIFY(ActionStruct action)
        {
            // structure param:
            // pos  type    action  value   update
            // 1    7       ==      1       1
            // pos = Item Position
            // type = 7 Reduce Damage
            // action = Operator == or set
            // value = value lol
            // update = if the client will update live

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 5)
            {
                ServerKernel.Log.SaveLog(string.Format("ACTION: incorrect param, pos type action value update, for action (id:{0})", action.Id), false, LogType.DEBUG);
                return false;
            }

            int pos = int.Parse(param[0]);
            int type = int.Parse(param[1]);
            string opt = param[2];
            long value = int.Parse(param[3]);
            bool update = int.Parse(param[4]) > 0;

            Item item;
            if (!m_pUser.Equipment.Items.TryGetValue((ItemPosition)pos, out item))
            {
                m_pUser.Send(ServerString.STR_ITEM_INEXIST);
                return false;
            }

            switch (type)
            {
                case 1: // itemtype
                    {
                        if (opt == "set")
                        {
                            DbItemtype itemt;
                            if (!ServerKernel.Itemtype.TryGetValue((uint)value, out itemt))
                            {
                                // new item doesnt exist
                                ServerKernel.Log.SaveLog(
                                    string.Format("ACTION: itemtype not found (type:{0}, action:{1})", value, action.Id),
                                    true, LogType.DEBUG);
                                return false;
                            }

                            if (item.Type / 1000 != itemt.Type / 1000)
                            {
                                ServerKernel.Log.SaveLog(
                                    string.Format("ACTION: cant change to different type (type:{0}, new:{1}, action:{2})",
                                        item.Type, value, action.Id), true, LogType.DEBUG);
                                return false;
                            }

                            if (!item.ChangeType(itemt.Type))
                                return false;
                        }
                        else if (opt == "==")
                        {
                            return item.Type == value;
                        }
                        else if (opt == "<")
                        {
                            return item.Type < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 2: // owner id
                case 3: // player id
                    return false;
                case 4: // dura
                    {
                        if (opt == "set")
                        {
                            if (value > ushort.MaxValue)
                                value = ushort.MaxValue;
                            else if (value < 0)
                                value = 0;

                            item.Durability = (ushort)value;
                        }
                        else if (opt == "==")
                        {
                            return item.Durability == value;
                        }
                        else if (opt == "<")
                        {
                            return item.Durability < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 5: // max dura
                    {
                        if (opt == "set")
                        {
                            if (value > ushort.MaxValue)
                                value = ushort.MaxValue;
                            else if (value < 0)
                                value = 0;

                            if (value < item.Durability)
                                item.Durability = (ushort)value;

                            item.MaximumDurability = (ushort)value;
                        }
                        else if (opt == "==")
                        {
                            return item.MaximumDurability == value;
                        }
                        else if (opt == "<")
                        {
                            return item.MaximumDurability < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 6:
                case 7: // position
                    {
                        return false;
                    }
                case 8: // gem1
                    {
                        if (opt == "set")
                        {
                            item.SocketOne = (SocketGem)value;
                        }
                        else if (opt == "==")
                        {
                            return item.SocketOne == (SocketGem)value;
                        }
                        else if (opt == "<")
                        {
                            return item.SocketOne < (SocketGem)value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 9: // gem2
                    {
                        if (opt == "set")
                        {
                            item.SocketTwo = (SocketGem)value;
                        }
                        else if (opt == "==")
                        {
                            return item.SocketTwo == (SocketGem)value;
                        }
                        else if (opt == "<")
                        {
                            return item.SocketTwo < (SocketGem)value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 10: // magic1
                    {
                        if (opt == "set")
                        {
                            if (value < 200 || value > 203)
                                return false;
                            item.Effect = (ItemEffect)value;
                        }
                        else if (opt == "==")
                        {
                            return item.Effect == (ItemEffect)value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 11: // magic2
                    return false;
                case 12: // magic3
                    {
                        if (opt == "set")
                        {
                            if (value < 0)
                                value = 0;
                            else if (value > 12)
                                value = 12;

                            item.Plus = (byte)value;
                        }
                        else if (opt == "==")
                        {
                            return item.Plus == value;
                        }
                        else if (opt == "<")
                        {
                            return item.Plus < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 13: // data
                    {
                        if (opt == "set")
                        {
                            if (value < 0)
                                value = 0;
                            else if (value > 20000)
                                value = 20000;

                            item.SocketProgress = (ushort)value;
                        }
                        else if (opt == "==")
                        {
                            return item.SocketProgress == value;
                        }
                        else if (opt == "<")
                        {
                            return item.SocketProgress < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 14: // reduce damage
                    {
                        if (opt == "set")
                        {
                            if (value < 0)
                                value = 0;
                            else if (value > 7)
                                value = 7;

                            item.ReduceDamage = (byte)value;
                        }
                        else if (opt == "==")
                        {
                            return item.ReduceDamage == value;
                        }
                        else if (opt == "<")
                        {
                            return item.ReduceDamage < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 15: // add life
                    {
                        if (opt == "set")
                        {
                            if (value < 0)
                                value = 0;
                            else if (value > 255)
                                value = 255;

                            item.Enchantment = (byte)value;
                        }
                        else if (opt == "==")
                        {
                            return item.Enchantment == value;
                        }
                        else if (opt == "<")
                        {
                            return item.Enchantment < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 16: // anti monster
                case 17: // chk sum
                case 18: // plunder
                case 19: // special flag
                    return false;
                case 20: // color
                    {
                        if (opt == "set")
                        {
                            if (!Enum.IsDefined(typeof(ItemColor), value))
                                return false;

                            item.Color = (ItemColor)value;
                        }
                        else if (opt == "==")
                        {
                            return item.Color == (ItemColor)value;
                        }
                        else if (opt == "<")
                        {
                            return item.Color < (ItemColor)value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 21: // add lev exp
                    {
                        if (opt == "set")
                        {
                            if (value < 0)
                                value = 0;
                            if (value > ushort.MaxValue)
                                value = ushort.MaxValue;

                            item.CompositionProgress = (ushort)value;
                        }
                        else if (opt == "==")
                        {
                            return item.CompositionProgress == value;
                        }
                        else if (opt == "<")
                        {
                            return item.CompositionProgress < value;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                case 22: // monopoly
                case 23: // inscribed
                case 24: // artifact type
                case 25: // artifact start
                case 26: // artifact expire
                case 27: // artifact stabilization
                case 28: // refinery type
                case 29: // refinery level
                    return false;
                default:
                    return false;
            }

            if (update)
                m_pUser.Send(item.InformationPacket(true));
            return true;
        }
        #endregion
        #region 528 - Jar Create
        private bool ACTION_ITEM_JAR_CREATE(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            if (m_pUser.Inventory.RemainingSpace() <= 0)
                return false;

            DbItemtype itemtype;
            if (!ServerKernel.Itemtype.TryGetValue(action.Data, out itemtype))
                return false;
            string[] Params = GetSafeParam(action.Param);
            var newItem = new DbItem
            {
                AddLife = 0,
                AddlevelExp = 0,
                AntiMonster = 0,
                ArtifactExpire = 0,
                ArtifactType = 0,
                ChkSum = 0,
                Color = 3,
                Data = 0,
                Gem1 = 0,
                Gem2 = 0,
                Ident = 0,
                Magic1 = 0,
                Magic2 = 0,
                ReduceDmg = 0,
                Plunder = 0,
                Specialflag = 0,
                Inscribed = 0,
                StackAmount = 1,
                RefineryExpire = 0,
                RefineryLevel = 0,
                RefineryType = 0,
                Type = itemtype.Type,
                Position = 0,
                PlayerId = m_pUser.Identity,
                Monopoly = 0,
                Magic3 = itemtype.Magic3,
                Amount = itemtype.Amount,
                AmountLimit = itemtype.AmountLimit
            };
            for (int i = 0; i < Params.Length; i++)
            {
                uint value = uint.Parse(Params[i]);
                if (value <= 0) continue;

                switch (i)
                {
                    case 0:
                        newItem.Amount = (ushort)value;
                        break;
                    case 1:
                        newItem.AmountLimit = (ushort)value;
                        break;
                    case 2:
                        // Socket Progress
                        newItem.Data = value;
                        break;
                    case 3:
                        if (Enum.IsDefined(typeof(SocketGem), (byte)value))
                            newItem.Gem1 = (byte)value;
                        break;
                    case 4:
                        if (Enum.IsDefined(typeof(SocketGem), (byte)value))
                            newItem.Gem2 = (byte)value;
                        break;
                    case 5:
                        if (Enum.IsDefined(typeof(ItemEffect), (ushort)value))
                            newItem.Magic1 = (byte)value;
                        break;
                    case 6:
                        // magic2.. w/e
                        break;
                    case 7:
                        if (value > 0 && value < 256)
                            newItem.Magic3 = (byte)value;
                        break;
                    case 8:
                        if (value > 0
                            && value < 8)
                            newItem.ReduceDmg = (byte)value;
                        break;
                    case 9:
                        if (value > 0
                            && value < 256)
                            newItem.AddLife = (byte)value;
                        break;
                    case 10:
                        newItem.Plunder = value;
                        break;
                    case 11:
                        if (value == 0)
                            value = 3;
                        if (Enum.IsDefined(typeof(ItemColor), value))
                            newItem.Color = (byte)value;
                        break;
                    case 12:
                        if (value > 0 && value < 256)
                            newItem.Monopoly = (byte)value;
                        break;
                    case 13:
                    case 14:
                    case 15:
                        // R -> For Steeds only
                        // G -> For Steeds only
                        // B -> For Steeds only
                        // G == 8 R == 16
                        newItem.Data = (value | (uint.Parse(Params[14]) << 8) | (uint.Parse(Params[13]) << 16));
                        break;
                }
            }

            m_pUser.Inventory.Create(newItem);

            MsgInteract pMsg = new MsgInteract
            {
                EntityIdentity = m_pUser.Identity,
                Amount = m_pUser.GetStatisticValue(6, 12)
            };
            m_pUser.Send(pMsg);
            return true;
        }
        #endregion
        #region 529 - Jar Verify
        private bool ACTION_ITEM_JAR_VERIFY(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Inventory == null)
                return false;

            if (m_pUser.Inventory.RemainingSpace() <= 0)
                return false;

            string[] param = GetSafeParam(action.Param);

            if (param.Length < 2) return false;

            uint amount = uint.Parse(param[1]);
            uint monster = uint.Parse(param[0]);

            if (m_pUser.GetStatisticValue(6, 5) != monster)
                return false;

            Item jar = null;
            foreach (var item in m_pUser.Inventory.Items.Values.Where(x => x.Type == action.Data))
            {
                if (item.MaximumDurability == monster)
                {
                    jar = item;
                    break;
                }
            }
            return jar != null && jar.Durability < amount;
        }
        #endregion
        #endregion
        #region 700 Syndicate
        #region 701
        private bool CREATESYNDICATE(string param, string szAccept)
        {
            string[] _params = GetSafeParam(param);

            if (m_pUser == null || m_pUser == null || m_pUser.Syndicate != null)
                return false;

            if (m_pUser.Level < int.Parse(_params[0]))
            {
                m_pUser.Send(string.Format(ServerString.STR_SYN_CREATE_LEVEL_NOT_ENOUGH, _params[0])); // _params[0]
                return false;
            }

            if (m_pUser.Silver < uint.Parse(_params[1]))
            {
                m_pUser.Send(string.Format(ServerString.STR_SYN_CREATE_MONEY_NOT_ENOUGH, _params[1])); // _params[1]
                return false;
            }

            m_pUser.CreateSyndicate(szAccept, uint.Parse(_params[1]));
            return true;
        }
        #endregion
        #region 702
        private bool ACTIONSYNDESTROY()
        {
            if (m_pUser == null || m_pUser == null || m_pUser.Syndicate == null)
                return false;

            if (m_pUser.Syndicate == null || m_pUser.SyndicateMember == null)
            {
                m_pUser.Send(ServerString.STR_NO_SYN_DISBAND);
                return false;
            }

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NO_DISBAND_LEADER);
                return false;
            }

            m_pUser.DisbandSyndicate();
            return true;
        }
        #endregion
        #region 705
        private bool ACTIONSYNSETASSISTANT(string szName)
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
            {
                return false;
            }

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NOT_AUTHORIZED);
                return false;
            }

            SyndicateMember pMember = m_pUser.Syndicate.Members.Values.FirstOrDefault(x => x.Name == szName);
            if (pMember == null) return false;

            return m_pUser.Syndicate.PromoteMember(m_pUser, pMember, SyndicateRank.DEPUTY_LEADER);
        }
        #endregion
        #region 706
        private bool ACTIONSYNCLEARRANK(string szName)
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
                return false;

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NOT_AUTHORIZED);
                return false;
            }

            SyndicateMember pMember = m_pUser.Syndicate.Members.Values.FirstOrDefault(x => x.Name == szName);
            if (pMember == null) return false;

            return m_pUser.Syndicate.DischargeMember(m_pUser, pMember);
        }
        #endregion
        #region 709
        private bool ACTIONSYNCHANGELEADER(string szName)
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
                return false;

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NOT_AUTHORIZED);
                return false;
            }

            return m_pUser.Syndicate.AbdicateSyndicate(m_pUser.SyndicateMember, szName);
        }
        #endregion
        #region 711
        private bool ACTIONSYNANTAGONIZE(string szName)
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
                return false;

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NOT_AUTHORIZED);
                return false;
            }

            return m_pUser.Syndicate.AntagonizeSyndicate(szName);
        }
        #endregion
        #region 712
        private bool ACTIONSYNCLEARANTAGONIZE(string szName)
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
                return false;

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NOT_AUTHORIZED);
                return false;
            }

            Syndicate syn = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == szName);
            if (syn == null)
            {
                m_pUser.Send(ServerString.STR_GUILD_INEXIST);
                return false;
            }

            return m_pUser.Syndicate.RemoveEnemy((ushort)syn.Identity);
        }
        #endregion
        #region 713
        private bool ACTIONSYNALLY()
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
                return false;

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NOT_AUTHORIZED);
                return false;
            }

            if (m_pUser.Team == null)
            {
                m_pUser.Send(ServerString.STR_NO_TEAM_JOINED);
                return false;
            }

            if (m_pUser.Team.MembersCount() > 2)
            {
                m_pUser.Send(ServerString.STR_GUILD_ALLY_TEAM_COUNT);
                return false;
            }

            Syndicate target = (from plr in m_pUser.Team.Members.Values where plr.SyndicateRank == SyndicateRank.GUILD_LEADER select plr.Syndicate).FirstOrDefault();
            return target != null && m_pUser.Syndicate.AllySyndicate(target);
        }
        #endregion
        #region 714
        private bool ACTIONSYNCLEARALLY(string szName)
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
                return false;

            if (m_pUser.SyndicateRank != SyndicateRank.GUILD_LEADER)
            {
                m_pUser.Send(ServerString.STR_NOT_AUTHORIZED);
                return false;
            }

            Syndicate syn = ServerKernel.Syndicates.Values.FirstOrDefault(x => x.Name == szName);
            if (syn == null)
            {
                m_pUser.Send(ServerString.STR_GUILD_INEXIST);
                return false;
            }

            return m_pUser.Syndicate.RemoveAlliance((ushort)syn.Identity);
        }
        #endregion
        #region 717
        private bool ACTIONSYNATTR(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Syndicate == null)
                return false;

            string[] param = GetSafeParam(action.Param);

            if (param.Length < 3)
                return false;

            string szField = param[0], szOpt = param[1];
            int nData = int.Parse(param[2]);
            Syndicate syn;
            if (param.Length >= 4 && !ServerKernel.Syndicates.TryGetValue(uint.Parse(param[3]), out syn))
                return false;
            if (param.Length < 4 && m_pUser != null && m_pUser != null && m_pUser.Syndicate != null)
                syn = m_pUser.Syndicate;
            else return false;

            if (syn == null)
                return false;

            switch (szField)
            {
                case "money":
                    switch (szOpt)
                    {
                        case "+=":
                            return syn.ChangeFunds(nData);
                        case "<":
                            return syn.SilverDonation < (uint)nData;
                    }
                    break;
                case "emoney":
                    switch (szOpt)
                    {
                        case "+=":
                            return syn.ChangeEmoneyFunds(nData);
                        case "<":
                            return syn.EmoneyDonation < (uint)nData;
                    }
                    break;
                case "membernum":
                    switch (szOpt)
                    {
                        case "<":
                            return syn.MemberCount < (uint)nData;
                    }
                    break;
                case "level":
                    switch (szOpt)
                    {
                        case "==":
                            return syn.Level == nData;
                        case "<":
                            return syn.Level < nData;
                    }
                    break;
            }
            return false;
        }
        #endregion
        #endregion
        #region 800 Monster
        #region 801 - Monster drop item
        private bool ACTIONMSGDROPITEM(ActionStruct action, Monster pMonster)
        {
            if (action == null) return false;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 2) return false;

            string type = param[0];
            uint data = uint.Parse(param[1]);

            int percent = 100;

            if (param.Length >= 3)
                percent = int.Parse(param[2]);

            switch (type)
            {
                case "dropitem":
                {
                    uint idUser = 0;
                    if (m_pUser != null)
                        idUser = m_pUser.Identity;
                    return pMonster.DropItem(data, idUser, 0, 0, 0, 0);
                }
                case "dropmoney":
                    {
                        percent %= 100;
                        uint dwMoneyDrop = (uint)(data * (percent + Calculations.Random.Next(100 - percent)) / 100);
                        if (dwMoneyDrop <= 0)
                            return false;
                        uint idUser = 0;
                        if (m_pUser != null)
                            idUser = m_pUser.Identity;
                        return pMonster.DropMoney(data, idUser);
                    }
            }

            return true;
        }
        #endregion
        #region 803 - Monster Drop Refinery
        private bool ACTIONMSTREFINERY(ActionStruct action, Monster pMonster)
        {
            if (pMonster == null)
                return false;

            byte refLevel = (byte) (action.Data%5);

            if (refLevel == 0)
                refLevel = (byte) Math.Max(ThreadSafeRandom.RandGet(0, 6)%6, 1);

            List<DbRefinery> pDrop = ServerKernel.Refineries.Values.Where(x => x.Level == refLevel).ToList();
            List<uint> remove = new List<uint>();

            foreach (var pRef in pDrop)
            {
                DbItemtype itemtype;
                if (!ServerKernel.Itemtype.TryGetValue(pRef.Id, out itemtype))
                {
                    remove.Add(pRef.Id);
                    continue;
                }
                if (itemtype.Monopoly == 9 || itemtype.Name.Contains("(B)"))
                {
                    remove.Add(pRef.Id);
                    continue;
                }
            }

            foreach (var rem in remove)
            {
                pDrop.RemoveAll(x => x.Id == rem);
            }

            DbRefinery refinery = null;
            try
            {
                refinery = pDrop[ThreadSafeRandom.RandGet()%pDrop.Count];
            }
            catch
            {
                return false;
            }

            if (refinery == null)
                return false;

            uint idUser = 0u;
            if (m_pUser != null)
                idUser = m_pUser.Identity;

            return pMonster.DropItem(refinery.Id, idUser, 0, 0, 0, 0);
        }
        #endregion
        #endregion
        #region 900 Family
        #region 901 - Family Create
        private bool ACTION_FAMILY_CREATE(ActionStruct action, string szAccept)
        {
            if (m_pUser == null)
                return false;

            if (m_pUser.Family != null)
            {
                m_pUser.Send(ServerString.STR_FAMILY_ALREADY_JOINED);
                return false;
            }

            uint dwPrice = 500000;
            uint dwDonation = 250000;
            byte pLev = 50;

            string[] pParam = GetSafeParam(action.Param);
            if (pParam.Length > 0)
                pLev = byte.Parse(pParam[0]);
            if (pParam.Length > 1)
                dwPrice = uint.Parse(pParam[1]);
            if (pParam.Length > 2)
                dwDonation = uint.Parse(pParam[2]);

            if (m_pUser.Level < 50)
            {
                m_pUser.Send(ServerString.STR_FAMILY_CREATE_LOW_LEVEL);
                return false;
            }

            if (!Handlers.CheckName(szAccept))
            {
                m_pUser.Send(ServerString.STR_FAMILY_INVALID_NAME);
                return false;
            }

            if (m_pUser.Silver < dwPrice)
            {
                m_pUser.Send(ServerString.STR_NOT_ENOUGH_MONEY);
                return false;
            }

            return m_pUser.CreateFamily(szAccept, dwPrice, dwDonation);
        }
        #endregion
        #region 902 - Family Disband
        private bool ACTION_FAMILY_DESTROY(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Family == null || m_pUser.FamilyMember == null)
                return false;
            return m_pUser.DisbandFamily();
        }
        #endregion
        #region 917 - Family Attribute
        private bool ACTION_FAMILY_ATTR(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Family == null)
                return false;

            string[] param = GetSafeParam(action.Param);

            if (param.Length < 3)
                return false;

            string szField = param[0], szOpt = param[1];
            int nData = int.Parse(param[2]);
            Family syn;
            if (param.Length >= 4 && !ServerKernel.Families.TryGetValue(uint.Parse(param[3]), out syn))
                return false;
            if (param.Length < 4 && m_pUser != null && m_pUser != null && m_pUser.Family != null)
                syn = m_pUser.Family;
            else return false;

            if (syn == null)
                return false;

            switch (szField)
            {
                case "money":
                    switch (szOpt)
                    {
                        case "+=":
                            return syn.ChangeFunds(nData);
                        case "<":
                            return syn.MoneyFunds < (uint)nData;
                    }
                    break;
                case "membernum":
                    switch (szOpt)
                    {
                        case "<":
                            return syn.MembersCount < (uint)nData;
                    }
                    break;
                case "level":
                    switch (szOpt)
                    {
                        case "==":
                            return syn.Level == nData;
                        case "<":
                            return syn.Level < nData;
                    }
                    break;
                case "bptower":
                    switch (szOpt)
                    {
                        case "==":
                            return syn.BpTower == nData;
                        case "<":
                            return syn.BpTower < nData;
                    }
                    break;
            }
            return false;
        }
        #endregion
        #region 918 - Family Increase Level
        private bool ACTION_FAMILY_UPLEV(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Family == null)
                return false;

            if (m_pUser.Family.Level >= 5)
                return false; // max level

            m_pUser.Family.Level += 1;
            m_pUser.Family.SendFamily(m_pUser);
            return true;
        }
        #endregion
        #region 919 - Family Increase BP Tower
        private bool ACTION_FAMILY_BPUPLEV(ActionStruct action)
        {
            if (m_pUser == null || m_pUser.Family == null)
                return false;

            if (m_pUser.Family.BpTower >= 4 || m_pUser.Family.BpTower >= m_pUser.Family.Level)
                return false; // max level

            m_pUser.Family.BpTower += 1;
            m_pUser.Family.SendFamily(m_pUser);
            return true;
        }
        #endregion
        #endregion
        #region 1000 User
        #region 1001 - User Attributes
        private bool USERATTR(string param)
        {
            if (m_pUser == null)
                return false;

            string[] Params = GetSafeParam(param);
            string type, opt, value, last = "";
            if (Params.Length < 3)
                return false;
            if (Params.Length > 3)
                last = Params[3];
            type = Params[0];
            opt = Params[1];
            value = Params[2];

            switch (type)
            {
                #region force (+=, ==, <)
                case "force":
                    {
                        switch (opt)
                        {
                            case "+=":
                                return m_pUser.ChangeForce(short.Parse(value));
                            case "==":
                                return m_pUser.Strength == ushort.Parse(value);
                            case "<":
                                return m_pUser.Strength < ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region speed (+=, ==, <)
                case "speed":
                    {
                        switch (opt)
                        {
                            case "+=":
                                return m_pUser.ChangeSpeed(short.Parse(value));
                            case "==":
                                return m_pUser.Agility == ushort.Parse(value);
                            case "<":
                                return m_pUser.Agility < ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region health (+=, ==, <)
                case "health":
                    {
                        switch (opt)
                        {
                            case "+=":
                                return m_pUser.ChangeHealth(short.Parse(value));
                            case "==":
                                return m_pUser.Vitality == ushort.Parse(value);
                            case "<":
                                return m_pUser.Vitality < ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region soul (+=, ==, <)
                case "soul":
                    {
                        switch (opt)
                        {
                            case "+=":
                                return m_pUser.ChangeSoul(short.Parse(value));
                            case "==":
                                return m_pUser.Spirit == ushort.Parse(value);
                            case "<":
                                return m_pUser.Spirit < ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region metempsychosis (==, <)
                case "metempsychosis":
                    {
                        switch (opt)
                        {
                            case "==":
                                return m_pUser.Metempsychosis == byte.Parse(value);
                            case "<":
                                return m_pUser.Metempsychosis < byte.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region nobility_rank (==, <)
                case "nobility":
                case "nobility_rank":
                    {
                        switch (opt)
                        {
                            case "==":
                                return m_pUser.Nobility.Level == (NobilityLevel) ushort.Parse(value);
                            case "<":
                                return m_pUser.Nobility.Level == (NobilityLevel) ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region level (+=, ==, <)
                case "level":
                    {
                        switch (opt)
                        {
                            case "+=":
                                {
                                    byte hue = byte.Parse(value);
                                    return m_pUser.AwardLevel(hue);
                                }
                            case "==":
                                return m_pUser.Level == ushort.Parse(value);
                            case "<":
                                return m_pUser.Level < ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region money (+=, ==, <)
                case "money":
                    {
                        switch (opt)
                        {
                            case "+=":
                                return m_pUser.ChangeMoney(long.Parse(value));
                            case "==":
                                return m_pUser.Silver == uint.Parse(value);
                            case "<":
                                return m_pUser.Silver < uint.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region e_money (+=, ==, <)
                case "e_money":
                    {
                        switch (opt)
                        {
                            case "+=":
                                return m_pUser.ChangeEmoney(long.Parse(value));
                            case "==":
                                return m_pUser.Emoney == uint.Parse(value);
                            case "<":
                                return m_pUser.Emoney < uint.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region profession (==, >=, <=, set)
                case "profession":
                    {
                        switch (opt)
                        {
                            case "==":
                                return m_pUser.Profession == ushort.Parse(value);
                            case ">=":
                                return m_pUser.Profession >= ushort.Parse(value);
                            case "<":
                                return m_pUser.Profession < ushort.Parse(value);
                            case "<=":
                                return m_pUser.Profession <= ushort.Parse(value);
                            case "set":
                                return m_pUser.SetProfession(ushort.Parse(value));
                        }
                        return false;
                    }
                #endregion
                #region First Profession (==, >=, <=)
                case "first_profession":
                    {
                        switch (opt)
                        {
                            case "==":
                                return m_pUser.FirstProfession == ushort.Parse(value);
                            case ">=":
                                return m_pUser.FirstProfession >= ushort.Parse(value);
                            case "<=":
                                return m_pUser.FirstProfession <= ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region Last Profession (==, >=, <=)
                case "last_profession":
                    {
                        switch (opt)
                        {
                            case "==":
                                return m_pUser.LastProfession == ushort.Parse(value);
                            case ">=":
                                return m_pUser.LastProfession >= ushort.Parse(value);
                            case "<=":
                                return m_pUser.LastProfession <= ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region pk (+=, ==, <)
                case "pk":
                    {
                        switch (opt)
                        {
                            case "==":
                                return m_pUser.PkPoints == ushort.Parse(value);
                            case "<":
                                return m_pUser.PkPoints < ushort.Parse(value);
                            case "+=":
                                return m_pUser.ChangePkPoints(int.Parse(value));
                        }
                        return false;
                    }
                #endregion
                #region exp (+=, ==, <)
                case "exp":
                    {
                        switch (opt)
                        {
                            case "+=":
                                long exp = long.Parse(value);
                                if (exp < 0)
                                    exp = 0;
                                return m_pUser.AwardExperience(exp, last == "nocontribute", true);
                            case "==":
                                return m_pUser.Experience == long.Parse(value);
                            case "<":
                                return m_pUser.Experience < long.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region vip (<, ==)
                case "vip":
                    {
                        switch (opt)
                        {
                            case "<":
                                return m_pUser.Owner.VipLevel < byte.Parse(value);
                            case "==":
                                return m_pUser.Owner.VipLevel == byte.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region iterator (=)
                case "iterator":
                    {
                        switch (opt)
                        {
                            case "=":
                                {
                                    m_pUser.Iterator = ulong.Parse(value);
                                    return true;
                                }
                        }
                        return false;
                    }
                #endregion
                #region rankshow (<, ==)
                case "rankshow":
                    {
                        switch (opt)
                        {
                            case ">":
                                return (ushort)m_pUser.SyndicateRank > ushort.Parse(value);
                            case "<":
                                return (ushort)m_pUser.SyndicateRank < ushort.Parse(value);
                            case "==":
                                return (ushort)m_pUser.SyndicateRank == ushort.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region crime (==, set)
                case "crime":
                    {
                        switch (opt)
                        {
                            case "==":
                                {
                                    if (value == "0")
                                        return m_pUser.QueryStatus(FlagInt.BLUE_NAME) == null;
                                    return m_pUser.QueryStatus(FlagInt.BLUE_NAME) != null;
                                }
                            case "set":
                                {
                                    if (value == "0")
                                    {
                                        m_pUser.DetachStatus(FlagInt.BLUE_NAME);
                                    }
                                    else
                                    {
                                        m_pUser.SetCrimeStatus(60);
                                    }
                                    return true;
                                }
                        }
                        return false;
                    }
                #endregion
                #region ep (+=, <, >, ==)
                case "ep":
                    {
                        switch (opt)
                        {
                            case "+=":
                                {
                                    return m_pUser.ChangeEp(sbyte.Parse(value));
                                }
                            case "<":
                                {
                                    return m_pUser.Stamina < sbyte.Parse(value);
                                }
                            case ">":
                                {
                                    return m_pUser.Stamina > sbyte.Parse(value);
                                }
                            case "==":
                                {
                                    return m_pUser.Stamina == sbyte.Parse(value);
                                }
                        }
                        return false;
                    }
                #endregion
                #region Attribute Points (+=)
                case "attr_points":
                    {
                        switch (opt)
                        {
                            case "+=":
                                return m_pUser.AwardAttributePoints(Convert.ToInt32(value));
                        }
                        return false;
                    }
                #endregion
                #region Virtue
                case "virtue":
                    switch (opt)
                    {
                        case "==":
                            return m_pUser.VirtuePoints == uint.Parse(value);
                        case "<":
                            return m_pUser.VirtuePoints < uint.Parse(value);
                        case "<=":
                            return m_pUser.VirtuePoints <= uint.Parse(value);
                        case ">":
                            return m_pUser.VirtuePoints > uint.Parse(value);
                        case ">=":
                            return m_pUser.VirtuePoints >= uint.Parse(value);
                        case "!=":
                        case "<>":
                            return m_pUser.VirtuePoints != uint.Parse(value);
                        case "+=":
                            {
                                int nVal = int.Parse(value);
                                if (nVal < 0)
                                {
                                    nVal *= -1;
                                    if (m_pUser.VirtuePoints - nVal < 0)
                                        return false;
                                    m_pUser.VirtuePoints -= (uint)nVal;
                                    return true;
                                }
                                if (nVal > 0)
                                {
                                    if (nVal + m_pUser.VirtuePoints > int.MaxValue)
                                        m_pUser.VirtuePoints = int.MaxValue;
                                    else
                                    {
                                        m_pUser.VirtuePoints += (uint)nVal;
                                    }
                                    return true;
                                }
                                return false;
                            }
                    }
                    return false;
                #endregion
                #region transformation
                case "transformation":
                    {
                        switch (opt)
                        {
                            case "==":
                                return m_pUser.QueryTransformation.Lookface == uint.Parse(value);
                            case "<>":
                            case "!=":
                                return m_pUser.QueryTransformation.Lookface != uint.Parse(value);
                        }
                        return false;
                    }
                #endregion
                #region study_points
                case "study":
                case "study_points":
                {
                    switch (opt)
                    {
                        case "+=":
                        {
                            return m_pUser.ChangeStudyPoints(int.Parse(value));
                        }
                        case "<=": return m_pUser.StudyPoints <= int.Parse(value);
                        case "<":
                        {
                            return m_pUser.StudyPoints < int.Parse(value);
                        }
                        case ">=": return m_pUser.StudyPoints >= int.Parse(value);
                        case ">":
                        {
                            return m_pUser.StudyPoints > int.Parse(value);
                        }
                        case "==":
                        {
                            return m_pUser.StudyPoints == int.Parse(value);
                        }
                    }
                    return false;
                }
                #endregion
                #region family_rank
                case "family_rank":
                {
                    switch (opt)
                    {
                        case "<":
                            return (ushort)m_pUser.FamilyPosition < ushort.Parse(value);
                        case "==":
                            return (ushort)m_pUser.FamilyPosition == ushort.Parse(value);
                    }
                    return false;
                }
                #endregion
                #region transform
                case "transform":
                {
                    switch (opt)
                    {
                        case "==": return m_pUser.Transformation == ushort.Parse(value);
                    }
                    return false;
                }
                #endregion
                #region look
                case "look":
                {
                    switch (opt)
                    {
                        case "==": return (m_pUser.Lookface % 10) == ushort.Parse(value);
                        case "set":
                            {
                                ushort usVal = ushort.Parse(value);
                                if (m_pUser.Gender == 1 && (usVal == 3 || usVal == 4))
                                {
                                    m_pUser.Body = (ushort)(1000 + usVal);
                                    return true;
                                }
                                else if (m_pUser.Gender == 0 && (usVal == 1 || usVal == 2))
                                {
                                    m_pUser.Body = (ushort)(2000 + usVal);
                                    return true;
                                }
                                return false;
                            }
                    }
                    return false;
                }
                #endregion
                #region body
                case "body":
                {
                    switch (opt)
                    {
                        case "set":
                        {
                            ushort usNewBody = ushort.Parse(value);
                            if (usNewBody == 1003 || usNewBody == 1004)
                            {
                                if (m_pUser.Body != 2001 && m_pUser.Body != 2002)
                                    return false; // to change body use the fucking item , asshole

                                if (m_pUser.Equipment.Items.ContainsKey(ItemPosition.GARMENT))
                                    m_pUser.Equipment.Remove(ItemPosition.GARMENT, Equipment.ItemRemoveMethod.REMOVE_TO_INVENTORY);
                                if (m_pUser.Equipment.Items.ContainsKey(ItemPosition.ALT_GARMENT))
                                    m_pUser.Equipment.Remove(ItemPosition.ALT_GARMENT, Equipment.ItemRemoveMethod.REMOVE_TO_INVENTORY);

                                m_pUser.Body = usNewBody;
                                return true;
                            }
                            if (usNewBody == 2001 || usNewBody == 2002)
                            {
                                if (m_pUser.Body != 1003 && m_pUser.Body != 1004)
                                    return false; // to change body use the fucking item , asshole

                                if (m_pUser.Equipment.Items.ContainsKey(ItemPosition.GARMENT))
                                    m_pUser.Equipment.Remove(ItemPosition.GARMENT, Equipment.ItemRemoveMethod.REMOVE_TO_INVENTORY);
                                if (m_pUser.Equipment.Items.ContainsKey(ItemPosition.ALT_GARMENT))
                                    m_pUser.Equipment.Remove(ItemPosition.ALT_GARMENT, Equipment.ItemRemoveMethod.REMOVE_TO_INVENTORY);

                                m_pUser.Body = usNewBody;
                                return true;
                            }
                            return false;
                        }
                    }
                    return false;
                }
                #endregion
                #region storage_money
                case "storage_money":
                {
                    switch (opt)
                    {
                        case "+=":
                            return m_pUser.ChangeStorageMoney(long.Parse(value));
                        case "==":
                            return m_pUser.MoneySaved == uint.Parse(value);
                        case "<":
                            return m_pUser.MoneySaved < uint.Parse(value);
                    }
                    return false;
                }
                #endregion
                #region syn_age
                case "syn_age":
                {
                    if (m_pUser.Syndicate == null)
                        return false;
                    int nValue = int.Parse(value);
                    int nDays = (int) (DateTime.Now - UnixTimestamp.ToDateTime(m_pUser.SyndicateMember.JoinDate)).Days;
                    switch (opt)
                    {
                        case "==": return nDays == nValue;
                        case "!=": return nDays != nValue;
                        case "<=": return nDays <= nValue;
                        case ">=": return nDays >= nValue;
                        case "<": return nDays < nValue;
                        case ">": return nDays > nValue;
                    }
                    return false;
                }
                #endregion
                default:
                    ServerKernel.Log.SaveLog("[1001] " + param + " not handled.", false, LogType.WARNING);
                    return false;
            }
        }
        #endregion
        #region 1002 - User Fill Attribute
        private bool USERFILLATTR(string param)
        {
            if (m_pUser == null)
                return false;

            switch (param)
            {
                case "life":
                    m_pUser.Life = m_pUser.MaxLife;
                    return true;
                case "mana":
                    m_pUser.Mana = m_pUser.MaxMana;
                    return true;
            }
            return false;
        }
        #endregion
        #region 1003 - User Change Map
        private bool USERCHGMAP(string param)
        {
            if (m_pUser == null)
                return false;

            string[] Params = GetSafeParam(param);
            uint Map = uint.Parse(Params[0]);
            ushort X = ushort.Parse(Params[1]);
            ushort Y = ushort.Parse(Params[2]);
            if (Map == 0 || X == 0 || Y == 0)
                return false;
            m_pUser.ChangeMap(X, Y, Map);
            return true;
        }
        #endregion
        #region 1004 - User Save Location
        private bool USERSAVELOCATION(string param)
        {
            if (m_pUser == null)
                return false;

            string[] Params = GetSafeParam(param);
            uint Map = uint.Parse(Params[0]);
            ushort X = ushort.Parse(Params[1]);
            ushort Y = ushort.Parse(Params[2]);

            if (Map == 0)
            {
                m_pUser.RecordMapIdentity = m_pUser.MapIdentity;
                m_pUser.RecordMapX = m_pUser.MapX;
                m_pUser.RecordMapY = m_pUser.MapY;
                return true;
            }

            if (X == 0 || Y == 0)
                return false;

            m_pUser.RecordMapIdentity = Map;
            m_pUser.RecordMapX = X;
            m_pUser.RecordMapY = Y;
            m_pUser.SetRecordPos(Map, X, Y);
            return true;
        }
        #endregion
        #region 1005 - User Hair
        private bool ACTIONUSERHAIR(ActionStruct action)
        {
            if (m_pUser == null) return false;

            string[] param = GetSafeParam(action.Param);

            if (param.Length < 2) return false;

            switch (param[0])
            {
                case "style":
                    {
                        if (m_pUser.Profession / 10 == 6 && m_pUser.Gender == 1)
                        {
                            m_pUser.Hair = 0;
                            return false;
                        }
                        m_pUser.Hair = (ushort)((ushort.Parse(param[1])) + (m_pUser.Hair - (m_pUser.Hair % 100)));
                        break;
                    }
                case "color":
                    {
                        m_pUser.Hair = (ushort)((m_pUser.Hair % 100) + (ushort.Parse(param[1]) * 100));
                        break;
                    }
                default:
                    return false;
            }
            return true;
        }
        #endregion
        #region 1006 - Restore User Position
        private bool USERCHGMAPRECORD()
        {
            if (m_pUser == null)
                return false;

            if (m_pUser.RecordMapIdentity == 0)
            {
                m_pUser.ChangeMap(430, 380, 1002);
            }
            else
            {
                m_pUser.ChangeMap(m_pUser.RecordMapX, m_pUser.RecordMapY, m_pUser.RecordMapIdentity);
            }
            return true;
        }
        #endregion
        #region 1008 - User transform
        private bool USERTRANSFORM(string szParam)
        {
            string[] param = GetSafeParam(szParam);
            if (param.Length < 4)
                return false;

            int unknown0 = int.Parse(param[0]);
            int unknown1 = int.Parse(param[1]);
            uint transform = uint.Parse(param[2]);
            int time = int.Parse(param[3]);

            return m_pUser.Transform(transform, time, true);
        }
        #endregion
        #region 1009 - User is Pure Class
        private bool ACTIONUSERISPURE()
        {
            return m_pUser.Profession == m_pUser.FirstProfession && m_pUser.Profession == m_pUser.LastProfession;
        }
        #endregion
        #region 1010 - User Talk
        private bool USERTALK(string param, uint data)
        {
            if (m_pUser == null)
                return false;

            m_pUser.Send(new MsgTalk(param, (ChatTone)data));
            return true;
        }
        #endregion
        #region 1020 - User Magic
        private bool ACTIONUSERMAGIC(string szParam)
        {
            if (m_pUser == null || m_pUser.Magics == null)
                return false;

            string[] param = GetSafeParam(szParam);
            if (param.Length < 2)
            {
                ServerKernel.Log.SaveLog(string.Format("ACTION[{0}]: Type 1020 not enough params, need 2", 0), false, LogType.WARNING);
                return false;
            }

            switch (param[0].ToLower())
            {
                case "check":
                    if (param.Length == 3)
                        return m_pUser.Magics.CheckLevel(ushort.Parse(param[1]), ushort.Parse(param[2]));
                    return m_pUser.Magics.CheckType(ushort.Parse(param[1]));
                case "learn":
                    if (param.Length == 3)
                        return m_pUser.Magics.Create(ushort.Parse(param[1]), byte.Parse(param[2]));
                    return m_pUser.Magics.Create(ushort.Parse(param[1]), 0);
                case "uplevel":
                    return m_pUser.Magics.UpLevelByTask(ushort.Parse(param[1]));
                case "addexp":
                    return m_pUser.Magics.AwardExp(ushort.Parse(param[1]), 0, int.Parse(param[2]));
                default:
                    ServerKernel.Log.SaveLog("Unhandled type on 1020:" + param[0], false, LogType.WARNING);
                    return false;
            }
        }
        #endregion
        #region 1021 - User WeaponSkill
        private bool USERWEAPONSKILL(string szParam)
        {
            if (m_pUser == null)
                return false;

            ushort nType = 0;
            long nValue = 0;
            string[] param = GetSafeParam(szParam);

            if (param.Length < 3)
                return false;

            nType = ushort.Parse(param[1]);
            nValue = long.Parse(param[2]);

            switch (param[0])
            {
                case "check":
                    DbWeaponSkill pSkill;
                    if (m_pUser.WeaponSkill.Skills.TryGetValue(nType, out pSkill))
                    {
                        return pSkill.Level >= nValue;
                    }
                    break;
                case "learn":
                    return m_pUser.WeaponSkill.Create(nType, (byte)nValue);
                case "addexp":
                    {
                        return m_pUser.WeaponSkill.AwardExperience(nType, (int)nValue);
                    }
            }

            return false;
        }
        #endregion
        #region 1022 - GM Log
        private bool ACTIONUSERLOG(string param)
        {
            char[] ar = { ' ' };
            string[] Params = param.Split(ar, 2);
            if (Params.Length < 2)
            {
                ServerKernel.Log.SaveLog("Could not write to the gm log. " + param, false);
                return false;
            }
            ServerKernel.Log.GmLog(Params[0], Params[1]);
            return true;
        }
        #endregion
        #region 1023 - Bonus
        private bool ACTIONUSERBONUS()
        {
            return m_pUser.DoBonus();
        }
        #endregion
        #region 1024 - Divorce
        private bool ACTION_USER_DIVORCE()
        {
            if (m_pUser == null) return false;

            if (m_pUser.Mate == "None") return false;

            Client pMate = ServerKernel.Players.Values.FirstOrDefault(x => x.Character.Name == m_pUser.Mate);

            if (pMate == null)
            {
                DbUser dbMate = Database.Characters.SearchByName(m_pUser.Mate);
                if (dbMate != null)
                {
                    dbMate.Mate = "None";
                    Database.Characters.SaveOrUpdate(dbMate);
                }
            }
            else
            {
                pMate.Character.Mate = "None";
            }
            m_pUser.Mate = "None";
            return true;
        }
        #endregion
        #region 1025 - User Marriage
        private bool ACTIONUSERMARRIAGE()
        {
            if (m_pUser == null) return false;
            return m_pUser.IsMarried();
        }
        #endregion
        #region 1026 - User Sex
        private bool ACTIONUSERSEX()
        {
            if (m_pUser == null) return false;
            return m_pUser.Gender == 1;
        }
        #endregion
        #region 1027 - User Effect

        private bool ACTIONUSEREFFECT(ActionStruct action)
        {
            if (m_pUser == null)
                return false;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 2) return false;

            switch (param[0])
            {
                case "self":
                    MsgName msg1 = new MsgName
                    {
                        Action = StringAction.ROLE_EFFECT,
                        Identity = m_pUser.Identity
                    };
                    msg1.Append(param[1]);
                    m_pUser.Screen.Send(msg1, true);
                    return true;
                case "couple":
                    if (m_pUser.Mate == "None")
                    {
                        return false;
                    }

                    Character mate = m_pUser.GetMateRole();
                    var msg0 = new MsgName
                    {
                        Action = StringAction.ROLE_EFFECT,
                        Identity = m_pUser.Identity
                    };
                    msg0.Append(param[1]);
                    m_pUser.Screen.Send(msg0, true);
                    msg0.Identity = mate.Identity;
                    mate.Screen.Send(msg0, true);
                    return true;
                case "team":
                    if (m_pUser.Team == null)
                        return false;
                    foreach (var member in m_pUser.Team.Members.Values)
                    {
                        var msg = new MsgName
                        {
                            Action = StringAction.ROLE_EFFECT,
                            Identity = member.Identity
                        };
                        msg.Append(param[1]);
                        member.Owner.Screen.Send(msg, true);
                    }
                    return true;
            }

            return false;
        }

        #endregion
        #region 1029 - User Media Play
        private bool ACTIONUSERMEDIAPLAY(ActionStruct action)
        {
            if (m_pUser == null)
                return false;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 2)
                return false;

            var msg = new MsgName { Action = StringAction.PLAYER_WAVE };
            msg.Append(param[1]);

            switch (param[0])
            {
                case "play":
                    m_pUser.Send(msg);
                    return true;
                case "broadcast":
                    m_pUser.Screen.Send(msg, true);
                    return true;
            }
            return false;
        }
        #endregion
        #region 1031 - Add User Title
        /// <summary>
        /// This type method will add a new title to the user titles table. It wont activate,
        /// it will set the title and endtime according to the user input. If the time is set
        /// 0, it will add 1 week seconds.
        /// </summary>
        /// <param name="param">Param is: title time | And time cannot be null, should be 0 or higher.</param>
        private bool ACTIONADDUSERTITLE(string param)
        {
            string[] Params = GetSafeParam(param); //not yet in version 5103
            if (Params.Length < 2)
                return false;

            byte title = byte.Parse(Params[0]);
            uint timestamp = uint.Parse(Params[1]);

            if (timestamp <= 0)
                timestamp = 604800;

            timestamp += (uint)UnixTimestamp.Timestamp();

            var newTitle = new DbTitle
            {
                Timestamp = timestamp,
                Title = title,
                Userid = m_pUser.Identity
            };

            new CqTitleRepository().SaveOrUpdate(newTitle);

            var tPacket = new MsgTitle();
            tPacket.Identity = m_pUser.Identity;
            tPacket.Action = TitleAction.ADD_TITLE;
            tPacket.SelectedTitle = (UserTitle)title;
            m_pUser.Send(tPacket);

            if (m_pUser.Title <= 0)
            {
                m_pUser.Title = title;
                m_pUser.Screen.RefreshSpawnForObservers();
            }

            return m_pUser.Titles.TryAdd(title, newTitle);
        }
        #endregion
        #region 1032 - Remove User Title
        private bool ACTIONREMOVEUSERTITLE(uint data)
        {
            if (data == 0)
                return true;
            if (m_pUser.Titles.ContainsKey((byte)data))
            {
                var tPacket = new MsgTitle();
                tPacket.Identity = m_pUser.Identity;
                tPacket.Action = TitleAction.REMOVE_TITLE;
                tPacket.SelectedTitle = (UserTitle)data;
                m_pUser.Send(tPacket);
                DbTitle trash;
                m_pUser.Titles.TryRemove((byte)data, out trash);
                return new CqTitleRepository().Delete(trash);
            }
            return false;
        }
        #endregion
        #region 1040 - User Rebirth
        private bool ACTIONUSERREBIRTH(ActionStruct action)
        {
            if (m_pUser == null || m_pUser == null)
            {
                ServerKernel.Log.SaveLog(string.Format("ACTION {0}: no user.", action.Id), false);
                return false;
            }

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 2)
            {
                ServerKernel.Log.SaveLog(string.Format("ACTION {0}: invalid param num", action.Id), false);
                return false;
            }

            ushort nProf = ushort.Parse(param[0]);
            ushort nLook = ushort.Parse(param[1]);

            if (!m_pUser.Rebirth(nProf, nLook)) return false;

            ServerKernel.Log.GmLog("rebirth", string.Format("User[{0}][{1}], prof[{2}], level[{3}], rebirth to prof[{4}] look[{5}]",
                m_pUser.Name, m_pUser.Identity, m_pUser.Profession, m_pUser.Level, nProf, nLook));
            return true;
        }
        #endregion
        #region 1041 - User WebPage
        private bool ACTIONUSERWEBPAGE(ActionStruct action)
        {
            if (m_pUser == null)
            {
                ServerKernel.Log.SaveLog(string.Format("ACTION {0}: no user.", action.Id), false);
                return false;
            }

            m_pUser.Send(action.Param, ChatTone.WEBSITE);
            return true;
        }
        #endregion
        #region 1042 - Send BBS
        private bool ACTIONUSERSENDBBS(ActionStruct action)
        {
            if (m_pUser == null)
            {
                ServerKernel.Log.SaveLog(string.Format("ACTION {0}: user not set", action.Id), false);
                return false;
            }

            m_pUser.Send(action.Param, (ChatTone)2206);
            return true;
        }
        #endregion
        #region 1045 - Fix Attribute
        private bool ACTIONUSERFIXATTR()
        {
            if (m_pUser == null) return false;

            if (m_pUser.Metempsychosis < 1)
            {
                m_pUser.Send("You have not been reborn.");
                return false;
            }

            m_pUser.FixAttributes();
            return true;
        }
        #endregion
        #region 1046 - User Open Dialog
        private bool USEROPENDIALOG(uint data)
        {
            if (m_pUser == null)
                return false;

            switch ((OpenWindow)data)
            {
                case OpenWindow.VIP_WAREHOUSE:
                    {
                        if (m_pUser.Owner.VipLevel == 0)
                            return false;
                        break;
                    }
            }
            m_pUser.Send(new MsgAction(m_pUser.Identity, data, m_pUser.MapX, m_pUser.MapY, GeneralActionType.OPEN_WINDOW));
            return true;
        }
        #endregion
        #region 1047 - Reallot Points
        private bool ACTION_USER_CHGMAP_REBORN(ActionStruct action)
        {
            if (m_pUser == null) return false;

            m_pUser.ResetAttrPoints();
            return true;
        }
        #endregion
        #region 1048 - User Exp Multiplier
        private bool USEREXPMULTIPLY(string szParam)
        {
            string[] param = GetSafeParam(szParam);
            if (param.Length < 2)
                return false;

            uint time = uint.Parse(param[1]);
            float multiply = int.Parse(param[0]) / 100f;
            m_pUser.SetExperienceMultiplier(time, multiply);
            return true;
        }
        #endregion
        #region 1052 - User WH Check password

        private bool USERWHPASSWORD(ActionStruct action)
        {
            if (m_pUser == null)
                return false;

            if (m_pUser.WarehousePassword == 0)
                return true;

            if (m_pUser.TemporaryString == null || m_pUser.TemporaryString == string.Empty)
                return false;

            if (m_pUser.TemporaryString.Length < 4 || m_pUser.TemporaryString.Length > ulong.MaxValue.ToString().Length)
                return false;

            ulong dwPassword = 0;
            if (!ulong.TryParse(m_pUser.TemporaryString, out dwPassword))
                return false;

            return m_pUser.WarehousePassword == dwPassword;
        }

        #endregion
        #region 1053 - User set WH Password

        private bool ACTIONUSERSETWHPASSWORD()
        {
            if (m_pUser == null || m_pUser.TemporaryString == null || m_pUser.TemporaryString == string.Empty)
                return false;

            if ((m_pUser.TemporaryString.Length < 4 && m_pUser.TemporaryString != "0") || m_pUser.TemporaryString.Length > ulong.MaxValue.ToString().Length)
            {
                m_pUser.Send(string.Format("Your password must be between 1000 and {0}.", ulong.MaxValue));
                return false;
            }

            ulong dwPass = 0;

            if (!ulong.TryParse(m_pUser.TemporaryString, out dwPass))
                return false;

            m_pUser.WarehousePassword = dwPass;
            return true;
        }

        #endregion
        #region 1054 - Open User Interface
        private bool OPENINTERFACE(uint data)
        {
            if (m_pUser == null)
                return false;

            m_pUser.Send(new MsgAction(m_pUser.Identity, data, m_pUser.MapX, m_pUser.MapY, GeneralActionType.OPEN_CUSTOM));
            return true;
        }
        #endregion
        #region 1060 - User Variable Comparison
        private bool USERVARCOMPARE(string szParam)
        {
            string[] param = GetSafeParam(szParam);
            if (param.Length < 3)
                return false;

            byte varId = VarId(param[0]);
            string opt = param[1];
            long value = long.Parse(param[2]);

            long varVal = 0;

            switch (varId)
            {
                case 0: varVal = m_pUser.IterData0; break;
                case 1: varVal = m_pUser.IterData1; break;
                case 2: varVal = m_pUser.IterData2; break;
                case 3: varVal = m_pUser.IterData3; break;
                case 4: varVal = m_pUser.IterData4; break;
                case 5: varVal = m_pUser.IterData5; break;
                case 6: varVal = m_pUser.IterData6; break;
                case 7: varVal = m_pUser.IterData7; break;
                default:
                    return false;
            }

            switch (opt)
            {
                case "==":
                    return varVal == value;
                case ">=":
                    return varVal >= value;
                case "<=":
                    return varVal <= value;
                case ">":
                    return varVal > value;
                case "<":
                    return varVal < value;
                case "!=":
                    return varVal != value;
                default:
                    return false;
            }
        }
        #endregion
        #region 1061 - User Variable Definition
        private bool USERVARDEFINE(string szParam)
        {
            string[] param = GetSafeParam(szParam);
            if (param.Length < 3)
                return false;

            byte varId = VarId(param[0]);
            string opt = param[1];
            long value = long.Parse(param[2]);

            try
            {
                switch (opt)
                {
                    case "set":
                        {
                            switch (varId)
                            {
                                case 0:
                                    m_pUser.IterData0 = value;
                                    return true;
                                case 1:
                                    m_pUser.IterData1 = value;
                                    return true;
                                case 2:
                                    m_pUser.IterData2 = value;
                                    return true;
                                case 3:
                                    m_pUser.IterData3 = value;
                                    return true;
                                case 4:
                                    m_pUser.IterData4 = value;
                                    return true;
                                case 5:
                                    m_pUser.IterData5 = value;
                                    return true;
                                case 6:
                                    m_pUser.IterData6 = value;
                                    return true;
                                case 7:
                                    m_pUser.IterData7 = value;
                                    return true;
                            }
                            break;
                        }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
        #endregion
        #region 1064 - User Variable Calculation
        private bool ACTIONUSERVARCALC(string szParam)
        {
            string[] param = GetSafeParam(szParam);
            if (param.Length < 3)
                return false;

            byte varId = VarId(param[0]);
            string opt = param[1];
            long value = long.Parse(param[2]);

            long varVal = 0;

            if (opt == "/=" && value == 0)
                return false; // division by zero

            switch (varId)
            {
                case 0:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData0 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData0 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData0 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData0 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData0 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                case 1:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData1 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData1 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData1 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData1 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData1 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                case 2:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData2 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData2 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData2 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData2 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData2 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                case 3:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData3 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData3 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData3 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData3 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData4 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                case 4:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData4 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData4 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData4 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData4 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData4 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                case 5:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData5 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData5 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData5 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData5 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData5 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                case 6:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData6 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData6 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData6 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData6 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData6 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                case 7:
                    {
                        switch (opt)
                        {
                            case "+=":
                                m_pUser.IterData7 += value;
                                return true;
                            case "-=":
                                m_pUser.IterData7 -= value;
                                return true;
                            case "*=":
                                m_pUser.IterData7 *= value;
                                return true;
                            case "/=":
                                m_pUser.IterData7 /= value;
                                return true;
                            case "mod=":
                                m_pUser.IterData7 %= value;
                                return true;
                            default:
                                return false;
                        }
                    }
                default:
                    return false;
            }
        }
        #endregion
        #region 1073 - Statistic Check
        private bool USERSTATISTICCOMPARE(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);
            if (param.Length < 3)
                return false;

            string szStc = GetParenthesys(param[0]);
            string opt = param[1];
            long value = long.Parse(param[2]);

            string[] pStc = szStc.Trim().Split(',');

            if (pStc.Length < 2)
                return false;

            uint idEvent = uint.Parse(pStc[0]);
            uint idType = uint.Parse(pStc[1]);
            ulong idx = idEvent + (idType << 32);

            DbStatistic dbStc;
            if (!m_pUser.Statistics.TryGetValue(idx, out dbStc))
                return false;

            switch (opt)
            {
                case ">=":
                    return dbStc.Data >= value;
                case "<=":
                    return dbStc.Data <= value;
                case ">":
                    return dbStc.Data > value;
                case "<":
                    return dbStc.Data < value;
                case "!=":
                case "<>":
                    return dbStc.Data != value;
                case "==":
                    return dbStc.Data == value;
            }

            return false;
        }
        #endregion
        #region 1074 - Statistic Set
        private bool USERSTATISTICOPE(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);
            if (param.Length < 3)
                return false;

            string szStc = GetParenthesys(param[0]);
            string opt = param[1];
            long value = long.Parse(param[2]);
            bool bUpdate = param[3] != "0";

            string[] pStc = szStc.Trim().Split(',');

            if (pStc.Length < 2)
                return false;

            uint idEvent = uint.Parse(pStc[0]);
            uint idType = uint.Parse(pStc[1]);
            ulong idx = idEvent + (idType << 32);

            DbStatistic dbStc;
            if (!m_pUser.Statistics.TryGetValue(idx, out dbStc))
            {
                dbStc = new DbStatistic
                {
                    PlayerIdentity = m_pUser.Identity,
                    PlayerName = m_pUser.Name,
                    EventType = idEvent,
                    DataType = idType,
                    Data = value < 0 ? 0 : (uint)value,
                    Timestamp = (uint)(bUpdate ? UnixTimestamp.Timestamp() : 0)
                };
                new StatisticRepository().SaveOrUpdate(dbStc);
                return m_pUser.Statistics.TryAdd(idx, dbStc);
            }

            switch (opt)
            {
                case "+=":
                    if (value == 0) return false;
                    if (value < 0)
                    {
                        if (dbStc.Data + value < 0)
                            dbStc.Data = 0;
                        else
                            dbStc.Data -= (uint)value;
                    }
                    else
                    {
                        if (dbStc.Data + value > uint.MaxValue)
                            dbStc.Data = uint.MaxValue;
                        else
                            dbStc.Data += (uint)value;
                    }
                    if (bUpdate)
                        dbStc.Timestamp = (uint)UnixTimestamp.Timestamp();
                    new StatisticRepository().SaveOrUpdate(dbStc);
                    return true;
                case "=":
                    if (value < 0) return false;
                    dbStc.Data = (uint)value;
                    if (bUpdate)
                        dbStc.Timestamp = (uint)UnixTimestamp.Timestamp();
                    new StatisticRepository().SaveOrUpdate(dbStc);
                    return true;
            }

            return false;
        }
        #endregion
        #region 1080 - Timestamp Set
        private bool ACTIONUSERTASKMANAGER(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);
            if (param.Length < 3)
                return false;

            string szStc = GetParenthesys(param[0]);
            string opt = param[1];
            long value = long.Parse(param[2]);

            string[] pStc = szStc.Trim().Split(',');

            if (pStc.Length <= 2)
                return false;

            uint idEvent = uint.Parse(pStc[0]);
            uint idType = uint.Parse(pStc[1]);
            ulong idx = idEvent + (idType << 32);
            byte mode = byte.Parse(pStc[2]);

            if (value < 0)
                return false;

            DbStatistic dbStc;
            if (!m_pUser.Statistics.TryGetValue(idx, out dbStc))
                return true;

            if (dbStc.Timestamp == 0)
                return true;

            switch (mode)
            {
                case 0: // seconds
                    {
                        int timeStamp = UnixTimestamp.Timestamp();
                        int nDiff = (int)(timeStamp - dbStc.Timestamp + value);
                        switch (opt)
                        {
                            case "==": return nDiff == value;
                            case "<": return nDiff < value;
                            case ">": return nDiff > value;
                            case "<=": return nDiff <= value;
                            case ">=": return nDiff >= value;
                            case "<>":
                            case "!=": return nDiff != value;
                        }
                        return false;
                    }
                case 1: // days
                    TimeSpan interval = DateTime.Now - UnixTimestamp.ToDateTime(dbStc.Timestamp);
                    switch (opt)
                    {
                        case "==": return interval.Days == value;
                        case "<": return interval.Days < value;
                        case ">": return interval.Days > value;
                        case "<=": return interval.Days <= value;
                        case ">=": return interval.Days >= value;
                        case "!=":
                        case "<>": return interval.Days != value;
                    }
                    return false;
                default:
                    ServerKernel.Log.SaveLog(
                        string.Format("Unhandled Time mode ({0}) on action (id:{1})", mode, action.Id), false, LogType.WARNING);
                    return false;
            }
        }
        #endregion
        #region 1081 - Interval Check
        private bool ACTION_USER_TASK_OPE(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);
            if (param.Length < 3)
                return false;

            string szStc = GetParenthesys(param[0]);
            string opt = param[1];
            long value = long.Parse(param[2]);

            string[] pStc = szStc.Trim().Split(',');

            uint idEvent = uint.Parse(pStc[0]);
            uint idType = uint.Parse(pStc[1]);
            ulong idx = idEvent + (idType << 32);

            DbStatistic dbStc;
            if (!m_pUser.Statistics.TryGetValue(idx, out dbStc))
            {
                dbStc = new DbStatistic
                {
                    PlayerIdentity = m_pUser.Identity,
                    PlayerName = m_pUser.Name,
                    EventType = idEvent,
                    DataType = idType,
                    Timestamp = (uint)(value)
                };
                new StatisticRepository().SaveOrUpdate(dbStc);
                return m_pUser.Statistics.TryAdd(idx, dbStc);
            }

            switch (opt)
            {
                case "set":
                    {
                        if (value > 0)
                            dbStc.Timestamp = (uint)value;
                        else
                            dbStc.Timestamp = (uint)UnixTimestamp.Timestamp();
                        new StatisticRepository().SaveOrUpdate(dbStc);
                        return true;
                    }
            }

            return false;
        }
        #endregion
        #region 1082 - User attach status
        private bool USERATTACHSTATUS(string szParam)
        {
            // self add 64 200 900 0
            string[] param = GetSafeParam(szParam);
            if (param.Length < 6)
                return false;

            string target = param[0].ToLower();
            string opt = param[1].ToLower();
            int status = StatusSet.InvertFlag(ulong.Parse(param[2]));
            int multiply = int.Parse(param[3]);
            uint seconds = uint.Parse(param[4]);
            int times = int.Parse(param[5]);
            // last param unknown

            if (target == "team" && m_pUser.Team == null)
                return false;

            if (target == "self")
            {
                m_pUser.AttachStatus(m_pUser, status, multiply, (int)seconds, times, 0, m_pUser.Identity);
                return true;
            }
            if (target == "team")
            {
                foreach (var member in m_pUser.Team.Members.Values)
                    member.AttachStatus(member, status, multiply, (int)seconds, times, 0, m_pUser.Identity);
                return true;
            }
            if (target == "couple")
            {
                if (m_pUser.GetMateRole() == null)
                    return false;
                m_pUser.AttachStatus(m_pUser, status, multiply, (int)seconds, times, 0, m_pUser.Identity);
                Character pTarget = m_pUser.GetMateRole();
                pTarget.AttachStatus(pTarget, status, multiply, (int)seconds, times, 0, m_pUser.Identity);
            }
            return false;
        }
        #endregion
        #region 1083 - User Add Blessing
        private bool USERADDGODTIME(string szParam)
        {
            string[] param = GetSafeParam(szParam);

            if (param.Length < 2)
                return false;

            string opt = param[0];
            uint minutes = uint.Parse(param[1]);

            switch (opt)
            {
                case "+=":
                    {
                        return m_pUser.AddBlessing(minutes);
                    }
            }

            return true;
        }
        #endregion
        #region 1086 - Get ExpBall Amount
        private bool ACTION_USER_EXPBALL_EXP(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 2)
                return false;

            uint dwExpTimes = uint.Parse(param[0]);
            byte idData = byte.Parse(param[1]);

            if (idData > 7) return false;

            long exp = ServerKernel.GetExpBallExperience(m_pUser.Level) / 600 * dwExpTimes;

            switch (idData)
            {
                case 0: m_pUser.IterData0 = exp; break;
                case 1: m_pUser.IterData1 = exp; break;
                case 2: m_pUser.IterData2 = exp; break;
                case 3: m_pUser.IterData3 = exp; break;
                case 4: m_pUser.IterData4 = exp; break;
                case 5: m_pUser.IterData5 = exp; break;
                case 6: m_pUser.IterData6 = exp; break;
                case 7: m_pUser.IterData7 = exp; break;
            }

            return true;
        }
        #endregion
        #region 1096 - User Status Add
        private bool ACTIONUSERSTATUSCREATE(ActionStruct action)
        {
            // sort leave_times remain_time end_time interval_time
            // 200 0 604800 0 604800 1
            if (action.Data == 0)
            {
                ServerKernel.Log.SaveLog("ERROR: invalid data num " + action.Id, false);
                return false;
            }

            string[] _params = GetSafeParam(action.Param);
            if (_params.Length < 5)
            {
                ServerKernel.Log.SaveLog("ERROR: invalid param num " + action.Id, false);
                return false;
            }

            uint sort = uint.Parse(_params[0]);
            uint leaveTimes = uint.Parse(_params[1]);
            uint remainTime = uint.Parse(_params[2]);
            uint intervalTime = uint.Parse(_params[4]);
            bool unknown = _params[5] != "0"; // ??

            var status = new StatusRepository().FindStatus(m_pUser.Identity, action.Data);
            if (status != null)
            {
                status.EndTime = (uint)UnixTimestamp.Timestamp() + remainTime;
                status.IntervalTime = intervalTime;
                status.LeaveTimes = leaveTimes;
                status.RemainTime = remainTime;
                status.Sort = sort;
                if (!new StatusRepository().SaveOrUpdate(status))
                {
                    ServerKernel.Log.SaveLog(string.Format("ERROR: Could not update status {0}[{2}] to {1}", status.Id, action.Data, status.Status), false);
                    return false;
                }
            }
            else
            {
                var query = new DbStatus
                {
                    EndTime = (uint)UnixTimestamp.Timestamp() + remainTime,
                    IntervalTime = intervalTime,
                    LeaveTimes = leaveTimes,
                    OwnerId = m_pUser.Identity,
                    Power = 0,
                    RemainTime = remainTime,
                    Status = action.Data,
                    Sort = sort,
                };
                if (!new StatusRepository().SaveOrUpdate(query))
                {
                    ServerKernel.Log.SaveLog("ERROR: Could not save status", false);
                    return false;
                }
            }

            m_pUser.AttachStatus(m_pUser, (int)action.Data, 0, (int)remainTime, (int)leaveTimes, 0);
            return true;
        }
        #endregion
        #region 1098 - Status Check
        private bool ACTION_USER_STATUS_CHECK(ActionStruct action)
        {
            if (m_pUser.Status == null) return false;

            string[] status = GetSafeParam(action.Param);

            switch (action.Data)
            {
                case 0: // check
                    foreach (var st in status)
                        if (m_pUser.QueryStatus(int.Parse(st)) == null)
                            return false;
                    return true;
                case 1:
                    foreach (var st in status)
                        if (m_pUser.QueryStatus(int.Parse(st)) != null)
                        {
                            m_pUser.DetachStatus(int.Parse(st));
                            DbStatus db = Database.Status.FindStatus(m_pUser.Identity, uint.Parse(st));
                            if (db != null)
                                Database.Status.Delete(db);
                        }
                    return true;
            }

            return false;
        }
        #endregion
        #endregion
        #region 1100 Team
        #region 1101 - Team Broadcast
        private bool ACTION_TEAM_BROADCAST(ActionStruct action)
        {
            if (m_pUser.Team == null || m_pUser.Team.Leader != m_pUser || m_pUser.Team.MembersCount() <= 1)
                return false;

            foreach (var member in m_pUser.Team.Members.Values)
                member.Send(action.Param, ChatTone.TEAM);

            return true;
        }
        #endregion
        #region 1102 - Team Attribute
        private bool ACTIONTEAMATTR(ActionStruct action)
        {
            if (m_pUser.Team == null)
                return false;

            string[] param = GetSafeParam(action.Param);

            if (param.Length < 3)
                return false;

            string szParam = param[0];
            string szOpt = param[1];
            int nValue = int.Parse(param[2]);

            switch (szParam)
            {
                case "count":
                    {
                        switch (szOpt)
                        {
                            case "<":
                                return m_pUser.Team.MembersCount() < nValue;
                            case ">":
                                return m_pUser.Team.MembersCount() > nValue;
                            case "<=":
                                return m_pUser.Team.MembersCount() <= nValue;
                            case ">=":
                                return m_pUser.Team.MembersCount() >= nValue;
                            case "==":
                                return m_pUser.Team.MembersCount() == nValue;
                            case "!=":
                            case "<>":
                                return m_pUser.Team.MembersCount() != nValue;
                        }
                        break;
                    }
                case "money":
                    {
                        foreach (var member in m_pUser.Team.Members.Values)
                        {
                            switch (szOpt)
                            {
                                case "+=":
                                    if (!member.AwardMoney(nValue))
                                        return false;
                                    break;
                                case ">=":
                                    if (member.Silver <= nValue)
                                        return false;
                                    break;
                                case "<=":
                                    if (member.Silver >= nValue)
                                        return false;
                                    break;
                                case ">":
                                    if (member.Silver < nValue)
                                        return false;
                                    break;
                                case "<":
                                    if (member.Silver > nValue)
                                        return false;
                                    break;
                                case "==":
                                    if (member.Silver != nValue)
                                        return false;
                                    break;
                            }
                        }
                        return true;
                    }
                case "level":
                    {
                        foreach (var member in m_pUser.Team.Members.Values)
                        {
                            switch (szOpt)
                            {
                                case ">=":
                                    if (member.Level <= nValue)
                                        return false;
                                    break;
                                case "<=":
                                    if (member.Level >= nValue)
                                        return false;
                                    break;
                                case ">":
                                    if (member.Level < nValue)
                                        return false;
                                    break;
                                case "<":
                                    if (member.Level > nValue)
                                        return false;
                                    break;
                                case "==":
                                    if (member.Level != nValue)
                                        return false;
                                    break;
                            }
                        }
                        return true;
                    }
                case "mate":
                    {
                        foreach (var member in m_pUser.Team.Members.Values)
                            if (member.Name != m_pUser.Mate)
                                return false;

                        return true;
                    }
                case "friend":
                    {
                        foreach (var member in m_pUser.Team.Members.Values)
                            if (!m_pUser.ContainsFriend(member.Identity))
                                return false;
                        return true;
                    }
                case "count_near":
                    {
                        foreach (var member in m_pUser.Team.Members.Values)
                            if (!Calculations.InScreen(m_pUser.MapX, m_pUser.MapY, member.MapX, member.MapY))
                                return false;
                        return true;
                    }
                case "study_points":
                {
                    foreach (var member in m_pUser.Team.Members.Values)
                    {
                        member.ChangeStudyPoints(nValue);
                    }
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region 1103 - Team Leavespace
        private bool ACTION_TEAM_LEAVESPACE(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 1)
                return false;

            int nSpace = int.Parse(param[0]);

            foreach (var member in m_pUser.Team.Members.Values)
                if (member.Inventory.RemainingSpace() < nSpace)
                    return false;

            return true;
        }
        #endregion
        #region 1104 - Team Add Item
        private bool ACTION_TEAM_ITEM_ADD(ActionStruct action)
        {
            foreach (var member in m_pUser.Team.Members.Values)
                member.Inventory.Create(action.Data);
            return true;
        }
        #endregion
        #region 1105 - Team Item Del
        private bool ACTION_TEAM_ITEM_DEL(ActionStruct action)
        {
            foreach (var member in m_pUser.Team.Members.Values)
                member.Inventory.Remove(action.Data, 1);
            return true;
        }
        #endregion
        #region 1106 - Team Item Check
        private bool ACTION_TEAM_ITEM_CHECK(ActionStruct action)
        {
            foreach (var member in m_pUser.Team.Members.Values)
                if (!member.Inventory.Contains(action.Data, 1))
                    return false;
            return true;
        }
        #endregion
        #region 1107 - Team Teleport
        private bool ACTION_TEAM_CHGMAP(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 3)
                return false;

            if (m_pUser.Team == null) return false;

            uint idMap = uint.Parse(param[0]);
            ushort x = ushort.Parse(param[1]);
            ushort y = ushort.Parse(param[2]);

            if (!ServerKernel.Maps.ContainsKey(idMap))
                return false;

            foreach (var member in m_pUser.Team.Members.Values)
            {
                if (m_pUser.MapIdentity == member.MapIdentity
                    &&
                    Calculations.GetDistance(m_pUser.MapX, m_pUser.MapY, member.MapX, member.MapY) <=
                    Calculations.SCREEN_DISTANCE)
                {
                    member.ChangeMap(x, y, idMap);
                }
            }

            return true;
        }
        #endregion
        #region 1108 - Team Is Leader
        private bool ACTION_TEAM_CHK_ISLEADER(ActionStruct action)
        {
            return m_pUser.Team.Leader == m_pUser;
        }
        #endregion
        #endregion
        #region 1500 General
        #region 1508 Lottery
        private bool ACTION_GENERAL_LOTTERY(ActionStruct action)
        {
            if (m_pUser == null) return false;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 1) return false;

            byte type = byte.Parse(param[0]);
            byte color = (byte)action.Data;

            //int retries = 0;
            diceAgain:
            byte rank = 8;
            if (Calculations.ChanceCalc(.25f))
                rank = 1;
            else if (Calculations.ChanceCalc(2f))
                rank = 2;
            else if (Calculations.ChanceCalc(4.5f))
                rank = 3;
            else if (Calculations.ChanceCalc(7.5f))
                rank = 4;
            else if (Calculations.ChanceCalc(10f))
                rank = 5;
            else if (Calculations.ChanceCalc(20f))
                rank = 6;
            else if (Calculations.ChanceCalc(40f))
                rank = 7;

            IList<DbGameLottery> lotto = null;
            if (rank > 5)
                lotto = new LotteryRepository().FetchAllRank5();
            else
                lotto = new LotteryRepository().FetchAllByColor(color);

            if (lotto == null)
            {
                m_pUser.Send("ERROR: Could not fetch any lottery item");
                return false;
            }

            for (int i = lotto.Count - 1; i >= 0; i--)
            {
                if (lotto[i].Rank != rank && lotto[i].Rank <= 5)
                    lotto.RemoveAt(i);
            }

            if (lotto.Count <= 0)
            {
                m_pUser.Send("ERROR: no lottery item to sort");
                return false;
            }

            DbGameLottery dbLotto = lotto[ThreadSafeRandom.RandGet(lotto.Count) % lotto.Count];

            if (!Calculations.ChanceCalc(Math.Min((byte) 100, dbLotto.Chance)))
            {
                goto diceAgain;
            }

            DbItemtype itemtype = ServerKernel.Itemtype.Values.FirstOrDefault(x => x.Type == dbLotto.ItemIdentity);
            if (itemtype == null)
            {
                //if (retries > 3)
                //{
                //    m_pUser.Send("ERROR: exceded tries itemtype invalid");
                //    return false;
                //}
                //retries++;
                goto diceAgain;
            }

            DbItem dbItem = new DbItem
            {
                PlayerId = m_pUser.Identity,
                Type = itemtype.Type,
                Amount = itemtype.Amount,
                AmountLimit = itemtype.AmountLimit,
                Magic3 = (byte)(dbLotto.Plus > 0 ? dbLotto.Plus : itemtype.Magic3),
                Gem1 = (byte)(dbLotto.SocketNum > 0 ? 255 : 0),
                Gem2 = (byte)(dbLotto.SocketNum > 1 ? 255 : 0),
                OwnerId = 923,
                Color = 3,
                StackAmount = 1
            };

            if (m_pUser.Inventory.IsFull)
            {
                m_pUser.Send("Inventory Full.");
                return false;
            }

            if (!m_pUser.Inventory.Create(dbItem))
            {
                //if (retries > 3)
                //{
                //    m_pUser.Send("ERROR: exceded tries couldnt create item");
                //    return false;
                //}
                //retries++;
                goto diceAgain;
            } else
            {
                //Add Lottery Effects
                var sPacket = new MsgName
                {
                    Identity = m_pUser.InteractingNpc.Identity,
                    Action = StringAction.ROLE_EFFECT
                };
                sPacket.Append("lottery");
                m_pUser.Send(sPacket);
            }

            if (rank < 5)
            {
                ServerKernel.SendMessageToAll(string.Format("{0} won a {1} from the lottery.", m_pUser.Name, dbLotto.Itemname), ChatTone.TALK);
            }
            else
            {
                m_pUser.Send(string.Format("You won {0} from the lottery.", dbLotto.Itemname));
            }

            return true;
        }
        #endregion
        #region 1509 Subclass
        private bool ACTION_GENERA_SUBCLASS_MANAGEMENT(ActionStruct action)
        {
            if (m_pUser == null) return false;

            if (m_pUser.Level < 70 && m_pUser.Metempsychosis < 1)
            {
                m_pUser.Send(ServerString.STR_SUBPRO_LOW_LEVEL);
                return false;
            }

            string[] values = GetSafeParam(action.Param);

            if (values.Length < 2) return false;
            string opt = values[0];
            SubClasses subpro = (SubClasses) int.Parse(values[1]);
            int value = 0;

            if (values.Length > 2)
                value = int.Parse(values[2]);

            switch (opt)
            {
                case "learn":
                {
                    if (m_pUser.SubClass.Professions.ContainsKey(subpro))
                    {
                        m_pUser.Send(ServerString.STR_SUBPRO_ALREADY_LEARNED);
                        return false;
                    }

                    if (!Enum.IsDefined(typeof (SubClasses), subpro))
                        return false;

                    return m_pUser.SubClass.Create(subpro);
                }
                case "check": // check if exists
                {
                    return m_pUser.SubClass.Professions.ContainsKey(subpro);
                }
                case "level":
                {
                    if (!m_pUser.SubClass.Professions.ContainsKey(subpro))
                    {
                        m_pUser.Send(ServerString.STR_SUBPRO_NOT_LEARNED);
                        return false;
                    }

                    if (!Enum.IsDefined(typeof(SubClasses), subpro))
                        return false;

                    if (value >= 9) return false;
                    return m_pUser.SubClass.Professions[subpro].Level >= value;
                }
                case "pro":
                {
                    return m_pUser.SubClass.Professions.ContainsKey(subpro) && m_pUser.SubClass.Professions[subpro].Promotion <= value;
                }
                case "promote":
                {
                    if (!m_pUser.SubClass.Professions.ContainsKey(subpro))
                    {
                        m_pUser.Send(ServerString.STR_SUBPRO_NOT_LEARNED);
                        return false;
                    }

                    if (!Enum.IsDefined(typeof(SubClasses), subpro))
                        return false;

                    return m_pUser.SubClass.Promote(subpro);
                }
                case "uplev":
                {
                    if (!m_pUser.SubClass.Professions.ContainsKey(subpro))
                    {
                        m_pUser.Send(ServerString.STR_SUBPRO_NOT_LEARNED);
                        return false;
                    }

                    if (!Enum.IsDefined(typeof(SubClasses), subpro))
                        return false;

                    return m_pUser.SubClass.Uplev(subpro);
                }
                default:
                    return false;
            }
            return false;
        }
        #endregion
        #region 1510 Line Skill PK enter
        private bool ACTION_GENERAL_SKILL_LINE_ENABLED()
        {
            if (m_pUser == null)
                return false;
            return ServerKernel.LineSkillPk.IsEnterEnable(m_pUser);
        }
        #endregion
        #endregion
        #region 1600 Elite PK
        #region 1600 - Add User
        private bool ACTION_ELITEPK_INSCRIBE(ActionStruct action)
        {
            return true;
        }
        #endregion
        #endregion
        #region 2000 Event
        #region 2001 - Event Set Status
        private bool ACTION_EVENT_SETSTATUS(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 3) return false;

            uint idMap = uint.Parse(param[0]);
            int nStatus = int.Parse(param[1]);
            int nFlag = int.Parse(param[2]);

            Map map;
            if (!ServerKernel.Maps.TryGetValue(idMap, out map))
                return false;

            map.SetStatus((byte)nStatus, nFlag != 0);

            return true;
        }
        #endregion
        #region 2002 - Delete NPC by Generator ID
        private bool ACTION_EVENT_DELNPC_GENID(ActionStruct action)
        {
            Generator pGen = null;

            for (int i = 0; i < ServerKernel.Generators.Count; i++)
            {
                if (ServerKernel.Generators[i].Identity == action.Data)
                {
                    pGen = ServerKernel.Generators[i];
                    break;
                }
            }

            if (pGen == null) return false;

            foreach (var mob in pGen.Collection.Values)
            {
                mob.SendLeaveFromBlock();
            }

            pGen.Collection.Clear();

            return true;
        }
        #endregion
        #region 2003 - Event Compare
        private bool ACTION_EVENT_COMPARE(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 3)
                return false;

            int nData1 = int.Parse(param[0]), nData2 = int.Parse(param[2]);
            string szOpt = param[1];

            switch (szOpt)
            {
                case "==":
                    return nData1 == nData2;
                case "<":
                    return nData1 < nData2;
                case ">":
                    return nData1 > nData2;
                case "<=":
                    return nData1 <= nData2;
                case ">=":
                    return nData1 >= nData2;
            }
            return false;
        }
        #endregion
        #region 2004 - Event Compare Unsigned
        private bool ACTION_EVENT_COMPARE_UNSIGNED(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 3)
                return false;

            uint nData1 = uint.Parse(param[0]), nData2 = uint.Parse(param[2]);
            string szOpt = param[1];

            switch (szOpt)
            {
                case "==":
                    return nData1 == nData2;
                case "<":
                    return nData1 < nData2;
                case ">":
                    return nData1 > nData2;
            }
            return false;
        }
        #endregion
        #region 2006 - Create a fucking monster
        private bool ACTION_EVENT_CREATEPET(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 7) return false;

            uint dwOwnerType = uint.Parse(param[0]);
            uint idOwner = uint.Parse(param[1]);
            uint idMap = uint.Parse(param[2]);
            ushort usPosX = ushort.Parse(param[3]);
            ushort usPosY = ushort.Parse(param[4]);
            uint idGen = uint.Parse(param[5]);
            uint idType = uint.Parse(param[6]);
            uint dwData = 0;
            string szName = "";

            if (param.Length >= 8)
                dwData = uint.Parse(param[7]);
            if (param.Length >= 9)
                szName = param[8];

            DbMonstertype dbMonster;
            if (!ServerKernel.Monsters.TryGetValue(idType, out dbMonster) || !ServerKernel.Maps.ContainsKey(idMap))
                return false;

            Generator pGen = ServerKernel.Generators.FirstOrDefault(x => x.Identity == idGen);
            if (pGen == null)
            {
                pGen = new Generator(idMap, idType, usPosX, usPosY, 1, 1);
                try
                {
                    ServerKernel.Generators.Add(pGen);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            Monster pMonster = new Monster(dbMonster, ServerKernel.Maps[idMap].NextMonsterIdentity, pGen)
            {
                MapIdentity = idMap,
                MapX = usPosX,
                MapY = usPosY,
                Action = EntityAction.STAND,
                AttackHitRate = (ushort)dbMonster.AttackSpeed,
                AttackRange = dbMonster.AttackRange,
                Life = (uint)dbMonster.Life,
                Mana = (ushort)dbMonster.Mana,
                ViewRange = dbMonster.ViewRange,
                Level = (byte)dbMonster.Level,
                Lookface = dbMonster.Lookface,
                OwnerIdentity = idOwner,
                Map = ServerKernel.Maps[idMap]
            };
            return pGen.SpawnMob(pMonster);
        }
        #endregion
        #region 2007 - Create New Npc
        private bool EVENTCREATENPC(string msg)
        {
            string[] _params = GetSafeParam(msg);
            if (_params.Length < 9)
                return false;

            string szName = _params[0];
            ushort nType = ushort.Parse(_params[1]);
            ushort nSort = ushort.Parse(_params[2]);
            ushort nLookface = ushort.Parse(_params[3]);
            uint nOwnerType = uint.Parse(_params[4]);
            uint nOwner = uint.Parse(_params[5]);
            uint idMap = uint.Parse(_params[6]);
            ushort nPosX = ushort.Parse(_params[7]);
            ushort nPosY = ushort.Parse(_params[8]);
            uint nLife = 0;
            uint idBase = 0;
            uint idLink = 0;
            uint setTask0 = 0;
            uint setTask1 = 0;
            uint setTask2 = 0;
            uint setTask3 = 0;
            uint setTask4 = 0;
            uint setTask5 = 0;
            uint setTask6 = 0;
            uint setTask7 = 0;
            int setData0 = 0;
            int setData1 = 0;
            int setData2 = 0;
            int setData3 = 0;
            string szData = "";
            if (_params.Length > 9)
            {
                nLife = uint.Parse(_params[9]);
                if (_params.Length > 10)
                    idBase = uint.Parse(_params[10]);
                if (_params.Length > 11)
                    idLink = uint.Parse(_params[11]);
                if (_params.Length > 12)
                    setTask0 = uint.Parse(_params[12]);
                if (_params.Length > 13)
                    setTask1 = uint.Parse(_params[13]);
                if (_params.Length > 14)
                    setTask2 = uint.Parse(_params[14]);
                if (_params.Length > 15)
                    setTask3 = uint.Parse(_params[15]);
                if (_params.Length > 16)
                    setTask4 = uint.Parse(_params[16]);
                if (_params.Length > 17)
                    setTask5 = uint.Parse(_params[17]);
                if (_params.Length > 18)
                    setTask6 = uint.Parse(_params[18]);
                if (_params.Length > 19)
                    setTask7 = uint.Parse(_params[19]);
                if (_params.Length > 20)
                    setData0 = int.Parse(_params[20]);
                if (_params.Length > 21)
                    setData1 = int.Parse(_params[21]);
                if (_params.Length > 22)
                    setData2 = int.Parse(_params[22]);
                if (_params.Length > 23)
                    setData3 = int.Parse(_params[23]);
                if (_params.Length > 24)
                    szData = _params[24];
            }

            if (!ServerKernel.Maps.ContainsKey(idMap))
                return false;

            var npc = new DbDynamicNPC
            {
                Name = szName,
                Base = idBase,
                Cellx = nPosX,
                Celly = nPosY,
                Data0 = setData0,
                Data1 = setData1,
                Data2 = setData2,
                Data3 = setData3,
                Datastr = szData,
                Defence = 0,
                Life = nLife,
                Maxlife = nLife,
                Linkid = idLink,
                Task0 = setTask0,
                Task1 = setTask1,
                Task2 = setTask2,
                Task3 = setTask3,
                Task4 = setTask4,
                Task5 = setTask5,
                Task6 = setTask6,
                Task7 = setTask7,
                Ownerid = nOwner,
                Ownertype = nOwnerType,
                Lookface = nLookface,
                Type = nType,
                Mapid = idMap,
                Sort = nSort
            };
            Database.DynamicNpcRepository.SaveOrUpdate(npc);
            var newNpc = new DynamicNpc(npc);
            newNpc.Save();
            newNpc.SendToRange();
            if (!newNpc.Map.GameObjects.TryAdd(newNpc.Identity, newNpc))
                Console.WriteLine("ERROR: Adding new npc via action.");
            return true;
        }
        #endregion
        #region 2008 - Count Monsters
        private bool ACTION_EVENT_COUNTMONSTER(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 5)
                return false;

            uint idMap = uint.Parse(param[0]);
            string szField = param[1];
            string szData = param[2];
            string szOpt = param[3];
            int nNum = int.Parse(param[4]);
            int nCount = 0;

            switch (szField)
            {
                case "name":
                    {
                        foreach (var gen in ServerKernel.Generators.Where(x => x.MapIdentity == idMap))
                            if (gen.MonsterName == szData)
                                nCount += gen.Collection.Values.Count(x => x.IsAlive);
                        break;
                    }
                case "gen_id":
                    {
                        Generator pGen = ServerKernel.Generators.FirstOrDefault(x => x.Identity == uint.Parse(szData));
                        if (pGen == null) return false;
                        nCount += pGen.Collection.Values.Count(x => x.IsAlive);
                        break;
                    }
            }

            switch (szOpt)
            {
                case "==":
                    return nCount == nNum;
                case "<":
                    return nCount < nNum;
                case ">":
                    return nCount > nNum;
            }

            return false;
        }
        #endregion
        #region 2009 - Delete Monster
        private bool ACTION_EVENT_DELETEMONSTER(ActionStruct action)
        {
            string[] param = GetSafeParam(action.Param);

            if (param.Length < 2)
                return false;

            uint idMap = uint.Parse(param[0]);
            uint idType = uint.Parse(param[1]);
            int nData = 0;
            string szName = "";

            if (param.Length >= 3)
                nData = int.Parse(param[2]);
            if (param.Length >= 4)
                szName = param[3];

            bool ret = false;

            foreach (var pGen in ServerKernel.Generators.Where(x => x.MapIdentity == idMap))
            {
                if (idType == pGen.RoleType || pGen.MonsterName == szName)
                {
                    foreach (var pRole in pGen.Collection.Values)
                        pRole.SendLeaveFromBlock();
                    pGen.Collection.Clear();
                    ret = true;
                }
            }

            return ret;
        }
        #endregion
        #region 2010 - Event BBS
        private bool ACTION_EVENT_BBS(ActionStruct action)
        {
            MsgTalk pMsg = new MsgTalk(action.Param, ChatTone.SYSTEM);
            ServerKernel.SendMessageToAll(pMsg);
            return true;
        }
        #endregion
        #region 2011 - Event Erase
        private bool ACTIONEVENTERASE(string param)
        {
            uint idMap;
            uint nType;
            //string szName;
            string[] _params = GetSafeParam(param);
            if (_params.Count() < 2)
                return false;

            Map map;
            if (!ServerKernel.Maps.TryGetValue(uint.Parse(_params[0]), out map))
                return false;

            map.DelNpcByType(uint.Parse(_params[1]));
            return true;
        }
        #endregion
        #region 2012 - Teleport Map
        private bool ACTION_EVENT_TELEPORT(ActionStruct action)
        {
            uint idSource, idTarget;
            ushort usMapX, usMapY;

            string[] param = GetSafeParam(action.Param);

            if (param.Length < 4)
                return false;

            if (!uint.TryParse(param[0], out idSource) || !uint.TryParse(param[1], out idTarget)
                || !ushort.TryParse(param[2], out usMapX) || !ushort.TryParse(param[3], out usMapY))
                return false;

            Map pSourceMap, pTargetMap;
            if (!ServerKernel.Maps.TryGetValue(idSource, out pSourceMap)
                || !ServerKernel.Maps.TryGetValue(idTarget, out pTargetMap))
                return false;

            if (pSourceMap.IsTeleportDisable())
                return false;

            if (pTargetMap[usMapX, usMapY].Access <= TileType.MONSTER)
                return false;

            foreach (var plr in pSourceMap.Players.Values)
                plr.ChangeMap(usMapX, usMapY, idTarget);

            return true;
        }
        #endregion
        #region 2013 - Map Action
        private bool ACTION_EVENT_MASSACTION(ActionStruct action)
        {
            uint idMap, idAction;
            int nAmount;

            string[] param = GetSafeParam(action.Param);
            if (param.Length < 3)
                return false;

            if (!uint.TryParse(param[0], out idMap) || !uint.TryParse(param[1], out idAction)
                || !int.TryParse(param[2], out nAmount))
                return false;

            Map pTargetMap;
            if (!ServerKernel.Maps.TryGetValue(idMap, out pTargetMap))
                return false;

            if (nAmount <= 0)
                nAmount = int.MaxValue;

            foreach (var plr in pTargetMap.Players.Values)
                if (nAmount-- > 0)
                    plr.GameAction.ProcessAction(idAction, m_pUser, m_pRole, m_pItem, null);

            return true;
        }
        #endregion
        #region 2014 - Syndicate Score Finish
        private bool ACTION_EVENT_SYN_SCORE_FINISH()
        {
            if (m_pRole == null) return false;

            var pNpc = m_pRole as DynamicNpc;
            if (pNpc == null) return false;

            if (!pNpc.Map.IsSynMap()) return false;
            
            int nRank = 0;
            foreach (var pScore in pNpc.Scores.Values.OrderByDescending(x => x.Score))
            {
                if (nRank >= 4)
                    break;
                Syndicate syn;
                if (ServerKernel.Syndicates.TryGetValue(pScore.Identity, out syn))
                {
                    uint dwMoney = ServerKernel.SYN_SCORE_MONEY_REWARD[nRank];
                    uint dwEMoney = ServerKernel.SYN_SCORE_EMONEY_REWARD[nRank];
                    


                    nRank++;
                }
            }

            pNpc.Scores.Clear();
            return false;
        }
        #endregion
        #endregion
        #region 2200 Detained Items
        #region 2205 - Open Interface
        private bool ACTION_DETAIN_INTERFACE(ActionStruct action)
        {
            // todo send items
            switch (action.Param)
            {
                case "target":
                    {
                        // m_pUser.Send(new MsgAction(m_pUser.Identity, (uint) OpenWindow.DETAIN_REDEEM, m_pUser.MapX, m_pUser.MapY, GeneralActionType.OPEN_WINDOW));
                        return true;
                    }
                case "hunter":
                    {
                        // m_pUser.Send(new MsgAction(m_pUser.Identity, (uint)OpenWindow.DETAIN_CLAIM, m_pUser.MapX, m_pUser.MapY, GeneralActionType.OPEN_WINDOW));
                        return true;
                    }
            }
            return false;
        }
        #endregion
        #endregion

        private string[] GetSafeParam(string param)
        {
            return param.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string GetParenthesys(string szParam)
        {
            int varIdx = szParam.IndexOf("(", StringComparison.CurrentCulture) + 1;
            int endIdx = szParam.IndexOf(")", StringComparison.CurrentCulture);
            return szParam.Substring(varIdx, endIdx - varIdx);
        }

        private byte VarId(string szParam)
        {
            int varIdx = szParam.IndexOf("(", StringComparison.CurrentCulture) + 1;
            return byte.Parse(szParam.Substring(varIdx, 1));
        }

        private string VariableReplace(string param)
        {
            if (m_pUser != null)
            {
                param = param.Replace("%user_id", m_pUser.Identity.ToString());
                param = param.Replace("%user_name", m_pUser.Name);
                param = param.Replace("%user_mate", m_pUser.Mate);
                param = param.Replace("%user_lev", m_pUser.Level.ToString());
                param = param.Replace("%account_id", m_pUser.Owner.Identity.ToString());
                param = param.Replace("%user_pro", m_pUser.Profession.ToString());
                param = param.Replace("%map_name", m_pUser.Map.Name);
                param = param.Replace("%user_map_x", m_pUser.MapX.ToString());
                param = param.Replace("%user_map_y", m_pUser.MapY.ToString());
                param = param.Replace("%user_map_id", m_pUser.MapIdentity.ToString());
                param = param.Replace("%user_virtue", m_pUser.VirtuePoints.ToString());
                // Item
                param = param.Replace("%item_data", m_pUser.LastUsedItem.ToString());
                param = param.Replace("%item_type", m_pUser.LastUsedItemtype.ToString());
                param = param.Replace("%item_time", m_pUser.LastUsedItemTime.ToString());
                param = param.Replace("%item_res", m_pUser.LastItemResource.ToString());
                // variables
                param = param.Replace("%iter_var_data0", m_pUser.IterData0.ToString());
                param = param.Replace("%iter_var_data1", m_pUser.IterData1.ToString());
                param = param.Replace("%iter_var_data2", m_pUser.IterData2.ToString());
                param = param.Replace("%iter_var_data3", m_pUser.IterData3.ToString());
                param = param.Replace("%iter_var_data4", m_pUser.IterData4.ToString());
                param = param.Replace("%iter_var_data5", m_pUser.IterData5.ToString());
                param = param.Replace("%iter_var_data6", m_pUser.IterData6.ToString());
                param = param.Replace("%iter_var_data7", m_pUser.IterData7.ToString());
                param = param.Replace("%iter_var_str0", m_pUser.IterString0);
                param = param.Replace("%iter_var_str1", m_pUser.IterString1);
                param = param.Replace("%iter_var_str2", m_pUser.IterString2);
                param = param.Replace("%iter_var_str3", m_pUser.IterString3);
                param = param.Replace("%iter_var_str4", m_pUser.IterString4);
                param = param.Replace("%iter_var_str5", m_pUser.IterString5);
                param = param.Replace("%iter_var_str6", m_pUser.IterString6);
                param = param.Replace("%iter_var_str7", m_pUser.IterString7);
                // other
                if (param.Contains("%levelup_exp"))
                {
                    DbLevexp db = ServerKernel.Levelxp.Values.FirstOrDefault(x => x.Level == m_pUser.Level);
                    if (db != null)
                        param = param.Replace("%levelup_exp", db.Exp.ToString());
                    else
                        param = param.Replace("%levelup_exp", "0");
                }
                // Map
                param = param.Replace("%map_owner_id", m_pUser.Map.OwnerIdentity.ToString());
                if (param.Contains("%iter_upquality_gem"))
                {
                    Item item;
                    if (m_pUser.Equipment.Items.TryGetValue((ItemPosition)m_pUser.Iterator, out item))
                        param = param.Replace("%iter_upquality_gem", item.GetUpQualityGemAmount().ToString());
                    else
                        param = param.Replace("%iter_upquality_gem", "0");
                }
                if (param.Contains("%iter_itembound"))
                {
                    Item item;
                    if (m_pUser.Equipment.Items.TryGetValue((ItemPosition)m_pUser.Iterator, out item))
                        param = param.Replace("%iter_itembound", item.Bound ? "1" : "0");
                    else
                        param = param.Replace("%iter_itembound", "0");
                }
                if (param.Contains("%iter_uplevel_gem"))
                {
                    Item item;
                    if (m_pUser.Equipment.Items.TryGetValue((ItemPosition)m_pUser.Iterator, out item))
                        param = param.Replace("%iter_uplevel_gem", item.GetUpgradeGemAmount().ToString());
                    else
                        param = param.Replace("%iter_uplevel_gem", "0");
                }

                if (m_pUser.Syndicate != null)
                {
                    param = param.Replace("%syn_id", m_pUser.Syndicate.Identity.ToString());
                    param = param.Replace("%syn_name", m_pUser.Syndicate.Name);
                    param = param.Replace("%syn_leadername", m_pUser.Syndicate.LeaderName);
                    param = param.Replace("%syn_level", "0");
                    param = param.Replace("%syn_membernum", m_pUser.Syndicate.MemberCount.ToString());
                    param = param.Replace("%syn_allynum", m_pUser.Syndicate.Allies.Count.ToString());
                    param = param.Replace("%syn_enemynum", m_pUser.Syndicate.Enemies.Count.ToString());
                }
                else
                {
                    param = param.Replace("%syn_id", "0");
                    param = param.Replace("%syn_name", "None");
                    param = param.Replace("%syn_leadername", "None");
                    param = param.Replace("%syn_level", "0");
                    param = param.Replace("%syn_membernum", "0");
                    param = param.Replace("%syn_allynum", "0");
                    param = param.Replace("%syn_enemynum", "0");
                }

                if (m_pUser.Family != null)
                {
                    param = param.Replace("%family_id", m_pUser.FamilyIdentity.ToString());
                    param = param.Replace("%family_name", m_pUser.FamilyName);
                    param = param.Replace("%family_leadername", m_pUser.Family.LeaderName);
                    param = param.Replace("%family_level", (m_pUser.Family.Level).ToString());
                    param = param.Replace("%family_bp_lev", m_pUser.Family.BpTower.ToString());
                    param = param.Replace("%family_funds", m_pUser.Family.MoneyFunds.ToString());
                    param = param.Replace("%family_bp_perc", m_pUser.Family.SharedPercent.ToString());
                }
                else
                {
                    param = param.Replace("%family_id", "0");
                    param = param.Replace("%family_name", "None");
                    param = param.Replace("%family_leadername", "None");
                    param = param.Replace("%family_level", "0");
                    param = param.Replace("%family_bp_lev", "0");
                    param = param.Replace("%family_funds", "0");
                    param = param.Replace("%family_bp_perc", "0");
                }

                if (m_pUser.InteractingNpc != null)
                {
                    var pNpc = m_pUser.InteractingNpc;
                    var dynaNpc = pNpc as DynamicNpc;
                    var nNpc = pNpc as GameNpc;
                    if (dynaNpc != null)
                    {
                        param = param.Replace("%npc_ownerid", dynaNpc.OwnerIdentity.ToString());
                        param = param.Replace("%data0", dynaNpc.Data0.ToString());
                        param = param.Replace("%data1", dynaNpc.Data1.ToString());
                        param = param.Replace("%data2", dynaNpc.Data2.ToString());
                        param = param.Replace("%data3", dynaNpc.Data3.ToString());
                    }
                    else if (nNpc != null)
                    {
                        param = param.Replace("%npc_ownerid", nNpc.OwnerIdentity.ToString());
                        param = param.Replace("%data0", nNpc.Data0.ToString());
                        param = param.Replace("%data1", nNpc.Data1.ToString());
                        param = param.Replace("%data2", nNpc.Data2.ToString());
                        param = param.Replace("%data3", nNpc.Data3.ToString());
                    }
                    else
                    {
                        param = param.Replace("%npc_ownerid", "0");
                        param = param.Replace("%data0", "0");
                        param = param.Replace("%data1", "0");
                        param = param.Replace("%data2", "0");
                        param = param.Replace("%data3", "0");
                    }
                }
            }
            if (m_pRole != null && m_pRole is Monster)
            {
                Monster pTemp = m_pRole as Monster;
                param = param.Replace("%user_name", pTemp.Name);
                param = param.Replace("%mst_id", pTemp.Identity.ToString());
                param = param.Replace("%mst_name", pTemp.Name);
                param = param.Replace("%mst_type", pTemp.Type.ToString());
                param = param.Replace("%mst_owner_id", pTemp.OwnerIdentity.ToString());
            }
            param = param.Replace("%iter_rand", ThreadSafeRandom.RandGet().ToString());
            param = param.Replace("%timestamp", UnixTimestamp.Timestamp().ToString());
            param = param.Replace("%time_stamp", UnixTimestamp.Timestamp().ToString());
            return param;
        }
    }
}
