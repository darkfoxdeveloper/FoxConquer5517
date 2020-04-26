using MsgServer.Structures.Entities;
using System.Collections.Generic;

namespace ServerCore.Networking.Packets
{
    public class TQDialog
    {
        private Character _player;
        private List<MsgTaskDialog> _replies;

        public TQDialog(Character player)
        {
            _player = player;
            _replies = new List<MsgTaskDialog>();
        }

        public void AddText(string text)
        {
            if (text.Length > 100)
            {
                if (text.Length > 980)
                    text = text.Substring(0, 980);
                int myLength = text.Length;
                while (myLength > 0)
                {
                    int lastIndex = 100;
                    if (myLength < 100)
                        lastIndex = myLength;
                    string txt = text.Substring(0, lastIndex);
                    text = text.Substring(lastIndex, myLength - lastIndex);
                    myLength -= lastIndex;
                    _replies.Add(new MsgTaskDialog(MsgTaskDialog.DIALOG, txt));
                }
            }
            else
                _replies.Add(new MsgTaskDialog(MsgTaskDialog.DIALOG, text));
        }

        public void SetAvatar(ushort npcMesh)
        {
            if (_replies.Count == 0) {
                _replies.Add(new MsgTaskDialog(MsgTaskDialog.AVATAR, "") { InputMaxLength = npcMesh });
            }
            else
            {
                _replies[0].InputMaxLength = npcMesh;
            }
        }

        public void AddOption(string text, byte optionId)
        {
            _replies.Add(new MsgTaskDialog(MsgTaskDialog.OPTION, text)
            {
                OptionId = optionId,
            });
        }

        public void AddInput(string text, byte id, byte maxLength)
        {
            _replies.Add(new MsgTaskDialog()
            {
                InputMaxLength = maxLength,
                InteractType = MsgTaskDialog.INPUT,
                OptionId = id,
                Text = text
            });
        }

        public void Show()
        {
            foreach (MsgTaskDialog reply in _replies)
                _player.Send(reply);
            _player.Send(new MsgTaskDialog(MsgTaskDialog.FINISH, ""){ DontDisplay = false});
            _replies.Clear();
        }
    }
}
