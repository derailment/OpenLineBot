namespace OpenLineBot.Models.Bot
{
    abstract public class BotBase
    {
        string _adminId = null;
        string _channelAccessToken = null;

        public string ChannelAccessToken
        {
            get
            {
                return _channelAccessToken;
            }
            protected set
            {
                _channelAccessToken = value;
            }
        }

        public string AdminId {
            get
            {
                return _adminId;
            }
            protected set
            {
                _adminId = value;
            }
        }

    }
}
