using System;
using MBS.Networking.Protocols.TransmissionControl;

namespace MBS.Networking.Protocols.HyperTextTransfer
{
	public class HyperTextTransferProtocol : TransmissionControlProtocol
	{
		public override int DefaultPort => 80;

		public void SendRequest(Client client, Request req)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(req.Method);
			sb.Append(' ');
			sb.Append(req.Path);
			sb.Append(' ');
			sb.Append(req.Protocol);
			sb.AppendLine();
			foreach (Header header in req.Headers)
			{
				sb.Append(header.Name);
				sb.Append(": ");
				sb.AppendLine(header.Value);
			}
			sb.AppendLine();
			sb.Append(req.Content);
			string datastr = sb.ToString();
			byte[] data = System.Text.Encoding.UTF8.GetBytes(datastr);

			client.Write(data);
			client.Flush();
		}
		public Response GetResponse(Client client)
		{
			byte[] data = client.ReadAvailable();
			if (data.Length == 0) return null;

			System.IO.MemoryStream ms = new System.IO.MemoryStream(data);
			System.IO.StreamReader reader = new System.IO.StreamReader(ms);

			string signature = reader.ReadLine();
			string[] sigparts = signature.Split(new char[] { ' ' }, 3);
			if (sigparts.Length != 3) return null;

			Response resp = new Response(Int32.Parse(sigparts[1]), sigparts[2], sigparts[0]);
			while (true)
			{
				string header = reader.ReadLine();
				if (String.IsNullOrEmpty(header)) break;

				string[] headerparts = header.Split(new char[] { ':' }, 2);
				if (headerparts.Length != 2) continue;

				resp.Headers.Add(headerparts[0].Trim(), headerparts[1].Trim());
			}

			string rest = reader.ReadToEnd();
			resp.Content = rest;
			return resp;
		}

		public void SendResponse(Client client, Response resp)
		{
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			System.IO.StreamWriter sw = new System.IO.StreamWriter(ms);

			sw.Write(resp.Protocol);
			sw.Write(' ');
			sw.Write(resp.ResponseCode.ToString());
			sw.Write(' ');
			sw.Write(resp.ResponseText);
			sw.WriteLine();

			foreach (Header header in resp.Headers)
			{
				sw.WriteLine(header.Name + ": " + header.Value);
			}
			sw.WriteLine();
			sw.Write(resp.Content);

			sw.Flush();
			sw.Close();

			byte[] data = ms.ToArray();
			client.Write(data);
			client.Flush();
		}
		public Request GetRequest(Client client)
		{
			byte[] data = client.ReadAvailable();

			System.IO.MemoryStream ms = new System.IO.MemoryStream(data);
			System.IO.StreamReader reader = new System.IO.StreamReader(ms);

			string header = reader.ReadLine();
			if (header == null) return null;

			string[] parts = header.Split(new char[] { ' ' }, 3);
			if (parts.Length != 3)
				throw new ArgumentException("request requires three parts - method (GET, PUT, OPTIONS, POST etc.), path, and protocol (HTTP/1.1 etc.)");

			Request req = new Request();
			req.Method = parts[0];
			req.Path = parts[1];
			req.Protocol = parts[2];

			while (true)
			{
				string line = reader.ReadLine();
				if (String.IsNullOrEmpty(line)) break;

				string[] lineParts = line.Split(new char[] { ':' }, 2);
				if (lineParts.Length == 2)
				{
					req.Headers.Add(lineParts[0].Trim(), lineParts[1].Trim());
				}
			}

			return req;
		}
	}
}
