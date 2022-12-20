using System;
using MBS.Networking.Services.FileSystem;
using UniversalEditor.Accessors;

namespace MBS.Networking.Protocols.HTTP
{
	public class HTTPProtocol : PlainText.PlainTextProtocol
	{
		public override int DefaultPort => 80;

		private const string PROTOCOL_VERSION = "HTTP/1.1";

		protected override void OnClientConnected(ClientConnectedEventArgs e)
		{
			while (e.Client.Available == 0)
			{
				System.Threading.Thread.Sleep(50);
				continue;
			}

			byte[] data = e.Client.ReadAvailable();
			System.IO.MemoryStream ms = new System.IO.MemoryStream(data);

			PlainText.Request req = ReadPacket(ms, PROTOCOL_VERSION) as PlainText.Request;
			if (req != null)
			{
				if (e.Server.Service == null) return;

				File file = (e.Server.Service as FileSystemService).GetFile(req.Path);

				System.Collections.Generic.Dictionary<string, object> args = new System.Collections.Generic.Dictionary<string, object>();
				args["Request"] = req;

				byte[] rdata = file?.GetData(args);
				if (rdata == null)
				{
					if (args.ContainsKey("Response"))
					{
						PlainText.Response resp = (args["Response"] as PlainText.Response);
						if (resp != null)
						{
							string response = CreatePacket(resp);
							byte[] responseData = System.Text.Encoding.UTF8.GetBytes(response);
							e.Client.Write(responseData);
						}
						if (rdata != null)
							e.Client.Write(rdata);
					}
					else
					{
						string response = CreatePacket(new PlainText.Response(404, "Not Found", PROTOCOL_VERSION));
						byte[] responseData = System.Text.Encoding.UTF8.GetBytes(response);
						e.Client.Write(responseData);
						if (rdata != null)
							e.Client.Write(rdata);
					}
				}
				else
				{
					if (args.ContainsKey("Response"))
					{
						PlainText.Response resp = (args["Response"] as PlainText.Response);
						if (resp != null)
						{
							string response = CreatePacket(resp);
							byte[] responseData = System.Text.Encoding.UTF8.GetBytes(response);
							e.Client.Write(responseData);
						}
						e.Client.Write(rdata);
					}
					else
					{
						PlainText.Response resp = new PlainText.Response(200, "OK", PROTOCOL_VERSION);
						string response = CreatePacket(resp);
						byte[] responseData = System.Text.Encoding.UTF8.GetBytes(response);
						e.Client.Write(responseData);

						e.Client.Write(rdata);
					}
				}
				e.Client.Close();
			}
		}
	}
}
