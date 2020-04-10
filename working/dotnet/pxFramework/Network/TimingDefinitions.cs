
namespace PhazeX.Network
{
    public class TimingDefinitions
    {
        //milliseconds
        public static int ListenSocket_Server = 50;
        public static int ListenSocket_Client = 50;
        public static int GetMessage_Client = 50;
        public static int GetMessage_Server = 50;
        public static int Heartbeat_Server = 1000;
        public static int Heartbeat_Client = 1000;
        public static int LoginTimeout_Client = 5000;
        public static int LoginTimeout_Server = 5000;
        public static int pxcpArtificalWait = 0;
        public static int ConnectionListener_Pending = 10;
    }
}
