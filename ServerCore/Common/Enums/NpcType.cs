// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Npc Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public static class NpcTypes
    {
        public const int
            NPC_NONE = 0, // Í¨ÓÃNPC
            SHOPKEEPER_NPC = 1, // ÉÌµêNPC
            TASK_NPC = 2, // ÈÎÎñNPC(ÒÑ×÷·Ï£¬½öÓÃÓÚ¼æÈÝ¾ÉÊý¾Ý)
            STORAGE_NPC = 3, // ¼Ä´æ´¦NPC
            TRUNCK_NPC = 4, // Ïä×ÓNPC
            FACE_NPC = 5, // ±äÍ·ÏñNPC
            FORGE_NPC = 6, // ¶ÍÔìNPC		(only use for client)
            EMBED_NPC = 7, // ÏâÇ¶NPC
            STATUARY_NPC = 9, // µñÏñNPC
            SYNFLAG_NPC = 10, // °ïÅÉ±ê¼ÇNPC
            ROLE_PLAYER = 11, // ÆäËûÍæ¼Ò		(only use for client)
            ROLE_HERO = 12, // ×Ô¼º			(only use for client)
            ROLE_MONSTER = 13, // ¹ÖÎï			(only use for client)
            BOOTH_NPC = 14, // °ÚÌ¯NPC		(CBooth class)
            SYNTRANS_NPC = 15, // °ïÅÉ´«ËÍNPC, ¹Ì¶¨ÄÇ¸ö²»ÒªÓÃ´ËÀàÐÍ! (ÓÃÓÚ00:00ÊÕ·Ñ)(LINKIDÎª¹Ì¶¨NPCµÄID£¬ÓëÆäËüÊ¹ÓÃLINKIDµÄ»¥³â)
            ROLE_BOOTH_FLAG_NPC = 16, // Ì¯Î»±êÖ¾NPC	(only use for client)
            ROLE_MOUSE_NPC = 17, // Êó±êÉÏµÄNPC	(only use for client)
            ROLE_MAGICITEM = 18, // ÏÝÚå»ðÇ½		(only use for client)
            ROLE_DICE_NPC = 19, // ÷»×ÓNPC
            ROLE_SHELF_NPC = 20, // ÎïÆ·¼Ü
            WEAPONGOAL_NPC = 21, // ÎäÆ÷°Ð×ÓNPC
            MAGICGOAL_NPC = 22, // Ä§·¨°Ð×ÓNPC
            BOWGOAL_NPC = 23, // ¹­¼ý°Ð×ÓNPC
            ROLE_TARGET_NPC = 24, // °¤´ò£¬²»´¥·¢ÈÎÎñ	(only use for client)
            ROLE_FURNITURE_NPC = 25, // ¼Ò¾ßNPC	(only use for client)
            ROLE_CITY_GATE_NPC = 26, // ³ÇÃÅNPC	(only use for client)
            ROLE_NEIGHBOR_DOOR = 27, // ÁÚ¾ÓµÄÃÅ
            ROLE_CALL_PET = 28, // ÕÙ»½ÊÞ	(only use for client)
            EUDEMON_TRAINPLACE_NPC = 29, // »ÃÊÞÑ±ÑøËù
            AUCTION_NPC = 30, // ÅÄÂòNPC	ÎïÆ·ÁìÈ¡NPC  LW
            ROLE_MINE_NPC = 31, // ¿óÊ¯NPC		
            ROLE_CTFBASE_NPC = 40,
            ROLE_3DFURNITURE_NPC = 101, // 3D¼Ò¾ßNPC 
            SYN_NPC_WARLETTER = 110; //Ôö¼ÓÐÂµÄ£Î£Ð£ÃÀàÐÍ¡¡×¨ÃÅÓÃÀ´¡¡ÏÂÕ½ÊéµÄ¡¡°ïÅÉ£Î£Ð£Ã
    }
}