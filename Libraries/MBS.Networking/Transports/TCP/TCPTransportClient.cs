using System;
using System.Net;

namespace MBS.Networking.Transports.TCP
{
	public class TCPTransportClient : TransportClient
	{
		private System.Net.Sockets.TcpClient underlyingClient = null;
		internal TCPTransportClient(System.Net.Sockets.TcpClient underlyingClient)
		{
			this.underlyingClient = underlyingClient;
		}

		public override int Available => (underlyingClient != null ? underlyingClient.Available : 0);

		public override bool IsConnected
		{
			get
			{
				bool maybeConnected = (underlyingClient != null && underlyingClient.Connected);
				return maybeConnected;

				if (!maybeConnected) return false;

				// this doesn't REALLY work
				if ((underlyingClient.Client.Poll(0, System.Net.Sockets.SelectMode.SelectWrite)) && (!underlyingClient.Client.Poll(0, System.Net.Sockets.SelectMode.SelectError)))
				{
					byte[] buff = new byte[1];
					if (underlyingClient.Client.Receive(buff, System.Net.Sockets.SocketFlags.Peek) == 0)
						return false;
				}

				return maybeConnected;
			}
		}
		protected override void ConnectInternal(IPAddress addr, int port)
		{
			underlyingClient.Connect(addr, port);
		}

		protected override void ReadInternal(byte[] data, int offset, int length)
		{
			this.underlyingClient.GetStream().Read(data, offset, length);
		}
		protected override void WriteInternal(byte[] data, int offset, int length)
		{
			this.underlyingClient.GetStream().Write(data, offset, length);
		}
		protected override void FlushInternal()
		{
			this.underlyingClient.GetStream().Flush();
		}
	}
}
