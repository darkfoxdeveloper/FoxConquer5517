// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1004 - Message Packet.cs
// Last Edit: 2016/11/23 08:18
// Created: 2016/11/23 08:18

using System.Drawing;
using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// Packet Type: 1004. This packet is sent to the client to display an in-game message from the server or from
    /// another player. It can also be used to direct the client during login to the world or to the character 
    /// creation screen. Generic messages can be found in the class below this one (ServerMessages class).
    /// </summary>
    public sealed class MsgTalk : PacketStructure
    {
        // Global-Scope Constant Declarations:
        public const string SYSTEM = "SYSTEM";
        public const string ALLUSERS = "ALLUSERS";

        // Local-Scope Variable Declarations:
        private const int MINIMUM_LENGTH = 31;
        private const byte AMOUNT_OF_STRINGS = 4;

        // Global-Scope Variable Declarations:
        public Color Hue;           // The hue of the message (default should be white).
        public ChatTone Tone;       // The channel tone for displaying the message, where it is displayed.
        public ChatStyle Style;     // The style of the message, how it is displayed.
        public uint Identity;       // The identity of the player the message is sent to (or the message id).
        public uint RecipientMesh;  // The mesh for the character receiving the message.
        public uint SenderMesh;     // The mesh for the character sending the message.
        public string Recipient;    // The receiver of the message, who the message is sent to.
        public string Sender;       // The sender of the message, who the message is from.
        public string Suffix;       // The suffix of the message, what is placed before the message.
        public string Message;      // The text for the message, what is sent in the message.
        // Notes about the packet: The message id can be the client id, or the current hour * 100 + the current
        // minute ((now.Hour * 100) + now.Minute). The suffix can be the date an offline message was sent on.

        /// <summary>
        /// Packet Type: 1004. This packet is sent to the client to display an in-game message from the server or from
        /// another player. It can also be used to direct the client during login to the world or to the character 
        /// creation screen. Generic messages can be found in the class below this one (ServerMessages class).
        /// </summary>
        /// <param name="receivedPacket">The received packet from the server's packet handler.</param>
        public MsgTalk(byte[] receivedPacket)
            : base(receivedPacket)
        {
            // Read in normal data values:
            Hue = Color.FromArgb(ReadInt(4));
            Tone = (ChatTone)ReadUShort(8);
            Style = (ChatStyle)ReadUShort(10);
            Identity = ReadUInt(12);
            RecipientMesh = ReadUInt(16);
            SenderMesh = ReadUInt(20);

            // Read in strings:
            Sender = ReadString(ReadByte(25), 26);
            Recipient = ReadString(ReadByte(26 + Sender.Length), 27 + Sender.Length);
            Suffix = ReadString(ReadByte(27 + Sender.Length + Recipient.Length), 28 + Sender.Length + Recipient.Length);
            Message = ReadString(ReadByte(28 + Sender.Length + Recipient.Length + Suffix.Length), 29 + Sender.Length + Recipient.Length + Suffix.Length);
        }

        /// <summary>
        /// Packet Type: 1004. This packet is sent to the client to display an in-game message from the server or from
        /// another player. It can also be used to direct the client during login to the world or to the character 
        /// creation screen. Generic messages can be found in the class below this one (ServerMessages class).
        /// </summary>
        /// <param name="message">The message text being sent in the packet to the player.</param>
        /// <param name="tone">The tone of the message (where it is displayed in the client).</param>
        public MsgTalk(string message, ChatTone tone)
            : base(0)
        {
            Hue = Color.White;
            Tone = tone;
            Style = ChatStyle.NORMAL;
            Recipient = ALLUSERS;
            Sender = SYSTEM;
            Suffix = "";
            Message = message;
        }

        /// <summary>
        /// Packet Type: 1004. This packet is sent to the client to display an in-game message from the server or from
        /// another player. It can also be used to direct the client during login to the world or to the character 
        /// creation screen. Generic messages can be found in the class below this one (ServerMessages class).
        /// </summary>
        /// <param name="message">The message text being sent in the packet to the player.</param>
        /// <param name="tone">The tone of the message (where it is displayed in the client).</param>
        /// <param name="color">The hue of the message (default is white).</param>
        public MsgTalk(string message, ChatTone tone, Color color)
            : base(0)
        {
            Hue = color;
            Tone = tone;
            Style = ChatStyle.NORMAL;
            Recipient = ALLUSERS;
            Sender = SYSTEM;
            Suffix = "";
            Message = message;
        }

        /// <summary>
        /// This method should not be called in a method outside the packet structure class. This method is called
        /// by the packet structure class during packet construction (when passing it as a byte array). For advanced
        /// packet construction, you may override this method and define how the array is constructed.
        /// </summary>
        protected override byte[] Build()
        {
            // Resize the array for the packet and write the static variables:
            Resize(MINIMUM_LENGTH + Recipient.Length + Sender.Length + Suffix.Length + Message.Length + 8);
            WriteHeader(Length - 8, PacketType.MSG_TALK);
            WriteInt(Hue.ToArgb(), 4);
            WriteUShort((ushort)Tone, 8);
            WriteUShort((ushort)Style, 10);
            WriteUInt(Identity, 12);
            WriteUInt(RecipientMesh, 16);
            WriteUInt(SenderMesh, 20);
            WriteByte(AMOUNT_OF_STRINGS, 24);

            // Write strings:
            WriteStringWithLength(Sender, 25);
            WriteStringWithLength(Recipient, 26 + Sender.Length);
            WriteStringWithLength(Suffix, 27 + Sender.Length + Recipient.Length);
            WriteStringWithLength(Message, 28 + Sender.Length + Recipient.Length + Suffix.Length);
            return base.Build();
        }
    }

    /// <summary>
    /// This class encapsulates predefined server messages, defined by the server for generic use. The class is 
    /// constructed upon first use, and messages are saved as byte arrays for more efficient sending. Specific messages
    /// for users should not be added to this class.
    /// </summary>
    public static class ServerMessages
    {
        /// <summary> This class contains messages to be sent upon login. </summary>
        public static class Login
        {
            public static byte[] AccountInJeopardy = new MsgTalk("Your account was just accessed from another computer outside your household network. You should consider changing your password immediately.", ChatTone.LOGIN);
            public static byte[] AlreadyLoggedIn = new MsgTalk("You are already logged in from another computer inside your household network. Please check your household to ensure that your account is secure.", ChatTone.LOGIN);
            public static byte[] AnswerOk = new MsgTalk("ANSWER_OK", ChatTone.LOGIN);
            public static byte[] MapServerOffline = new MsgTalk("The map server is currently offline. Please try again in a few minutes. If this problem persists, please contact support.", ChatTone.LOGIN);
            public static byte[] NewRole = new MsgTalk("NEW_ROLE", ChatTone.LOGIN);
            public static byte[] OutdatedClient = new MsgTalk("Your client is out of date. Please update your client and try again.", ChatTone.LOGIN);
            public static byte[] TransferFailed = new MsgTalk("The server is unable to process your authentication request. If this problem persists, please contact support.", ChatTone.LOGIN);
            public static byte[] ServerFull = new MsgTalk("The server is full.", ChatTone.LOGIN);
        }

        /// <summary> This class contains messages to be sent upon character creation. </summary>
        public static class CharacterCreation
        {
            public static byte[] AccessDenied = new MsgTalk("You have not been granted access to Character Creation by the server.", ChatTone.CHARACTER_CREATION);
            public static byte[] AccountHasCharacter = new MsgTalk("This account already have an character registered.", ChatTone.CHARACTER_CREATION);
            public static byte[] AnswerOk = new MsgTalk("ANSWER_OK", ChatTone.CHARACTER_CREATION);
            public static byte[] InvalidName = new MsgTalk("The name you entered contains an invalid character.", ChatTone.CHARACTER_CREATION);
            public static byte[] NameTaken = new MsgTalk("The name you entered has already been taken.", ChatTone.CHARACTER_CREATION);
        }

        /// <summary> This class contains messages for welcoming players to the server and to events. </summary>
        public static class Welcome
        {
            public static byte[] LoginWelcome = new MsgTalk("Welcome to the world of Conquer Online!", ChatTone.SYSTEM);
        }
    }
}