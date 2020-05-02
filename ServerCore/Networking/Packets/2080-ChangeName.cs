namespace ServerCore.Networking.Packets
{
    public enum ChangeNameMode
    {
        /// <summary>
        /// This contains the name after a normal request.
        /// </summary>
        CHANGE_NAME = 0,
        /// <summary>
        /// If the name has been changed successfuly, reply with this mode.
        /// </summary>
        CHANGED_SUCCESSFULY = 1,
        /// <summary>
        /// This is sent as reply when someone inputs a already existent name.
        /// </summary>
        NAME_IN_USE = 2,
        /// <summary>
        /// This is the common screen asking for how many times the user has changed his name
        /// and how many times he still can change it.
        /// </summary>
        REQUEST_INFO = 3,
        /// <summary>
        /// This is sent when the user has an invalid username or he can change his name
        /// for free. This should not be sent if the character name has invalid characters.
        /// Because first of all, if he input invalid characters, you don't have to register
        /// the name change. This will reply with the same Mode.
        /// </summary>
        CHANGE_NAME_ERROR = 4
    }

    public sealed class MsgChangeName : PacketStructure
    {
        public MsgChangeName()
            : base(PacketType.MSG_CHANGE_NAME, 34, 26)
        {

        }

        public MsgChangeName(byte[] packet)
            : base(packet)
        {

        }

        public MsgChangeName(ushort editedAmount, ushort canEditAmount)
            : base(PacketType.MSG_CHANGE_NAME, 34, 26)
        {
            Mode = ChangeNameMode.REQUEST_INFO;
            Param1 = editedAmount;
            Param2 = canEditAmount;
        }

        public ChangeNameMode Mode
        {
            get { return (ChangeNameMode)ReadUShort(4); }
            set { WriteUShort((ushort)value, 4); }
        }

        public ushort Param1
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        public ushort Param2
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public string Name
        {
            get { return ReadString(16, 10); }
            set { WriteString(value, 16, 10); }
        }
    }
}