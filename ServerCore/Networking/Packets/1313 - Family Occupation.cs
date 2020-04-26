// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1313 - Family Occupation.cs
// Last Edit: 2016/11/23 08:57
// Created: 2016/11/23 08:57
namespace ServerCore.Networking.Packets
{
    public enum FamilyPromptType
    {
        REQUEST_NPC = 6, // Npc Click Client -> Server -> Client
        ANNOUNCE_WAR_BEGIN = 7, // Call to war Server -> Client
        ANNOUNCE_WAR_ACCEPT = 8 // Answer Ok to annouce Client -> Server
    }

    /// <summary>
    /// This packet is called when the user clicks on the Clan War NPCs. It shows who owns that map and also have some types with
    /// invitations or requests.
    /// </summary>
    public sealed class MsgFamilyOccupy : PacketStructure
    {
        public MsgFamilyOccupy()
            : base(PacketType.MSG_FAMILY_OCCUPY, 144, 136)
        {

        }

        public MsgFamilyOccupy(byte[] packet)
            : base(packet)
        {

        }

        public FamilyPromptType Type
        {
            get { return (FamilyPromptType)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint RequestNpc
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Type2
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public string Winner
        {
            get { return ReadString(16, 20); }
            set { WriteString(value, 16, 20); }
        }

        public string CityName
        {
            get { return ReadString(16, 56); }
            set { WriteString(value, 16, 56); }
        }

        public uint OccupyDays
        {
            get { return ReadUInt(96); }
            set { WriteUInt(value, 96); }
        }

        public uint DailyPrize
        {
            get { return ReadUInt(100); }
            set { WriteUInt(value, 100); }
        }

        public uint WeeklyPrize
        {
            get { return ReadUInt(104); }
            set { WriteUInt(value, 104); }
        }

        public uint GoldFee
        {
            get { return ReadUInt(120); }
            set { WriteUInt(value, 120); }
        }
    }
}