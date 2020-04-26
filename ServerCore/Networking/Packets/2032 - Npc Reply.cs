// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2032 - Npc Reply.cs
// Last Edit: 2016/11/23 09:01
// Created: 2016/11/23 09:00
namespace ServerCore.Networking.Packets
{
    public sealed class MsgTaskDialog : PacketStructure
    {
        /// <summary>
        /// The types of NPC Dialog.
        /// </summary>
        public const byte
            CLIENT_REQUEST = 0,
            DIALOG = 1,
            OPTION = 2,
            INPUT = 3,
            AVATAR = 4,
            LAY_NPC = 5,
            MESSAGE_BOX = 6,
            FINISH = 100,
            ANSWER = 101,
            TEXT_INPUT = 102;

        /// <summary>
        /// This method will create a empty reply packet with default length.
        /// </summary>
        public MsgTaskDialog()
            : base(PacketType.MSG_TASK_DIALOG, 24, 16)
        {

        }


        public MsgTaskDialog(byte interactType, string text)
            : base(PacketType.MSG_TASK_DIALOG, 25 + text.Length, 17 + text.Length)
        {
            InteractType = interactType;
            OptionId = 255;
            DontDisplay = true;
            Text = text;
        }

        /// <summary>
        /// This method will deserialize a packet.
        /// </summary>
        /// <param name="packet">The Npc Reply packet to be deserialized.</param>
        public MsgTaskDialog(byte[] packet)
            : base(packet)
        {

        }

        public uint TaskId
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public ushort InputMaxLength
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public byte OptionId
        {
            get { return ReadByte(10); }
            set { WriteByte(value, 10); }
        }

        public byte InteractType
        {
            get { return ReadByte(11); }
            set { WriteByte(value, 11); }
        }

        public bool DontDisplay
        {
            get { return ReadBoolean(12); }
            set { WriteBoolean(value, 12); }
        }

        public string Text
        {
            get { return ReadString(ReadByte(13), 14); }
            set
            {
                Resize(25 + value.Length);
                WriteStringWithLength(value, 13);
            }
        }
    }
}