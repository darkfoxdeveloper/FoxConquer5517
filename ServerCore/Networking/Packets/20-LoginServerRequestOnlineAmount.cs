// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: FelipeVieira
// File Created by: Felipe Vieira Vendramini
// zfserver - Core - 20 - Login Server Request Online Amount.cs
// File Created: 2015/08/20 02:22

using Core.Common.Enums;
using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgLoginSvPlayerAmount : PacketStructure
    {
        /// <summary>
        /// This constructor build the packet to ask for the online amount.
        /// </summary>
        public MsgLoginSvPlayerAmount()
            : base(PacketType.LOGIN_REQUEST_ONLINE_NUMBER, 16, 8)
        {
            Type = LoginPlayerAmountRequest.REQUEST_ONLINE_AMOUNT;
        }

        public MsgLoginSvPlayerAmount(ushort usValue, LoginPlayerAmountRequest request)
            : base(PacketType.LOGIN_REQUEST_ONLINE_NUMBER, 16, 8)
        {
            Amount = usValue;
            Type = request;
        }

        /// <summary>
        /// Gets a built packet and deserialize it for data reading.
        /// </summary>
        /// <param name="pMsg">The buffer that will be read.</param>
        public MsgLoginSvPlayerAmount(byte[] pMsg)
            : base(pMsg)
        {
            
        }

        public LoginPlayerAmountRequest Type
        {
            get { return (LoginPlayerAmountRequest) ReadUShort(4); }
            set { WriteUShort((ushort) value, 4); }
        }

        public ushort Amount
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }
    }
}
