// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Quiz Show User Object.cs
// Last Edit: 2016/12/06 20:35
// Created: 2016/12/06 20:35

namespace MsgServer.Structures
{
    public class QuizShowUserObject
    {
        public uint UserIdentity { get; set; }
        public string Name { get; set; }
        public ushort Points { get; set; }
        public ushort Experience { get; set; } // 600 = 1 expball
        public ushort TimeTaken { get; set; } // in seconds
        public int LastQuestion { get; set; }
        public ushort Rank { get; set; }
        public bool Canceled { get; set; }

        public QuizShowUserObject()
        {
            UserIdentity = 0;
            Name = "None";
            Points = 0;
            Experience = 0;
            TimeTaken = 0;
            LastQuestion = 0;
            Rank = 30000;
        }
    }
}