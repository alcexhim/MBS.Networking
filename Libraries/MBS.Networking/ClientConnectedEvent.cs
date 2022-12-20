using System;
namespace MBS.Networking
{
	public class ClientConnectedEventArgs : EventArgs
	{
		public Client Client { get; private set; }
		public ClientConnectedEventArgs(Client client)
		{
			this.Client = client;
		}
	}
	public delegate void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs e);
}
