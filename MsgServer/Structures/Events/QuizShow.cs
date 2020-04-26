// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - MsgServer - Quiz Show.cs
// Last Edit: 2016/12/06 20:49
// Created: 2016/12/06 20:35

using System;
using System.Collections.Generic;
using System.Linq;
using DB.Entities;
using DB.Repositories;
using MsgServer.Network;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using ServerCore.Common;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;

namespace MsgServer.Structures.Events
{
    public class QuizShowEvent : IOnTimer
    {
        private TimeOut m_pNextQuestion = new TimeOut(ServerKernel.QUIZ_TIME_PER_QUESTION);
        private TimeOutMS m_pEventCheck = new TimeOutMS(800);
        private Dictionary<uint, DbGameQuiz> m_quizShow = new Dictionary<uint, DbGameQuiz>();
        private List<DbGameQuiz> m_temporaryQuestions = new List<DbGameQuiz>(ServerKernel.QUIZ_MAX_QUESTION);
        private Dictionary<uint, QuizShowUserObject> m_quizUserInformation = new Dictionary<uint, QuizShowUserObject>();

        private int m_nActualQuestion = 0;

        private QuizState m_state = QuizState.STOPPED;

        public QuizShowEvent()
        {
            var list = new QuizShowRepository().FetchAll();
            foreach (var obj in list)
            {
                m_quizShow.Add(obj.Identity, obj);
            }
        }

        public void ReloadQuestions()
        {
            if (m_state > QuizState.STOPPED) return;

            m_quizShow.Clear();
            var list = new QuizShowRepository().FetchAll();
            foreach (var obj in list)
            {
                m_quizShow.Add(obj.Identity, obj);
            }
        }

        public bool InsertPlayer(Character pRole)
        {
            QuizShowUserObject plrObj;
            if (!m_quizUserInformation.ContainsKey(pRole.Identity))
            {
                plrObj = new QuizShowUserObject
                {
                    Experience = 0,
                    Name = pRole.Name,
                    Points = 0,
                    TimeTaken = 0,
                    UserIdentity = pRole.Identity,
                    LastQuestion = 0
                };
                m_quizUserInformation.Add(pRole.Identity, plrObj);
            }
            else
            {
                plrObj = m_quizUserInformation[pRole.Identity];
            }

            if (m_state == QuizState.STARTING)
            {
                // send initial packet
                var pMsg = new MsgQuiz
                {
                    Type = QuizShowType.START_QUIZ,
                    QuestionAmount = ServerKernel.QUIZ_MAX_QUESTION,
                    TimePerQuestion = ServerKernel.QUIZ_TIME_PER_QUESTION,
                    TimeTillStart = (ushort)(60 - DateTime.Now.Second),
                    FirstPrize = ServerKernel.QUIZ_SHOW_AWARD[1],
                    SecondPrize = ServerKernel.QUIZ_SHOW_AWARD[2],
                    ThirdPrize = ServerKernel.QUIZ_SHOW_AWARD[3]
                };
                pRole.Send(pMsg);
            }
            else if (m_state == QuizState.RUNNING)
            {
                var pMsg = new MsgQuiz
                {
                    Type = QuizShowType.START_QUIZ,
                    QuestionAmount = ServerKernel.QUIZ_MAX_QUESTION,
                    TimePerQuestion = ServerKernel.QUIZ_TIME_PER_QUESTION,
                    TimeTillStart = (ushort)m_pNextQuestion.GetRemain(),
                    FirstPrize = ServerKernel.QUIZ_SHOW_AWARD[1],
                    SecondPrize = ServerKernel.QUIZ_SHOW_AWARD[2],
                    ThirdPrize = ServerKernel.QUIZ_SHOW_AWARD[3]
                };
                pRole.Send(pMsg);
                pMsg = new MsgQuiz
                {
                    Type = QuizShowType.AFTER_REPLY,
                    CurrentScore = plrObj.Points,
                    TimeTaken = plrObj.TimeTaken,
                    Rank = 19
                };
                pRole.Send(pMsg);
            }

            return true;
        }

        /// <summary>
        /// On this method the user will award it's prize every question.
        /// </summary>
        public bool UserAnswer(Character pRole, ushort nQuestion, ushort nReply)
        {
            try
            {
                if (m_state <= QuizState.STOPPED || m_state >= QuizState.ENDED) return false;

                QuizShowUserObject plrObj = null;

                if (!m_quizUserInformation.ContainsKey(pRole.Identity))
                {
                    plrObj = new QuizShowUserObject
                    {
                        Experience = 0,
                        Name = pRole.Name,
                        Points = 0,
                        TimeTaken = 0,
                        UserIdentity = pRole.Identity,
                        LastQuestion = nQuestion
                    };
                    m_quizUserInformation.Add(pRole.Identity, plrObj);
                }

                plrObj = m_quizUserInformation[pRole.Identity];

                if (plrObj.LastQuestion == nQuestion)
                    return false; // player already answered

                int expBallAmount = 0;
                var pQuestion = m_temporaryQuestions[nQuestion - 1];
                if (pQuestion.Correct == nReply)
                {
                    expBallAmount = (ServerKernel.QUIZ_MAX_EXPERIENCE / ServerKernel.QUIZ_MAX_QUESTION);
                    pRole.AwardExperience(
                        (ServerKernel.GetExpBallExperience(pRole.Level) / 600)
                        // gets the exp and divides by 600
                        * expBallAmount);
                    // multiply by the correct answer tax
                    plrObj.Points += (ushort)(m_pNextQuestion.GetRemain());
                }
                else
                {
                    expBallAmount = (ServerKernel.QUIZ_MAX_EXPERIENCE / (ServerKernel.QUIZ_MAX_QUESTION * 4));
                    pRole.AwardExperience(
                        (ServerKernel.GetExpBallExperience(pRole.Level) / 600)
                        // gets the exp and divides by 600
                        * expBallAmount);
                    // multiply by the correct answer tax
                    plrObj.Points += 1;
                }

                plrObj.TimeTaken += (ushort)((m_pNextQuestion.GetRemain() - ServerKernel.QUIZ_TIME_PER_QUESTION) * -1);
                plrObj.Experience += (ushort)expBallAmount;
                plrObj.LastQuestion = nQuestion - 1;

                var pMsg = new MsgQuiz
                {
                    Type = QuizShowType.AFTER_REPLY,
                    CurrentScore = plrObj.Points,
                    TimeTaken = plrObj.TimeTaken,
                    Rank = plrObj.Rank
                };
                var rank = RankingStrings();
                pMsg.AddString(rank[0].Name, rank[0].Points, rank[0].TimeTaken);
                pMsg.AddString(rank[1].Name, rank[1].Points, rank[1].TimeTaken);
                pMsg.AddString(rank[2].Name, rank[2].Points, rank[2].TimeTaken);
                pRole.Send(pMsg);
                return true;
            }
            catch
            {
                // should not happen
                ServerKernel.Log.SaveLog("Could not add reward to user on Quiz Show", true, "quiz", LogType.ERROR);
            }
            return false;
        }

        public void Cancel(uint idUser)
        {
            if (m_quizUserInformation.ContainsKey(idUser))
                m_quizUserInformation[idUser].Canceled = true;
        }

        public bool IsRunning()
        {
            return m_state > QuizState.STOPPED && m_state < QuizState.ENDED;
        }

        public void OnTimer()
        {
            if (m_quizShow.Count < ServerKernel.QUIZ_MAX_QUESTION) return; // no questions, no quiz

            DateTime now = DateTime.Now;

            if (ServerKernel.QUIZ_SHOW_HOUR.Contains(now.Hour + 1))
            {
                if (now.Minute == 55
                    && now.Second == 0
                    && m_state < QuizState.STARTING)
                {
                    ServerKernel.SendMessageToAll("Quiz show will start in 5 minutes.", ChatTone.TOP_LEFT);
                }

                // Quiz starting
                if (now.Minute == 59
                    && now.Second <= 1
                    && m_state < QuizState.STARTING)
                {
                    if (now.DayOfWeek == DayOfWeek.Sunday
                        && now.Hour+1 == 22)
                        return;

                    ReloadQuestions();

                    // reset basic variable
                    m_nActualQuestion = 0;
                    m_quizUserInformation.Clear();
                    m_temporaryQuestions.Clear();

                    // start the quiz
                    m_state = QuizState.STARTING;
                    // and send the initial packet :)
                    var pMsg = new MsgQuiz
                    {
                        Type = QuizShowType.START_QUIZ,
                        TimeTillStart = (ushort)(60 - now.Second),
                        TimePerQuestion = ServerKernel.QUIZ_TIME_PER_QUESTION,
                        QuestionAmount = ServerKernel.QUIZ_MAX_QUESTION,
                        FirstPrize = ServerKernel.QUIZ_SHOW_AWARD[1],
                        SecondPrize = ServerKernel.QUIZ_SHOW_AWARD[2],
                        ThirdPrize = ServerKernel.QUIZ_SHOW_AWARD[3]
                    };
                    // send to all players
                    foreach (var plr in ServerKernel.Players.Values)
                    {
                        // create the user object that will be held by the server while it's alive
                        var plrObj = new QuizShowUserObject
                        {
                            Experience = 0,
                            Name = plr.Character.Name,
                            Points = 0,
                            TimeTaken = 0,
                            UserIdentity = plr.Identity,
                            Canceled = false
                        };
                        m_quizUserInformation.Add(plr.Identity, plrObj); // save the info
                        plr.Send(pMsg); // send packet to client
                    }

                    // quiz will only happen if there is at least 20 questions
                    if (m_quizShow.Count > ServerKernel.QUIZ_MAX_QUESTION)
                    {
                        List<KeyValuePair<uint, DbGameQuiz>> tempList = new List<KeyValuePair<uint, DbGameQuiz>>();
                        Random rand = new Random();

                        foreach (var question in m_quizShow.Values)
                        {
                            tempList.Add(new KeyValuePair<uint, DbGameQuiz>((uint)rand.Next(), question));
                        }

                        int num = 0;
                        foreach (var question in tempList.OrderBy(x => x.Key).Where(question => num++ < ServerKernel.QUIZ_MAX_QUESTION))
                            m_temporaryQuestions.Add(question.Value);
                    }
                    else
                    {
                        if (m_quizShow.Count < ServerKernel.QUIZ_MAX_QUESTION)
                        {
                            m_state = QuizState.STOPPED;
                            return;
                        }
                        // we have exactly 20 questions :) so ok
                        foreach (var question in m_quizShow.Values)
                            m_temporaryQuestions.Add(question);
                    }
                    // send message to all (supposing they didn't receive the window lol)
                    ServerKernel.SendMessageToAll(ServerString.STR_QUIZ_SHOW_START, ChatTone.TOP_LEFT);
                }
            }
            if (ServerKernel.QUIZ_SHOW_HOUR.Contains(now.Hour) &&
                now.Minute <= (ServerKernel.QUIZ_MAX_QUESTION * ServerKernel.QUIZ_TIME_PER_QUESTION) / 60)
            {
                // quiz started
                if (m_state == QuizState.STARTING
                    && now.Minute == 0)
                {
                    m_state = QuizState.RUNNING;
                    m_pNextQuestion.Startup(ServerKernel.QUIZ_TIME_PER_QUESTION);
                    m_pEventCheck.Startup(800);

                    DbGameQuiz question = m_temporaryQuestions[m_nActualQuestion++];
                    var pMsg = new MsgQuiz
                    {
                        Type = QuizShowType.QUESTION_QUIZ,
                        QuestionNumber = (ushort)(m_nActualQuestion),
                        LastCorrectAnswer = 0,
                        ExperienceAwarded = 1,
                        TimeTakenTillNow = 0,
                        CurrentScore = 0
                    };
                    pMsg.AddString(question.Question, question.Answer0, question.Answer1, question.Answer2,
                        question.Answer3);
                    foreach (var plr in ServerKernel.Players.Values)
                        plr.Send(pMsg);
                }

                // quiz running
                if (m_state == QuizState.RUNNING
                    && m_pNextQuestion.ToNextTime()
                    && m_nActualQuestion < ServerKernel.QUIZ_MAX_QUESTION)
                {
                    foreach (var usr in m_quizUserInformation.Values)
                    {
                        if (usr.LastQuestion < m_nActualQuestion)
                        {
                            usr.Points += 1;
                            usr.TimeTaken += ServerKernel.QUIZ_TIME_PER_QUESTION;
                        }
                    }

                    UpdateRanking();

                    DbGameQuiz question = m_temporaryQuestions[m_nActualQuestion++];
                    var pMsg = new MsgQuiz
                    {
                        Type = QuizShowType.QUESTION_QUIZ,
                        QuestionNumber = (ushort)m_nActualQuestion,
                        LastCorrectAnswer = m_temporaryQuestions[m_nActualQuestion - 2].Correct
                    };
                    pMsg.AddString(question.Question, question.Answer0, question.Answer1, question.Answer2,
                        question.Answer3);
                    foreach (var plr in ServerKernel.Players.Values.Where(x => !x.Character.QuizCanceled))
                    {
                        var plrObj = m_quizUserInformation.Values.FirstOrDefault(x => x.UserIdentity == plr.Identity);
                        if (plrObj == null)
                        {
                            plrObj = new QuizShowUserObject
                            {
                                Experience = 0,
                                Name = plr.Character.Name,
                                Points = 0,
                                TimeTaken = 0,
                                UserIdentity = plr.Identity,
                                LastQuestion = 0
                            };
                        }

                        if (plrObj.LastQuestion < m_nActualQuestion - 2) pMsg.LastCorrectAnswer = 0;
                        pMsg.CurrentScore = plrObj.Points;
                        pMsg.ExperienceAwarded = plrObj.Experience;
                        pMsg.TimeTakenTillNow = plrObj.TimeTaken;
                        plrObj.LastQuestion = m_nActualQuestion - 1;
                        plr.Send(pMsg);
                    }
                    if (m_nActualQuestion >= ServerKernel.QUIZ_MAX_QUESTION)
                    {
                        m_state = QuizState.ENDED;
                    }
                }

                if (m_state == QuizState.ENDED
                    && m_pNextQuestion.ToNextTime())
                {
                    foreach (var usr in m_quizUserInformation.Values)
                    {
                        if (usr.LastQuestion < m_nActualQuestion)
                        {
                            usr.Points += 1;
                            usr.TimeTaken += ServerKernel.QUIZ_TIME_PER_QUESTION;

                            Client pClient;
                            if (ServerKernel.Players.TryGetValue(usr.UserIdentity, out pClient))
                            {
                                var pMsg = new MsgQuiz
                                {
                                    Type = QuizShowType.AFTER_REPLY,
                                    CurrentScore = usr.Points,
                                    TimeTaken = usr.TimeTaken,
                                    Rank = usr.Rank
                                };
                                var rank = RankingStrings();
                                pMsg.AddString(rank[0].Name, rank[0].Points, rank[0].TimeTaken);
                                pMsg.AddString(rank[1].Name, rank[1].Points, rank[1].TimeTaken);
                                pMsg.AddString(rank[2].Name, rank[2].Points, rank[2].TimeTaken);
                                pClient.Send(pMsg);
                            }
                        }

                        Client pUser = null;
                        if (ServerKernel.Players.TryGetValue(usr.UserIdentity, out pUser))
                        {
                            try
                            {
                                pUser.Character.QuizPoints += usr.Points;
                                int i = 0;
                                foreach (var tmp in m_quizUserInformation.Values.OrderByDescending(x => x.Points))
                                {
                                    if (i++ > 3) break;
                                    if (tmp.UserIdentity == usr.UserIdentity)
                                    {
                                        long amount =
                                            (ServerKernel.GetExpBallExperience(pUser.Character.Level) / 600) *
                                            ServerKernel.QUIZ_SHOW_AWARD[i];
                                        pUser.Character.AwardExperience(amount);

                                        ushort emoney = ServerKernel.QUIZ_SHOW_EMONEY[i];
                                        uint money = ServerKernel.QUIZ_SHOW_MONEY[i];
                                        pUser.Character.AwardEmoney(emoney);
                                        pUser.Character.AwardMoney(money);
                                        pUser.Character.Send(
                                            string.Format("You awarded {0} CPs and {2} for winning on {1} place on Quiz Show.",
                                                emoney, i, money));
                                    }
                                }

                                MsgQuiz pMsg = new MsgQuiz
                                {
                                    Type = QuizShowType.FINISH_QUIZ,
                                    Score = usr.Rank,
                                    Rank = usr.TimeTaken,
                                    FirstPrize = usr.Points,
                                    FinalPrize = usr.Experience
                                };
                                QuizShowUserObject[] pList = RankingStrings();
                                pMsg.AddString(pList[0].Name, pList[0].Points, pList[0].TimeTaken);
                                pMsg.AddString(pList[1].Name, pList[1].Points, pList[1].TimeTaken);
                                pMsg.AddString(pList[2].Name, pList[2].Points, pList[2].TimeTaken);
                                pUser.Send(pMsg);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            try
                            {
                                // disconnected? still have prize to claim
                                DbUser dbObj = new CharacterRepository().SearchByIdentity(usr.UserIdentity);
                                if (dbObj == null) continue;

                                Character pTemp = new Character(null, dbObj, null);
                                pTemp.QuizPoints += usr.Points;

                                int i = 1;
                                foreach (var tmp in m_quizUserInformation.Values.OrderByDescending(x => x.Points))
                                {
                                    if (i++ > 3) break;
                                    if (tmp.UserIdentity == usr.UserIdentity)
                                    {
                                        long amount =
                                            (ServerKernel.GetExpBallExperience(pTemp.Level) / 600) *
                                            ServerKernel.QUIZ_SHOW_AWARD[i - 1];
                                        pTemp.AwardExperience(amount);
                                    }
                                }
                                pTemp.Save();
                                pTemp = null;
                            }
                            catch
                            {

                            }
                        }
                    }

                    ServerKernel.SendMessageToAll(ServerString.STR_QUIZ_SHOW_ENDED, ChatTone.TOP_LEFT);
                    m_state = QuizState.STOPPED;
                }
            }
        }

        public void UpdateRanking()
        {
            ushort rank = 1;
            foreach (var obj in m_quizUserInformation.Values.OrderByDescending(x => x.Points))
                obj.Rank = rank++;
        }

        private QuizShowUserObject[] RankingStrings()
        {
            UpdateRanking();
            QuizShowUserObject[] ret = new QuizShowUserObject[3];
            for (int i = 0; i < 3; i++)
                ret[i] = new QuizShowUserObject();
            int nCount = 0;
            foreach (var usr in m_quizUserInformation.Values.OrderByDescending(x => x.Points))
            {
                if (nCount >= 3) break;
                ret[nCount++] = usr;
            }
            return ret;
        }

        enum QuizState : byte
        {
            STOPPED = 0,
            STARTING = 1,
            RUNNING = 2,
            ENDED = 3
        }
    }
}