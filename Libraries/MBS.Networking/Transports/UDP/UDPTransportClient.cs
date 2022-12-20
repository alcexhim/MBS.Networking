using System;
using System.Net;

namespace MBS.Networking.Transports.UDP
{
	public class UDPTransportClient : TransportClient
	{
		private System.Net.Sockets.UdpClient udp = new System.Net.Sockets.UdpClient();

		public override int Available => udp.Available;

		public override bool IsConnected => true;

		protected override void ConnectInternal(IPAddress addr, int port)
		{
			Port = port;
			udp.Connect(addr, port);
		}

		protected override void FlushInternal()
		{
		}

		public int Port { get; private set; } = 0;

		protected override void ReadInternal(byte[] data, int offset, int length)
		{
			IPEndPoint ep = new IPEndPoint(IPAddress.Any, Port);
			udp.Receive(ref ep);
		}

		protected override void WriteInternal(byte[] data, int offset, int length)
		{
			udp.Send(data, data.Length);
		}
		protected override void CloseInternal()
		{
			udp.Close();
		}
	}
}
