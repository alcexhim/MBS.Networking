using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking.Protocols.PlainText
{
	public abstract class PlainTextProtocol : Protocol
	{	
		public string CreatePacket(Packet pkt)
		{
			StringBuilder sbContent = new StringBuilder();
			if (pkt is Request)
			{
				Request req = (pkt as Request);

				sbContent.Append(req.Method.ToUpper());
				sbContent.Append(' ');
				sbContent.Append(req.Path);
				sbContent.Append(' ');
				sbContent.Append(req.Protocol.ToUpper());
				sbContent.AppendLine();
			}
			else if (pkt is Response)
			{
				Response resp = (pkt as Response);

				sbContent.Append(resp.Protocol.ToUpper());
				sbContent.Append(' ');
				sbContent.Append(resp.ResponseCode.ToString());
				sbContent.Append(' ');
				sbContent.Append(resp.ResponseText);
				sbContent.AppendLine();
			}

			foreach (Header header in pkt.Headers)
			{
				sbContent.Append(header.Name);
				sbContent.Append(':');
				sbContent.Append(' ');
				sbContent.Append(header.Value);
				sbContent.AppendLine();
			}
			sbContent.AppendLine();

			if (!String.IsNullOrEmpty(pkt.Content)) sbContent.AppendLine(pkt.Content);
			return sbContent.ToString();
		}

		public Packet ReadPacket(System.IO.Stream stream, string protocol)
		{
			byte[] block = new byte[4096];
			stream.Read(block, 0, block.Length);
			string content = System.Text.Encoding.Default.GetString(block);
			return Packet.Parse(content, protocol);
		}
	}
}
