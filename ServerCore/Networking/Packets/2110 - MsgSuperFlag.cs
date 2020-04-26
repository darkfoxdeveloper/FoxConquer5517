// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2110 - MsgSuperFlag.cs
// Last Edit: 2017/01/04 18:02
// Created: 2017/01/04 18:02

namespace ServerCore.Networking.Packets
{
    public sealed class MsgSuperFlag : PacketStructure
    {
        public MsgSuperFlag()
            : base(PacketType.MSG_SUPER_FLAG, 40, 32)
        {
            
        }

        public MsgSuperFlag(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Action
        {
            get { return ReadUInt(4); }
            set {  WriteUInt(value, 4); }
        }

        public uint ItemIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint CarryIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Durability
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint LocationCount
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public string Name
        {
            get { return ReadString(32, 48); }
            set {  WriteString(value, 32, 48); }
        }

        public void AddLocation(uint idLoc, uint idMap, ushort mapX, ushort mapY, string szName)
        {
            int offset = (int) (32 + (LocationCount*48));
            LocationCount += 1;
            Resize((int) (40 + LocationCount*48));
            WriteHeader(Length - 8, PacketType.MSG_SUPER_FLAG);
            WriteUInt(idLoc, offset);
            WriteUInt(idMap, offset + 4);
            WriteUInt(mapX, offset + 8);
            WriteUInt(mapY, offset + 12);
            WriteString(szName, 32, offset + 16);
        }
    }
}