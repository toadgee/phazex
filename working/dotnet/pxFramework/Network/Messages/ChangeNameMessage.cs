using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* change name message
	 * 
	 * NAME							BYTES
	 * type							1
	 * player ID					1
	 * new name length				1
	 * new name                     3 - 255
	 * old name length              1
	 * old name   					3 - 255
	 */
    public class ChangeNameMessage : Message
    {
        public delegate void ChangeNameMessageDelegate(string oldname, string newname, Player p);

        private string _oldName;
        public string OldName { get { return _oldName; } }
        private string _newName;
        public string NewName { get { return _newName; } }
        private int _playerId;
        public int PlayerId { get { return _playerId; } }
        public ChangeNameMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _playerId = (int)msg[1];
            _newName = Message.ByteArrayToString(msg, 3, (short)msg[2]);
            int pos = (int)msg[2] + 3;
            _oldName = Message.ByteArrayToString(msg, (short)(pos + 1), (short)msg[pos]);
        }

        public ChangeNameMessage(int player_id, string new_name, string old_name) : base()
        {
            if (new_name.Length == 0) throw new Exception("New name cannot be empty");
            if (old_name.Length == 0) throw new Exception("Old name cannot be empty");
            if (old_name.Length < 3) throw new Exception("Old name must be at least 3 characters long");
            if (new_name.Length < 3) throw new Exception("New name must be at least 3 characters long");
            if (new_name.Length > 255) throw new Exception("New name must be less than 256 characters long");
            if (old_name.Length > 255) throw new Exception("Old name must be less than 256 characters long");

            _oldName = old_name;
            _newName = new_name;
            _playerId = player_id;

            int i = 0;
            msg = new byte[4 + old_name.Length + new_name.Length];
            msg[0] = (byte)pxMessages.ChangeName;
            msg[1] = (byte)player_id;
            msg[2] = (byte)new_name.Length;
            i = Message.StringToByteArray(new_name, msg, 3);
            msg[i] = (byte)old_name.Length;
            i = Message.StringToByteArray(old_name, msg, (short)(i + 1));
        }

        public override string ToString()
        {
            return "Change name from " + _oldName + " to " + _newName + " [" + _playerId + "]";
        }

        public void Validate(int [] ids)
        {
            if (msg[0] != (byte)pxMessages.ChangeName)
            {
                throw new BadMessageException("Change Name Message : Message does not start with the proper type", ((int)pxMessages.ChangeName).ToString());
            }
            if ((msg.Length < 10) || (msg.Length > 514))
            {
                throw new BadMessageException("Change Name Message : Message length is invalid", "10 - 514", msg.Length.ToString());
            }
            int tmp;
            tmp = (int)msg[1];
            bool found = false;
            foreach (int id in ids)
            {
                if (tmp == id)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new BadMessageException("Change Name Message : Unknown player ID", "", tmp.ToString());
            }

            int len = 4;
            tmp = (int)msg[2];
            len += tmp;
            if ((tmp < 3) || (tmp > 255))
            {
                throw new BadMessageException("Change Name Message : New name length is invalid", "3 - 255", tmp.ToString());
            }

            tmp = (int)msg[len - 1];
            len += tmp;
            if ((tmp < 3) || (tmp > 255))
            {
                throw new BadMessageException("Change Name Message : Old name length is invalid", "3 - 255", tmp.ToString());
            }

            if (msg.Length != len)
            {
                throw new BadMessageException("Change Name Message : Message length encoded is invalid", len.ToString(), msg.Length.ToString());
            }
        }
    }
}
