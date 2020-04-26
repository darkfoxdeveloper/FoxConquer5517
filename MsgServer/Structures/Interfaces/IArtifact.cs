// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IArtifact.cs
// Last Edit: 2016/11/23 23:17
// Created: 2016/11/23 23:17

using DB.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Interfaces
{
    public struct IArtifact
    {
        public bool Avaiable;
        public uint ItemIdentity;
        public uint ArtifactType;
        public uint ArtifactLevel;
        public uint ArtifactExpireTime;
        public uint ArtifactStartTime;
        public uint StabilizationPoints;
        public DbItemtype Artifact;

        public MsgItemStatus BuildPacket()
        {
            int now = UnixTimestamp.Timestamp();
            var purify = new MsgItemStatus();
            if (now < ArtifactExpireTime)
                purify.Append(ItemIdentity, 6, ArtifactType, ArtifactLevel, (uint)(ArtifactExpireTime - now));
            else
                purify.Append(ItemIdentity, 6, ArtifactType, ArtifactLevel, 0);
            return purify;
        }
    }
}