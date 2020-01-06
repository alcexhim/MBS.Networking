using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MBS.Networking.Protocols.UniversalDatagram
{
	public abstract class UniversalDatagramProtocol : Protocol
	{
		private UdpClient mvarUnderlyingClient = new UdpClient();
		public UdpClient UnderlyingClient { get { return mvarUnderlyingClient; } }

	}
}
