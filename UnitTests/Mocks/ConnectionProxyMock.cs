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

        public bool SignOff(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.SignOff(ipAndPort);
        }

        public void GetStartMsg(bool isRicartAlgorithm)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetStartMsg(isRicartAlgorithm);
        }

        public object[] GetHosts(string ipAndPortCallee)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.GetHosts(ipAndPortCallee);
        }

        public void AddNewHost(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.AddNewHost(ipAndPort);
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

        public string ReadResource(string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            return host.Server.ReadResource(ipAndPort);
        }

        public void UpdateResource(string updateStr, string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.UpdateResource(updateStr, ipAndPort);
        }

        public void GetSyncRequest_CT(long id, string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetSyncRequest_CT(id, ipAndPort);
        }

        public void GetReleasedMsg_CT(long id, string fromIpAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetReleasedMsg_CT(id, fromIpAndPort);
        }

        public void GetAcceptResponse_CT()
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetAcceptResponse_CT();
        }

        public void GetSyncRequest_RA(int timestamp, long id, string ipAndPort)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetSyncRequest_RA(timestamp, id, ipAndPort);
        }

        public void GetAcceptResponseRA(string fromIpAndPort, int timestamp)
        {
            var host = _hostLookupWithUrls[_curUrl];
            host.Server.GetAcceptResponseRA(fromIpAndPort, timestamp);
        }

    }
}
