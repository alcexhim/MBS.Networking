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

		private System.Threading.Thread tServer = null;
		private void tServer_ThreadStart()
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

		private TransportServer _svr = null;
		public void Start()
		{
			tServer = new System.Threading.Thread(tServer_ThreadStart);
			tServer.Start();
		}

		public void Stop()
		{
			if (tServer != null)
			{
				tServer.Abort();
				tServer = null;
			}

			if (_svr != null)
			{
				_svr.Stop();
			}
			_svr = null;
		}

		private void svr_ClientConnected(object sender, TransportClientConnectedEventArgs e)
		{
			OnClientConnected(new ClientConnectedEventArgs(new Client(e.Client)));
		}
	}
}
