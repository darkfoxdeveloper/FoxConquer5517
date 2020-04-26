// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - DB - Table Names.cs
// Last Edit: 2016/12/15 10:29
// Created: 2016/11/23 08:02
namespace DB
{
    public class TableName
    {
        // Table prefix
        public const string TABLE_PREFIX = "cq_";

        // Table names
        public const string ACCOUNT_TABLE = "account";
        public const string AD_LOG = "ad_log";
        public const string AD_QUEUE = "ad_queue";
        public const string ACTION = TABLE_PREFIX + "action";
        public const string ARENA = TABLE_PREFIX + "arena";
        public const string BONUS = TABLE_PREFIX + "bonus";
        public const string BUSINESS = TABLE_PREFIX + "business";
        public const string CARRY = TABLE_PREFIX + "carry";
        public const string CONFIG = TABLE_PREFIX + "config";
        public const string DISDAIN = TABLE_PREFIX + "disdain";
        public const string DYNAMAP = TABLE_PREFIX + "dynamap";
        public const string DYNANPC = TABLE_PREFIX + "dynanpc";
        public const string ENEMY = TABLE_PREFIX + "enemy";
        public const string FRIEND = TABLE_PREFIX + "friend";
        public const string FAMILY = TABLE_PREFIX + "family";
        public const string FAMILY_ATTR = TABLE_PREFIX + "family_attr";
        public const string FLOWER = TABLE_PREFIX + "flower";
        public const string FUSE = TABLE_PREFIX + "fuse";
        public const string GENERATOR = TABLE_PREFIX + "generator";
        public const string GOODS = TABLE_PREFIX + "goods";
        public const string ITEM = TABLE_PREFIX + "item";
        public const string ITEMADDITION = TABLE_PREFIX + "itemaddition";
        public const string ITEMTYPE = TABLE_PREFIX + "itemtype";
        public const string LEVEXP = TABLE_PREFIX + "levexp";
        public const string LOTTERY = TABLE_PREFIX + "lottery";
        public const string MAGIC = TABLE_PREFIX + "magic";
        public const string MAGICTYPE = TABLE_PREFIX + "magictype";
        public const string MONSTER_MAGIC = TABLE_PREFIX + "monster_magic";
        public const string MAP = TABLE_PREFIX + "map";
        public const string NPC = TABLE_PREFIX + "npc";
        public const string PASSWAY = TABLE_PREFIX + "passway";
        public const string PK_BONUS = TABLE_PREFIX + "pk_bonus";
        public const string PK_ITEM = TABLE_PREFIX + "pk_item";
        public const string POINT_ALLOT = TABLE_PREFIX + "point_allot";
        public const string PORTAL = TABLE_PREFIX + "portal";
        public const string QUIZ = TABLE_PREFIX + "quiz";
        public const string SUPERMAN = TABLE_PREFIX + "superman";
        public const string SYNDICATE = TABLE_PREFIX + "syndicate";
        public const string SYNDICATE_ATTR = TABLE_PREFIX + "synattr";
        public const string SYN_ADVERTISE = TABLE_PREFIX + "syn_advertise";
        public const string TASK = TABLE_PREFIX + "task";
        public const string TUTOR = TABLE_PREFIX + "tutor";
        public const string TUTOR_ACCESS = TABLE_PREFIX + "tutor_access";
        public const string TUTOR_BATTLE_LIMIT_TYPE = TABLE_PREFIX + "tutor_battle_limit_type";
        public const string TUTOR_CONTRIBUTION = TABLE_PREFIX + "tutor_contributions";
        public const string TUTOR_TYPE = TABLE_PREFIX + "tutor_type";
        public const string USER = TABLE_PREFIX + "user";
        public const string NAME_CHANGE_LOG = TABLE_PREFIX + "user_name_log";
        public const string WEAPON_SKILL = TABLE_PREFIX + "weapon_skill";
        public const string DYNA_RANK_REC = "dyna_rank_rec";
        public const string DYNA_RANK_TYPE = "dyna_rank_type";
        public const string DYNAMICRANKREC = "dynamic_rank";
        public const string DYNAMICRANKTYPE = "dynamic_rank_type"; // this table wont load, just for reference
    }
}