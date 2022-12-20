using System;
namespace MBS.Networking.Transports.TCP
{
	public class TCPTransport : Transport
	{
		protected override TransportClient CreateClientInternal(Protocol protocol)
		{
			System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
			return new TCPTransportClient(client);
		}
		protected override TransportServer CreateServerInternal(Protocol protocol)
		{
			System.Net.Sockets.TcpListener server = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, protocol.Port);
			return new TCPTransportServer(server);
		}
	}
}
