using MBS.Networking.Protocols.PlainText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking.Protocols.Voice.SessionInitiation
{
	public class SIPRequest : Request
	{
		protected override string DefaultProtocol { get { return "SIP/2.0"; } }
	}
	public class SIPResponse : Response
	{
		protected override string DefaultProtocol { get { return "SIP/2.0"; } }
	}
}
