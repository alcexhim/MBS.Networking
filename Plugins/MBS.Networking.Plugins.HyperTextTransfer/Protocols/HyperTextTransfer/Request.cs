using System;
namespace MBS.Networking.Protocols.HyperTextTransfer
{
	public class Request : Packet
	{
		public string Method { get; set; } = "GET";
		public string Path { get; set; } = "/";

		public Request() : base(null)
		{
		}
		public Request(string method, string path, string protocol) : base(protocol)
		{
			Method = method;
			Path = path;
		}
	}
}
