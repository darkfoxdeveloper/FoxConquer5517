// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1027 - Embed Gem.cs
// Last Edit: 2016/11/23 08:30
// Created: 2016/11/23 08:30
namespace ServerCore.Networking.Packets
{
    public enum EmbedMode : ushort
    {
        GEM_ADD = 0,
        GEM_REMOVE = 1
    }

    public sealed class MsgGemEmbed : PacketStructure
    {
        public MsgGemEmbed()
            : base(PacketType.MSG_GEM_EMBED, 28, 20)
        {

        }

        public MsgGemEmbed(byte[] packet)
            : base(packet)
        {

        }

        public uint MainIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint MinorIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public ushort HoleNum
        {
            get { return ReadUShort(16); }
            set { WriteUShort(value, 16); }
        }

        public EmbedMode Mode
        {
            get { return (EmbedMode)ReadUShort(18); }
            set { WriteUShort((ushort)value, 18); }
        }
    }
}