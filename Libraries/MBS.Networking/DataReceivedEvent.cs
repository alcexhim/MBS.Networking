using System;
using System.IO;

namespace MBS.Networking
{
	public class DataReceivedEventArgs
	{
		public TransportClient Client { get; }

		public DataReceivedEventArgs(TransportClient client)
		{
			Client = client;
		}
	}
	public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
}
