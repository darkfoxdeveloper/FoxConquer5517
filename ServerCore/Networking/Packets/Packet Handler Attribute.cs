// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Packet Handler Attribute.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This attribute class provides the server with an associating declarative for packet handlers. Any packet
    /// handling method declared using this attribute will be added to the packet processor (the red-black tree
    /// used for storing packet handlers by packet identity).
    /// </summary>
    public sealed class PacketHandlerType : Attribute, IPacketAttribute
    {
        /// <summary>
        /// This attribute class provides the server with an associating declarative for packet handlers. Any packet
        /// handling method declared using this attribute will be added to the packet processor (the red-black tree
        /// used for storing packet handlers by packet identity).
        /// </summary>
        /// <param name="packetType">The type of packet the handler will process.</param>
        public PacketHandlerType(PacketType packetType) { Type = packetType; }
        public IComparable Type { get; set; }
    }

    /// <summary> This interface defines a packet attribute class for the packet processor class. </summary>
    public interface IPacketAttribute
    {
        IComparable Type { get; set; }
    }
}