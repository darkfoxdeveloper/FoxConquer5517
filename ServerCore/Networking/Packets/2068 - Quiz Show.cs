// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2068 - Quiz Show.cs
// Last Edit: 2016/11/23 09:16
// Created: 2016/11/23 09:16
namespace ServerCore.Networking.Packets
{
    public sealed class MsgQuiz : PacketStructure
    {
        public MsgQuiz(byte[] receivedPacket)
            : base(receivedPacket)
        {

        }

        public MsgQuiz()
            : base(PacketType.MSG_QUIZ, 300, 292)
        {

        }

        public QuizShowType Type
        {
            get { return (QuizShowType)ReadUShort(4); }
            set { WriteUShort((ushort)value, 4); }
        }

        /// <summary>
        /// Switch 1
        /// </summary>
        public ushort TimeTillStart
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        /// <summary>
        /// Switch 2 and 3
        /// </summary>
        public ushort QuestionNumber
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        /// <summary>
        /// Switch 4
        /// </summary>
        public ushort Score
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        /// <summary>
        /// Switch 1
        /// </summary>
        public ushort QuestionAmount
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        /// <summary>
        /// Switch 2 Last Correct Answer and Switch 3 is the selected answer
        /// </summary>
        public ushort LastCorrectAnswer
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        /// <summary>
        /// Switch 4
        /// </summary>
        public ushort TimeTaken
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        /// <summary>
        /// Switch 5
        /// </summary>
        public ushort FinalPrize
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        /// <summary>
        /// Switch 1
        /// </summary>
        public ushort TimePerQuestion
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }

        /// <summary>
        /// Switch 2
        /// </summary>
        public ushort ExperienceAwarded
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }

        /// <summary>
        /// Switch 4
        /// </summary>
        public ushort Rank
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }

        /// <summary>
        /// Switch 1
        /// </summary>
        public ushort FirstPrize
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        /// <summary>
        /// Switch 2
        /// </summary>
        public ushort TimeTakenTillNow
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        /// <summary>
        /// Switch 1
        /// </summary>
        public ushort SecondPrize
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        /// <summary>
        /// Switch 2
        /// </summary>
        public ushort CurrentScore
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        /// <summary>
        /// Switch 1
        /// </summary>
        public ushort ThirdPrize
        {
            get { return ReadUShort(16); }
            set { WriteUShort(value, 16); }
        }

        public byte StringAmount
        {
            get { return ReadByte(24); }
            set { WriteByte(value, 24); }
        }

        private int m_nStringAdd = 0;

        /// <summary>
        /// This method will add the strings to the ranking packet.
        /// </summary>
        public void AddString(string szName, ushort usScore, ushort usTime)
        {
            int offset = 24 + (m_nStringAdd * 20);
            //string szString = string.Format("{0} {1} {2}", szName, usScore, usTime);
            //m_nStringAdd += szString.Length + 1;
            m_nStringAdd += 1;
            //StringAmount += 1;
            WriteByte((byte) m_nStringAdd, 20);
            Resize(50 + (m_nStringAdd * 20));
            WriteHeader(Length - 8, PacketType.MSG_QUIZ);
            //WriteStringWithLength(szString, offset);
            WriteString(szName, 16, offset);
            WriteUShort(usScore, offset + 16);
            WriteUShort(usScore, offset + 18);
        }

        public void AddString(string szQuestion, string szAnswer0, string szAnswer1, string szAnswer2, string szAnswer3)
        {
            StringAmount = 5;
            int lenght = szQuestion.Length + szAnswer0.Length + szAnswer1.Length + szAnswer2.Length + szAnswer3.Length + 4;
            Resize(50 + lenght);
            int startOffset = 25;
            WriteHeader(Length - 8, PacketType.MSG_QUIZ);
            WriteStringWithLength(szQuestion, startOffset);
            WriteStringWithLength(szAnswer0, startOffset + 1 + szQuestion.Length);
            WriteStringWithLength(szAnswer1, startOffset + 2 + szQuestion.Length + szAnswer0.Length);
            WriteStringWithLength(szAnswer2, startOffset + 3 + szQuestion.Length + szAnswer0.Length + szAnswer1.Length);
            WriteStringWithLength(szAnswer3, startOffset + 4 + szQuestion.Length + szAnswer0.Length + szAnswer1.Length + szAnswer2.Length);
        }
    }

    public enum QuizShowType : ushort
    {
        NONE = 0,
        START_QUIZ = 1,
        QUESTION_QUIZ = 2,
        QUIZ_REPLY = 3,
        AFTER_REPLY = 4,
        FINISH_QUIZ = 5,
        QUIT_QUIZ = 8
    }
}