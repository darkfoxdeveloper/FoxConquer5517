// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 10005 - MsgWalk.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/23 13:29

using System;
using MsgServer.Structures.World;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleWalk(Client pClient, MsgWalk pMsg)
        {
            if (pClient.Character != null && 
                pClient.Character.Identity == pMsg.Identity)
            {
                byte direction;
                ushort x;
                ushort y;

                // Get the data from the packet:
                switch (pMsg.Action)
                {
                    case MovementType.WALK:
                    case MovementType.RUN:
                        direction = (byte)(pMsg.Direction % 8);
                        x = (ushort)(pClient.Character.MapX + WALK_X_COORDS[direction]);
                        y = (ushort)(pClient.Character.MapY + WALK_Y_COORDS[direction]);
                        break;
                    case MovementType.RIDE:
                        direction = (byte)(pMsg.Direction % 24);
                        x = (ushort)(pClient.Character.MapX + DELTA_WALK_X_COORDS[direction]);
                        y = (ushort)(pClient.Character.MapY + DELTA_WALK_Y_COORDS[direction]);
                        break;
                    default:
                        pClient.Character.Kickback(pClient.Character.MapX, pClient.Character.MapY);
                        return;

                }

                Tile tile = pClient.Character.Map[x, y];
                if (tile.Access > TileType.NPC && !pClient.Character.IsFreeze
                    && !(pClient.Character.QueryStatus(Core.Common.Enums.FlagInt.CTF_FLAG) != null 
                    && pClient.Character.Map.QueryRegion(RegionType.REGION_PK_PROTECTED, x, y)))
                {
                    // The packet is valid. Assign character data:
                    pClient.Character.MapX = x;
                    pClient.Character.MapY = y;
                    pClient.Character.Direction = (FacingDirection)direction;
                    pClient.Character.Action = EntityAction.STAND;
                    pClient.Character.Elevation = tile.Elevation;
                    pClient.Tile = tile;

                    // Send the movement back to the message server and client:
                    pClient.Send(pMsg);
                    pClient.Screen.SendMovement(pMsg);

                    if (pClient.Character.QueryStatus(Core.Common.Enums.FlagInt.RIDING) != null)
                    {
                        if (pClient.Character.Vigor > 1)
                            pClient.Character.Vigor -= 2;
                    }

                    pClient.Character.ProcessOnMove();
                    return;
                }
                pClient.Character.Kickback(pClient.Character.MapX, pClient.Character.MapY);
            }
            else
            {
                Console.WriteLine("RoleID:{0} tried MsgWalk::{1}", pMsg.Identity, pMsg.Action);
            }
        }
    }
}