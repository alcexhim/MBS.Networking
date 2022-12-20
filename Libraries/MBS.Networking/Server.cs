using System;
namespace MBS.Networking
{
	public class Server
	{
		public Protocol Protocol { get; set; }
		public Service Service { get; set; }
		public Transport Transport { get; set; }

		public event ClientConnectedEventHandler ClientConnected;
		protected virtual void OnClientConnected(ClientConnectedEventArgs e)
		{
			ClientConnected?.Invoke(this, e);
		}

		private TransportServer _svr = null;
		public void Start()
		{
			if (_svr != null)
			{
				// _svr.Stop();
				_svr = null;
			}

			_svr = Transport.CreateServer(Protocol);
			_svr.ClientConnected += svr_ClientConnected;
			_svr.Start();
		}

		private void svr_ClientConnected(object sender, TransportClientConnectedEventArgs e)
		{
			OnClientConnected(new ClientConnectedEventArgs(new Client(e.Client)));
		}
	}
}
