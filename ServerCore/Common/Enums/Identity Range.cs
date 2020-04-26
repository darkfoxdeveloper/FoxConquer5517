// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Identity Range.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    public class IdentityRange
    {
        public const int SCENEID_FIRST = 1;
        public const int SYSNPCID_FIRST = 1;
        public const int SYSNPCID_LAST = 99999;
        public const int DYNANPCID_FIRST = 100000;
        public const int DYNANPCID_LAST = 199999;
        public const int SCENEID_LAST = 299999;

        public const int SCENE_NPC_MIN = 200000;
        public const int SCENE_NPC_MAX = 299999;

        public const int NPCSERVERID_FIRST = 400001;
        public const int MONSTERID_FIRST = 400001;
        public const int MONSTERID_LAST = 499999;
        public const int PETID_FIRST = 500001;
        public const int PETID_LAST = 599999;
        public const int NPCSERVERID_LAST = 699999;

        public const int CALLPETID_FIRST = 700001;
        public const int CALLPETID_LAST = 799999;

        public const int MAPITEM_FIRST = 800001;
        public const int MAPITEM_LAST = 899999;

        public const int TRAPID_FIRST = 900001;
        public const int MAGICTRAPID_FIRST = 900001;
        public const int MAGICTRAPID_LAST = 989999;
        public const int SYSTRAPID_FIRST = 990001;
        public const int SYSTRAPID_LAST = 999999;
        public const int TRAPID_LAST = 999999;

        public const int DYNAMAP_FIRST = 1000000;

        public const int PLAYER_ID_FIRST = 1000000;
        public const int PLAYER_ID_LAST = 1999999999;
    }
}
