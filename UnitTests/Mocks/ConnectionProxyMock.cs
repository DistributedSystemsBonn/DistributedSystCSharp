using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using DS_Network.Network;

namespace UnitTests.Mocks
{
    /// <summary>
    /// Use proxy to test cook computing library
    /// Moq doesnt make approachable mocks
    /// </summary>
    public class ConnectionProxyMock : IConnectionProxy
    {
        private Dictionary<string, Host> _hostLookupWithUrls;
        private string _curUrl = String.Empty;

        public ConnectionProxyMock(Dictionary<String, Host> hostLookupWithUrls)
        {
            _hostLookupWithUrls = hostLookupWithUrls;
        }

        public string[] SystemListMethods()
        {
            throw new NotImplementedException();
        }

        public object[] SystemMethodSignature(string MethodName)
        {
            throw new NotImplementedException();
        }

        public string SystemMethodHelp(string MethodName)
        {
            throw new NotImplementedException();
        }

        public bool AllowAutoRedirect
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public X509CertificateCollection ClientCertificates
        {
            get { throw new NotImplementedException(); }
        }

        public string ConnectionGroupName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public CookieContainer CookieContainer
        {
            get { throw new NotImplementedException(); }
        }

        public ICredentials Credentials
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool EnableCompression
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Expect100Continue
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public WebHeaderCollection Headers
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Id
        {
            get { throw new NotImplementedException(); }
        }

        public int Indentation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool KeepAlive
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public XmlRpcNonStandard NonStandard
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool PreAuthenticate
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Version ProtocolVersion
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IWebProxy Proxy
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public CookieCollection ResponseCookies
        {
            get { throw new NotImplementedException(); }
        }

        public WebHeaderCollection ResponseHeaders
        {
            get { throw new NotImplementedException(); }
        }

        public int Timeout
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Url
        {
            get { return _curUrl; }
            set { _curUrl = value; }
        }

        public bool UseEmptyParamsTag
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool UseIndentation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool UseIntTag
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool UseStringTag
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string UserAgent
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Encoding XmlEncoding
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string XmlRpcMethod
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public event XmlRpcRequestEventHandler RequestEvent;
        public event XmlRpcResponseEventHandler ResponseEvent;
        public bool @join(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.@join(ipAndPort);
        }

        public bool signOff(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.signOff(ipAndPort);
        }

        public bool start()
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.start();
        }

        public object[] getHosts(string ipAndPortCallee)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.getHosts(ipAndPortCallee);
        }

        public void addNewHost(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.addNewHost(ipAndPort);
        }

        public bool ReceiveElectionMsg(string id)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.ReceiveElectionMsg(id);
        }

        public void SetMasterNode(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.SetMasterNode(ipAndPort);
        }

        public string readResource(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.readResource(ipAndPort);
        }

        public void updateResource(string updateStr, string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.updateResource(updateStr, ipAndPort);
        }

        public void GetSyncRequest(int timestamp, long id, string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetSyncRequest(timestamp, id, ipAndPort);
        }

        public void GetAcceptResponse(string fromIpAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetAcceptResponse(fromIpAndPort);
        }

        public void GetReleasedMsg(string fromIpAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetReleasedMsg(fromIpAndPort);
        }
    }
}
