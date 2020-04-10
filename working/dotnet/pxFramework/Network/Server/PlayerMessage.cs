
namespace PhazeX.Network.Server
{
    internal class PlayerMessage
    {
        private ServerPlayer _player = null;
        private Message _message = null;
        public ServerPlayer Player
        {
            get { return _player; }
        }
        public Message MessageObj
        {
            get { return _message; }
        }
        public PlayerMessage(ServerPlayer sp, Message msg)
        {
            _player = sp;
            _message = msg;
        }
    }
}
