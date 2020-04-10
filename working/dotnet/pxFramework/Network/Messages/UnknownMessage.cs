using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    public class UnknownMessage : Message
    {
        public delegate void UnknownMessageDelegate(byte [] msg);
        public int Identifier
        {
            get
            {
                if (msg.Length > 0) return (int)msg[0];
                return -1;
            }
        }

        public UnknownMessage(byte [] b) : base(b)
        {
            
        }

        public override string ToString()
        {
            if (msg.Length > 0) return "Unknown Message starting with " + (int)msg[0];
            return "Unknown Message of 0 length";
        }
    }
}
