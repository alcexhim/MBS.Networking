//
//  DaemonApplication.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2021 Mike Becker's Software
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using MBS.Framework;
using UniversalEditor.Accessors;

namespace MBS.Networking
{
	public abstract class NetworkApplication : Application
	{
		public Server.ServerCollection Servers { get; } = new Server.ServerCollection();

		protected override int StartInternal()
		{
			bool running = true;

			foreach (Server svr in Servers)
			{
				svr.Start();
			}

			while (running)
			{
			}

			foreach (Server svr in Servers)
			{
				svr.Stop();
			}
			return 0;
		}

		/*
		private void __OnClientConnected(System.Net.Sockets.TcpClient client)
		{
			ClientConnectingEventArgs ee = new ClientConnectingEventArgs(client);
			OnClientConnecting(ee);

			if (ee.Cancel)
			{
				client.Close();
				return;
			}

			StreamAccessor sa = new StreamAccessor(client.GetStream());

			Dictionary<string, string> headers = new Dictionary<string, string>();

			string head1 = sa.Reader.ReadLine();
			string[] head1parts = head1.Split(new char[] { ' ' });

			string method = head1parts[0];
			string uri = head1parts[1];
			string ver = head1parts[2];

			string line = String.Empty;
			do
			{
				line = sa.Reader.ReadLine();

				if (line != String.Empty)
				{
					string[] parts = line.Split(new char[] { ':' });
					headers[parts[0].Trim()] = parts[1].Trim();
				}
			}
			while (line != String.Empty);

			WebResponse resp = new WebResponse();
			OnClientConnected(new ClientConnectedEventArgs(client, new WebRequest(method, uri, ver, headers), resp));

			MemoryAccessor ma = new MemoryAccessor();
			ma.Writer.WriteFixedLengthString(String.Format("HTTP/1.1 {0} {1}\r\n", resp.ResponseCode, resp.ResponseText ?? GetDefaultResponseText(resp.ResponseCode)));
			foreach (KeyValuePair<string, string> kvp in resp.Headers)
			{
				ma.Writer.WriteLine(String.Format("{0}: {1}", kvp.Key, kvp.Value));
			}
			ma.Writer.WriteLine("\r\n");

			byte[] data = resp.ma.ToArray();
			ma.Writer.WriteBytes(data);

			ma.Flush();
			ma.Close();

			byte[] payload = ma.ToArray();
			client.GetStream().Write(payload, 0, payload.Length);
			client.GetStream().Close();
		}

		protected virtual string GetDefaultResponseText(int responseCode)
		{
			switch (responseCode)
			{
				case 200: return "OK";
				case 404: return "Not Found";
			}
			return String.Empty;
		}

		public event EventHandler<ClientConnectingEventArgs> ClientConnecting;
		protected virtual void OnClientConnecting(ClientConnectingEventArgs e)
		{
			ClientConnecting?.Invoke(this, e);
		}
		public event EventHandler<ClientConnectedEventArgs> ClientConnected;
		protected virtual void OnClientConnected(ClientConnectedEventArgs e)
		{
			ClientConnected?.Invoke(this, e);
		}
		*/
	}
}
