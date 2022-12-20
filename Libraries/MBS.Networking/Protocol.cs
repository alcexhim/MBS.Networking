using System;
namespace MBS.Networking
{
	public abstract class Protocol
	{
		public abstract int DefaultPort { get; }
		public int Port { get; set; } = -1;

		public Protocol()
		{
			Port = DefaultPort;
		}
	}
}
