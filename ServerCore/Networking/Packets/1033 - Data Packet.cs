using System;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgData : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1033
        /// Build the date time packet to be sent to configure the hour on the client.
        /// </summary>
        public MsgData()
            : base(44)
        {
            DateTime time = DateTime.Now;
            WriteHeader(Length - 8, PacketType.MSG_DATA);
            WriteInt(time.Year - 1900, 8);
            WriteInt(time.Month - 1, 12);
            WriteInt(time.DayOfYear, 16);
            WriteInt(time.Day, 20);
            WriteInt(time.Hour, 24);
            WriteInt(time.Minute, 28);
            WriteInt(time.Second, 32);
        }
    }
}