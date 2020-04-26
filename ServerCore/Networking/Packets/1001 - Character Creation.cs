// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1001 - Character Creation.cs
// Last Edit: 2016/11/23 08:11
// Created: 2016/11/23 08:08
namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1001. This packet is sent to the server after the player has finished customizing and naming a
    /// new character from the character creation window. The packet contains the player's selected gender, profession,
    /// and name, and the player's mac address. 
    /// </summary>
    public sealed class MsgRegister : PacketStructure
    {
        /// <summary>
        /// Packet Type: 1001. This packet is sent to the server after the player has finished customizing and naming a
        /// new character from the character creation window. The packet contains the player's selected gender, profession,
        /// and name, and the player's mac address. 
        /// </summary>
        /// <param name="array">The received packet.</param>
        public MsgRegister(byte[] array)
            : base(array)
        {
        }

        /// <summary>
        /// Offset 4 - True if the creation window is being closed or the player clicked on the back button to return
        /// to the main login screen.
        /// </summary>
        public bool CancelRequest
        {
            get { return ReadBoolean(4); }
        }

        /// <summary>
        /// Offset 20 - The name of the character being created. It must be checked by the database and server 
        /// before being used on the created character.
        /// </summary>
        public string Name
        {
            get { return ReadString(15, 24); }
        }

        /// <summary>
        /// Offset 52 - The type of body for the character being created. Must be checked by the server since a
        /// bot can easily change this value to any type of entity supported by the client.
        /// </summary>
        public ushort Body
        {
            get { return ReadUShort(72); }
        }

        /// <summary>
        /// Offset 54 - The profession of the new character being created. It must be checked by the server since a
        /// bot can easily change this value to any profession supported by the client (but not supported by the 
        /// server).
        /// </summary>
        public ushort Profession
        {
            get { return ReadUShort(74); }
        }

        /// <summary>
        /// Offset 60 - The mac address of the client connected to the server. Used to track special server offers
        /// (one offer per person - not per account). 
        /// </summary>
        public string MacAddress
        {
            get { return ReadString(12, 80); }
        }
    }
}