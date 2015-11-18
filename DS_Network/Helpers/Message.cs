using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_Network.Helpers
{
    public class Message //TODO: Is it derived from base class?
    {
        private bool _isMasterNode = false;
        private string _message = String.Empty;

        public bool IsMasterNode
        {
            get
            {
                return this._isMasterNode;
            }
            set
            {
                this._isMasterNode = value;
            }
        }

        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }

        public Message(bool isMasterNode, string message)
        {
            _isMasterNode = isMasterNode;
            _message = message;
        }
    }
}
