using System;
using System.Collections.Generic;
using System.Text;
using PhazeX.Options;

namespace PhazeX.Network.Messages
{
    /* current phaze message
	 *
	 * NAME							BYTES
	 * type							1
	 * phaze number					1
	 * 
	 */
    public class CurrentPhazeMessage : Message
    {
        public delegate void CurrentPhazeMessageDelegate(int num);

        private int _currentPhaze;
        public int CurrentPhaze { get { return _currentPhaze; } }

        public CurrentPhazeMessage(byte [] b, GameRules rules) : base(b)
        {
            Validate(rules);
            _currentPhaze = (int)msg[1];
        }

        public CurrentPhazeMessage(int num):base()
        {
            _currentPhaze = num;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.CurrentPhaze;
            msg[1] = (byte)num;
        }

        public override string ToString()
        {
            return "Current Phaze : " + _currentPhaze;
        }

        public void Validate(GameRules rules)
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Current Phaze Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.CurrentPhaze)
            {
                throw new BadMessageException("Current Phaze Message : Message does not start with the proper type");
            }
            int tmp;

            tmp = (int)msg[1];
            if ((tmp < 0) || (tmp > (rules.PhazeRules.Count() - 1)))
            {
                throw new BadMessageException("Current Phaze Message : Current phaze number field is invalid");
            }
        }
    }
}
