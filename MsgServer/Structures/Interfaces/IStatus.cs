// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - IStatus.cs
// Last Edit: 2016/11/23 10:31
// Created: 2016/11/23 10:31

namespace MsgServer.Structures.Interfaces
{
    public interface IStatus
    {
        /// <summary>
        /// This method will get the status id.
        /// </summary>
        int Identity { get; }
        /// <summary>
        /// This method will check if the status still valid and running.
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// This method will return the power of the status. This wont make percentage checks. The value is a short.
        /// </summary>
        int Power { get; set; }

        int Time { get; }

        byte Level { get; }
        /// <summary>
        /// This method will get the status information into another param.
        /// </summary>
        /// <param name="pInfo">The structure that will be filled with the information.</param>
        bool GetInfo(ref StatusInfoStruct pInfo);
        /// <summary>
        /// This method will override the old values from the status.
        /// </summary>
        /// <param name="nPower">The new power of the status.</param>
        /// <param name="nSecs">The remaining time to the status.</param>
        /// <param name="nTimes">How many times the status will appear. If StatusMore.</param>
        /// <param name="wCaster">The identity of the caster.</param>
        bool ChangeData(int nPower, int nSecs, int nTimes = 0, uint wCaster = 0);
        bool IncTime(int nMilliSecs, int nLimit);
        bool ToFlash();
        uint CasterId { get; }
        bool IsUserCast { get; }
        void OnTimer();
    }
}