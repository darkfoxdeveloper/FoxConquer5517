// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - 2068 - MsgQuiz.cs
// Last Edit: 2016/12/07 10:26
// Created: 2016/12/07 10:26

using MsgServer.Structures.Entities;
using ServerCore.Common;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleQuizShow(Character pRole, MsgQuiz pMsg)
        {
            switch (pMsg.Type)
            {
                case QuizShowType.QUIZ_REPLY:
                    {
                        if (pRole.QuizCanceled)
                            return;
                        ServerKernel.QuizShow.UserAnswer(pRole, pMsg.QuestionNumber, pMsg.LastCorrectAnswer);
                        break;
                    }
                case QuizShowType.QUIT_QUIZ:
                    {
                        pRole.CancelQuiz();
                        break;
                    }
                default:
                    ServerKernel.Log.SaveLog("Not expected type on packet[2068]:" + pMsg.Type, true, LogType.WARNING);
                    break;
            }
        }
    }
}