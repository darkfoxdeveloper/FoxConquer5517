// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Task Action Type.cs
// Last Edit: 2016/12/06 14:22
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum TaskActionType
    {
        // System
        ACTION_SYS_FIRST = 100,
        ACTION_MENUTEXT = 101,
        ACTION_MENULINK = 102,
        ACTION_MENUEDIT = 103,
        ACTION_MENUPIC = 104,
        ACTION_MENUBUTTON = 110,
        ACTION_MENULISTPART = 111,
        ACTION_MENUCREATE = 120,
        ACTION_RAND = 121,
        ACTION_RANDACTION = 122,
        ACTION_CHKTIME = 123,
        ACTION_POSTCMD = 124,
        ACTION_BROCASTMSG = 125,
        ACTION_MESSAGEBOX = 126,
        ACTION_EXECUTEQUERY = 127,
        ACTION_SYS_LIMIT = 199,

        //NPC
        ACTION_NPC_FIRST = 200,
        ACTION_NPC_ATTR = 201,
        ACTION_NPC_ERASE = 205,
        ACTION_NPC_MODIFY = 206,
        ACTION_NPC_RESETSYNOWNER = 207,
        ACTION_NPC_FIND_NEXT_TABLE = 208,
        ACTION_NPC_ADD_TABLE = 209,
        ACTION_NPC_DEL_TABLE = 210,
        ACTION_NPC_DEL_INVALID = 211,
        ACTION_NPC_TABLE_AMOUNT = 212,
        ACTION_NPC_SYS_AUCTION = 213,
        ACTION_NPC_DRESS_SYNCLOTHING = 214,
        ACTION_NPC_TAKEOFF_SYNCLOTHING = 215,
        ACTION_NPC_AUCTIONING = 216,
        ACTION_NPC_LIMIT = 299,

        //Map
        ACTION_MAP_FIRST = 300,
        ACTION_MAP_MOVENPC = 301,
        ACTION_MAP_MAPUSER = 302,
        ACTION_MAP_BROCASTMSG = 303,
        ACTION_MAP_DROPITEM = 304,
        ACTION_MAP_SETSTATUS = 305,
        ACTION_MAP_ATTRIB = 306,
        ACTION_MAP_REGION_MONSTER = 307,
        ACTION_MAP_CHANGEWEATHER = 310,
        ACTION_MAP_CHANGELIGHT = 311,
        ACTION_MAP_MAPEFFECT = 312,
        ACTION_MAP_CREATEMAP = 313,
        ACTION_MAP_FIREWORKS = 314,
        ACTION_MAP_LIMIT = 399,

        //Item
        ACTION_ITEMONLY_FIRST = 400,
        ACTION_ITEM_REQUESTLAYNPC = 401,
        ACTION_ITEM_COUNTNPC = 402,
        ACTION_ITEM_LAYNPC = 403,
        ACTION_ITEM_DELTHIS = 498,
        ACTION_ITEMONLY_LIMIT = 499,
        ACTION_ITEM_FIRST = 500,
        ACTION_ITEM_ADD = 501,
        ACTION_ITEM_DEL = 502,
        ACTION_ITEM_CHECK = 503,
        ACTION_ITEM_HOLE = 504,
        ACTION_ITEM_REPAIR = 505,
        ACTION_ITEM_MULTIDEL = 506,
        ACTION_ITEM_MULTICHK = 507,
        ACTION_ITEM_LEAVESPACE = 508,
        ACTION_ITEM_UPEQUIPMENT = 509,
        ACTION_ITEM_EQUIPTEST = 510,
        ACTION_ITEM_EQUIPEXIST = 511,
        ACTION_ITEM_EQUIPCOLOR = 512,
        ACTION_ITEM_REMOVE_ANY = 513,
        ACTION_ITEM_CHECKRAND = 516,
        ACTION_ITEM_MODIFY = 517,
        ACTION_ITEM_JAR_CREATE = 528,
        ACTION_ITEM_JAR_VERIFY = 529,
        ACTION_ITEM_LIMIT = 599,

        //Dyn NPCs
        ACTION_NPCONLY_FIRST = 600,
        ACTION_NPCONLY_CREATENEW_PET = 601,
        ACTION_NPCONLY_DELETE_PET = 602,
        ACTION_NPCONLY_MAGICEFFECT = 603,
        ACTION_NPCONLY_MAGICEFFECT2 = 604,
        ACTION_NPCONLY_LIMIT = 699,

        // Syndicate
        ACTION_SYN_FIRST = 700,
        ACTION_SYN_CREATE = 701,
        ACTION_SYN_DESTROY = 702,
        ACTION_SYN_DONATE = 703,
        ACTION_SYN_CREATE_SUB = 708,
        ACTION_SYN_COMBINE_SUB = 710,
        ACTION_SYN_ATTR = 717,
        ACTION_SYN_ALLOCATE_SYNFUND = 729,
        ACTION_SYN_RENAME = 731,
        ACTION_SYN_DEMISE = 704,
        ACTION_SYN_SET_ASSISTANT = 705,
        ACTION_SYN_CLEAR_RANK = 706,
        ACTION_SYN_PRESENT_MONEY = 707,
        ACTION_SYN_CHANGE_LEADER = 709,
        ACTION_SYN_ANTAGONIZE = 711,
        ACTION_SYN_CLEAR_ANTAGONIZE = 712,
        ACTION_SYN_ALLY = 713,
        ACTION_SYN_CLEAR_ALLY = 714,
        ACTION_SYN_KICKOUT_MEMBER = 715,
        ACTION_SYN_CREATENEW_PET = 716,
        ACTION_SYN_CHANGESYN = 718,
        ACTION_SYN_CHANGE_SUBNAME = 719,
        ACTION_SYN_FIND_NEXT_SYN = 720,
        ACTION_SYN_FIND_BY_NAME = 721,
        ACTION_SYN_FIND_NEXT_SYNMEMBER = 722,
        ACTION_SYN_SAINT = 724,
        ACTION_SYN_RANK = 726,
        ACTION_SYN_UPMEMBERLEVEL = 728,
        ACTION_SYN_APPLLY_ATTACKSYN = 730,
        ACTION_SYN_LIMIT = 799,

        //Monsters
        ACTION_MST_FIRST = 800,
        ACTION_MST_DROPITEM = 801,
        ACTION_MST_MAGIC = 802,
        ACTION_MST_REFINERY = 803,
        ACTION_MST_LIMIT = 899,

        //Family
        ACTION_FAMILY_FIRST = 900,
        ACTION_FAMILY_CREATE = 901,
        ACTION_FAMILY_DESTROY = 902,
        ACTION_FAMILY_DONATE = 903,
        ACTION_FAMILY_DEMISE = 904,
        ACTION_FAMILY_ANTAGONIZE = 911,
        ACTION_FAMILY_CLEAR_ANTAGONIZE = 912,
        ACTION_FAMILY_ALLY = 913,
        ACTION_FAMILY_CLEAR_ALLY = 914,
        ACTION_FAMILY_KICKOUT_MEMBER = 915,
        ACTION_FAMILY_ATTR = 917,
		ACTION_FAMILY_UPLEV = 918,
		ACTION_FAMILY_BPUPLEV = 919,
        ACTION_FAMILY_LIMIT = 999,

        //User
        ACTION_USER_FIRST = 1000,
        ACTION_USER_ATTR = 1001,
        /* attrname opt value other
         * force opt (+=, ==, <) value
         * speed opt (+=, ==, <) value
         * health opt (+=, ==, <) value
         * soul opt (+=, ==, <) value
         * metempsychosis opt (==, <) value
         * nobility_rank opt (==, <) value
         * level opt (+=, ==, <) value
         * money opt (+=, ==, <) value
         * e_money opt (+=, ==, <) value
         * e_money2 opt (+=, ==, <) value
         * profession opt (==, <=, >=, set) value
         * first_profession  opt (==, <=, >=) value
         * last_profession  opt (==, <=, >=) value
         * pk opt (+=, ==, <) value
         * exp opt (+=, ==, <) value (if don't want to add conttribution to mentor, last param is nocontribute)
         * vip opt (<, ==) value
         * subclass type opt (<=, >=, ==, +=) value
         * Subclass types
         * MARTIAL_ARTIST = 1,
         * WARLOCK = 2,
         * CHI_MASTER = 3,
         * SAGE = 4,
         * APOTHECARY = 5,
         * PERFORMER = 6,
         * WRANGLER = 9
         */
        ACTION_USER_FULL = 1002, // Fill the user attributes. param is the attribute name. life/mana/xp/sp
        ACTION_USER_CHGMAP = 1003, // Mapid Mapx Mapy savelocation
        ACTION_USER_RECORDPOINT = 1004, // Records the user location, so he can be teleported back there later.
        ACTION_USER_HAIR = 1005,
        ACTION_USER_CHGMAPRECORD = 1006,
        ACTION_USER_CHGLINKMAP = 1007,
        ACTION_USER_TRANSFORM = 1008,
        ACTION_USER_ISPURE = 1009,
        ACTION_USER_TALK = 1010,
        ACTION_USER_MAGIC = 1020,
        ACTION_USER_WEAPONSKILL = 1021,
        ACTION_USER_LOG = 1022,
        ACTION_USER_BONUS = 1023,
        ACTION_USER_DIVORCE = 1024,
        ACTION_USER_MARRIAGE = 1025,
        ACTION_USER_SEX = 1026,
        ACTION_USER_EFFECT = 1027,
        ACTION_USER_TASKMASK = 1028,
        ACTION_USER_MEDIAPLAY = 1029,
        ACTION_USER_SUPERMANLIST = 1030,
        ACTION_USER_ADD_TITLE = 1031,
        ACTION_USER_REMOVE_TITLE = 1032,
        ACTION_USER_CREATEMAP = 1033,
        ACTION_USER_ENTER_HOME = 1034,
        ACTION_USER_ENTER_MATE_HOME = 1035,
        ACTION_USER_CHKIN_CARD2 = 1036,
        ACTION_USER_CHKOUT_CARD2 = 1037,
        ACTION_USER_FLY_NEIGHBOR = 1038,
        ACTION_USER_UNLEARN_MAGIC = 1039,
        ACTION_USER_REBIRTH = 1040,
        ACTION_USER_WEBPAGE = 1041,
        ACTION_USER_BBS = 1042,
        ACTION_USER_UNLEARN_SKILL = 1043,
        ACTION_USER_DROP_MAGIC = 1044,
        ACTION_USER_FIX_ATTR = 1045,
        ACTION_USER_OPEN_DIALOG = 1046,
        ACTION_USER_CHGMAP_REBORN = 1047,
        ACTION_USER_EXP_MULTIPLY = 1048,
        ACTION_USER_DEL_WPG_BADGE = 1049,
        ACTION_USER_CHK_WPG_BADGE = 1050,
        ACTION_USER_TAKESTUDENTEXP = 1051,
        ACTION_USER_WH_PASSWORD = 1052,
        ACTION_USER_SET_WH_PASSWORD = 1053,
        ACTION_USER_OPENINTERFACE = 1054,
        ACTION_USER_VAR_COMPARE = 1060,
        ACTION_USER_VAR_DEFINE = 1061,
        ACTION_USER_VAR_CALC = 1064,
        ACTION_USER_STC_COMPARE = 1073,
        ACTION_USER_STC_OPE = 1074,
        ACTION_USER_TASK_MANAGER = 1080,
        ACTION_USER_TASK_OPE = 1081,
        ACTION_USER_ATTACH_STATUS = 1082,
        ACTION_USER_GOD_TIME = 1083,
        ACTION_USER_EXPBALL_EXP = 1086,
        ACTION_USER_STATUS_CREATE = 1096,
        ACTION_USER_STATUS_CHECK = 1098,

        //User -> Team
        ACTION_TEAM_BROADCAST = 1101,
        ACTION_TEAM_ATTR = 1102,
        ACTION_TEAM_LEAVESPACE = 1103,
        ACTION_TEAM_ITEM_ADD = 1104,
        ACTION_TEAM_ITEM_DEL = 1105,
        ACTION_TEAM_ITEM_CHECK = 1106,
        ACTION_TEAM_CHGMAP = 1107,
        ACTION_TEAM_CHK_ISLEADER = 1501,

        // User -> General
        ACTION_GENERAL_LOTTERY = 1508,
        ACTION_GENERA_SUBCLASS_MANAGEMENT = 1509,
        ACTION_GENERAL_SKILL_LINE_ENABLED = 1510,

        // User -> Elite PK
        ACTION_ELITEPK_INSCRIBE = 1601,
        ACTION_ELITEPK_UNINSCRIBE = 1602,
        ACTION_ELITEPK_CHECKPRIZE = 1603,
        ACTION_ELITEPK_AWARDPRIZE = 1604,

        ACTION_USER_LIMIT = 1999,

        //Events
        ACTION_EVENT_FIRST = 2000,
        ACTION_EVENT_SETSTATUS = 2001,
        ACTION_EVENT_DELNPC_GENID = 2002,
        ACTION_EVENT_COMPARE = 2003,
        ACTION_EVENT_COMPARE_UNSIGNED = 2004,
        ACTION_EVENT_CHANGEWEATHER = 2005,
        ACTION_EVENT_CREATEPET = 2006,
        ACTION_EVENT_CREATENEW_NPC = 2007,
        ACTION_EVENT_COUNTMONSTER = 2008,
        ACTION_EVENT_DELETEMONSTER = 2009,
        ACTION_EVENT_BBS = 2010,
        ACTION_EVENT_ERASE = 2011,
        ACTION_EVENT_TELEPORT = 2012,
        ACTION_EVENT_MASSACTION = 2013,
        ACTION_EVENT_SYN_SCORE_FINISH = 2014,
        ACTION_EVENT_LIMIT = 2099,

        //Traps
        ACTION_TRAP_FIRST = 2100,
        ACTION_TRAP_CREATE = 2101,
        ACTION_TRAP_ERASE = 2102,
        ACTION_TRAP_COUNT = 2103,
        ACTION_TRAP_ATTR = 2104,
        ACTION_TRAP_LIMIT = 2199,

        // Detained Item
        ACTION_DETAIN_FIRST = 2200,
        ACTION_DETAIN_DIALOG = 2205,
        ACTION_DETAIN_LIMIT = 2299,

        //Wanted
        ACTION_WANTED_FIRST = 3000,
        ACTION_WANTED_NEXT = 3001,
        ACTION_WANTED_NAME = 3002,
        ACTION_WANTED_BONUTY = 3003,
        ACTION_WANTED_NEW = 3004,
        ACTION_WANTED_ORDER = 3005,
        ACTION_WANTED_CANCEL = 3006,
        ACTION_WANTED_MODIFYID = 3007,
        ACTION_WANTED_SUPERADD = 3008,
        ACTION_POLICEWANTED_NEXT = 3010,
        ACTION_POLICEWANTED_ORDER = 3011,
        ACTION_POLICEWANTED_CHECK = 3012,
        ACTION_WANTED_LIMIT = 3099,

        //Magic
        ACTION_MAGIC_FIRST = 4000,
        ACTION_MAGIC_ATTACHSTATUS = 4001,
        ACTION_MAGIC_ATTACK = 4002,
        ACTION_MAGIC_LIMIT = 4099
    }
}