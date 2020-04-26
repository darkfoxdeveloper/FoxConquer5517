// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Log Writer.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System;
using System.IO;

namespace ServerCore.Common
{
    public class LogWriter
    {
        public const string STR_CONSOLE_MSG = "{0} - {1}";

        public const string STR_GMLOG_FORMAT = "{0} - {1}"; // {0} is message {1} is date
        public const string STR_GMLOG_FOLDER = @"gmlog\";
        public const string STR_GMLOG_SUBFOLDER = "yyyyMM";

        public const string STR_SYSLOG_FORMAT = "{0} [{1}] - {2}";
        public const string STR_SYSLOG_FOLDER = @"syslog\";
        public const string STR_SYSLOG_GAMESERVER = "CQ_Server";
        public const string STR_SYSLOG_NPCSERVER = "NPC_Server";
        public const string STR_SYSLOG_ANALYTIC = "Analytic";
        public const string STR_SYSLOG_DATABASE = "Database";

        private readonly string _szMainDirectory = Path.GetPathRoot(Environment.CurrentDirectory) + @"zfserver\";

        /// <summary>
        /// Start a new instance and create the necessary folders.
        /// </summary>
        public LogWriter(string szPath)
        {
            _szMainDirectory = szPath;
            CheckFolders();
        }

        /// <summary>
        /// This method will write the message to the log in the main file and wont show
        /// it on the console.
        /// </summary>
        /// <param name="szMessage">The message buffer that will be written.</param>
        public void SaveLog(string szMessage)
        {
            SaveLog(szMessage, false);
        }

        /// <summary>
        /// This method will write the message to the log in the default file.
        /// </summary>
        /// <param name="szMessage">The message buffer that will be written.</param>
        /// <param name="bConsole">If the message will be printed in the console.</param>
        public void SaveLog(string szMessage, bool bConsole)
        {
            SaveLog(szMessage, bConsole, STR_SYSLOG_GAMESERVER);
        }

        /// <summary>
        /// This method will write the message to the default file with the required log type.
        /// </summary>
        /// <param name="szMessage">The message that will be written.</param>
        /// <param name="bConsole">If the message will be printed in the console.</param>
        /// <param name="ltLog">The kind of message that will be shown.</param>
        public void SaveLog(string szMessage, bool bConsole, LogType ltLog)
        {
            SaveLog(szMessage, bConsole, STR_SYSLOG_GAMESERVER, ltLog);
        }

        /// <summary>
        /// This method should be used when it should not show date time settings.
        /// </summary>
        /// <param name="bConsole"></param>
        /// <param name="szMessage"></param>
        /// <param name="szFileName"></param>
        public void SaveLog(bool bConsole, string szMessage, string szFileName)
        {
            CheckFolders();

            string szFilePath = _szMainDirectory + STR_SYSLOG_FOLDER + szFileName;

            if (bConsole)
                Console.WriteLine(szMessage);

            WriteToFile(FormatSysString(szMessage, 0, false), szFilePath);
        }

        /// <summary>
        /// This method will write the message on the required file with the defined parameters.
        /// </summary>
        /// <param name="szMessage">The message that will be written.</param>
        /// <param name="bConsole">If the message will be printed in the console.</param>
        /// <param name="szFileName">The file name where the log will be written.</param>
        /// <param name="ltLog">The kind of message that will be shown.</param>
        public void SaveLog(string szMessage, bool bConsole, string szFileName, LogType ltLog = LogType.MESSAGE)
        {
            CheckFolders();

            string szDefault = szMessage;
            szMessage = FormatSysString(szMessage, ltLog);

            string szFilePath = _szMainDirectory + STR_SYSLOG_FOLDER + szFileName;

            switch (ltLog)
            {
                case LogType.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogType.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.EXCEPTION:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.WARNING:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            if (bConsole)
                Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm:ss") + " - " + szDefault);

            WriteToFile(szMessage, szFilePath);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void GmLog(string szFileName, string szMessage)
        {
            GmLog(szFileName, szMessage, false);
        }

        public void GmLog(string szFileName, string szMessage, bool bConsole)
        {
            CheckFolders();

            szFileName = GetGmFolder() + szFileName;

            string szOriginal = szMessage;
            szMessage = FormatGmString(szOriginal);

            WriteToFile(szMessage, szFileName);
        }

        public void WriteToFile(string szFullMessage, string szFilePath)
        {
            bool bStop = false;

            szFilePath = szFilePath + DateTime.Now.ToString("yyyy-M-dd") + ".log";

            if (!File.Exists(szFilePath))
                File.Create(szFilePath).Close();

            while (!bStop)
            {
                try
                {
                    var pWriter = File.AppendText(szFilePath);
                    pWriter.WriteLine(szFullMessage);
                    pWriter.Close();
                    bStop = true;
                }
                catch
                {

                }
            }
        }

        private string FormatSysString(string szMessage, LogType ltType, bool bTime = true)
        {
            if (bTime)
                return string.Format(STR_SYSLOG_FORMAT, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ltType, szMessage);
            return string.Format(szMessage);
        }

        private string FormatGmString(string szMessage)
        {
            return string.Format(STR_GMLOG_FORMAT, szMessage, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private string GetGmFolder()
        {
            return _szMainDirectory + STR_GMLOG_FOLDER + DateTime.Now.ToString(STR_GMLOG_SUBFOLDER) + @"\";
        }

        /// <summary>
        /// This method will check if the folders are created to avoid a exception while
        /// writing to the log.
        /// </summary>
        private void CheckFolders()
        {
            try
            {
                if (!Directory.Exists(_szMainDirectory + STR_GMLOG_FOLDER))
                    Directory.CreateDirectory(_szMainDirectory + STR_GMLOG_FOLDER);
                if (!Directory.Exists(_szMainDirectory + STR_GMLOG_FOLDER + DateTime.Now.ToString(STR_GMLOG_SUBFOLDER) + @"\"))
                    Directory.CreateDirectory(_szMainDirectory + STR_GMLOG_FOLDER + DateTime.Now.ToString(STR_GMLOG_SUBFOLDER) + @"\");
                if (!Directory.Exists(_szMainDirectory + STR_SYSLOG_FOLDER))
                    Directory.CreateDirectory(_szMainDirectory + STR_SYSLOG_FOLDER);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public enum LogType
    {
        MESSAGE,
        DEBUG,
        WARNING,
        ERROR,
        EXCEPTION
    }
}