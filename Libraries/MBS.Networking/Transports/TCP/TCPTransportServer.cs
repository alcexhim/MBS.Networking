using System;
namespace MBS.Networking.Transports.TCP
{
	public class TCPTransportServer : TransportServer
	{
		private System.Net.Sockets.TcpListener underlyingServer = null;
		public TCPTransportServer(System.Net.Sockets.TcpListener underlyingServer)
		{
			this.underlyingServer = underlyingServer;
		}

		protected override void StartInternal()
		{
			underlyingServer.Start();
		}
		protected override TransportClient AcceptClientInternal()
		{
			System.Net.Sockets.TcpClient client = this.underlyingServer.AcceptTcpClient();
			return new TCPTransportClient(client);
		}
	}
}
