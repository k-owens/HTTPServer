using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Server.core
{
    public interface IConnection
    {
        int ReceiveResponse(byte[] buffer);
        int SendReply(byte[] buffer);
    }
}
