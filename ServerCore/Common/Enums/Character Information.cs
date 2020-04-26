// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Character Information.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50
namespace ServerCore.Common.Enums
{
    /// <summary>
    /// This enumeration type defines the possible body types for a character in Conquer Online, defined by the
    /// client's character creation window. Two genders, both with a thin and heavy body build.
    /// </summary>
    public enum BodyType : ushort
    {
        THIN_MALE = 1003,
        HEAVY_MALE = 1004,
        THIN_FEMALE = 2001,
        HEAVY_FEMALE = 2002
    }

    /// <summary>
    /// This enumeration type defines the possible professions for a character in Conquer Online, defined by the
    /// client's "ProfessionalName.ini" file.
    /// </summary>
    public enum ProfessionType : ushort
    {
        // Trojan Professions:
        INTERN_TROJAN = 10,
        TROJAN = 11,
        VETERAN_TROJAN = 12,
        TIGER_TROJAN = 13,
        DRAGON_TROJAN = 14,
        TROJAN_MASTER = 15,

        // Warrior Professions:
        INTERN_WARRIOR = 20,
        WARRIOR = 21,
        BRASS_WARRIOR = 22,
        SILVER_WARRIOR = 23,
        GOLD_WARRIOR = 24,
        WARRIOR_MASTER = 25,

        // Archer Professions:
        INTERN_ARCHER = 40,
        ARCHER = 41,
        EAGLE_ARCHER = 42,
        TIGER_ARCHER = 43,
        DRAGON_ARCHER = 44,
        ARCHER_MASTER = 45,

        // Ninja Profession:
        INTERN_NINJA = 50,
        NINJA = 51,
        MIDDLE_NINJA = 52,
        DARK_NINJA = 53,
        MYSTIC_NINJA = 54,
        NINJA_MASTER = 55,

        //Monk Profession:
        INTERN_MONK = 60,
        MONK = 61,
        DHYANA_MONK = 62,
        DHARMA_MONK = 63,
        PRAJNA_MONK = 64,
        NIRVANA_MONK = 65,

        // Taoist Professions:
        INTERN_TAOIST = 100,
        TAOIST = 101,
        WATER_TAOIST = 132,
        WATER_WIZARD = 133,
        WATER_MASTER = 134,
        WATER_SAINT = 135,
        FIRE_TAOIST = 142,
        FIRE_WIZARD = 143,
        FIRE_MASTER = 144,
        FIRE_SAINT = 145
    }
}