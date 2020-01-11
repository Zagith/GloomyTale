using System;

namespace GloomyTale.Communication
{
    public class SerializableWorldServer
    {
        #region Properties

        public Guid Id { get; set; }

        public string WorldGroup { get; set; }

        public int ChannelId { get; set; }

        public int AccountLimit { get; set; }

        public string EndPointIp { get; set; }

        public int EndPointPort { get; set; }

        #endregion
    }
}
