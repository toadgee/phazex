using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Helpers;

namespace PhazeX.Network.Messages
{
    /* login message
	 * 
	 * NAME							BYTES			POS
	 * type							1				0
	 * player id                    2               1
	 * ready?						1				3
     * computer player?             1               4
	 * freshness                    1               5
	 * client version major			1				6
	 * client version minor			1				7
	 * client version build         1				8
	 * library version major		1				9
	 * library version minor		1				10
	 * library version build		1				11
	 * username ptr					2				12
	 * client title ptr				2				14
	 * .net framework version ptr	2				16
	 * os version ptr				2				18
	 * username length              1				*
	 * username						3 - 255			*
	 * client title length			2				*
	 * client title string			1 - 1024		*
	 * framework version length     2				*				
	 * framework version string     1 - 1024		*
	 * os version length            2				*
	 * os version string            1 - 1024		*
	 */
    public class LoginMessage : Message
    {
        public delegate void LoginMessageDelegate(Player p, bool fresh);
        public LoginMessage(
            string username, int id, bool ready, bool cp, bool fresh, int major, int minor, int build
            , string title, string framework, string os, int lib_major, int lib_minor, int lib_build) : base()
        {
            if ((username.Length < 3) || (username.Length > 255))
            {
                throw new Exception("Username is not valid size.");
            }


            if ((title.Length < 1) || (title.Length > 1024))
            {
                throw new Exception("Title is not valid size.");
            }
            

            if ((framework.Length < 1) || (framework.Length > 1024))
            {
                throw new Exception("Framework title is not valid size.");
            }
            
            //no control over os variable
            if (os.Length > 1024)
            {
                os = os.Substring(0, 1024);
            }
            if ((os == null) || (os.Length < 1))
            {
                os = "Unknown operating system.";
            }


            _username = username;
            _id = id;
            _ready = ready;
            _computerPlayer = cp;
            _fresh = fresh;
            _major = major;
            _minor = minor;
            _build = build;
            _title = title;
            _framework = framework;
            _os = os;
            _lib_major = lib_major;
            _lib_minor = lib_minor;
            _lib_build = lib_build;

            

            
            //figure out length
            short length = 27;
            length += (short)framework.Length;
            length += (short)username.Length;
            length += (short)title.Length;
            length += (short)os.Length;

            msg = new byte[length];

            int ptr = 0;
            msg[0] = (byte)pxMessages.Login;
            msg[1] = (byte)(id >> 8);
            msg[2] = (byte)id;

            if (ready) msg[3] = (byte)1;
            else msg[3] = (byte)0;

            if (cp) msg[4] = (byte)1;
            else msg[4] = (byte)0;

            if (fresh) msg[5] = (byte)1;
            else msg[5] = (byte)0;

            msg[6] = (byte)major;
            msg[7] = (byte)minor;
            msg[8] = (byte)build;
            msg[9] = (byte)(lib_major);
            msg[10] = (byte)(lib_minor);
            msg[11] = (byte)(lib_build);

            //will do these later!
            msg[12] = (byte)0; //username ptr 1
            msg[13] = (byte)0; //username ptr 2
            msg[14] = (byte)0; //client title ptr 1
            msg[15] = (byte)0; //client title ptr 2
            msg[16] = (byte)0; //.net framework version ptr 1
            msg[17] = (byte)0; //.net framework version ptr 2
            msg[18] = (byte)0; //os version ptr 1
            msg[19] = (byte)0; //os version ptr 2

            ptr = 20;

            msg[12] = (byte)(ptr >> 8);
            msg[13] = (byte)ptr;
            msg[ptr] = (byte)username.Length;
            ptr = Message.StringToByteArray(username, msg, (short)(ptr + 1));

            
            msg[14] = (byte)(ptr >> 8);
            msg[15] = (byte)ptr;
            msg[ptr] = (byte)(title.Length >> 8);
            msg[ptr + 1] = (byte)(title.Length);
            ptr = Message.StringToByteArray(title, msg, (short)(ptr + 2));


            msg[16] = (byte)(ptr >> 8);
            msg[17] = (byte)ptr;
            msg[ptr] = (byte)(framework.Length >> 8);
            msg[ptr + 1] = (byte)(framework.Length);
            ptr = Message.StringToByteArray(framework, msg, (short)(ptr + 2));

            msg[18] = (byte)(ptr >> 8);
            msg[19] = (byte)ptr;
            msg[ptr] = (byte)(os.Length >> 8);
            msg[ptr + 1] = (byte)(os.Length);
            ptr = Message.StringToByteArray(os, msg, (short)(ptr + 2));
        }

        private string _username;
        public string Username { get { return _username; } }
        private int _id;
        public int Id { get { return _id; } }
        private bool _ready;
        public bool Ready { get { return _ready; } }
        private bool _computerPlayer;
        public bool ComputerPlayer { get { return _computerPlayer; } }
        private bool _fresh;
        public bool Fresh { get { return _fresh; } }
        private int _major;
        public int Major { get { return _major; } }
        private int _minor;
        public int Minor { get { return _minor; } }
        private int _build;
        public int Build { get { return _build; } }
        private string _title;
        public string Title { get { return _title; } }
        private string _framework;
        public string Framework { get { return _framework; } }
        private string _os;
        public string Os { get { return _os; } }
        private int _lib_major;
        public int Lib_Major { get { return _lib_major; } }
        private int _lib_minor;
        public int Lib_Minor { get { return _lib_minor; } }
        private int _lib_build;
        public int Lib_Build { get { return _lib_build; } }

        public LoginMessage(byte [] b) : base(b)
        {
            Validate();
            int ptr, length;

            _id = (((int)msg[1]) << 8) + (int)msg[2];
            _ready = ((int)msg[3] == 1);
            _computerPlayer = ((int)msg[4] == 1);
            _fresh = ((int)msg[5] == 1);
            _major = (int)msg[6];
            _minor = (int)msg[7];
            _build = (int)msg[8];
            _lib_major = (int)msg[9];
            _lib_minor = (int)msg[10];
            _lib_build = (int)msg[11];

            ptr = (((int)msg[12]) << 8) + (int)msg[13];
            length = (int)msg[ptr];
            _username = Message.ByteArrayToString(msg, (short)(ptr + 1), (short)length);

            ptr = (((int)msg[14]) << 8) + (int)msg[15];
            length = (((int)msg[ptr]) << 8) + (int)msg[ptr + 1];
            _title = Message.ByteArrayToString(msg, (short)(ptr + 2), (short)length);

            ptr = (((int)msg[16]) << 8) + (int)msg[17];
            length = (((int)msg[ptr]) << 8) + (int)msg[ptr + 1];
            _framework = Message.ByteArrayToString(msg, (short)(ptr + 2), (short)length);

            ptr = (((int)msg[18]) << 8) + (int)msg[19];
            length = (((int)msg[ptr]) << 8) + (int)msg[ptr + 1];
            _os = Message.ByteArrayToString(msg, (short)(ptr + 2), (short)length);
        }

        public override string ToString()
        {
            return "Login Message for " + _username + " [" + _id + "]";
        }

        private void Validate()
        {
            //min length for message is 31
            //max length for message is 3352
            if ((msg.Length < 31) || (msg.Length > 3352))
            {
                throw new BadMessageException("Login Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Login)
            {
                throw new BadMessageException("Login Message : Message does not start with the proper type");
            }

            int tmp;

            tmp = (int)msg[3];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Login Message : Ready field is invalid");
            }

            tmp = (int)msg[4];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Login Message : Computer Player field is invalid");
            }

            tmp = (int)msg[5];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Login Message : Freshness field is invalid");
            }


            int len = 20;
            int val;
            tmp = (int)msg[12];
            tmp <<= 8;
            tmp += (int)msg[13];
            val = (int)msg[tmp];
            if ((val < 3) || (val > 255))
            {
                throw new BadMessageException("Login message : Username length is invalid");
            }
            len += val + 1;

            tmp = (int)msg[14];
            tmp <<= 8;
            tmp += (int)msg[15];
            val = (int)msg[tmp];
            val <<= 8;
            val += (int)msg[tmp + 1];
            if ((val < 1) || (val > 1024))
            {
                throw new BadMessageException("Login Message : Client title length is invalid");
            }
            len += val + 2;

            tmp = (int)msg[16];
            tmp <<= 8;
            tmp += (int)msg[17];
            val = (int)msg[tmp];
            val <<= 8;
            val += (int)msg[tmp + 1];
            if ((val < 1) || (val > 1024))
            {
                throw new BadMessageException("Login Message : Framework version length is invalid");
            }
            len += val + 2;

            tmp = (int)msg[18];
            tmp <<= 8;
            tmp += (int)msg[19];
            val = (int)msg[tmp];
            val <<= 8;
            val += (int)msg[tmp + 1];
            if ((val < 1) || (val > 1024))
            {
                throw new BadMessageException("Login Message : OS version length is invalid");
            }
            len += val + 2;

            if (msg.Length != len)
            {
                throw new BadMessageException("Login Message : Encoded message length is invalid");
            }
        }
    }
}
