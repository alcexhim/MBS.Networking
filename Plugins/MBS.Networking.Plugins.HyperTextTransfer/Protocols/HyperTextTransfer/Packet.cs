using System;
namespace MBS.Networking.Protocols.HyperTextTransfer
{
	public abstract class Packet
	{
		internal Packet(string protocol)
		{
			Protocol = protocol;
		}

		public string Protocol { get; set; } = "HTTP/1.1";
		public Header.HeaderCollection Headers { get; } = new Header.HeaderCollection();
		public string Content { get; set; } = String.Empty;
	}
}
