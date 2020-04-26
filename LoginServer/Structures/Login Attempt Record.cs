// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: Felipe Vieira
// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - LoginServer - Login Attempt Record.cs
// Last Edit: 2016/11/23 10:05
// Created: 2016/11/23 10:04

using ServerCore.Common;

namespace LoginServer.Structures
{
    public sealed class LoginAttemptRecord
    {
        private TimeOut m_pTimeOut = new TimeOut(10);
        private TimeOut m_pUnlock = new TimeOut(60);

        private string m_szAddress;
        private int m_bTries;

        public LoginAttemptRecord(string ipAddress)
        {
            m_szAddress = ipAddress;
            m_pTimeOut.Update();
        }

        /// <summary>
        /// The IP Address that tried to login on the server.
        /// </summary>
        public string IpAddress { get { return m_szAddress; } }

        /// <summary>
        /// User may login 5 times every 10 seconds, no mather if it has successfully logged in or not.
        /// If it exceeds the 5 tries, user will be locked for 1 minute.
        /// </summary>
        public bool Enabled
        {
            get
            {
                if (!m_pTimeOut.ToNextTime())
                    m_bTries++;

                if (m_bTries > 5)
                {
                    if (!m_pUnlock.IsTimeOut())
                        m_pUnlock.Update();
                    else
                    {
                        m_bTries = 0;
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }
    }
}