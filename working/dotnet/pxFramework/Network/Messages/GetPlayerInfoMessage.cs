using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* get player info message
	 *
	 * NAME							BYTES
	 * type							1
	 * user id						1
	 */
    public class GetPlayerInfoMessage : Message
    {
        public GetPlayerInfoMessage(int userid) : base()
        {

            _id = userid;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.PlayerInfo;
            msg[1] = (byte)userid;
        }

        private int _id;
        public int _Id { get { return _id; } }
        public GetPlayerInfoMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _id = (int)msg[1];
        }

        public override string ToString()
        {
            return "Get Player Info request";
        }

        public void Validate(int [] ids)
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Get Player Info Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GetPlayerInfo)
            {
                throw new BadMessageException("Get Player Info Message : Message does not start with the proper type");
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
                throw new BadMessageException("Get Player Info Message : Unknown player ID");
            }
        }
    }
}
