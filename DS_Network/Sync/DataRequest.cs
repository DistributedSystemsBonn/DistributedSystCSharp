﻿namespace DS_Network.Sync
{
    public class DataRequest
    {
        public int Time { get; set; }
        public long Id { get; set; }
        public long CallerId { get; set; }

        public string IpAndPort { get; set; }
    }
}
