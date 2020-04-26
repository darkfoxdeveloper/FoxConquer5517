// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Item Default.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common
{
    public static class ItemDefault
    {
        public static readonly int[] ARTIFACT_STABILIZATION_POINTS = { 0, 10, 30, 50, 100, 150, 200, 300 };
        public static readonly int[] REFINERY_STABILIZATION_POINTS = { 0, 10, 30, 70, 150, 270, 500 };

        public static readonly int[] TALISMAN_SOCKET_QUALITY_ADDITION = { 0, 0, 0, 0, 0, 0, 5, 10, 40, 1000 };
        public static readonly int[] TALISMAN_SOCKET_PLUS_ADDITION = { 0, 6, 30, 80, 240, 740, 2220, 6660, 20000, 60000, 62000, 64000, 72000 };
        public static readonly int[] TALISMAN_SOCKET_HOLE_ADDITION0 = { 0, 160, 800 };
        public static readonly int[] TALISMAN_SOCKET_HOLE_ADDITION1 = { 0, 2000, 6000 };

        public const int MAX_UPGRADE_CHECK = 10;
        // Single hand weapon define 400000
        public const int SWEAPON_NONE = 00000;
        public const int SWEAPON_BLADE = 10000;
        public const int SWEAPON_SWORD = 20000;
        public const int SWEAPON_BACKSWORD = 21000;
        public const int SWEAPON_HOOK = 30000;
        public const int SWEAPON_WHIP = 40000;
        public const int SWEAPON_AXE = 50000;
        public const int SWEAPON_HAMMER = 60000;
        public const int SWEAPON_CLUB = 80000;
        public const int SWEAPON_SCEPTER = 81000;
        public const int SWEAPON_DAGGER = 90000;
        // Double hand weapon define 500000
        public const int DWEAPON_BOW = 00000;
        public const int DWEAPON_GLAVE = 10000;
        public const int DWEAPON_POLEAXE = 30000;
        public const int DWEAPON_LONGHAMMER = 40000;
        public const int DWEAPON_SPEAR = 60000;
        public const int DWEAPON_WANT = 61000;
        public const int DWEAPON_HALBERT = 80000;
        // Other weapon define 600000
        public const int SWEAPON_KATANA = 01000;
        public const int SWEAPON_PRAYER_BEADS = 10000;
    }

    public static class SpecialItem
    {
        //
        public const uint TYPE_DRAGONBALL = 1088000;
        public const uint TYPE_METEOR = 1088001;
        public const uint TYPE_METEORTEAR = 1088002;
        public const uint TYPE_TOUGHDRILL = 1200005;
        public const uint TYPE_STARDRILL = 1200006;
        //
        public const uint TYPE_DRAGONBALL_SCROLL = 720028; // Amount 10
        public const uint TYPE_METEOR_SCROLL = 720027; // Amount 10
        public const uint TYPE_METEORTEAR_PACK = 723711; // Amount 5
        //
        public const uint TYPE_STONE1 = 730001;
        public const uint TYPE_STONE2 = 730002;
        public const uint TYPE_STONE3 = 730003;
        public const uint TYPE_STONE4 = 730004;
        public const uint TYPE_STONE5 = 730005;
        public const uint TYPE_STONE6 = 730006;
        public const uint TYPE_STONE7 = 730007;
        public const uint TYPE_STONE8 = 730008;
        //
        public const uint TYPE_MOUNT_ID = 300000;
        //
        public const uint TYPE_EXP_BALL = 723700;

        public static readonly int[] BOWMAN_ARROWS =
        {
            1050000, 1050001, 1050002, 1050020, 1050021, 1050022, 1050023, 1050030, 1050031, 1050032, 1050033, 1050040,
            1050041, 1050042, 1050043, 1050050, 1050051, 1050052 
        };

        public const uint IRON_ORE = 1072010;
        public const uint COPPER_ORE = 1072020;
        public const uint EUXINITE_ORE = 1072031;
        public const uint SILVER_ORE = 1072040;
        public const uint GOLD_ORE = 1072050;

        public const uint OBLIVION_DEW = 711083;
        public const uint MEMORY_AGATE = 720828;

        public const uint CLOUDSAINTS_JAIR = 750000;
    }

    public enum ItemSort
    {
        ITEMSORT_WEAPON_SINGLE_HAND = 4,
        ITEMSORT_WEAPON_DOUBLE_HAND = 5,
        ITEMSORT_WEAPON_SINGLE_HAND2 = 6,
        ITEMSORT_USABLE = 7,
        ITEMSORT_WEAPON_SHIELD = 9,
        ITEMSORT_USABLE2 = 10,
        ITEMSORT_USABLE3 = 12,
        ITEMSORT_ACCESSORY = 3,
        ITEMSORT_TWOHAND_ACCESSORY = 35,
        ITEMSORT_ONEHAND_ACCESSORY = 36,
        ITEMSORT_BOW_ACCESSORY = 37,
        ITEMSORT_SHIELD_ACCESSORY = 38
    }
}
