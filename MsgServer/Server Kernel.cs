// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Server Kernel.cs
// Last Edit: 2016/12/19 17:36
// Created: 2016/11/23 10:23

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DB;
using DB.Entities;
using MsgServer.Network;
using MsgServer.Network.GameServer;
using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Events;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Qualifier;
using MsgServer.Structures.Society;
using MsgServer.Structures.World;
using ServerCore.Common;
using ServerCore.Common.Enums;

namespace MsgServer
{
    public static class ServerKernel
    {
        // server attribute
        public static byte MAX_UPLEVEL = 140;

        public static LoginClient LoginServer;

        public static GameSocket GameServer;

        public static SessionFactory MySqlFactory;

        public static string Username = "test";

        public static string Password = "test";

        public static ushort MaxOnlinePlayer = 300;

        public static int GamePort = 5816;

        /// <summary>
        /// The server name shown in the Game Server Console Title.
        /// </summary>
        public static string ServerName = "Ftw! Masters";

        /// <summary>
        /// The message sent by this server to the LoginServer after a successfull connection
        /// to confirm if the server is compatible and enabled to login.
        /// </summary>
        public static string HelloSendString = "rDOLjXHL3bFkyMVk";

        /// <summary>
        /// The message that the LoginServer should send after a successfull connection to 
        /// confirm if the server is compatible and enabled to login.
        /// </summary>
        public static string HelloReplyString = "fzYPi0xsRGiBmj6X";

        public static string TransferKey = "EypKhLvYJ3zdLCTyz9Ak8RAgM78tY5F32b7CUXDuLDJDFBH8H67BWy9QThmaN5Vb";

        public static string TransferSalt = "MyqVgBf3ytALHWLXbJxSUX4uFEu3Xmz2UAY9sTTm8AScB7Kk2uwqDSnuNJske4By";

        public static string Blowfish = "BC234xs45nme7HU9";

        /// <summary>
        /// The Ip Address of the login server where this will be connected to.
        /// </summary>
        public static string LoginServerAddress = "127.0.0.1";

        /// <summary>
        /// The port that the login server is listening for incoming connections.
        /// </summary>
        public static int LoginServerPort = 9865;

        /// <summary>
        /// The Ini Parser used to read the configuration file from the LoginServer.
        /// </summary>
        public static IniFileName ConfigReader;

        /// <summary>
        /// The logger class used to write exceptions or errors thrown by the server.
        /// </summary>
        public static LogWriter Log = new LogWriter(Environment.CurrentDirectory + @"\");

        /// <summary>
        /// The time when the server has first started.
        /// </summary>
        public static DateTime ServerStartTime;

        #region Dictionaries

        // read-only are dictionaries
        public static Dictionary<uint, ActionStruct> GameActions = new Dictionary<uint, ActionStruct>();
        public static Dictionary<uint, TaskStruct> GameTasks = new Dictionary<uint, TaskStruct>();
        public static Dictionary<uint, DbItemtype> Itemtype = new Dictionary<uint, DbItemtype>();
        public static Dictionary<uint, DbItemAddition> ItemAddition = new Dictionary<uint, DbItemAddition>();
        public static Dictionary<byte, DbLevexp> Levelxp = new Dictionary<byte, DbLevexp>(140);
        public static Dictionary<uint, DbPortal> Portals = new Dictionary<uint, DbPortal>();
        public static Dictionary<uint, DbMagictype> Magictype = new Dictionary<uint, DbMagictype>();
        public static ConcurrentDictionary<uint, MagicTypeOp> Magictypeops = new ConcurrentDictionary<uint, MagicTypeOp>();
        public static Dictionary<uint, DbCqRebirth> Rebirths = new Dictionary<uint, DbCqRebirth>();
        public static Dictionary<uint, DbPointAllot> PointAllot = new Dictionary<uint, DbPointAllot>();
        public static Dictionary<uint, DbGoods> Goods = new Dictionary<uint, DbGoods>();
        public static Dictionary<uint, DbMonstertype> Monsters = new Dictionary<uint, DbMonstertype>();
        public static Dictionary<uint, DbGenerator> MonsterSpawn = new Dictionary<uint, DbGenerator>();
        public static Dictionary<uint, DbRefinery> Refineries = new Dictionary<uint, DbRefinery>();
        public static Dictionary<ushort, IHonorReward> HonorRewards = new Dictionary<ushort, IHonorReward>(1000);
        // data that won't be ordered and are read-only
        public static List<MentorBattleLimit> MentorBattleLimits = new List<MentorBattleLimit>(400);
        public static List<DbMentorType> MentorTypes = new List<DbMentorType>();
        public static List<Generator> Generators = new List<Generator>();
        public static List<DbMonsterMagic> MonsterMagics = new List<DbMonsterMagic>();
        public static List<SpecialDrop> SpecialDrop = new List<SpecialDrop>();
        // dynamic data
        public static ConcurrentDictionary<uint, LoginRequest> LoginQueue = new ConcurrentDictionary<uint, LoginRequest>();
        public static ConcurrentDictionary<uint, Client> Players = new ConcurrentDictionary<uint, Client>(500, 500);
        public static ConcurrentDictionary<uint, Client> CharacterCreation = new ConcurrentDictionary<uint, Client>();
        public static ConcurrentDictionary<uint, Map> Maps = new ConcurrentDictionary<uint, Map>(1000, 500);
        public static ConcurrentDictionary<uint, Syndicate> Syndicates = new ConcurrentDictionary<uint, Syndicate>();
        public static ConcurrentDictionary<uint, DbDynaRankRec> Nobility = new ConcurrentDictionary<uint, DbDynaRankRec>();
        public static ConcurrentDictionary<uint, IKoCount> KoBoard = new ConcurrentDictionary<uint, IKoCount>(3, 100);
        public static ConcurrentDictionary<uint, FlowerObject> FlowerRankingDict = new ConcurrentDictionary<uint, FlowerObject>();
        public static ConcurrentDictionary<uint, Family> Families = new ConcurrentDictionary<uint, Family>();
        public static ConcurrentDictionary<uint, QualifierRankObj> ArenaRecord = new ConcurrentDictionary<uint, QualifierRankObj>();
        public static ConcurrentDictionary<uint, DetainedObject> DetainedObjects = new ConcurrentDictionary<uint, DetainedObject>();
        public static List<QuestJar> PlayerQuests = new List<QuestJar> ();
        #endregion

        #region Event Configuration

        #region Syndicate Recruitment

        public static SyndicateRecruitment SyndicateRecruitment = new SyndicateRecruitment();

        #endregion

        #region Capture the Flag

        public static readonly int[] CTF_MONEY_REWARD =
        {
            1200000000,
            1000000000,
            800000000,
            650000000,
            500000000,
            400000000,
            300000000,
            200000000
        };

        public static readonly uint[] CTF_EMONEY_REWARD =
        {
            300000,
            200000,
            100000,
            60000,
            50000,
            40000,
            30000,
            20000
        };

        public static CaptureTheFlag CaptureTheFlag = new CaptureTheFlag();

        #endregion
        
        #region Quiz Show

        public static QuizShowEvent QuizShow;
        public static readonly int[] QUIZ_SHOW_HOUR = { 00, 02, 12, 14, 18, 22 };
        public static readonly ushort[] QUIZ_SHOW_AWARD = { 0, 60000, 45000, 30000 };
        public static readonly ushort[] QUIZ_SHOW_EMONEY = { 0, 10750, 6450, 4300 };
        public static readonly uint[] QUIZ_SHOW_MONEY = { 0, 1500000, 1000000, 500000 };
        public const ushort QUIZ_TIME_PER_QUESTION = 30; // in seconds
        public const ushort QUIZ_MAX_QUESTION = 20;
        public const ushort QUIZ_MAX_EXPERIENCE = 18000;

        #endregion

        #region Pigeon

        public static Pigeon Broadcast;

        #endregion

        #region Flower

        public static FlowerRanking FlowerRanking;

        #endregion

        #region Arena Qualifier

        public static ArenaEngine ArenaQualifier = new ArenaEngine();

        private static int m_nNextMapId = 900001;

        public static int NextQualifierMap()
        {
            return Interlocked.Increment(ref m_nNextMapId);
        }

        #endregion

        #region Skill Pk Tournament



        #endregion

        #region Score Pk Tournament

        public const uint SCORE_PK_MAPID = 7503;
        public static ScorePkEvent ScorePkEvent = new ScorePkEvent();

        #endregion

        #region Syndicate Score PK War

        public static uint[] SYN_SCORE_MONEY_REWARD =
        {
            300000000,
            250000000,
            150000000,
            100000000
        };

        public static uint[] SYN_SCORE_EMONEY_REWARD =
        {
            200000,
            150000,
            100000,
            50000
        };

        public static SyndicateScoreWar SyndicateScoreWar = new SyndicateScoreWar();

        #endregion

        #region Line Skill PK

        public static LineSkillPkTournament LineSkillPk = new LineSkillPkTournament();

        public static uint[] LineSkillGoldReward =
        {
            15000000,
            10000000,
            5000000,
            2000000
        };

        public static uint[] LineSkillEmoneyReward =
        {
            21500,
            10750,
            6450,
            2150
        };

        #endregion

        #endregion

        public static int OnlineRecord;

        // methods
        public static void SendMessageToAll(byte[] pMsg)
        {
            foreach (var plr in Players.Values)
                plr.Send(pMsg);
        }

        public static void SendMessageToAll(string message, ChatTone tone)
        {
            foreach (var plr in Players.Values)
            {
                plr.SendMessage(message, tone);
            }
        }

        public static long GetExpBallExperience(byte pLevel)
        {
            if (pLevel >= MAX_UPLEVEL) return 0;
            long exp = 0;
            try
            {
                Random rand = new Random();
                float[] percentage =
                {
                    0, 30000, 30000, 60000, 30000, 30000, 20000, 20000, 12000, 8571.40f, 4285.70f,
                    3333.30f, 3000, 2608.60f, 2307.60f, 2400, 1764.70f, 1621.60f, 1363.60f, 1200, 1052.60f,
                    1071.40f, 1034.40f, 1016.90f, 909.09f, 895.52f, 909.09f, 923.07f, 833.33f, 722.89f, 652.17f,
                    659.34f, 652.17f, 582.52f, 512.82f, 495.86f, 495.86f, 491.80f, 447.76f, 387.09f, 375,
                    348.83f, 319.14f, 255.31f, 223.04f, 212.01f, 212.76f, 215.82f, 192.30f, 166.20f, 198.67f,
                    198.67f, 189.87f, 175.43f, 153.06f, 143.88f, 143.54f, 160.42f, 129.58f, 111.73f, 106.00f,
                    105.82f, 103.80f, 89.285f, 77.922f, 71.942f, 71.684f, 70.838f, 64.935f, 56.657f, 73.260f,
                    72.028f, 72.463f, 57.692f, 50.293f, 72.202f, 71.942f, 72.815f, 65.359f, 56.232f, 54.347f,
                    54.102f, 52.724f, 46.620f, 40.567f, 38.535f, 38.338f, 37.783f, 34.944f, 30.000f, 28.818f,
                    28.666f, 36.101f, 31.965f, 27.497f, 25.795f, 25.369f, 24.701f, 22.607f, 19.448f, 18.450f,
                    18.143f, 16.510f, 13.015f, 11.198f, 10.507f, 10.359f, 9.8830f, 9.1799f, 6.5139f, 6.1728f,
                    5.5504f, 5.4724f, 5.3787f, 5.2877f, 5.2521f, 5.1997f, 5.1150f, 4.3465f, 3.5176f, 8.9405f,
                    7.4497f, 6.2086f, 5.1737f, 4.3112f, 3.5928f, 2.9940f, 2.4950f, 2.3504f, 2.3504f, 0.5876f,
                    0.3917f, 0.2611f, 0.1741f, 0.1160f, 0.0773f, 0.0516f, 0.0344f, 0.0229f, 0.0152f
                };
                float factor1 = percentage[pLevel] / 100f;
                float factor2 = (rand.Next(975, 1025) / 1000f);
                exp = (long)(Levelxp[pLevel].Exp * factor1 * factor2);
            }
            catch
            {
                Console.WriteLine("Could not fetch GetExpBallExperience(byte {0}, byte {1})", pLevel, MAX_UPLEVEL);
            }

            return exp;
        }

        public static DynamicNpc GetDynaNpcByIdentity(uint idNpc)
        {
            foreach (var pMap in Maps.Values)
                foreach (var pNpc in pMap.GameObjects.Values)
                    if (pNpc.Identity == idNpc && pNpc is DynamicNpc)
                        return pNpc as DynamicNpc;
            return null;
        }

        /// <summary>
        /// The Identities of the default (cities) warehouses.
        /// TwinCity, Market, StoneCity, Desert, Phoenix, Bird & Ape
        /// </summary>
        public static readonly uint[] SystemWarehouses = { 230, 231, 232, 233, 234, 235, 236, (uint)ItemPosition.SASH_S, (uint)ItemPosition.SASH_M, (uint)ItemPosition.SASH_L };

        public static string NextSyndicateEvent()
        {
            DateTime now = DateTime.Now;
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                {
                    if (now.Hour < 21)
                        return "Capture the Flag";
                    return "Guild Contest";
                }
                case DayOfWeek.Monday:
                {
                    if (now.Hour < 21)
                        return "Guild Contest";
                    return "Capture the Flag";
                }
                case DayOfWeek.Sunday:
                {
                    return "Guild Contest";
                }
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                case DayOfWeek.Friday:
                {
                    return "Capture the Flag";
                }
            }
            return "Error";
        }
    }
}