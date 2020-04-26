// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Magic Sort.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public static class MagicSort
    {
        public const int MAGICSORT_ATTACK = 1,
            MAGICSORT_RECRUIT = 2, // support auto active.
            MAGICSORT_CROSS = 3,
            MAGICSORT_FAN = 4, // support auto active(random).
            MAGICSORT_BOMB = 5,
            MAGICSORT_ATTACHSTATUS = 6,
            MAGICSORT_DETACHSTATUS = 7,
            MAGICSORT_SQUARE = 8,
            MAGICSORT_JUMPATTACK = 9, // move, a-lock
            MAGICSORT_RANDOMTRANS = 10, // move, a-lock
            MAGICSORT_DISPATCHXP = 11,
            MAGICSORT_COLLIDE = 12, // move, a-lock & b-synchro
            MAGICSORT_SERIALCUT = 13, // auto active only.
            MAGICSORT_LINE = 14, // support auto active(random).
            MAGICSORT_ATKRANGE = 15, // auto active only, forever active.
            MAGICSORT_ATKSTATUS = 16, // support auto active, random active.
            MAGICSORT_CALLTEAMMEMBER = 17,
            MAGICSORT_RECORDTRANSSPELL = 18,
            MAGICSORT_TRANSFORM = 19,
            MAGICSORT_ADDMANA = 20, // support self target only.
            MAGICSORT_LAYTRAP = 21,
            MAGICSORT_DANCE = 22, // ÌøÎè(only use for client)
            MAGICSORT_CALLPET = 23, // ÕÙ»½ÊÞ
            MAGICSORT_VAMPIRE = 24, // ÎüÑª£¬power is percent award. use for call pet
            MAGICSORT_INSTEAD = 25, // ÌæÉí. use for call pet
            MAGICSORT_DECLIFE = 26, // ¿ÛÑª(µ±Ç°ÑªµÄ±ÈÀý)
            MAGICSORT_GROUNDSTING = 27, // µØ´Ì,
            MAGICSORT_VORTEX = 28,
            MAGICSORT_ACTIVATESWITCH = 29,
            MAGICSORT_SPOOK = 30,
            MAGICSORT_WARCRY = 31,
            MAGICSORT_RIDING = 32,
            MAGICSORT_ATTACHSTATUS_AREA = 34,
            MAGICSORT_REMOTEBOMB = 35, // fuck tq i dont know what name to use _|_
            MAGICSORT_KNOCKBACK = 38,
            MAGICSORT_DASHWHIRL = 40,
            MAGICSORT_PERSEVERANCE = 41,
            MAGICSORT_SELFDETACH = 46,
            MAGICSORT_DETACHBADSTATUS = 47,
            MAGICSORT_CLOSE_LINE = 48,
            MAGICSORT_COMPASSION = 50,
            MAGICSORT_TEAMFLAG = 51,
            MAGICSORT_INCREASEBLOCK = 52,
            MAGICSORT_OBLIVION = 53,
            MAGICSORT_STUNBOMB = 54,
            MAGICSORT_TRIPLEATTACK = 55; // gotta find better names

        public const int MAGICDAMAGE_ALT = 26; // ·¨ÊõÐ§¹ûµÄ¸ß²îÏÞÖÆ
        public const int AUTOLEVELUP_EXP = -1; // ×Ô¶¯Éý¼¶µÄ±êÖ¾
        public const int DISABLELEVELUP_EXP = 0; // ²»Éý¼¶µÄ±êÖ¾
        public const int AUTOMAGICLEVEL_PER_USERLEVEL = 10; // Ã¿10¼¶£¬·¨ÊõµÈ¼¶¼ÓÒ»¼¶
        public const int USERLEVELS_PER_MAGICLEVEL = 10; // Íæ¼ÒµÈ¼¶±ØÐëÊÇ·¨ÊõµÈ¼¶µÄ10±¶

        public const int KILLBONUS_PERCENT = 5; // É±ËÀ¹ÖÎïµÄ¶îÍâEXP½±Àø
        public const int HAVETUTORBONUS_PERCENT = 10; // ÓÐµ¼Ê¦µÄÇé¿öÏÂ¶îÍâEXP½±Àø
        public const int WITHTUTORBONUS_PERCENT = 20; // ºÍµ¼Ê¦Ò»ÆðÁ·µÄÇé¿öÏÂEXP½±Àø

        public const int MAGIC_DELAY = 1000; // Ä§·¨DELAY
        public const int MAGIC_DECDELAY_PER_LEVEL = 100; // Ã¿¸ö¡°·¨ÊõµÈ¼¶¡±¼õÉÙµÄÄ§·¨DELAY
        public const int RANDOMTRANS_TRY_TIMES = 10; // Ëæ»úË²ÒÆµÄ³¢ÊÔ´ÎÊý
        public const int DISPATCHXP_NUMBER = 20; // ¼ÓXPµÄÊýÁ¿
        public const int COLLIDE_POWER_PERCENT = 80; // ³å×²Ôö¼Ó¹¥»÷Á¦µÄ°Ù·Ö±È
        public const int COLLIDE_SHIELD_DURABILITY = 3; // ³å×²Òª¼õÉÙµÄ¶ÜÅÆÊÙÃü
        public const int LINE_WEAPON_DURABILITY = 2; // Ö±Ïß¹¥»÷Òª¼õÉÙµÄÎäÆ÷ÊÙÃü
        public const int MAX_SERIALCUTSIZE = 10; // Ë³ÊÆÕ¶µÄÊýÁ¿
        public const int AWARDEXP_BY_TIMES = 1; // °´´ÎÊý¼Ó¾­ÑéÖµ
        public const int AUTO_MAGIC_DELAY_PERCENT = 150; // Á¬ÐøÄ§·¨¹¥»÷Ê±Ôö¼ÓµÄDELAY
        public const int BOW_SUBTYPE = 500; // ¹­µÄSUBTYPE
        public const int POISON_MAGIC_TYPE = 1501; // use for more status
        public const int DEFAULT_MAGIC_FAN = 120; // 
        public const int STUDENTBONUS_PERCENT = 5;		// µ¼Ê¦É±ËÀÒ»Ö»¹ÖÎïÍ½µÜµÃµ½µÄ¾­Ñé°Ù·Ö±È

        public const int MAGIC_KO_LIFE_PERCENT = 15; // ±ØÉ±¼¼ÄÜÈ¥ÑªÉÏÏÞ
        public const int MAGIC_ESCAPE_LIFE_PERCENT = 15; // ÌÓÅÜ¼¼ÄÜÓÐÐ§µÄÉúÃüÉÏÏÞ
    }
}