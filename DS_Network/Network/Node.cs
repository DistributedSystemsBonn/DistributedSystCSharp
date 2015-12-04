using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using DS_Network.Helpers;

namespace DS_Network.Network
{
    public class Node
    {
        private Dictionary<int, String> _hostLookup = new Dictionary<int, string>();
        private IPAddress _address;

        //TODO: put WCF service to constructor as parameter and use it in methods (like join...)
        public Node(IConnectionService client) //ServiceReference1.Service1Client client
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _address = ip;
                }
            }
            
            if (_address == null)
            {
                throw new Exception("Cannot find proper ip address");
            }
        }

        public void ProcessCommand(string command)
        {
            var commandArr = StringHelper.GetCommandArray(command);
            var commandName = commandArr[0];

            if (StringHelper.IsWithParameter(commandArr))
            {
                if (commandName != "Join")
                {
                    throw new ArgumentException("Only join command can be with parameter");
                }
                var commandParameter = commandArr[1];
                IPAddress toJoinAddress = StringHelper.ConvertIpAddress(commandParameter);
                Join(toJoinAddress.ToString());
            }
            else if (commandArr.Length == 1)
            {
                if (commandName == "Signoff")
                {
                    SignOff();
                }
                else if (commandName == "Start")
                {
                    Start();
                }
            }
            else
            {
                throw new ArgumentException("Number of parameters shouldn't be more than 1 or command should be without parameter");
            }
        }

        public void Join(String address)
        {
            Console.WriteLine("Join operation with address " + address);



            //TODO: join. send message to just one machine. And then it propagates the message
        }

        public void AddNewComputer(string ip)
        {
            throw new NotImplementedException();
        }

        public void SignOff()
        {
            //TODO: signoff message (we need to ask, is this a broadcast message or one-node message)???
        }

        public void Start()
        {
            //TODO: 1. send message to all other nodes
            //IN LOOP for 20 seconds
            //2. wait random amount of time
            //3. read string variable from master node
            //4. append some random english word to this string
            //5. write updated string to master node
            //END LOOP

            //6. Node fetches from Master node the final string
            //7. And writes this final string on screen

            //NOTE: read and write operations should be syncronized
        }

    }
}
