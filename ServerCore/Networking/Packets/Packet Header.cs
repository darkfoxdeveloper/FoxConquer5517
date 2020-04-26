// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Packet Header.cs
// Last Edit: 2016/12/02 07:34
// Created: 2016/11/23 07:52
namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This structure encapsulates a packet's header information. It contains the length of the packet in offset 
    /// zero as an unsigned short, and the identity of the packet in offset 2 as an unsigned short.
    /// </summary>
    public struct PacketHeader
    {
        public ushort Length;
        public PacketType Identity;
    }

    /// <summary>
    /// This enumeration type defines the types of packets that can be sent and received by the client / server. It
    /// also defines the types of packets sent between the message and map servers (which is normally just the 
    /// original packet type plus base 10000 instead of base 1000).
    /// </summary>
    public enum PacketType : ushort
    {
        LOGIN_AUTH_REQUEST = 11,
        LOGIN_AUTH_CONFIRM = 12,
        LOGIN_COMPLETE_AUTHENTICATION = 13,
        LOGIN_REQUEST_ONLINE_NUMBER = 20,
        LOGIN_REQUEST_USER_ONLINE = 21,
        LOGIN_REQUEST_KICKOUT = 22,
        LOGIN_REQUEST_DISCONNECTION = 23,
        LOGIN_REQUEST_SERVER_INFO = 24,
        LOGIN_REQUEST_SERVER_STATE = 25,
        LOGIN_REQUEST_USER_SIGNIN = 26,
        LOGIN_RECEIVE_INFORMATION = 50,

        // client and game server
        // Between 1000-29999
        MSG_REGISTER = 1001,
        MSG_TALK = 1004,
        MSG_USER_INFO = 1006,
        MSG_ITEM_INFORMATION = 1008,
        MSG_ITEM = 1009,
        MSG_NAME = 1015,
        MSG_FRIEND = 1019,
        MSG_INTERACT = 1022,
        MSG_TEAM = 1023,
        MSG_ALLOT = 1024,
        MSG_WEAPON_SKILL = 1025,
        MSG_TEAM_MEMBER = 1026,
        MSG_GEM_EMBED = 1027,
        MSG_DATA = 1033,
        MSG_DETAIN_ITEM_INFO = 1034,
        MSG_SOLIDIFY = 1038,
        MSG_PLAYER_ATTRIB_INFO = 1040,
        MSG_CONNECT = 1052,
        MSG_CONNECT_EX = 1055,
        MSG_TRADE = 1056,
        MSG_SYNP_OFFER = 1058,
        MSG_ENCRYPT_CODE = 1059,
        MSG_ACCOUNT1 = 1060,
        MSG_DUTY_MIN_CONTRI = 1061,
        MSG_SELF_SYN_MEM_AWARD_RANK = 1063,
        MSG_METE_SPECIAL = 1066,
        MSG_ACCOUNT = 1086,
        MSG_PC_NUM = 1100,
        MSG_MAP_ITEM = 1101,
        MSG_ACCOUNT_SOFT_KB = 1102,
        MSG_MAGIC_INFO = 1103,
        MSG_FLUSH_EXP = 1104,
        MSG_MAGIC_EFFECT = 1105,
        MSG_SYNDICATE_ATTRIBUTE_INFO = 1106,
        MSG_SYNDICATE = 1107,
        MSG_ITEM_INFO_EX = 1108,
        MSG_NPC_INFO_EX = 1109,
        MSG_MAP_INFO = 1110,
        MSG_SYN_MEMBER_INFO = 1112,
        MSG_INVITE_TRANS = 1126,
        MSG_VIP_USER_HANDLE = 1128,
        MSG_VIP_FUNCTION_VALID_NOTIFY = 1129,
        MSG_TITLE = 1130,
        MSG_TASK_STATUS = 1134,
        MSG_TASK_DETAIL_INFO = 1135,
        MSG_FLOWER = 1150,
        MSG_RANK = 1151,
        MSG_FAMILY = 1312,
        MSG_FAMILY_OCCUPY = 1313,
        MSG_NPC_INFO = 2030,
        MSG_NPC = 2031,
        MSG_TASK_DIALOG = 2032,
        MSG_FRIEND_INFO = 2033,
        MSG_DATA_ARRAY = 2036,
        MSG_TRADE_BUDDY = 2046,
        MSG_TRADE_BUDDY_INFO = 2047,
        MSG_EQUIP_LOCK = 2048,
        MSG_PIGEON = 2050,
        MSG_PIGEON_QUERY = 2051,
        MSG_PEERAGE = 2064,
        MSG_GUIDE = 2065,
        MSG_GUIDE_INFO = 2066,
        MSG_GUIDE_CONTRIBUTE = 2067,
        MSG_QUIZ = 2068,
        MSG_QUIZ_SPONSOR = 2069,
        MSG_RELATION = 2071,
        MSG_QUENCH = 2076,
        MSG_ITEM_STATUS = 2077,
        MSG_USER_IP_INFO = 2078,
        MSG_SERVER_INFO = 2079,
        MSG_CHANGE_NAME = 2080,
        MSG_FACTION_RANK_INFO = 2101,
        MSG_SYN_MEMBER_LIST = 2102,
        MSG_SUPER_FLAG = 2110,
        MSG_TOTEM_POLE_INFO = 2201,
        MSG_WEAPONS_INFO = 2202,
        MSG_TOTEM_POLE = 2203,
        MSG_QUALIFYING_INTERACTIVE = 2205,
        MSG_QUALIFYING_FIGHTERS_LIST = 2206,
        MSG_QUALIFYING_RANK = 2207,
        MSG_QUALIFYING_SEASON_RANK_LIST = 2208,
        MSG_QUALIFYING_DETAIL_INFO = 2209,
        MSG_ARENIC_SCORE = 2210,
        MSG_ARENIC_WITNESS = 2211,
        MSG_ELITE_PK_ARENIC = 2218,
        MSG_PK_ELITE_MATCH_INFO = 2219,
        MSG_PK_STATISTIC = 2220,
        MSG_PK_ENABLE = 2221,
        MSG_ELITE_PK_SCORE = 2222,
        MSG_ELITE_PK_GAME_RANK_INFO = 2223,
        MSG_WAR_FLAG = 2224,
        MSG_SYN_RECUIT_ADVERTISING = 2225,
        MSG_SYN_RECRUIT_ADVERTISING_LIST = 2226,
        MSG_SYN_RECRUIT_ADVERTISING_OPT = 2227,
        MSG_SUB_PRO = 2320,
        MSG_AURA = 2410,
        MSG_WALK = 10005,
        MSG_ACTION = 10010,
        MSG_PLAYER = 10014,
        MSG_USER_ATTRIB = 10017,

        // NPC Server 30000-65534
        NPC_CHARACTERS_INFO = 30001,
        NPC_CHARACTER_CHGMAP = 30002,
        NPC_MONSTER_SPAWN = 30010,
        NPC_MONSTER_MOVE = 30011,
        NPC_MONSTER_ATTACK = 30012,
        NPC_MONSTER_DIE = 30013,
        NPC_MONSTER_KILL = 30014
    }
}