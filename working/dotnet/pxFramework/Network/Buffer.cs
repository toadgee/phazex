using System.Net.Sockets;

namespace PhazeX.Network
{
    public class Buffer
    {
        //buffer size incremental, grow by 256kb when needed
        private const int _bufinc = 262144;

        //buffer size
        private int _bufsz;
        
        //buffer to store data
        private byte[] _buffer;

        //starting point in the buffer of stuff we haven't yet processed
        private int _bufstart;
        
        //ending point in the buffer, points to the first free byte
        private int _bufend;

        /// <summary>
        /// Create the buffer object.
        /// </summary>
        public Buffer()
        {
            _bufsz = _bufinc;
            _buffer = new byte[_bufsz];
            _bufstart = 0;
            _bufend = 0;
        }


        /// <summary>
        /// Reads data in from a socket.
        /// </summary>
        /// <param name="s">The socket to read from </param>
        /// <param name="bytes_to_read">The number of bytes to read from the socket</param>
        public void ReadFromSocket(Socket s, int bytes_to_read)
        {
            if ((s == null) || (bytes_to_read <= 0)) return;
            
            //make sure we have enough space by requesting space in the buffer
            this.request(bytes_to_read);

            //receive the bytes and modify the end tag in the buffer accordingly
            _bufend += s.Receive(_buffer, _bufend, bytes_to_read, SocketFlags.None);
        }

        /// <summary>
        /// Request space in the buffer. Will move data around and/or grow
        /// the buffer when necessary.
        /// </summary>
        /// <param name="space">Number of bytes to request</param>
        protected void request(int space)
        {
            //early out!
            if (_bufend + space < _bufsz) return;

            //move data around in memory, trying to get free space at end
            if (_bufstart != 0)
            {
                for (int ptr = _bufstart; ptr < _bufend; ptr++)
                {
                    _buffer[ptr - _bufstart] = _buffer[ptr];
                }
                _bufend -= _bufstart;
                _bufstart = 0;
            }

            //grow buffer, if necessary
            while ((_bufsz - _bufend) < space)
            {
                _bufsz += _bufinc;
                byte[] new_buffer = new byte[_bufsz];
                for (int ptr = _bufstart; ptr < _bufend; ptr++)
                {
                    new_buffer[ptr] = _buffer[ptr];
                }
                _buffer = new_buffer;
            }
        }


        /// <summary>
        /// Figures out if we can decode at least one mesage
        /// in the buffer
        /// </summary>
        /// <returns>True if one message exists in the buffer, false otherwise</returns>
        public bool CanDecode()
        {
            //check to see if we can load the length
            if (_bufend - _bufstart < 2) return false;
            
            //load length
            int length = _buffer[_bufstart];
            length <<= 8;
            length += _buffer[_bufstart + 1];

            return (_bufend - _bufstart >= length);

            //return (Message.FindMessageEnd(_buffer, _bufstart, _bufend) != -1);
        }

        public byte[] Decode()
        {
            if (!this.CanDecode()) return null;
            //int message_end = Message.FindMessageEnd(_buffer, _bufstart, _bufend);

            //load length
            int length = _buffer[_bufstart];
            length <<= 8;
            length += _buffer[_bufstart + 1];

            _bufstart += 2;

            byte[] retval = new byte[length];
            for (int c = _bufstart; c < _bufstart + length; c++)
            {
                retval[c - _bufstart] = _buffer[c];
            }

            _bufstart += length;
            //_bufstart = message_end + Message.Message_END.Length;

            return retval;
        }
    }
}
