using System;
namespace MBS.Networking
{
	public class TransportClientConnectedEventArgs
	{
		public TransportClient Client { get; private set; } = null;

		public TransportClientConnectedEventArgs(TransportClient client)
		{
			Client = client;
		}
	}
	public delegate void TransportClientConnectedEventHandler(object sender, TransportClientConnectedEventArgs e);
}
