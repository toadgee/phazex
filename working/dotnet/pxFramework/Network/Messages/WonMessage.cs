using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /*  won message
	 * 
	 * NAME							BYTES
	 * type							1
	 * points						2
	 * number of ids				1
	 * player id					1 *
	 *
	 */
    public class WonMessage : Message
    {
        public delegate void WonMessageDelegate(List<Player> players, int points);

        private int [] _playerIds;
        public int [] PlayerIds { get { return _playerIds; } }
        private int _points;
        public int Points { get { return _points; } }
        public WonMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _points = (int)msg[1];
            _points <<= 8;
            _points += msg[2];
            int count = msg[3];
            if (count == 0) _playerIds = null;
            else
            {
                _playerIds = new int[count];

                for (int ctr = 0; ctr < count; ctr++)
                {
                    _playerIds[ctr] = (int)msg[4 + ctr];
                }

            }
        }

        public WonMessage(int[] winning_ids, int points) : base()
        {
            _playerIds = winning_ids;
            _points = points;

            msg = new byte[4 + winning_ids.Length];
            msg[0] = (byte)pxMessages.Won;
            msg[1] = (byte)(points >> 8);
            msg[2] = (byte)(points);
            msg[3] = (byte)(winning_ids.Length);
            int ptr = 0;
            foreach (int i in winning_ids)
            {
                msg[4 + ptr] = (byte)i;
                ptr++;
            }
        }

        public override string ToString()
        {
            return "Won message";
        }

        public void Validate(int [] ids)
        {
            if (msg.Length < 4)
            {
                throw new BadMessageException("Won Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Won)
            {
                throw new BadMessageException("Won Message : Message does not start with the proper type");
            }

            int tmp;

            tmp = (int)msg[1];
            tmp <<= 8;
            tmp += (int)msg[2];
            if ((tmp < 0) || (tmp > 30000))
            {
                throw new BadMessageException("Won Message : Points field is invalid (" + tmp + ")");
            }

            tmp = (int)msg[3];
            if ((tmp < 0) || (tmp > ids.Length))
            {
                throw new BadMessageException("Won Message : Number of IDs field is invalid : " + tmp + " : " + ids.Length);
            }

            int num_players = tmp;


            for (int ctr = 0; ctr < num_players; ctr++)
            {
                tmp = (int)msg[4 + ctr];
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
                    throw new BadMessageException("Discard Skip Message : Unknown player ID");
                }
            }
        }
    }
}
