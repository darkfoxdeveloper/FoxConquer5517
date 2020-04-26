// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Flower Ranking.cs
// Last Edit: 2016/12/06 20:39
// Created: 2016/12/06 20:39

using System;
using System.Linq;
using DB.Repositories;
using MsgServer.Network;
using ServerCore.Common.Enums;

namespace MsgServer.Structures.Events
{
    public class FlowerRanking
    {
        public FlowerRanking()
        {
            var plrList = new CharacterRepository().FetchAll();
            var rank = new FlowerRepository().FetchAll();
            foreach (var plr in rank)
            {
                var player = plrList.FirstOrDefault(x => x.Identity == plr.PlayerIdentity);
                if (player == null) continue;
                FlowerObject obj = new FlowerObject(plr)
                {
                    PlayerName = plr.PlayerName,
                    RedRoses = player.RedRoses,
                    WhiteRoses = player.WhiteRoses,
                    Orchids = player.Orchids,
                    Tulips = player.Tulips,
                };
                ServerKernel.FlowerRankingDict.TryAdd(player.Identity, obj);
            }
            ServerKernel.Log.SaveLog("Flower Ranking loaded...", true);
        }

        public bool AddFlowers(FlowerType flower, uint dwAmount, uint idTarget)
        {
            FlowerObject obj;
            if (!ServerKernel.FlowerRankingDict.TryGetValue(idTarget, out obj))
            {
                Client target;
                if (!ServerKernel.Players.TryGetValue(idTarget, out target) || target.Character == null)
                    return false;

                obj = new FlowerObject(idTarget, target.Character.Name);
                ServerKernel.FlowerRankingDict.TryAdd(idTarget, obj);
            }

            switch (flower)
            {
                case FlowerType.RED_ROSE:
                    {
                        obj.RedRoses += dwAmount;
                        obj.RedRosesToday += dwAmount;
                        break;
                    }
                case FlowerType.WHITE_ROSE:
                    {
                        obj.WhiteRoses += dwAmount;
                        obj.WhiteRosesToday += dwAmount;
                        break;
                    }
                case FlowerType.ORCHID:
                    {
                        obj.Orchids += dwAmount;
                        obj.OrchidsToday += dwAmount;
                        break;
                    }
                case FlowerType.TULIP:
                    {
                        obj.Tulips += dwAmount;
                        obj.TulipsToday += dwAmount;
                        break;
                    }
            }

            return new FlowerRepository().SaveOrUpdate(obj.Database);
        }

        public FlowerObject FetchUser(uint idUser)
        {
            return ServerKernel.FlowerRankingDict.Values.FirstOrDefault(x => x.PlayerIdentity == idUser);
        }

        public FlowerObject FetchUser(string szName)
        {
            return ServerKernel.FlowerRankingDict.Values.FirstOrDefault(x => x.PlayerName == szName);
        }

        public FlowerObject[] RedRosesRanking()
        {
            FlowerObject[] list = new FlowerObject[100];
            int i = 0;
            foreach (FlowerObject flowerObject in ServerKernel.FlowerRankingDict.Values.OrderByDescending(x => x.RedRoses))
            {
                list[i++] = flowerObject;
            }
            Array.Resize(ref list, i + 1);
            return list;
        }

        public FlowerObject[] WhiteRosesRanking()
        {
            FlowerObject[] list = new FlowerObject[100];
            int i = 0;
            foreach (FlowerObject flowerObject in ServerKernel.FlowerRankingDict.Values.OrderByDescending(x => x.WhiteRoses))
            {
                list[i++] = flowerObject;
            }
            Array.Resize(ref list, i + 1);
            return list;
        }

        public FlowerObject[] OrchidsRanking()
        {
            FlowerObject[] list = new FlowerObject[100];
            int i = 0;
            foreach (FlowerObject flowerObject in ServerKernel.FlowerRankingDict.Values.OrderByDescending(x => x.Orchids))
            {
                list[i++] = flowerObject;
            }
            Array.Resize(ref list, i + 1);
            return list;
        }

        public FlowerObject[] TulipsRanking()
        {
            FlowerObject[] list = new FlowerObject[100];
            int i = 0;
            foreach (FlowerObject flowerObject in ServerKernel.FlowerRankingDict.Values.OrderByDescending(x => x.Tulips))
            {
                list[i++] = flowerObject;
            }
            Array.Resize(ref list, i + 1);
            return list;
        }
    }
}