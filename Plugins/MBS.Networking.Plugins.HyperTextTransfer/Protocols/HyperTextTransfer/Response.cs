using System;
namespace MBS.Networking.Protocols.HyperTextTransfer
{
	public class Response : Packet
	{
		public int ResponseCode { get; set; } = 200;
		public string ResponseText { get; set; } = "OK";

		public string Content { get; set; } = null;

		public Response(int responseCode, string responseText, string protocol, string content = null, Header[] headers = null) : base(protocol)
		{
			ResponseCode = responseCode;
			ResponseText = responseText;
			Content = content;
			if (headers != null)
			{
				foreach (Header header in headers)
				{
					Headers.Add(header);
				}
			}
		}
	}
}
