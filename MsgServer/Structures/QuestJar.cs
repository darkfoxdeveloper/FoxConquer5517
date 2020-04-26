using DB.Entities;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Items;
using ServerCore.Common;
using System.Collections.Generic;
using System.Linq;

namespace MsgServer.Structures
{
    public class QuestJar
    {
        public DbMonstertype Monster { get; set; }
        public Character Player { get; set; }
        public uint Kills { get; set; }
        public bool Finished { get; set; }

        public QuestJar(Character player, DbMonstertype monster)
        {
            Player = player;
            Monster = monster;
            Kills = 0;
        }

        public void AddKills(uint n = 1)
        {
            Item cloudSaintsJar = Player.Inventory.GetByType(SpecialItem.CLOUDSAINTS_JAIR);
            if (cloudSaintsJar != null)
            {
                if (cloudSaintsJar.MaximumDurability == Monster.Id)
                {
                    Kills += n;
                }
            }
        }

        public uint RequiredKills
        {
            get
            {
                switch (Monster.Id)
                {
                    case 1:// Pheasant
                        {
                            return 30;
                        }
                    case 2:// Turtledove
                        {
                            return 100;
                        }
                    case 3:// Robin
                        {
                            return 100;
                        }
                    case 4:// Apparition
                        {
                            return 100;
                        }
                }
                return 300;
            } set
            {
                RequiredKills = value;
            }
        }

        public bool IsFinished(bool rewardIfFinished = false)
        {
            if (Kills >= RequiredKills)
            {
                Finished = true;
            }
            if (rewardIfFinished && Finished) Finish();
            return Finished;
        }

        public void Finish()
        {
            Item cloudSaintsJar = Player.Inventory.GetByType(SpecialItem.CLOUDSAINTS_JAIR);
            Player.Inventory.Remove(cloudSaintsJar.Identity);
            Finished = true;
            // Reward code here :)
        }
    }

    public static class QuestJarManager {
        public static QuestJar CurrentQuest(Character player)
        {
            QuestJar quest = ServerKernel.PlayerQuests.Where(x => x.Player.Identity == player.Identity && !x.Finished).FirstOrDefault();
            return quest;
        }

        public static QuestJar Quests(Character player)
        {
            QuestJar quest = ServerKernel.PlayerQuests.Where(x => x.Player.Identity == player.Identity).FirstOrDefault();
            return quest;
        }

        public static List<QuestJar> QuestsFinished(Character player)
        {
            List<QuestJar> quests = ServerKernel.PlayerQuests.Where(x => x.Player.Identity == player.Identity && x.Finished).ToList();
            return quests;
        }

        public static QuestJar NewQuest(Character player, uint QuestID)
        {
            DbMonstertype mType = ServerKernel.Monsters.Where(x => x.Value.Id == QuestID).FirstOrDefault().Value;
            QuestJar q = new QuestJar(player, mType);
            ServerKernel.PlayerQuests.Add(q);
            return q;
        }
    }
}
