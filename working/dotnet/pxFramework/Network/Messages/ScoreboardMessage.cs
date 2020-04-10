using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Options;

namespace PhazeX.Network.Messages
{
    /* scoreboard message
	 *
	 * NAME						    BYTES
	 * type                         1
	 * player id                    1
	 * session count                2
	 * dealt                        1*(0 = no, 1 = yes)
	 * made phaze                   1*(0 = no, 1 = yes)
	 * phaze number                 1*
	 * points                       2*
	 * was skipped by count         1*
	 * player id                    1**
	 * skipped count                1*
	 * player id                    1**
	 */
    public class ScoreboardMessage : Message
    {
        public delegate void ScoreboardMessageDelegate(Player p);

        private Scoreboard _scoreboard;
        public Scoreboard ScoreboardObj { get { return _scoreboard; } }
        private int _playerId;
        public int PlayerId { get { return _playerId; } }

        public ScoreboardMessage(byte[] b, int [] ids, GameRules rules) : base(b)
        {
            Validate(ids, rules);
            _playerId = (int)msg[1];
            _scoreboard = new Scoreboard();
            Session sess = null;
            int cnt = 0;
            int numSessions = ((int)msg[2] << 8) + ((int)msg[3]);
            int ptr = 4;
            for (int ctr = 0; ctr < numSessions; ctr++)
            {
                sess = new Session(((int)msg[ptr + 0] == 1), ((int)msg[ptr + 2]));
                sess.MadePhaze = ((int)msg[ptr + 1] == 1);
                sess.Points = (((int)msg[ptr + 3]) << 8) + ((int)msg[ptr + 4]);
                ptr += 5;
                cnt = (int)msg[ptr + 0];
                ptr += 1;
                for (int ctr2 = 0; ctr2 < cnt; ctr2++)
                {
                    sess.AddSkippedBy((int)msg[ptr]);
                    ptr++;
                }
                cnt = (int)msg[ptr + 0];
                ptr += 1;
                for (int ctr2 = 0; ctr2 < cnt; ctr2++)
                {
                    sess.AddPlayerSkipped((int)msg[ptr]);
                    ptr++;
                }
                _scoreboard.AddSession(sess);
            }
        }

        public ScoreboardMessage(Scoreboard s, int id) : base()
        {

            _scoreboard = s;
            _playerId = id;

            //figure out length before we construct
            int ctr;
            Session s2;
            int length = 4;
            for (ctr = 0; ctr < s.Count; ctr++)
            {
                length += 7; //for each session plus player ids
                length += s[ctr].SkippedBy.Count;
                length += s[ctr].PlayerSkipped.Count;
            }


            msg = new byte[length];
            int ptr = 0;
            msg[0] = (byte)pxMessages.Scoreboard; //type
            msg[1] = (byte)id;//player id

            int sc = s.Count;
            msg[2] = (byte)(sc >> 8); //session count (1 of 2)
            msg[3] = (byte)(sc);//session count (2 of 2)

            int ctr2, i;
            ptr = 4;
            for (ctr = 0; ctr < s.Count; ctr++)
            {
                s2 = s[ctr];

                //dealt (len + 0)
                if (s2.Dealt) msg[ptr] = (byte)1;
                else msg[ptr] = (byte)0;

                //made phaze (len + 1)
                if (s2.MadePhaze) msg[ptr + 1] += (byte)1;
                else msg[ptr + 1] += (byte)0;

                //phaze number (len + 2)
                msg[ptr + 2] = (byte)(s2.PhazeNumber);

                //points (len + 3), (len + 4)
                msg[ptr + 3] += (byte)(s2.Points >> 8);
                msg[ptr + 4] += (byte)(s2.Points);

                i = s2.SkippedBy.Count;
                msg[ptr + 5] = (byte)i;
                ptr += 6;
                for (ctr2 = 0; ctr2 < i; ctr2++)
                {
                    msg[ptr] = (byte)s2.SkippedBy[ctr2];
                    ptr++;
                }

                i = s2.PlayerSkipped.Count;
                msg[ptr] = (byte)i;
                ptr += 1;
                for (ctr2 = 0; ctr2 < i; ctr2++)
                {
                    msg[ptr] = (byte)s2.PlayerSkipped[ctr2];
                    ptr++;
                }
            }
        }

        public override string ToString()
        {
            return "Scoreboard for [" + _playerId + "]";
        }

        public void Validate(int [] ids, GameRules rules)
        {
            int hands_transpired;
            if (msg.Length < 4)
            {
                throw new BadMessageException("Scoreboard Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Scoreboard)
            {
                throw new BadMessageException("Scoreboard Message : Message does not start with the proper type");
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
                throw new BadMessageException("Scoreboard Message : Unknown initial player ID");
            }

            tmp = (int)msg[2];
            tmp <<= 8;
            tmp += (int)msg[3];
            if ((tmp < 0) || (tmp > 1024))
            {
                throw new BadMessageException("Scoreboard Message : Session count field is invalid");
            }
            hands_transpired = tmp;

            int sc = tmp;
            int len = 4;
            for (int ctr = 0; ctr < sc; ctr++)
            {
                tmp = (int)msg[len];
                if ((tmp != 0) && (tmp != 1))
                {
                    throw new BadMessageException("Scoreboard Message : Dealt field is invalid");
                }

                tmp = (int)msg[len + 1];
                if ((tmp != 0) && (tmp != 1))
                {
                    throw new BadMessageException("Scoreboard Message : Made phaze field is invalid");
                }

                tmp = (int)msg[len + 2];
                if ((tmp < 0) || (tmp > rules.PhazeRules.Count() - 1))
                {
                    throw new BadMessageException("Scoreboard Message : Phaze number field is invalid");
                }

                tmp = (int)msg[len + 3];
                tmp <<= 8;
                tmp += (int)msg[len + 4];


                if ((tmp < 0) || (tmp > (rules.GetMaxCardPoints() * rules.HandCards)))
                {
                    throw new BadMessageException("Scoreboard Message : Points field is invalid (" + tmp + ")");
                }

                tmp = (int)msg[len + 5];
                if ((tmp < 0) || (tmp > 255))
                {
                    throw new BadMessageException("Scoreboard Message : Skipped by field is invalid");
                }

                int t = tmp;
                len += 6;
                for (int ctr2 = 0; ctr2 < t; ctr2++)
                {
                    tmp = (int)msg[len + ctr2];
                    found = false;
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
                        throw new BadMessageException("Scoreboard Message : Unknown player ID * : " + tmp);
                    }
                }
                len += t;

                tmp = (int)msg[len];
                t = tmp;
                len += 1;
                for (int ctr2 = 0; ctr2 < t; ctr2++)
                {
                    tmp = (int)msg[len + ctr2];
                    found = false;
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
                        throw new BadMessageException("Scoreboard Message : Unknown player ID ** : " + tmp);
                    }
                }
                len += t;


            }
        }
    }
}
