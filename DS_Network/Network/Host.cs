namespace DS_Network.Network
{
    public class Host
    {
        private Node _client;
        private Server _server;

        public Node Client
        {
            get { return _client; }
        }

        public Server Server
        {
            get { return _server; }
        }

        public Host(Node client, Server server)
        {
            _client = client;
            _server = server;
            _server.Run();
        }
    }
}
