// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1128 - MsgVipUserHandle.cs
// Last Edit: 2017/01/27 17:30
// Created: 2017/01/27 17:28

using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleVipUserHandle(Character pUser, MsgVipUserHandle pMsg)
        {
            if (pUser.Map.IsPrisionMap() || pUser.Map.IsPkField() || pUser.Map.IsSynMap() || pUser.Map.IsFamilyMap()
                || pUser.Map.IsTeleportDisable() || pUser.Map.IsChgMapDisable())
                return;

            switch (pMsg.Type)
            {
                case VIPTeleportTypes.SELF_TELEPORT:
                {
                    if (pMsg.Location > VIPTeleportLocations.BIRD_ILAND)
                    {
                        if (!pUser.CanUserPortalTeleport)
                        {
                            pUser.Send(string.Format(ServerString.STR_VIPTELE_PORTAL_REMAIN, pUser.PortalTeleportWaitTime));
                            return;
                        }
                    }
                    else
                    {
                        if (!pUser.CanUseCityTeleport)
                        {
                            pUser.Send(string.Format(ServerString.STR_VIPTELE_CITY_REMAIN, pUser.CityTeleportWaitTime));
                            return;
                        }
                    }
                    // todo get location
                    switch (pMsg.Location)
                    {
                            // twin city
                        case VIPTeleportLocations.TWIN_CITY:
                        case VIPTeleportLocations.TC_SQUARE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.WP_ALTAR: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.WP_APPARATION: pUser.ChangeMap(430, 378, 1002); break; 
                        case VIPTeleportLocations.WP_POLTERGIEST: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.WP_TURTLEDOVE: pUser.ChangeMap(430, 378, 1002); break;
                            // maple forest
                        case VIPTeleportLocations.PHOENIX_CASTLE:
                        case VIPTeleportLocations.MF_BRIDGE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.MF_LAKE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.MF_MINE_CAVE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.MF_TO_APE_CITY: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.MF_VILLAGE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.MF_WATER_CAVE: pUser.ChangeMap(430, 378, 1002); break;
                            // love canyon
                        case VIPTeleportLocations.APE_CITY:
                        case VIPTeleportLocations.AC_EAST: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.AC_NORTH: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.AC_SOUTH: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.AC_SQUARE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.AC_WEST: pUser.ChangeMap(430, 378, 1002); break;
                            // desert city
                        case VIPTeleportLocations.DESERT_CITY:
                        case VIPTeleportLocations.DC_ANCIENT_MAZE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.DC_MOON_SPRING: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.DC_SOUTH: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.DC_SQUARE: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.DC_VILLAGE: pUser.ChangeMap(430, 378, 1002); break;
                            // bird island
                        case VIPTeleportLocations.BIRD_ILAND:
                        case VIPTeleportLocations.BI_CENTER: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.BI_NORTH_EAST: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.BI_NORTH_WEST: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.BI_SOUTH_WEST: pUser.ChangeMap(430, 378, 1002); break;
                        case VIPTeleportLocations.BI_SQUARE: pUser.ChangeMap(430, 378, 1002); break;
                    }
                    break;
                }
            }
        }
    }
}