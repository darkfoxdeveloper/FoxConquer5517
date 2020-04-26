// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 1001 - MsgRegister.cs
// Last Edit: 2016/11/24 10:26
// Created: 2016/11/23 12:39

using System;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        private static string[] _invalidNameChar =
        {
            "{", "}", "[", "]", "(", ")", "\"", "[gm]", "[pm]", "'", "´", "`", "admin", "helpdesk", " ",
            "bitch", "puta", "whore", "ass", "fuck", "cunt", "fdp"
        };
        private const uint _START_MAP = 1002;
        private static readonly ushort[] m_startX = { 430, 423, 439, 428, 452, 464, 439 };
        private static readonly ushort[] m_startY = { 378, 394, 384, 365, 365, 378, 396 };

        public static void HandleRegister(Client pClient, MsgRegister pMsg)
        {
            Client trash;
            if (pMsg.CancelRequest)
            {
                try
                {
                    pClient.Disconnect();
                }
                catch
                {
                    ServerKernel.Players.TryRemove(pClient.Identity, out trash);
                    ServerKernel.CharacterCreation.TryRemove(pClient.Identity, out trash);
                    ServerKernel.CharacterCreation.TryRemove(pClient.AccountIdentity, out trash);
                }
                return;
            }

            if (ServerKernel.CharacterCreation.TryGetValue(pClient.AccountIdentity, out trash))
            {
                trash = null;

                if (CheckName(pMsg.Name))
                {
                    var pRepository = new CharacterRepository();
                    if (pRepository.AccountHasCharacter(pClient.AccountIdentity))
                    {
                        //DisconnectWithMsg(pClient, ServerMessages.CharacterCreation.AccountHasCharacter);
                        pClient.Send(ServerMessages.CharacterCreation.AccountHasCharacter);
                        return;
                    }
                    if (pRepository.CharacterExists(pMsg.Name))
                    {
                        //DisconnectWithMsg(pClient, ServerMessages.CharacterCreation.NameTaken);
                        pClient.Send(ServerMessages.CharacterCreation.NameTaken);
                        return;
                    }

                    ProfessionType profession = ((ProfessionType)pMsg.Profession > ProfessionType.INTERN_TAOIST
                        ? ProfessionType.INTERN_TAOIST
                        : (ProfessionType)(pMsg.Profession / 10 * 10));
                    if (!Enum.IsDefined(typeof(BodyType), pMsg.Body) ||
                        !Enum.IsDefined(typeof(ProfessionType), profession))
                    {
                        // The client is a proxy exploiting the server. Disconnect the client.
                        DisconnectWithMsg(pClient, ServerMessages.CharacterCreation.AccessDenied);
                        return;
                    }

                    switch (profession)
                    {
                        case ProfessionType.INTERN_ARCHER:
                        case ProfessionType.INTERN_NINJA:
                        case ProfessionType.INTERN_TAOIST:
                        case ProfessionType.INTERN_TROJAN:
                        case ProfessionType.INTERN_WARRIOR:
                        case ProfessionType.INTERN_MONK:
                            break;
                        default:
                            {
                                DisconnectWithMsg(pClient, ServerMessages.CharacterCreation.AccessDenied);
                                return;
                            }
                    }

                    ushort hair = 410;
                    uint lookface = 0;
                    if (pMsg.Body == (ushort)BodyType.THIN_MALE || pMsg.Body == (ushort)BodyType.HEAVY_MALE)
                    {
                        if ((pMsg.Profession / 10) == 5)
                        {
                            lookface = (uint)(new Random().Next(103, 107));
                        }
                        else if ((pMsg.Profession / 10) == 6)
                        {
                            lookface = (uint)(new Random().Next(109, 113));
                        }
                        else
                        {
                            lookface = (uint)(new Random().Next(1, 102));
                        }
                    }
                    else
                    {
                        hair = 410;
                        if ((pMsg.Profession / 10) == 5)
                        {
                            lookface = (uint)(new Random().Next(291, 295));
                        }
                        else if ((pMsg.Profession / 10) == 6)
                        {
                            lookface = (uint)(new Random().Next(300, 304));
                        }
                        else
                        {
                            lookface = (uint)(new Random().Next(201, 290));
                        }
                    }

                    #region Initial HairStyle and Lookface for monks
                    switch (profession)
                    {
                        case ProfessionType.INTERN_MONK:
                            if (pMsg.Body == (ushort)BodyType.THIN_MALE)
                            {
                                lookface = (uint)(new Random().Next(109, 113));
                                hair = 400;
                            } else if (pMsg.Body == (ushort)BodyType.HEAVY_MALE)
                            {
                                lookface = (uint)(new Random().Next(129, 133));
                                hair = 400;
                            } else if (pMsg.Body == (ushort)BodyType.THIN_FEMALE)
                            {
                                lookface = (uint)(new Random().Next(300, 304));
                            }
                            else if (pMsg.Body == (ushort)BodyType.HEAVY_FEMALE)
                            {
                                lookface = (uint)(new Random().Next(325, 329));
                            }
                            break;
                    }
                    #endregion

                    DbPointAllot points =
                        ServerKernel.PointAllot.Values.FirstOrDefault(
                            x => x.Profession == ((pMsg.Profession - (pMsg.Profession % 10)) / 10) && x.Level == 1);
                    if (points == null)
                    {
                        pClient.Send(new MsgTalk("Could not fetch class attribute points.", ChatTone.CHARACTER_CREATION));
                        return;
                    }

                    int idx = new Random().Next(m_startX.Length - 1);
                    ushort startX = m_startX[idx];
                    ushort startY = m_startY[idx];

                    switch (profession)
                    {
                        case ProfessionType.INTERN_TROJAN:
                        case ProfessionType.INTERN_WARRIOR:
                        case ProfessionType.INTERN_ARCHER:
                        case ProfessionType.INTERN_NINJA:
                        case ProfessionType.INTERN_TAOIST:
                        case ProfessionType.INTERN_MONK:
                            {
                                break;
                            }
                        default:
                            DisconnectWithMsg(pClient, ServerMessages.CharacterCreation.AccessDenied);
                            return;
                    }

                    uint money = 10000, emoney = 270;
                    if (pClient.VipLevel == 6)
                    {
                        money *= 10;
                        emoney *= 5;
                    }

                    ushort startLife = (ushort) (((points.Agility + points.Strength + points.Spirit)*3) + points.Vitality*24);

                    var newUser = new DbUser
                    {
                        AccountId = pClient.AccountIdentity,
                        Name = pMsg.Name,
                        Lookface = pMsg.Body + (lookface * 10000),
                        Profession = (byte)profession,
                        Mate = "None",
                        AdditionalPoints = 0,
                        Agility = points.Agility,
                        Strength = points.Strength,
                        Vitality = points.Vitality,
                        Spirit = points.Spirit,
                        AutoAllot = 1,
                        AutoExercise = 0,
                        BoundEmoney = 4300,
                        Business = 255,
                        CoinMoney = 0,
                        CurrentLayout = 0,
                        Donation = 0,
                        Emoney = emoney,
                        Experience = 0,
                        Level = 1,
                        FirstProfession = 0,
                        Metempsychosis = 0,
                        Flower = 0,
                        HomeId = 0,
                        LastLogin = 0,
                        LastLogout = 0,
                        LastProfession = 0,
                        Life = startLife,
                        LockKey = 0,
                        Hair = hair,
                        Mana = 0,
                        MapId = _START_MAP,
                        MapX = startX,
                        MapY = startY,
                        MeteLevel = 0,
                        Money = money,
                        MoneySaved = 0,
                        Orchids = 0,
                        PkPoints = 0,
                        RedRoses = 0,
                        StudentPoints = 0,
                        Tulips = 0,
                        Virtue = 0,
                        WhiteRoses = 0,
                        EnlightPoints = 0,
                        HeavenBlessing = (uint)(UnixTimestamp.Timestamp() + 60 * 60 * 24 * 30),
                        ExperienceExpires = (uint)(UnixTimestamp.Timestamp() + 60 * 60 * 24),
                        ExperienceMultipler = 10
                    };

                    if (pRepository.CreateNewCharacter(newUser))
                    {
                        uint idUser = newUser.Identity;

                        try
                        {
                            GenerateInitialStatus(idUser, profession);
                        }
                        catch
                        {
                            ServerKernel.Log.SaveLog("Could not create initial status for character " + idUser, true, LogType.ERROR);
                        }
                        ServerKernel.Log.SaveLog(string.Format("User [({0}){1}] has created character {2}.",
                            pClient.AccountIdentity, idUser, newUser.Name), true);

                        pClient.Send(ServerMessages.CharacterCreation.AnswerOk);
                        return;
                    }
                }
                else
                {
                    //DisconnectWithMsg(pClient, ServerMessages.CharacterCreation.InvalidName);
                    pClient.Send(ServerMessages.CharacterCreation.InvalidName);
                    return;
                }
            }
            else
            {
                DisconnectWithMsg(pClient, ServerMessages.CharacterCreation.AccessDenied);
                return;
            }
        }

        public static bool CheckName(string szName)
        {
            for (int i = 0; i < szName.Length; i++)
            {
                char c = szName[i];
                if (c < ' ')
                    return false;
                switch (c)
                {
                    case ' ':
                    case ';':
                    case ',':
                    case '/':
                    case '\\':
                    case '=':
                    case '%':
                    case '@':
                    case '\'':
                    case '"':
                    case '[':
                    case ']':
                    case '?':
                    case '{':
                    case '}':
                        return false;
                }
            }

            string lower = szName.ToLower();
            foreach (string part in _invalidNameChar)
                if (lower.Contains(part)) return false;

            return true;
        }

        private static void GenerateInitialStatus(uint idRole, ProfessionType profession)
        {
            Item item = null;

            uint[] dwMonkItems = { 143006, 120006, 136006, 300000, 610026, 610026, 151016, 160016, 201006, 202006, 203006 };
            uint[] dwTrojanItems = { 118006, 120006, 130006, 300000, 410026, 480026, 150016, 160016, 201006, 202006, 203006 };
            uint[] dwWarriorItems = { 111006, 120006, 131006, 300000, 561026, 150016, 160016, 201006, 202006, 203006 };
            uint[] dwTaoistItems = { 114006, 121006, 134006, 300000, 421026, 152016, 160016, 201006, 202006, 203006 };
            uint[] dwArcherItems = { 113006, 120006, 133006, 300000, 500016, 150016, 160016, 201006, 202006, 203006 };
            uint[] dwNinjaItems = { 112006, 120006, 135006, 300000, 601026, 601026, 150016, 160016, 201006, 202006, 203006 };

            #region Class Items

            for (int i = 0; i < 9; i++)
            {
                item = new Item
                {
                    StackAmount = 1
                };
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:
                        {
                            item.Type = 1000000;
                            item.PlayerIdentity = idRole;
                            break;
                        }
                    case 3:
                    case 4:
                    case 5:
                        {
                            item.Type = 1001000;
                            item.PlayerIdentity = idRole;
                            break;
                        }
                    case 6: // 723753
                        {
                            item.Type = 723753;
                            item.PlayerIdentity = idRole;
                            break;
                        }
                    case 7:
                        {
                            switch (profession)
                            {
                                case ProfessionType.INTERN_MONK:
                                    item.Type = 610301;
                                    break;
                                case ProfessionType.INTERN_NINJA:
                                    item.Type = 601301;
                                    break;
                                case ProfessionType.INTERN_TAOIST:
                                    item.Type = 421301;
                                    break;
                                case ProfessionType.INTERN_ARCHER:
                                    item.Type = 501301;
                                    break;
                                default:
                                    item.Type = 410301;
                                    break;
                            }
                            item.Position = ItemPosition.RIGHT_HAND;
                            item.PlayerIdentity = idRole;
                            break;
                        }
                    case 8:
                        {
                            item.Type = 132005;
                            item.Position = ItemPosition.ARMOR;
                            item.PlayerIdentity = idRole;
                            break;
                        }
                }
                item.Save();
            }

            #endregion

            #region Extra items

            uint[] list = { };

            switch (profession)
            {
                case ProfessionType.INTERN_MONK:
                    list = dwMonkItems;
                    break;
                case ProfessionType.INTERN_TROJAN:
                    list = dwTrojanItems;
                    break;
                case ProfessionType.INTERN_WARRIOR:
                    list = dwWarriorItems;
                    break;
                case ProfessionType.INTERN_ARCHER:
                    list = dwArcherItems;
                    break;
                case ProfessionType.INTERN_TAOIST:
                    list = dwTaoistItems;
                    break;
                case ProfessionType.INTERN_NINJA:
                    list = dwNinjaItems;
                    break;
            }

            foreach (var idType in list)
            {
                item = new Item
                {
                    Type = idType,
                    PlayerIdentity = idRole,
                    Bound = true,
                    Plus = 0,
                    Position = 0,
                    StackAmount = 1
                };
                //if (!item.IsMount() && (item.GetItemSubtype() < 200 || item.GetItemSubtype() > 203))
                //{
                //    item.ReduceDamage = 3;
                //    item.Enchantment = 100;
                //}
                //if (item.IsEquipment() 
                //    && (item.GetItemSubtype() < 200 || item.GetItemSubtype() > 203) 
                //    && item.Type != 300000)
                //{
                //    if (profession == ProfessionType.TAOIST)
                //    {
                //        item.SocketOne = SocketGem.REFINED_PHOENIX_GEM;
                //    }
                //    else
                //    {
                //        item.SocketOne = SocketGem.REFINED_DRAGON_GEM;
                //    }
                //}
                //else if (item.GetItemSubtype() == 201)
                //{
                //    item.SocketOne = SocketGem.REFINED_THUNDER_GEM;
                //}
                //else if (item.GetItemSubtype() == 202)
                //{
                //    item.SocketOne = SocketGem.REFINED_GLORY_GEM;
                //}
                item.Save();
            }

            #endregion

            #region Starting Skills

            switch (profession)
            {
                case ProfessionType.INTERN_MONK:
                {
                        DbMagic mgc = new DbMagic
                        {
                            OwnerId = idRole,
                            Type = 10490
                        };
                        Database.Magics.SaveOrUpdate(mgc);
                    break;
                }
            }

            #endregion
        }
    }
}