using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /*  start game timer message
	 * 
	 * NAME							BYTES
	 * type							1
	 * time							1
	 *
	 */
    public class StartGameTimerMessage : Message
    {
        public delegate void StartGameTimerMessageDelegate(int seconds);
        private int _val;
        public int Val { get { return _val; } }
        public StartGameTimerMessage(byte [] b) : base(b)
        {
            Validate();
            _val = (int)msg[1];
        }

        public StartGameTimerMessage(int val) : base()
        {
            _val = val;
            
            msg = new byte[2];
            msg[0] = (byte)pxMessages.StartGameTimer;
            msg[1] = (byte)val;
        }

        public override string ToString()
        {
            return "Start Game in " + _val;
        }

        public void Validate()
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Start Game Timer Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.StartGameTimer)
            {
                throw new BadMessageException("Start Game Timer Message : Message does not start with the proper type");
            }
        }
    }
}
