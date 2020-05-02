// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2077 - Purification Activate.cs
// Last Edit: 2016/11/23 09:18
// Created: 2016/11/23 09:18
namespace ServerCore.Networking.Packets
{
    public enum PurificationType : byte
    {
        REFINERY = 0,
        ADD_REFINERY = 2,
        DRAGON_SOUL = 5,
        ADD_DRAGON_SOUL = 6,
        REMOVE_DRAGON_SOUL = 7
    }

    public sealed class MsgItemStatus : PacketStructure
    {
        public MsgItemStatus()
            : base(PacketType.MSG_ITEM_STATUS, 16, 8)
        {

        }

        public MsgItemStatus(byte[] packet)
            : base(packet)
        {

        }

        public uint Type
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Count
        {
            get { return ReadUInt(4); }
            set
            {
                Resize((int)(8 + (value * 28) + 8));
                WriteHeader(Length - 8, PacketType.MSG_ITEM_STATUS);
                WriteUInt(value, 4);
            }
        }

        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public PurificationType Mode
        {
            get { return (PurificationType)ReadByte(12); }
            set { WriteByte((byte)value, 12); }
        }

        /// <summary>
        /// The itemtype of the refinery or dragon soul.
        /// </summary>
        public uint PurificationIdentity
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint Level
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint Percent
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint Time
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        /// <summary>
        /// This method will append the dragon soul data into the packet to be sent to the client.
        /// </summary>
        /// <param name="Target">The item unique identification.</param>
        /// <param name="Type">The Dragon Soul type is 6.</param>
        /// <param name="PurifyIdentity">The Dragon Soul itemtype.</param>
        /// <param name="PurifyLevel">The Dragon Soul Phase.</param>
        /// <param name="PurifyDuration">The remaining seconds of the dragon soul time.</param>
        /// <returns>If the packet has been succesfuly written.</returns>
        public bool Append(uint Target, uint Type, uint PurifyIdentity, uint PurifyLevel,
            uint PurifyDuration)
        {
            Count += 1;
            var offset = (ushort)(8 + (Count - 1) * 28);
            WriteUInt(Target, offset);
            WriteUInt(Type, offset + 4);
            WriteUInt(PurifyIdentity, offset + 8);
            WriteUInt(PurifyLevel, offset + 12);
            WriteUInt(PurifyDuration, offset + 20);
            WriteUInt(0, offset + 24);
            return true;
        }

        /// <summary>
        /// This method will append the refinery data into the packet to be sent to the client.
        /// </summary>
        /// <param name="Target">The item unique identification.</param>
        /// <param name="Type">The refinery type is 2.</param>
        /// <param name="PurifyIdentity">The Refinery type id.</param>
        /// <param name="PurifyLevel">The Refinery level.</param>
        /// <param name="PurifyPercent">The percent amount that the refinery addicts.</param>
        /// <param name="PurifyDuration">The remaining seconds of the refinery time.</param>
        /// <returns>If the packet has been succesfuly written.</returns>
        public bool Append(uint Target, uint Type, uint PurifyIdentity, uint PurifyLevel,
            uint PurifyPercent, uint PurifyDuration)
        {
            Count += 1;
            var offset = (ushort)(8 + (Count - 1) * 28);
            WriteUInt(Target, offset);
            WriteUInt(Type, offset + 4);
            WriteUInt(PurifyIdentity, offset + 8);
            WriteUInt(PurifyLevel, offset + 12);
            WriteUInt(PurifyPercent, offset + 16);
            WriteUInt(PurifyDuration, offset + 20);
            WriteUInt(0, offset + 24);
            return true;
        }
    }
}