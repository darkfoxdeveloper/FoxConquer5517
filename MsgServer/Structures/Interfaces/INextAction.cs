// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - INextAction.cs
// Last Edit: 2016/12/06 14:23
// Created: 2016/12/06 14:23

namespace MsgServer.Structures.Interfaces
{
    public struct INextAction
    {
        public byte Task;
        /// <summary>
        /// The task id that will be executed.
        /// </summary>
        public uint Identity;
        public bool IsInput;
    }
}