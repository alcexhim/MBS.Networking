using System;
namespace MBS.Networking.Transports.UDP
{
	public class UDPTransport : Transport
	{
		protected override TransportClient CreateClientInternal(Protocol protocol)
		{
			return new UDPTransportClient();
		}

		protected override TransportServer CreateServerInternal(Protocol protocol)
		{
			return new UDPTransportServer();
		}
	}
}
