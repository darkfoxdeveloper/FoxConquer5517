// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - MsgServer - 10010 - MsgAction.cs
// Last Edit: 2017/02/09 19:57
// Created: 2017/02/04 14:45

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Enums;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleAction(Client pClient, MsgAction pMsg)
        {
            if (pClient == null)
                return;

            Character pUser = pClient.Character;
            switch (pMsg.Action)
            {
                #region 74 - Set Location

                case GeneralActionType.SET_LOCATION:
                {
                    try
                    {
                        pMsg.X = pUser.MapX;
                        pMsg.Y = pUser.MapY;
                        pMsg.Data = pUser.MapIdentity;
                        pMsg.Details = 2;

                        Map map;
                        if (!ServerKernel.Maps.TryGetValue(pUser.MapIdentity, out map))
                        {
                            map = ServerKernel.Maps[1002];
                            pUser.MapIdentity = pMsg.Data = map.Identity;
                            pUser.MapX = pMsg.X = 430;
                            pUser.MapY = pMsg.Y = 378;
                            pUser.Elevation = map[430, 378].Elevation;
                            pMsg.Details = 2;
                        }
                        else
                        {
                            pMsg.Data = map.MapDoc;
                        }

                        Tile tile = map[pMsg.X, pMsg.Y];
                        if (tile.Access <= TileType.PORTAL)
                        {
                            map = ServerKernel.Maps[1002];
                            pUser.MapIdentity = pMsg.Data = map.Identity;
                            pUser.MapX = pMsg.X = 430;
                            pUser.MapY = pMsg.Y = 378;
                            pUser.Elevation = map[430, 378].Elevation;
                            pMsg.Details = 2;
                        }

                        pUser.Send(new MsgMapInfo(map.Identity, map.MapDoc, map.WarFlag));
                        pUser.Send(pMsg);
                        map.AddClient(pUser);
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog(string.Format("Join map thread failed ({0}){1}", pUser.Identity,
                            pUser.Name));
                        pUser.Disconnect();
                    }
                    break;
                }

                    #endregion
                #region 75 - Set Hotkeys

                case GeneralActionType.HOTKEYS:
                {
                    #region Create Warehouses

                    foreach (uint i in ServerKernel.SystemWarehouses)
                        if (pClient.Character.Warehouses.ContainsKey(i))
                            {
                                ServerKernel.Log.GmLog(@"character_loading", string.Format("Failed to create warehouse [{0}] for user [{1}]", i, pClient.Identity));
                            }
                        else
                            {
                                pClient.Character.Warehouses.Add(i, new Warehouse(pClient.Character, (ushort)i));
                            }

                    #endregion

                    #region Items

                    ICollection<DbItem> allUserItems = new ItemRepository().FetchByUser(pClient.Character.Identity).OrderBy(x => x.Position).ToList();
                    if (allUserItems != null)
                    {
                        foreach (DbItem item in allUserItems)
                        {
                            if (!ServerKernel.Itemtype.ContainsKey(item.Type))
                            {
                                ServerKernel.Log.GmLog("DeleteItem",
                                    string.Format(
                                        "Item [Id:{0}][Type:{8}][Pos:{9}][Plus:{1}][Dura:{2}/{3}][Enchant:{4}][Bless:{5}][Plunder:{6}][Data:{7}][Artifact:{10}][Stabilization:{11}][Refinery:{12}][Stabilization:{13}]"
                                        , item.Id, item.Magic3, item.Amount, item.AmountLimit, item.AddLife,
                                        item.ReduceDmg,
                                        item.Plunder, item.Data, item.Type, item.Position, item.ArtifactType,
                                        item.ArtifactStabilization, item.RefineryType, item.RefineryStabilization));
                                Database.Items.Delete(item);
                                continue;
                            }
                            Item item0 = new Item(pClient.Character, item);
                            if (item.Position == 0)
                            {
                                // Inventory
                                if (!pClient.Character.Inventory.Add(item0, false))
                                    ServerKernel.Log.GmLog("character_loading",
                                        string.Format("Error to load item [{0}] for user [{1}] position [{2}]",
                                            item.Id,
                                            pClient.Identity, item.Position));
                                continue;
                            }
                            if (item.Position > 0 && item.Position <= 30)
                            {
                                // Equipment
                                if (!pClient.Character.Equipment.Add(item0, true))
                                    ServerKernel.Log.GmLog("character_loading",
                                        string.Format("Error to load item [{0}] for user [{1}] position [{2}]",
                                            item.Id,
                                            pClient.Identity, item.Position));
                                continue;
                            }
                            if (item.Position >= 230 && item.Position <= 236)
                            {
                                // Warehouse
                                try
                                {
                                    if (
                                        !pClient.Character.Warehouses[item.Position].Items.ContainsKey(
                                            item0.Identity))
                                        pClient.Character.Warehouses[item.Position].Add(item0, true);
                                }
                                catch (Exception ex)
                                {
                                    ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.ERROR);
                                }
                                // TODO Handle house wh...
                                // TODO before the other todo, add house system lol
                            }
                            #region Sash
                            if (item.Position == (uint)ItemPosition.SASH_S || item.Position == (uint)ItemPosition.SASH_M || item.Position == (uint)ItemPosition.SASH_L)
                            {
                                try
                                {
                                    if (!pClient.Character.Warehouses[item.Position].Items.ContainsKey(item0.Identity))
                                    {
                                        pClient.Character.Warehouses[item.Position].Add(item0, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.ERROR);
                                }
                            }
                            #endregion
                        }
                    }

                    var detained = DetainedObject.FindByHunter(pUser.Identity);
                    var reward = DetainedObject.FindRewards(pUser.Identity);
                    var redeem = DetainedObject.FindByTarget(pUser.Identity);

                    if (detained != null)
                    {
                        foreach (var item in detained)
                        {
                            pUser.Send(item.GetPacket());
                        }
                    }
                    if (reward != null)
                    {
                        foreach (var item in reward)
                        {
                            pUser.Send(item.GetPacket(DetainMode.CLAIM_PAGE));
                            pUser.Send(item.GetPrizePacket());
                        }
                    }
                    if (redeem != null)
                    {
                        foreach (var item in redeem)
                        {
                            pUser.Send(item.GetPacket(DetainMode.DETAIN_PAGE));
                        }
                    }

                    #endregion

                    SubclassLogin(pUser);
                    pUser.Nobility.SendNobilityIcon();
                    pUser.Send(pMsg);
                    break;
                }

                    #endregion
                #region 76 - Confirm Friends

                case GeneralActionType.CONFIRM_FRIENDS:
                {
                    #region Mentor/Friend/Trade Partner

                    var friend = new FriendRepository().GetUserFriends(pClient.Character.Identity);
                    if (friend != null)
                    {
                        foreach (var fr in friend)
                        {
                            pClient.Character.AddFriend(fr);
                        }
                        var msg = new MsgFriend
                        {
                            Identity = pClient.Character.Identity,
                            Name = pClient.Character.Name,
                            Online = true,
                            Mode = RelationAction.SET_ONLINE_FRIEND
                        };
                        foreach (var fr in pClient.Character.Friends.Values.Where(x => x.IsOnline))
                            fr.User.Send(msg);
                    }

                    var enemy = new EnemyRepository().GetUserEnemies(pClient.Character.Identity);
                    if (enemy != null)
                    {
                        foreach (var en in enemy)
                        {
                            pClient.Character.AddEnemy(en);
                        }
                        var msg = new MsgFriend
                        {
                            Identity = pClient.Character.Identity,
                            Name = pClient.Character.Name,
                            Online = true,
                            Mode = RelationAction.SET_ONLINE_ENEMY
                        };
                        foreach (var en in ServerKernel.Players.Values)
                        {
                            if (en.Character != null)
                            {
                                if (en.Character.Enemies != null
                                    && en.Character.Enemies.ContainsKey(pClient.Character.Identity))
                                    en.Send(msg);
                            }
                        }
                    }

                    var buddy = new BusinessRepository().FetchByUser(pClient.Identity);
                    if (buddy != null)
                    {
                        foreach (var tp in buddy)
                        {
                            TradePartner tradeBuddy = new TradePartner(pClient.Character, tp);
                            pClient.Send(tradeBuddy.ToArray(TradePartnerType.ADD_PARTNER));
                            pClient.Character.TradePartners.TryAdd(tp.Business, tradeBuddy);
                        }
                    }

                    var apprentices = new MentorRepository().FetchApprentices(pClient.Character.Identity);
                    if (apprentices != null)
                    {
                        foreach (var app in apprentices)
                        {
                            Student obj = new Student(pClient.Character);
                            if (
                                !obj.Create(app.StudentIdentity,
                                    uint.Parse(UnixTimestamp.ToDateTime(app.Date).ToString("yyyymmdd"))))
                                continue;

                            pClient.Character.Apprentices.TryAdd(obj.Identity, obj);
                            if (obj.IsOnline && obj.Role.SharedBattlePower > 0)
                            {
                                obj.Role.Send(string.Format(ServerString.STR_GUIDE_TUTOR_LOGIN,
                                    pClient.Character.Name));
                                obj.Role.SendExtraBattlePower();
                            }
                        }
                    }

                    var mentor = new MentorRepository().FetchMentor(pClient.Character.Identity);
                    if (mentor != null)
                    {
                        Guide guide = new Guide(pClient.Character);
                        if (!guide.Create(mentor.GuideIdentity,
                                uint.Parse(UnixTimestamp.ToDateTime(mentor.Date).ToString("yyyymmdd"))))
                        {
                            ServerKernel.Log.SaveLog(
                                string.Format("Failed to create Guide.Create(Character {0})", mentor.Identity),
                                false, LogType.WARNING);
                        }
                        else
                        {
                            if (mentor.BetrayalFlag > 0 && UnixTimestamp.Timestamp() > mentor.BetrayalFlag)
                            {
                                new MentorRepository().Delete(mentor);
                                guide = null;
                            }
                            else
                            {
                                pClient.Character.Mentor = guide;
                                if (guide.IsOnline)
                                {
                                    guide.Role.Send(string.Format(ServerString.STR_GUIDE_STUDENT_LOGIN,
                                        pClient.Character.Name));
                                    ushort shared = pClient.Character.SharedBattlePower;
                                    if (shared > 0)
                                        pClient.Character.UpdateClient(ClientUpdateType.EXTRA_BATTLE_POWER, shared);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Load Statistics

                    var list = new StatisticRepository().FetchList(pClient.Character.Identity);
                    if (list != null)
                    {
                        foreach (var st in list)
                        {
                            pClient.Character.Statistics.TryAdd(st.EventType + (st.DataType << 32), st);
                        }
                    }

                    #endregion

                    pUser.DailyReset();

                    pUser.PkExploit.Create();

                    pClient.Character.GameAction.ProcessAction(1000000, pClient.Character, null, null, "");

                    pUser.Send(pMsg);
                    break;
                }

                    #endregion
                #region 77 - Confirm Weapon Skill

                case GeneralActionType.CONFIRM_PROFICIENCIES:
                {
                    pUser.WeaponSkill.SendAll();
                    pUser.RecalculateAttributes();
                    pUser.Send(pMsg);
                    break;
                }

                    #endregion
                #region 78 - Confirm Spells

                case GeneralActionType.CONFIRM_SPELLS:
                {
                    pUser.LoadMagics();

                    if (pUser.Metempsychosis == 2 && !pUser.Magics.CheckType(9876))
                        pUser.Magics.Create(9876, 0);

                    if (pClient.Character.Gender == 1
                        && pClient.Character.Level >= 40
                        && pClient.Character.RedRoses < uint.Parse(DateTime.Now.ToString("yyyyMMdd")))
                    {
                        pClient.Send(new MsgAction(pClient.Identity, 1244u, 0, 0, GeneralActionType.OPEN_CUSTOM));
                    }

                    if (pClient.Character.CoinMoney > 0)
                        pClient.Send(new MsgAction(pClient.Identity, 1197u, 0, 0, GeneralActionType.OPEN_CUSTOM));

                    pClient.Send(pMsg);
                    break;
                }

                    #endregion
                #region 79 - Change Direction

                case GeneralActionType.CHANGE_DIRECTION:
                {
                    pUser.Direction = pMsg.Direction;
                    pUser.Screen.Send(pMsg, true);
                    break;
                }

                    #endregion
                #region 81 - Change Action

                case GeneralActionType.CHANGE_ACTION:
                {
                    pUser.Action = (EntityAction) pMsg.Data;

                    if (pUser.Action == EntityAction.COOL && pUser.IsCoolEnabled)
                    {
                        //string effect = string.Empty;
                        int sum = pUser.ItemPowerSum;
                        if (sum == 40)
                        {
                            pMsg.Data = pMsg.Details |= (uint) (pUser.Profession*0x00010000 + 0x01000000);
                        }
                        else if (sum > 18)
                        {
                            pMsg.Details |= (uint) (pUser.Profession*0x010000);
                        }
                        //if (pUser.Profession / 10 == 1)
                        //{
                        //    if (sum == 36)
                        //    {
                        //        effect = "warrior";
                        //    }
                        //    else if (sum > 18)
                        //    {
                        //        effect = "warrior-s";
                        //    }
                        //}
                        //else if (pUser.Profession / 10 == 2)
                        //{
                        //    if (sum == 36)
                        //    {
                        //        effect = "fighter";
                        //    }
                        //    else if (sum > 18)
                        //    {
                        //        effect = "fighter-s";
                        //    }
                        //}
                        //else if (pUser.Profession / 10 == 4)
                        //{
                        //    if (sum == 36)
                        //    {
                        //        effect = "archer";
                        //    }
                        //    else if (sum > 18)
                        //    {
                        //        effect = "archer-s";
                        //    }
                        //}
                        //else if (pUser.Profession / 10 >= 10)
                        //{
                        //    if (sum == 36)
                        //    {
                        //        effect = "taoist";
                        //    }
                        //    else if (sum > 18)
                        //    {
                        //        effect = "taoist-s";
                        //    }
                        //}
                        //if (!string.IsNullOrEmpty(effect))
                        //    pUser.SendEffect(effect, true);
                    }
                    pUser.Screen.Send(pMsg, true);
                    break;
                }

                    #endregion
                #region 85 - Use Portal

                case GeneralActionType.USE_PORTAL:
                {
                    try
                    {
                        Tile tile = pUser.Map[pMsg.LeftData, pMsg.RightData];
                        
                        if (tile.Access != TileType.PORTAL)
                        {
                            if (pUser.Map.RevivePoint.Value.X != 0
                                && pUser.Map.RevivePoint.Value.Y != 0)
                            {
                                pUser.ChangeMap((ushort) pUser.Map.RevivePoint.Value.X,
                                    (ushort) pUser.Map.RevivePoint.Value.Y,
                                    pUser.Map.RebornMapIdentity > 0 ? pUser.Map.RebornMapIdentity : 1002u);
                            }
                            else
                            {
                                pUser.ChangeMap(430, 378, 1002);
                            }
                            return;
                        }
                        if (Calculations.GetDistance(pUser.MapX, pUser.MapY,
                            pMsg.LeftData, pMsg.RightData) > 2)
                        {
                            pUser.Send(new MsgTalk(ServerString.STR_INVALID_JUMP, ChatTone.TOP_LEFT));
                            pUser.Disconnect();
                            return;
                        }

                        IPassway pass =
                            pUser.Map.Portals.Values.FirstOrDefault(
                                x => x.PasswayMap == pUser.MapIdentity && x.PasswayIndex == tile.Index);

                        if (pass != null)
                        {
                            pUser.ChangeMap((ushort) pass.PortalX, (ushort) pass.PortaLy, pass.PortalMap);
                            return;
                        }

                        ServerKernel.Log.SaveLog(string.Format("Portal idx [{0}] not found on map [{1}]", tile.Index,
                            pUser.MapIdentity));
                        ServerKernel.Log.SaveLog(
                            string.Format("mapid:{0},portal:{1},x:{2},y:{3}", pUser.Map.Identity, tile.Index,
                                pMsg.LeftData, pMsg.RightData), true, "PortalUnhandled", LogType.WARNING);
                        pUser.ChangeMap(430, 378, 1002);
                    }
                    catch
                    {
                        pUser.Send(ServerString.STR_INVALID_COORDINATE);
                        pUser.Disconnect();
                    }
                    break;
                }

                    #endregion
                #region 93 - Xp Clear

                case GeneralActionType.XP_CLEAR:
                {
                    pClient.Character.DetachStatus(FlagInt.START_XP);
                    break;
                }

                #endregion
                #region 94 - Revive

                case GeneralActionType.REVIVE:
                {
                    if (pClient.Character.IsAlive || !pClient.Character.CanRevive())
                        return;

                    pClient.Character.Reborn(pMsg.Data == 0);
                    break;
                }

                #endregion
                #region 95 - Delete Role

                case GeneralActionType.DEL_ROLE:
                {
                    if (pUser.WarehousePassword > 0 && pMsg.Data != pUser.WarehousePassword)
                        return;

                    pUser.Disconnect();
                    pUser.Delete();
                    Database.Characters = new CharacterRepository();
                    break;
                }

                    #endregion
                #region 96 - Change PK Mode

                case GeneralActionType.CHANGE_PK_MODE:
                {
                    pUser.PkMode = (PkModeType) pMsg.LeftData;
                    pUser.Send(pMsg);
                    break;
                }

                    #endregion
                #region 97 - Confirm Syndicate

                case GeneralActionType.CONFIRM_GUILD:
                {
                    try
                    {
                        DbCqSynattr syndicate = new SyndicateMembersRepository().FetchByUser(pClient.Character.Identity);
                        if (syndicate != null)
                        {
                            Syndicate pSyn;
                            if (ServerKernel.Syndicates.TryGetValue(syndicate.SynId, out pSyn))
                            {
                                pClient.Character.Syndicate = pSyn;
                                pClient.Character.SyndicateMember = pClient.Character.Syndicate.Members[pClient.Character.Identity];
                                pClient.Character.SyndicateIdentity = pClient.Character.Syndicate.Identity;
                                pClient.Character.SyndicateRank = pClient.Character.SyndicateMember.Position;
                                pClient.Character.SyndicateMember.SendSyndicate();
                                pClient.Character.Syndicate.SendName(pClient.Character);
                                pClient.Character.Syndicate.SendRelation(pClient.Character);

                                if (pClient.Character.Syndicate.Arsenal != null)
                                    pClient.Character.UpdateClient(ClientUpdateType.GUILD_BATTLEPOWER, pClient.Character.Syndicate.Arsenal.SharedBattlePower(pClient));
                            }
                        }

                        DbFamilyMember family = new FamilyMemberRepository().FetchByUser(pClient.Character.Identity);
                        if (family != null)
                        {
                            Family pFamily;
                            if (ServerKernel.Families.TryGetValue(family.FamilyIdentity, out pFamily))
                            {
                                pClient.Character.Family = pFamily;
                                pClient.Character.FamilyMember = pFamily.Members.Values.FirstOrDefault(x => x.Identity == pClient.Character.Identity);
                                pClient.Character.FamilyIdentity = pClient.Character.FamilyIdentity;
                                pClient.Character.FamilyPosition = pClient.Character.FamilyMember.Position;
                                pClient.Character.FamilyName = pClient.Character.Family.Name;
                                pClient.Character.SetNames();
                                pClient.Character.Family.SendFamily(pClient.Character);
                                pClient.Character.Family.SendRelation(pClient.Character);
                            }
                        }
                    }
                    catch (Exception ex) { ServerKernel.Log.SaveLog("ERROR: Failed to load syndicate user. Console error: " + ex); }

                    pUser.Send(pMsg);
                    break;
                }

                    #endregion
                #region 99 - Mine

                case GeneralActionType.MINE:
                {
                    if (!pClient.Character.IsAlive)
                    {
                        pClient.Character.Send(ServerString.STR_DIE);
                        return;
                    }

                    if (!pClient.Character.Map.IsMineField())
                    {
                        pClient.Character.Send(ServerString.STR_NO_MINE);
                        return;
                    }

                    pClient.Character.Mine();
                    break;
                }

                #endregion
                #region 102 - Request Entity Spawn

                case GeneralActionType.REQUEST_ENTITY_SPAWN:
                {
                    Character pTarget;
                    if (pUser.Map.Players.TryGetValue(pMsg.Data, out pTarget)
                        &&
                        Calculations.InScreen(pUser.MapX, pUser.MapY, pTarget.MapX,
                            pTarget.MapY))
                        pUser.ExchangeSpawnPackets(pTarget);

                    break;
                }

                    #endregion
                #region 106 - Query Team Member

                case GeneralActionType.QUERY_TEAM_MEMBER:
                {
                    if (pClient.Character.Team == null) return;
                    pClient.Character.Team.SendMemberPosition(pClient.Character, pMsg);
                    break;
                }

                #endregion
                #region 111 - Create Booth

                case GeneralActionType.CREATE_BOOTH:
                {
                    if (!pClient.Character.Map.IsBoothEnable())
                        return;

                    if (pClient.Character.Booth.Vending)
                    {
                        pClient.Character.Booth.Destroy();
                    }
                    if (!pClient.Character.Booth.Create()) return;

                    pMsg.Data = pClient.Character.Booth.Identity;
                    pMsg.X = pClient.Character.Booth.MapX;
                    pMsg.Y = pClient.Character.Booth.MapY;
                    pClient.Send(pMsg);
                    break;
                }

                #endregion
                #region 114 - Get Surroundings

                case GeneralActionType.GET_SURROUNDINGS:
                {
                    if (pClient.Character.Booth != null && pClient.Character.Booth.Vending)
                    {
                        pClient.Character.Booth.Destroy();
                        pClient.Send(pMsg);
                    }

                    pUser.UpdateClient(ClientUpdateType.VIP_LEVEL, pUser.Owner.VipLevel, false);
                    pUser.Send(new MsgVipFunctionValidNotify());
                    pUser.Screen.LoadSurroundings();
                    pUser.Screen.RefreshSpawnForObservers();
                    pUser.Send(pMsg);
                    break;
                }

                    #endregion
                #region 118 - Abort Transformation
                case GeneralActionType.ABORT_TRANSFORM:
                {
                    if (pClient.Character.QueryTransformation != null)
                        pClient.Character.ClearTransformation();
                    break;
                }
                #endregion
                #region 120 - End Fly

                case GeneralActionType.END_FLY:
                {
                    if (pUser.QueryStatus(FlagInt.FLY) != null)
                        pUser.DetachStatus(FlagInt.FLY);
                    break;
                }

                #endregion
                #region 117 - Observe Equipment
                case GeneralActionType.OBSERVE_EQUIPMENT:
                {
                    Client pTarget, pObserver;
                    if (ServerKernel.Players.TryGetValue(pMsg.Data, out pTarget)
                        && ServerKernel.Players.TryGetValue(pMsg.Identity, out pObserver))
                    {
                        foreach (var item in pTarget.Character.Equipment.Items.Values.Where(x => x.Position <= ItemPosition.CROP))
                        {
                            pClient.Send(item.BuildViewItem());
                            item.SendPurification(pClient.Character);
                        }

                        pTarget.Character.Send(string.Format("{0} is observing your gears carefully.", pClient.Character.Name), ChatTone.TALK);

                        var pStr = new MsgName
                        {
                            Identity = pTarget.Identity,
                            Action = StringAction.QUERY_MATE
                        };
                        pStr.Append(pTarget.Character.Mate);
                        pUser.Send(pStr);
                        pStr.Action = StringAction.ROLE_EFFECT;
                        pUser.Send(pStr);
                        pClient.Send(pMsg);
                    }
                    break;
                }
                #endregion
                #region 123 - View Enemy Information

                case GeneralActionType.VIEW_ENEMY_INFO:
                {
                    Relationship pRel;
                    if (!pClient.Character.Enemies.TryGetValue(pMsg.Data, out pRel))
                    {
                        pClient.Send(pMsg);
                        return;
                    }

                    MsgFriendInfo pMsgInfo = new MsgFriendInfo
                    {
                        Identity = pRel.Identity,
                        IsEnemy = true
                    };
                    if (pRel.IsOnline)
                    {
                        Character pRole = pRel.User.Character;
                        pMsgInfo.Mate = pRole.Mate;
                        pMsgInfo.Level = pRole.Level;
                        pMsgInfo.Mesh = pRole.Lookface;
                        pMsgInfo.SyndicateIdentity = (ushort)pRole.SyndicateIdentity;
                        pMsgInfo.SyndicateRank = pRole.SyndicateRank;
                        pMsgInfo.Profession = (byte)pRole.Profession;
                        pMsgInfo.PkPoints = pRole.PkPoints;
                        pMsgInfo.Identity = pRole.Identity;
                    }
                    pClient.Send(pMsgInfo);
                    break;
                }

                #endregion
                #region 132 - Complete Login

                case GeneralActionType.COMPLETE_LOGIN:
                {
                    Console.WriteLine("132 complete login");
                    break;
                }

                    #endregion
                #region 137 - Jump

                case GeneralActionType.JUMP:
                {
                    double distance = Calculations.GetDistance(pMsg.LeftData, pMsg.RightData, pUser.MapX,
                        pUser.MapY);

                    uint dwVigorConsume = 0;
                    bool bCanJump = true;
                    if (pClient.Character.QueryStatus(FlagInt.RIDING) != null)
                    {
                        dwVigorConsume = (uint) (1*distance);
                        if (dwVigorConsume > 0 && dwVigorConsume > pClient.Character.Vigor)
                            bCanJump = false;
                    }

                    if (!pUser.IsFreeze && pUser.IsAlive && bCanJump
                        && !(pClient.Character.QueryStatus(FlagInt.CTF_FLAG) != null
                    && pClient.Character.Map.QueryRegion(RegionType.REGION_PK_PROTECTED, pMsg.LeftData, pMsg.RightData)))
                    {
                        Tile tile = pUser.Map[pMsg.LeftData, pMsg.RightData];
                        if (tile.Access <= TileType.MONSTER)
                        {
                            pUser.Kickback(pUser.MapX, pUser.MapY);
                            pUser.Send(ServerString.STR_MOVE_UNABLE);
                            return;
                        }

                        if (Calculations.InScreen(pMsg.LeftData, pMsg.RightData, pUser.MapX,
                            pUser.MapY))
                        {
                            int deltaX = pMsg.LeftData - pUser.MapX;
                            int deltaY = pMsg.RightData - pUser.MapY;
                            if (
                                !pUser.Map.SampleElevation((int) distance, pUser.MapX,
                                    pUser.MapY,
                                    deltaX, deltaY, pUser.Elevation))
                            {
                                pUser.Kickback(pUser.MapX, pUser.MapY);
                                pUser.Send(ServerString.STR_MOVE_UNABLE);
                                return;
                            }
                            //
                            pMsg.Direction = pUser.Direction = (FacingDirection) Calculations
                                .GetDirectionSector(pMsg.LeftData, pMsg.RightData, pUser.MapX,
                                    pUser.MapY);
                            pUser.MapX = pMsg.LeftData;
                            pUser.MapY = pMsg.RightData;
                            pUser.Elevation = tile.Elevation;
                            pUser.ProcessOnJump();

                            if (pClient.Character.QueryStatus(FlagInt.RIDING) != null)
                            {
                                pClient.Character.Vigor -= dwVigorConsume;
                            }

                            pUser.Send(pMsg);
                            pUser.Screen.SendMovement(pMsg);
                        }
                        else
                        {
                            pUser.Send(ServerString.STR_INVALID_JUMP);
                            pUser.Disconnect();
                        }
                    }
                    else
                    {
                        pUser.Kickback(pMsg.X, pMsg.Y);
                        pUser.Send(ServerString.STR_MOVE_UNABLE);
                    }
                    break;
                }

                    #endregion
                #region 145 - Set Ghost

                case GeneralActionType.DIE_QUESTION:
                {
                    if (pClient.Character.IsAlive)
                        return;
                    pClient.Character.SetGhost();
                    break;
                }

                #endregion
                #region 146 - End Teleport

                case GeneralActionType.END_TELEPORT:
                {
                    pUser.Screen.RefreshSpawnForObservers();
                    break;
                }

                    #endregion
                #region 148 - View Friend Information

                case GeneralActionType.VIEW_FRIEND_INFO:
                {
                    Relationship pRel;
                    if (!pClient.Character.Friends.TryGetValue(pMsg.Data, out pRel))
                    {
                        pClient.Send(pMsg);
                        return;
                    }

                    MsgFriendInfo pMsgInfo = new MsgFriendInfo
                    {
                        Identity = pRel.Identity
                    };
                    if (pRel.IsOnline)
                    {
                        Character pRole = pRel.User.Character;
                        pMsgInfo.Mate = pRole.Mate;
                        pMsgInfo.Level = pRole.Level;
                        pMsgInfo.Mesh = pRole.Lookface;
                        pMsgInfo.SyndicateIdentity = (ushort)pRole.SyndicateIdentity;
                        pMsgInfo.SyndicateRank = pRole.SyndicateRank;
                        pMsgInfo.Profession = (byte) pRole.Profession;
                        pMsgInfo.PkPoints = pRole.PkPoints;
                        pMsgInfo.Identity = pRole.Identity;
                    }
                    pClient.Send(pMsgInfo);
                    break;
                }

                #endregion
                #region 151 - Change Face

                case GeneralActionType.CHANGE_FACE:
                {
                    if (!pUser.ReduceMoney(500, true))
                        return;

                    pUser.Avatar = (ushort) pMsg.Data;
                    break;
                }

                    #endregion
                #region 152 - Trade Partner Info
                case GeneralActionType.VIEW_PARTNER_INFO:
                {
                    Client pClientTarget;
                    if (!ServerKernel.Players.TryGetValue(pMsg.Data, out pClientTarget))
                        return;

                    if (pUser.TradePartners.ContainsKey(pClientTarget.Identity))
                    {
                        Character pTarget = pClientTarget.Character;
                        MsgTradeBuddyInfo pInfo = new MsgTradeBuddyInfo
                        {
                            Identity = pTarget.Identity,
                            Level = pTarget.Level,
                            Lookface = pTarget.Lookface,
                            Name = pTarget.Mate,
                            PkPoints = pTarget.PkPoints,
                            Profession = (ProfessionType) pTarget.Profession,
                            SyndicateIdentity = pTarget.SyndicateIdentity,
                            SyndicateRank = pTarget.SyndicateRank
                        };
                        pUser.Send(pInfo);
                        pUser.Send(pMsg);
                    }
                    break;
                }
                #endregion
                #region 161 - Away
                case GeneralActionType.AWAY:
                {
                    if (pMsg.Identity != pUser.Identity)
                        return;

                    pUser.Away = pMsg.Data != 0;

                    if (pUser.Away && pUser.Action != EntityAction.SIT)
                        pUser.Action = EntityAction.SIT;

                    if (!pUser.Away && pUser.Action != EntityAction.STAND)
                        pUser.Action = EntityAction.STAND;

                    pUser.Screen.Send(pMsg, true);
                    break;
                }
                #endregion
                #region 178 - Change Look
                case GeneralActionType.CHANGE_LOOK:
                {
                    pMsg.Identity = pUser.Identity;
                    pUser.CurrentLayout = (byte) (pMsg.Data%4);
                    pUser.Screen.Send(pMsg, true);
                    break;
                }
                #endregion
                #region 251 - Complete Login

                case GeneralActionType.UNKNOWN1:
                {
                    LoadTitles(pUser);
                    pUser.UpdateClient(ClientUpdateType.PK_POINTS, pUser.PkPoints);
                    pUser.Screen.RefreshSpawnForObservers();
                    pUser.Send(pMsg);

                    int now = UnixTimestamp.Timestamp();

                    var dbStatus = new StatusRepository().LoadStatus(pUser.Identity);
                    if (dbStatus != null)
                    {
                        foreach (var stts in dbStatus)
                        {
                            if (now > stts.EndTime)
                            {
                                new StatusRepository().Delete(stts);
                                continue;
                            }

                            int remaining = (int)(stts.EndTime - now);
                            pUser.AttachStatus(pUser, (int)stts.Status, 0, remaining, 1, 0, pUser.Identity);
                            if (stts.Status == FlagInt.CURSED)
                                pUser.UpdateClient(ClientUpdateType.CURSED_TIMER, (uint)remaining,
                                    false);
                        }
                    }

                    pUser.SendBless();

                    if (pUser.RemainingLuckyTime > 0)
                        pUser.UpdateClient(ClientUpdateType.LUCKY_TIME_TIMER,
                            pUser.RemainingLuckyTime, false);

                    if (pUser.HasMultipleExp)
                        pUser.UpdateClient(ClientUpdateType.DOUBLE_EXP_TIMER,
                            pUser.RemainingExperienceSeconds, false);

                    pUser.UpdateClient(ClientUpdateType.MERCHANT, 255, false);

                    foreach (var syn in ServerKernel.Syndicates.Values.Where(x => !x.Deleted))
                    {
                        syn.SendName(pUser);
                    }

                    QualifierRankObj userData;
                    if (ServerKernel.ArenaRecord.TryGetValue(pUser.Identity, out userData))
                    {
                        pUser.ArenaQualifier = userData;
                    }
                    else
                    {
                        ServerKernel.ArenaQualifier.GenerateFirstData(pUser);
                    }

                    if (pUser.Name.ToLower().Contains("[Z"))
                    {
                        //MsgChangeName pName = new MsgChangeName
                        //{
                        //    Mode = ChangeNameMode.CHANGE_NAME_ERROR
                        //};
                        //pUser.Send(pName);
                        //// todo set request
                    }

                    pUser.RecalculateAttributes();
                    pUser.Life = pUser.MaxLife;

                    pUser.Send(new MsgAction(pUser.CurrentLayout, pUser.CurrentLayout, pUser.CurrentLayout, 0, GeneralActionType.CHANGE_LOOK));

                    int bonusAmount = pUser.BonusCount();
                    if (bonusAmount > 0)
                    {
                        pUser.Send(string.Format(ServerString.STR_BONUS, bonusAmount), ChatTone.CENTER);
                    }

                    pUser.LoginComplete = true;
                    pUser.Send(pMsg);
                    break;
                }

                #endregion
                #region 310 - Observe Friend Equipment

                case GeneralActionType.OBSERVE_FRIEND_EQUIPMENT:
                {
                    Client pUserTarget;
                    if (!ServerKernel.Players.TryGetValue(pMsg.Data, out pUserTarget))
                    {
                        pUser.Send("Target not found.");
                        return;
                    }

                    Character pTarget = pUserTarget.Character;
                    pTarget.SendWindowSpawnTo(pUser);

                    foreach (Item item in pTarget.Equipment.Items.Values.Where(x => x.Position <= ItemPosition.CROP))
                    {
                        var msg = item.InformationPacket();
                        msg.Identity = pTarget.Identity;
                        msg.Itemtype = item.Type;
                        msg.ItemMode = ItemMode.VIEW;
                        pUser.Send(msg);
                    }

                    pTarget.Send(string.Format("{0} is observing your equipments carefully.", pUser.Name));

                    var strMsg = new MsgName
                    {
                        Action = StringAction.QUERY_MATE,
                        Identity = pTarget.Identity
                    };
                    strMsg.Append(pTarget.Mate);
                    pUser.Send(strMsg);
                    pUser.Send(pMsg);
                    break;
                }

                #endregion
                #region 408 - Query Attributes

                case GeneralActionType.UNKNOWN2:
                {
                    pUser.RecalculateAttributes();
                    pUser.SendStatus();
                    break;
                }

                #endregion
                default:
                {
                    if (pUser.IsPm)
                        pUser.Send(string.Format("MsgAction::{0} missing handler", pMsg.Action));
                    break;
                }
            }
        }
    }
}