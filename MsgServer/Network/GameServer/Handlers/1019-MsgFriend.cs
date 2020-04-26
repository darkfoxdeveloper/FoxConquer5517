// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1019 - MsgFriend.cs
// Last Edit: 2016/12/07 10:01
// Created: 2016/12/07 09:59

using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        private static readonly uint[] FRIEND_VIP_AMOUNT = { 50, 60, 70, 80, 90, 100, 120 };

        public static void HandleFriendPacket(Character pRole, MsgFriend pMsg)
        {
            // pRole -> Request
            // pRoleTarget -> Target Friend
            // pRole -> SetFriendRequest(pRoleTarget->Identity)
            // pRoleTarget -> FetchFriendRequest(pRole->Identity)
            switch (pMsg.Mode)
            {
                #region Request Friend

                case RelationAction.REQUEST_FRIEND:
                {
                    Client pTarget;
                    if (ServerKernel.Players.TryGetValue(pMsg.Identity, out pTarget))
                    {
                        if (pTarget.Character == null) return;
                        Character pRoleTarget = pTarget.Character;

                        uint dwVipUsr = FRIEND_VIP_AMOUNT[pRole.Owner.VipLevel],
                            dwVipTgt = FRIEND_VIP_AMOUNT[pRoleTarget.Owner.VipLevel];

                        if (pRole.Friends.Count >= dwVipUsr)
                        {
                            pRole.Send(string.Format("You can only have {0} friends.", dwVipUsr));
                            return;
                        }

                        if (pRoleTarget.Friends.Count >= dwVipTgt)
                        {
                            pRole.Send("The target has reached it´s max friend amount.");
                            return;
                        }

                        if (pRoleTarget.FetchFriendRequest(pRole.Identity))
                        {
                            pRole.CreateFriend(pRoleTarget);
                            pRoleTarget.ClearFriendRequest();
                        }
                        else
                        {
                            pRole.SetFriendRequest(pRoleTarget.Identity);
                            pMsg.Identity = pRole.Identity;
                            pMsg.Name = pRole.Name;
                            pRoleTarget.Send(pMsg);
                            pRole.Send(ServerString.STR_MAKE_FRIEND_SENT);
                        }
                    }
                    break;
                }

                    #endregion
                #region Break

                case RelationAction.REMOVE_FRIEND:
                {
                    pRole.DeleteFriend(pMsg.Identity);
                    break;
                }

                    #endregion
                #region Remove Enemy
                case RelationAction.REMOVE_ENEMY:
                {
                    pRole.DeleteEnemy(pMsg.Identity);
                    break;
                }
                #endregion
                default:
                    ServerKernel.Log.SaveLog("Not handled 1019:" + pMsg.Mode, true, LogType.WARNING);
                    GamePacketHandler.Report(pMsg);
                    break;
            }
        }
    }
}