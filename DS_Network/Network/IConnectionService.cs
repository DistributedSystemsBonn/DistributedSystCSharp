﻿using System;
using CookComputing.XmlRpc;

namespace DS_Network.Network
{
    public interface IConnectionService
    {
        [XmlRpcMethod("Host.join")]
        bool join(string ipAndPort);

        //void AddNewComputer(string ip);

        [XmlRpcMethod("Host.signOff")]
        bool signOff(String ipAndPort);

        [XmlRpcMethod("Host.start")]
        bool start(int id);

        [XmlRpcMethod("Host.getHosts")]
        //[XmlRpcMethod("getHosts")]
        Object[] getHosts(String ipAndPortCallee);

        [XmlRpcMethod("Host.addNewHost")]
        void addNewHost(String ipAndPort);
    }
}

