using System.Text;

namespace PhazeX.Network
{
    public class Message
    {
        //public static byte[] Message_END = new byte[] { 255, 255, 255, 255,
        //                                               255, 255, 255, 255,
        //                                               255, 255, 255, 255,
        //                                               255, 255, 255, 255 };

        public static string ByteArrayToString(byte[] array, short start, short length)
        {
            StringBuilder sb = new StringBuilder(length);
            for (short c = start; c < start + length; c++)
            {
                sb.Append((char)array[c]);
            }
            return sb.ToString();
        }

        public static int StringToByteArray(string s, byte[] array, int start)
        {
            char[] ca = s.ToCharArray();
            for (short ctr = 0; ctr < ca.Length; ctr++)
            {
                array[start + ctr] = (byte)(ca[ctr]);
            }
            return (short)(start + ca.Length);
        }


        ///// <summary>
        ///// Finds the where the first message_end exists in the msg. Thus, the first message
        ///// is from start to whatever this returns. It returns -1 if can't find it.
        ///// </summary>
        ///// <param name="msg">The buffer</param>
        ///// <param name="start">Start point of looking in the buffer</param>
        ///// <param name="end">First blank spot in the buffer</param>
        ///// <returns>Where the message_end starts or -1 if not found.</returns>
        //public static int FindMessageEnd(byte [] msg, int start, int end)
        //{
        //    int message_end_start = start;
        //    int i = 0;
        //    for (int c = start; c < end; c++)
        //    {
        //        if (msg[c] == Message_END[i])
        //        {
        //            i++;
        //            if (i >= Message_END.Length) return message_end_start;
        //        }
        //        else
        //        {
        //            i = 0; //reset looking in message_end
        //            message_end_start = c + 1;
        //        }
        //    }
        //    return -1;
        //}


        public static byte[] Encode(byte[] msg)
        {
            byte[] new_msg = new byte[msg.Length + 2];
            //byte[] new_msg = new byte[msg.Length + Message_END.Length];
            
            msg.CopyTo(new_msg, 2);
            new_msg[0] = (byte)(msg.Length >> 8);
            new_msg[1] = (byte)(msg.Length);
            //for (short c = 0; c < Message_END.Length; c++)
            //{
            //    new_msg[msg.Length + c] = Message_END[c];
            //}
            return new_msg;
        }

        










        public byte [] MessageText { get { return msg; } }
        protected bool _log = true;
        public bool Log { get { return _log; } }
        protected byte [] msg = null;
        public Message(byte [] text)
        {
            msg = text;
        }
        public Message()
        {
            //used for future interop
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(msg.Length);
            foreach (byte b in msg)
            {
                sb.Append((char)b);
            }
            return sb.ToString();
        }
    }
}
