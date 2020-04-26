// World Conquer Project 2.0 - Phoenix Project Based
// Source Development by Felipe Vieira (FTW! Masters)
// Source Infrastructure by Gareth Jensen (Akarui)
// 
// Computer User: FelipeVieira
// File Created by: Felipe Vieira Vendramini
// zfserver - Core - 21 - Online Player List.cs
// File Created: 2015/09/03 08:10

namespace ServerCore.Networking.Packets
{
    public sealed class MsgLoginPlayerInfo : PacketStructure
    {
        public MsgLoginPlayerInfo(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }


    }
}
