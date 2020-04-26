// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Captcha Box.cs
// Last Edit: 2016/12/28 17:50
// Created: 2016/12/28 17:40

using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures
{
    public sealed class CaptchaBox
    {
        private TimeOut m_pExpire = new TimeOut(60);

        public long Value1 { get; set; }
        public long Value2 { get; set; }
        public long Result { get; set; }
        public string Message { get; set; }

        public void OnOk(Character pUsrAccept)
        {
            if (Value1 + Value2 != Result)
                pUsrAccept.Disconnect("WRONG CAPTCHA ANSWER");
        }

        public void OnCancel(Character pUsrAccept)
        {
            if (Value1 + Value2 == Result)
                pUsrAccept.Disconnect("WRONG CAPTCHA ANSWER");
        }

        public void OnTimer(Character pOwner)
        {
            if (m_pExpire.IsActive() && m_pExpire.IsTimeOut(60))
                pOwner.Disconnect("PROBABLY AN AUTOCLICKER");
        }

        public void Generate()
        {
            Value1 = ThreadSafeRandom.RandGet(0, 10);
            Value2 = ThreadSafeRandom.RandGet(0, 10);
            if (Calculations.ChanceCalc(50f))
                Result = Value1 + Value2;
            else
            {
                long temp = Value1 + Value2;
                Result = (Value1 + Value2) * (ThreadSafeRandom.RandGet(7, 12)/100);
            }
            Message = "Hello! If you don't reply this captcha or give the wrong answer, you will be disconnected! " +
                      string.Format("So, answer me OK if this equation is correct or Cancel if not. {0}+{1}={2}", Value1,
                          Value2, Result);
        }

        public void Send(Character pTarget)
        {
            m_pExpire.Startup(60);
            pTarget.Send(new MsgTaskDialog(MsgTaskDialog.MESSAGE_BOX, Message));
        }
    }
}