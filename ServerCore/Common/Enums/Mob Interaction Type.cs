// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Mob Interaction Type.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public enum MobInteractionType
    {
        ATKUSER_LEAVEONLY = 0,				// Ö»»áÌÓÅÜ
        ATKUSER_PASSIVE = 0x01,				// ±»¶¯¹¥»÷
        ATKUSER_ACTIVE = 0x02,				// Ö÷¶¯¹¥»÷
        ATKUSER_RIGHTEOUS = 0x04,				// ÕýÒåµÄ(ÎÀ±ø»òÍæ¼ÒÕÙ»½ºÍ¿ØÖÆµÄ¹ÖÎï)
        ATKUSER_GUARD = 0x08,				// ÎÀ±ø(ÎÞÊÂ»ØÔ­Î»ÖÃ)
        ATKUSER_PPKER = 0x10,				// ×·É±ºÚÃû
        ATKUSER_JUMP = 0x20,				// »áÌø
        ATKUSER_FIXED = 0x40,				// ²»»á¶¯µÄ
        ATKUSER_FASTBACK = 0x0080,				// ËÙ¹é
        ATKUSER_LOCKUSER = 0x0100,				// Ëø¶¨¹¥»÷Ö¸¶¨Íæ¼Ò£¬Íæ¼ÒÀë¿ª×Ô¶¯ÏûÊ§
        ATKUSER_LOCKONE = 0x0200,				// Ëø¶¨¹¥»÷Ê×ÏÈ¹¥»÷×Ô¼ºµÄÍæ¼Ò
        ATKUSER_ADDLIFE = 0x0400,				// ×Ô¶¯¼ÓÑª
        ATKUSER_EVIL_KILLER = 0x0800,				// °×ÃûÉ±ÊÖ
        ATKUSER_WING = 0x1000,				// ·ÉÐÐ×´Ì¬
        ATKUSER_NEUTRAL = 0x2000,				// ÖÐÁ¢
        ATKUSER_ROAR = 0x4000,				// ³öÉúÊ±È«µØÍ¼Å­ºð
        ATKUSER_NOESCAPE = 0x8000,				// ²»»áÌÓÅÜ
        ATKUSER_EQUALITY = 0x10000,				// ²»ÃêÊÓ
    }
}