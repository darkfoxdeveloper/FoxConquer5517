// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Game Packet Handler.cs
// Last Edit: 2017/01/27 17:17
// Created: 2016/12/29 21:30

using System;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer
{
    public sealed class GamePacketHandler
    {
        #region 1001 - MsgRegister
        [PacketHandlerType(PacketType.MSG_REGISTER)]
        public void ProcessRegister(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character != null)
                return;

            Handlers.Handlers.HandleRegister(pClient, new MsgRegister(pBuffer));
        }
        #endregion
        #region 1004 - MsgTalk
        [PacketHandlerType(PacketType.MSG_TALK)]
        public void ProcessTalk(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.ProcessChatMessage(pClient, new MsgTalk(pBuffer));
        }
        #endregion
        #region 1009 - MsgItem
        [PacketHandlerType(PacketType.MSG_ITEM)]
        public void ProcessItem(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleItem(pClient.Character, new MsgItem(pBuffer));
        }
        #endregion
        #region 1015 - MsgName
        [PacketHandlerType(PacketType.MSG_NAME)]
        public void ProcessName(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleStringPacket(pClient.Character, new MsgName(pBuffer));
        }
        #endregion
        #region 1019 - MsgFriend
        [PacketHandlerType(PacketType.MSG_FRIEND)]
        public void ProcessFriend(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
                || pClient.Character == null)
                return;
            Handlers.Handlers.HandleFriendPacket(pClient.Character, new MsgFriend(pBuffer));
        }
        #endregion
        #region 1022 - MsgInteract
        [PacketHandlerType(PacketType.MSG_INTERACT)]
        public void ProcessInteract(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleInteract(pClient.Character, new MsgInteract(pBuffer));
        }
        #endregion
        #region 1023 - MsgTeam
        [PacketHandlerType(PacketType.MSG_TEAM)]
        public void ProcessTeam(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
                || pClient.Character == null)
                return;
            Handlers.Handlers.HandleTeamAction(pClient.Character, new MsgTeam(pBuffer));
        }
        #endregion
        #region 1024 - MsgAllot
        [PacketHandlerType(PacketType.MSG_ALLOT)]
        public void ProcessAllot(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleAllot(pClient.Character, new MsgAllot(pBuffer));
        }
        #endregion
        #region 1027 - MsgGemEmbed
        [PacketHandlerType(PacketType.MSG_GEM_EMBED)]
        public void ProcessGemEmbed(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleEmbedGem(pClient.Character, new MsgGemEmbed(pBuffer));
        }
        #endregion
        #region 1038 - MsgSolidify
        [PacketHandlerType(PacketType.MSG_SOLIDIFY)]
        public void ProcessSolidify(Client pClient, byte[] pBuffer)
        {
            if (pClient.Character == null)
                return;
            Handlers.Handlers.HandleSolidify(pClient.Character, new MsgSolidify(pBuffer));
        }
        #endregion
        #region 1040 - MsgPlayerAttribInfo
        [PacketHandlerType(PacketType.MSG_PLAYER_ATTRIB_INFO)]
        public void ProcessPlayerAttribInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            MsgPlayerAttribInfo pMsg = new MsgPlayerAttribInfo(pBuffer);
            Client pTarget;
            if (pMsg.Identity != pClient.Identity)
            {
                if (!ServerKernel.Players.TryGetValue(pMsg.Identity, out pTarget))
                    return;
                pTarget.Character.SendStatus(pClient.Character);
            }
            else
            {
                pClient.Character.RecalculateAttributes();
                pClient.Character.SendStatus();
            }
        }
        #endregion
        #region 1052 - MsgConnect
        [PacketHandlerType(PacketType.MSG_CONNECT)]
        public void ProcessConnect(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character != null)
                return;
            Handlers.Handlers.HandleConnect(pClient, new MsgConnect(pBuffer));
        }
        #endregion
        #region 1056 - MsgTrade
        [PacketHandlerType(PacketType.MSG_TRADE)]
        public void ProcessTrade(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
                || pClient.Character == null)
                return;
            Handlers.Handlers.HandleTrade(pClient.Character, new MsgTrade(pBuffer));
        }
        #endregion
        #region 1058 - MsgSynpOffer
        [PacketHandlerType(PacketType.MSG_SYNP_OFFER)]
        public void ProcessSynpOffer(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
               || pClient.Character == null
               || pClient.Character.Syndicate == null)
                return;
            
            pClient.Character.SyndicateMember.SendCharacterInformation();
        }
        #endregion
        #region 1063 - MsgSynMemAwawrdRank
        [PacketHandlerType(PacketType.MSG_SELF_SYN_MEM_AWARD_RANK)]
        public void ProcessSelfSynMemAwardRank(Client pClient, byte[] pBuffer)
        {
            if (pClient.Character == null)
                return;
            Handlers.Handlers.HandleSelfSynMemAwardRank(pClient.Character, new MsgSelfSynMemAwardRank(pBuffer));
        }
        #endregion
        #region 1066 - MsgMeteSpecial
        [PacketHandlerType(PacketType.MSG_METE_SPECIAL)]
        public void ProcessMeteSpecial(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleMeteSpecial(pClient.Character, new MsgMeteSpecial(pBuffer));
        }
        #endregion
        #region 1101 - MsgMapItem
        [PacketHandlerType(PacketType.MSG_MAP_ITEM)]
        public void ProcessMapItem(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleMapItem(pClient.Character, new MsgMapItem(pBuffer));
        }
        #endregion
        #region 1102 - MsgAccountSoftKb
        [PacketHandlerType(PacketType.MSG_ACCOUNT_SOFT_KB)]
        public void ProcessAccountSoftKb(Client pClient, byte[] pBuffer)
        {
            if (pClient.Character == null)
                return;
            Handlers.Handlers.HandleWarehouse(pClient.Character, new MsgAccountSoftKb(pBuffer));
        }
        #endregion
        #region 1107 - MsgSyndicate
        [PacketHandlerType(PacketType.MSG_SYNDICATE)]
        public void ProcessMsgSyndicate(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
               || pClient.Character == null)
                return;
            Handlers.Handlers.HandleSyndicate(pClient.Character, new MsgSyndicate(pBuffer));
        }
        #endregion
        #region 1128 - MsgVipUserHandle
        [PacketHandlerType(PacketType.MSG_VIP_USER_HANDLE)]
        public void ProcessVipUserHandle(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleVipUserHandle(pClient.Character, new MsgVipUserHandle(pBuffer));
        }
        #endregion
        #region 1130 - MsgTitle
        [PacketHandlerType(PacketType.MSG_TITLE)]
        public void ProcessTitle(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleTitlePacket(pClient.Character, new MsgTitle(pBuffer));
        }
        #endregion
        #region 1134 - MsgTaskStatus
        [PacketHandlerType(PacketType.MSG_TASK_STATUS)]
        public void ProcessTaskStatus(Client pClient, byte[] pBuffer)
        {
            
        }
        #endregion
        #region 1135 - MsgTaskDetailInfo
        [PacketHandlerType(PacketType.MSG_TASK_DETAIL_INFO)]
        public void ProcessTaskDetailInfo(Client pClient, byte[] pBuffer)
        {
            
        }
        #endregion
        #region 1312 - MsgFamily
        [PacketHandlerType(PacketType.MSG_FAMILY)]
        public void ProcessFamily(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleFamily(pClient.Character, new MsgFamily(pBuffer));

        }
        #endregion
        #region 1313 - MsgFamilyOccupy
        [PacketHandlerType(PacketType.MSG_FAMILY_OCCUPY)]
        public void ProcessFamilyOccupy(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleFamilyOccupy(pClient.Character, new MsgFamilyOccupy(pBuffer));
        }
        #endregion
        #region 2030 - MsgNpcInfo
        [PacketHandlerType(PacketType.MSG_NPC_INFO)]
        public void ProcessNpcInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient.Character == null) return;
            Handlers.Handlers.HandleNpcInfo(pClient.Character, new MsgNpcInfo(pBuffer));
        }
        #endregion
        #region 2031 - MsgNpc
        [PacketHandlerType(PacketType.MSG_NPC)]
        public void ProcessNpc(Client pClient, byte[] pBuffer)
        {
            if (pClient.Character == null) return;
            Handlers.Handlers.HandleTaskDialog(pClient.Character, new MsgTaskDialog(pBuffer));
        }
        #endregion
        #region 2032 - MsgTaskDialog
        [PacketHandlerType(PacketType.MSG_TASK_DIALOG)]
        public void ProcessTaskDialog(Client pClient, byte[] pBuffer)
        {
            if (pClient.Character == null) return;
            Handlers.Handlers.HandleTaskDialog(pClient.Character, new MsgTaskDialog(pBuffer));
        }
        #endregion
        #region 2036 - MsgDataArray
        [PacketHandlerType(PacketType.MSG_DATA_ARRAY)]
        public void ProcessDataArray(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleComposition(pClient.Character, new MsgDataArray(pBuffer));
        }
        #endregion
        #region 2046 - MsgTradeBuddy
        [PacketHandlerType(PacketType.MSG_TRADE_BUDDY)]
        public void ProcessTradeBuddy(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleTradeBuddy(pClient.Character, new MsgTradeBuddy(pBuffer));
        }
        #endregion
        #region 2048 - MsgEquipLock
        [PacketHandlerType(PacketType.MSG_EQUIP_LOCK)]
        public void ProcessEquipLock(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleItemLock(pClient.Character, new MsgEquipLock(pBuffer));
        }
        #endregion
        #region 2050 - MsgPigeon
        [PacketHandlerType(PacketType.MSG_PIGEON)]
        public void ProcessPigeon(Client pClient, byte[] pbuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandlePigeon(pClient.Character, new MsgPigeon(pbuffer));
        }
        #endregion
        #region 2064 - MsgPeerage
        [PacketHandlerType(PacketType.MSG_PEERAGE)]
        public void ProcessPeerage(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleNobility(pClient.Character, new MsgPeerage(pBuffer));
        }
        #endregion
        #region 2065 - MsgGuide
        [PacketHandlerType(PacketType.MSG_GUIDE)]
        public void ProcessMsgGuide(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleGuideRequest(pClient.Character, new MsgGuide(pBuffer));
        }
        #endregion
        #region 2066 - Mentor Information
        [PacketHandlerType(PacketType.MSG_GUIDE_INFO)]
        public void ProcessGuideInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleGuideInfo(pClient.Character, new MsgGuideInfo(pBuffer));
        }
        #endregion
        #region 2067 - Mentor Contribution
        [PacketHandlerType(PacketType.MSG_GUIDE_CONTRIBUTE)]
        public void ProcessGuideContribute(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleGuideContribution(pClient.Character, new MsgContribute(pBuffer));
        }
        #endregion
        #region 2068 - Quiz Packet
        [PacketHandlerType(PacketType.MSG_QUIZ)]
        public void ProcessQuizShow(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
                || pClient.Character == null)
                return;
            Handlers.Handlers.HandleQuizShow(pClient.Character, new MsgQuiz(pBuffer));
        }
        #endregion
        #region 2076 - MsgQuench
        [PacketHandlerType(PacketType.MSG_QUENCH)]
        public void ProcessQuench(Client pClient, byte[] pBuffer)
        {
            if (pClient.Character == null)
                return;
            Handlers.Handlers.HandleQuench(pClient.Character, new MsgQuench(pBuffer));
        }
        #endregion
        #region 2080 - MsgChangeName
        [PacketHandlerType(PacketType.MSG_CHANGE_NAME)]
        public void ProcessChangeName(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleChangeName(pClient.Character, new MsgChangeName(pBuffer));
        }
        #endregion
        #region 2101 - MsgFactionRankInfo
        [PacketHandlerType(PacketType.MSG_FACTION_RANK_INFO)]
        public void ProcessFactionRankInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
                || pClient.Character == null
                || pClient.Character.Syndicate == null)
                return;
            Handlers.Handlers.HandleFactionRankInfo(pClient.Character, new MsgFactionRankInfo(pBuffer));
        }
        #endregion
        #region 2102 - MsgSynMemberList
        [PacketHandlerType(PacketType.MSG_SYN_MEMBER_LIST)]
        public void ProcessSynMemberList(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
               || pClient.Character == null
               || pClient.Character.Syndicate == null)
                return;
            Handlers.Handlers.HandleSynMemberList(pClient.Character, new MsgSynMemberList(pBuffer));
        }
        #endregion
        #region 2110 - MsgSuperFlag
        [PacketHandlerType(PacketType.MSG_SUPER_FLAG)]
        public void ProcessSuperFlag(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleSuperFlag(pClient.Character, new MsgSuperFlag(pBuffer));
        }
        #endregion
        #region 2202 - MsgWeaponsInfo
        [PacketHandlerType(PacketType.MSG_WEAPONS_INFO)]
        public void ProcessWeaponsInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
               || pClient.Character == null
               || pClient.Character.Syndicate == null)
                return;
            Handlers.Handlers.HandleWeaponsInfo(pClient.Character, new MsgWeaponsInfo(pBuffer));
        }
        #endregion
        #region 2203 - MsgTotemPole
        [PacketHandlerType(PacketType.MSG_TOTEM_POLE)]
        public void ProcessTotemPole(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
                || pClient.Character == null
                || pClient.Character.Syndicate == null)
                return;

            Handlers.Handlers.HandleTotemPole(pClient.Character, new MsgTotemPole(pBuffer));
        }
        #endregion
        #region 2205 - MsgQualifyingInteractive
        [PacketHandlerType(PacketType.MSG_QUALIFYING_INTERACTIVE)]
        public void ProcessQualifyingInteractive(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleQualifyingInteractive(pClient.Character, new MsgQualifyingInteractive(pBuffer));
        }
        #endregion
        #region 2206 - MsgQualifyingFightersList
        [PacketHandlerType(PacketType.MSG_QUALIFYING_FIGHTERS_LIST)]
        public void ProcessQualifyingFightersList(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleQualifyingFightersList(pClient.Character, new MsgQualifyingFightersList(pBuffer));
        }
        #endregion
        #region 2207 - MsgQualifyingRank

        [PacketHandlerType(PacketType.MSG_QUALIFYING_RANK)]
        public void ProcessQualifyingRank(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleQualifyingRank(pClient.Character, new MsgQualifyingRank(pBuffer));
        }
        #endregion
        #region 2208 - MsgQualifyingSeasonRankList
        [PacketHandlerType(PacketType.MSG_QUALIFYING_SEASON_RANK_LIST)]
        public void ProcessQualifyingSeasonRankList(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleQualifyingSeasonRankList(pClient.Character, new MsgQualifyingSeasonRankList(pBuffer));
        }
        #endregion
        #region 2209 - MsgQualifyingDetailInfo
        [PacketHandlerType(PacketType.MSG_QUALIFYING_DETAIL_INFO)]
        public void ProcessQualifyingDetailInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleQualifyingDetailInfo(pClient.Character, new MsgQualifyingDetailInfo(pBuffer));
        }
        #endregion
        #region 2211 - MsgArenicWitness
        [PacketHandlerType(PacketType.MSG_ARENIC_WITNESS)]
        public void ProcessArenicWitness(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleArenicWitness(pClient.Character, new MsgArenicWitness(pBuffer));
        }
        #endregion
        #region 2219 - MsgPKEliteMatchInfo
        [PacketHandlerType(PacketType.MSG_PK_ELITE_MATCH_INFO)]
        public void ProcessMsgPkEliteMatchInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandlePkEliteMatchInfo(pClient.Character, new MsgPKEliteMatchInfo(pBuffer));
        }
        #endregion
        #region 2220 - MsgPkStatistic
        [PacketHandlerType(PacketType.MSG_PK_STATISTIC)]
        public void ProcessPkStatistic(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandlePkStatistic(pClient.Character, new MsgPkStatistic(pBuffer));
        }
        #endregion
        #region 2223 - MsgElitePKGameRankInfo
        [PacketHandlerType(PacketType.MSG_ELITE_PK_GAME_RANK_INFO)]
        public void ProcessElitePkGameRankInfo(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleElitePKGameRankInfo(pClient.Character, new MsgElitePKGameRankInfo(pBuffer));
        }
        #endregion
        #region 2225 - MsgSynRecruitAdvertising
        [PacketHandlerType(PacketType.MSG_SYN_RECUIT_ADVERTISING)]
        public void ProcessSynRecruitAdvertising(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleSynRecruitAdvertising(pClient.Character, new MsgSynRecuitAdvertising(pBuffer));
        }
        #endregion
        #region 2226 - MsgSynRecruitAdvertisingList
        [PacketHandlerType(PacketType.MSG_SYN_RECRUIT_ADVERTISING_LIST)]
        public void ProcessSynRecruitAdvertisingList(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            MsgSynRecruitAdvertisingList pMsg = new MsgSynRecruitAdvertisingList(pBuffer);
            ServerKernel.SyndicateRecruitment.SendSyndicates(pMsg.StartIndex, pMsg, pClient.Character);
        }
        #endregion
        #region 2227 - MsgSynRecruitAdvertisingOpt
        [PacketHandlerType(PacketType.MSG_SYN_RECRUIT_ADVERTISING_OPT)]
        public void ProcessSynRecruitAdvertisingOpt(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;
            Handlers.Handlers.HandleSynAdvertisingOpt(pClient.Character, new MsgSynRecruitAdvertisingOpt(pBuffer));
        }
        #endregion
        #region 2224 - MsgWarFlag
        [PacketHandlerType(PacketType.MSG_WAR_FLAG)]
        public void ProcessWarFlag(Client pClient, byte[] pBuffer)
        {
            if (pClient == null
                || pClient.Character == null)
                return;

            Handlers.Handlers.HandleWarFlag(pClient.Character, new MsgWarFlag(pBuffer));
        }
        #endregion
        #region 2320 - MsgSubPro
        [PacketHandlerType(PacketType.MSG_SUB_PRO)]
        public void ProcessSubPro(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleSubPro(pClient.Character, new MsgSubPro(pBuffer));
        }
        #endregion
        #region 10005 - MsgWalk
        [PacketHandlerType(PacketType.MSG_WALK)]
        public void ProcessWalk(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleWalk(pClient, new MsgWalk(pBuffer));
        }
        #endregion
        #region 10010 - MsgAction
        [PacketHandlerType(PacketType.MSG_ACTION)]
        public void ProcessAction(Client pClient, byte[] pBuffer)
        {
            if (pClient == null || pClient.Character == null)
                return;

            Handlers.Handlers.HandleAction(pClient, new MsgAction(pBuffer));
        }
        #endregion

        /// <summary>
        /// This function reports a missing packet handler to the console. It writes the length and type of the
        /// packet, then a packet dump to the console.
        /// </summary>
        /// <param name="packet">The packet buffer being reported.</param>
        public static void Report(byte[] packet)
        {
            ushort length = BitConverter.ToUInt16(packet, 0);
            ushort identity = BitConverter.ToUInt16(packet, 2);

            // Print the packet and the packet header:
            ServerKernel.Log.SaveLog(String.Format("Missing Packet Handler: {0} (Length: {1})", identity, length), true, "missing_packet", LogType.DEBUG);
            string aPacket = "";

            for (int index = 0; index < length; index++)
                aPacket += String.Format("{0:X2} ", packet[index]);

            ServerKernel.Log.SaveLog(aPacket, true, "missing_packet-" + identity, LogType.DEBUG);
        }
    }
}