using System;
namespace MBS.Networking
{
	public class Client
	{
		// these three map to the Universal Editor paradigm of
		public Service Service { get; set; }		// ObjectModel,
		public Protocol Protocol { get; set; }		// DataFormat, and
		public Transport Transport { get; set; }	// Accessor			, respectively.

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:MBS.Networking.Client"/> is connected.
		/// </summary>
		/// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
		public bool IsConnected { get { return (client != null && client.IsConnected); } }

		public int Available => client == null ? 0 : client.Available;

		public Client()
		{
		}
		internal Client(TransportClient client)
		{
			this.client = client;
		}

		private TransportClient client = null;

		public void Read(byte[] data)
		{
			Read(data, 0, data.Length);
		}
		public void Read(byte[] data, int offset, int length)
		{
			if (client != null) client.Read(data, offset, length);
		}
		public byte[] ReadAvailable()
		{
			byte[] data = new byte[Available];
			Read(data, 0, data.Length);
			return data;
		}
		public void Write(string data)
		{
			Write(System.Text.Encoding.UTF8.GetBytes(data));
		}
		public void Write(byte[] data)
		{
			Write(data, 0, data.Length);
		}
		public void Write(byte[] data, int offset, int length)
		{
			if (client != null) client.Write(data, offset, length);
		}
		public void Flush()
		{
			if (client != null) client.Flush();
		}

		/// <summary>
		/// Connects to the configured host
		/// </summary>
		public void Connect(System.Net.IPAddress addr, int port)
		{
			if (client == null)
			{
				t = new System.Threading.Thread(t_ParameterizedThreadStart);
				t.Start(new object[] { addr, port });
			}
		}

		private System.Threading.Thread t = null;
		private void t_ParameterizedThreadStart(object parm)
		{
			object[] parms = (object[])parm;
			client = Transport.CreateClient(Protocol);
			
			System.Net.IPAddress addr = (System.Net.IPAddress)parms[0];
			int port = (int)parms[1];
			client.Connect(addr, port);

			while (true)
			{
				if (client.Available > 0)
				{
					OnDataReceived(new DataReceivedEventArgs(client));
				}
				System.Threading.Thread.Sleep(50);
			}
		}


		public bool WaitForData(int timeout = 10000)
		{
			int tt = 100;
			int ctimeout = 0;
			while (client.Available == 0)
			{
				System.Threading.Thread.Sleep(tt);
				ctimeout += tt;
				if (timeout > -1 && ctimeout > timeout)
					return false;
			}
			return true;
		}

		public event DataReceivedEventHandler DataReceived;
		protected virtual void OnDataReceived(DataReceivedEventArgs e)
		{
			DataReceived?.Invoke(this, e);
		}
	}
}
