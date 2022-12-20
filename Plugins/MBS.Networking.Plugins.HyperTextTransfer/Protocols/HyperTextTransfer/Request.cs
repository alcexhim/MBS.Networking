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

		public static Request Parse(string line)
		{
			string[] firstparts = line.Split(new char[] { ' ' }, 3);
			if (firstparts.Length != 3)
				throw new FormatException("must be three segments separated by space");

			return new Request(firstparts[0], firstparts[1], firstparts[2]);
		}
	}
}
