using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.Accessors;
using UniversalEditor.IO;

namespace MBS.Networking.Protocols.PlainText
{
	public abstract class Packet
	{
		/// <summary>
		/// Determines the default protocol name for use with this <see cref="Packet" />.
		/// </summary>
		protected virtual string DefaultProtocol { get { return String.Empty; } }

		public Packet()
		{
			mvarProtocol = DefaultProtocol;
		}

		private string mvarProtocol = String.Empty;
		/// <summary>
		/// Gets or sets the protocol name used in this <see cref="Packet" />.
		/// </summary>
		public string Protocol { get { return mvarProtocol; } set { mvarProtocol = value; } }

		private Header.HeaderCollection mvarHeaders = new Header.HeaderCollection();
		public Header.HeaderCollection Headers { get { return mvarHeaders; } }

		protected virtual void OnPrepare()
		{

		}
		public void Prepare()
		{
			OnPrepare();
		}

		public static Packet Parse(string value, string protocol)
		{
			Packet packet = null;
			Reader reader = new Reader(new MemoryAccessor(System.Text.Encoding.UTF8.GetBytes(value)));

			string firstLine = reader.ReadLine();
			if (firstLine.ToUpper().StartsWith(protocol.ToUpper() + " "))
			{
				Response response = new Response();

				string[] firstLineParts = firstLine.Split(new char[] { ' ' }, 3);
				if (firstLineParts.Length < 2) throw new FormatException("First line must have at least 2 parts");

				response.Protocol = firstLineParts[0];
				response.ResponseCode = Int32.Parse(firstLineParts[1]);
				if (firstLineParts.Length > 2) response.ResponseText = firstLineParts[2];

				packet = response;
			}
			else if (firstLine.ToUpper().EndsWith(" " + protocol.ToUpper()))
			{
				string[] firstLineParts = firstLine.Split(new char[] { ' ' });

				Request req = new Request();
				req.Method = firstLineParts[0];
				req.Path = firstLineParts[1];
				req.Protocol = firstLineParts[2];
				packet = req;
			}
			if (packet != null)
			{
				bool readingHeaders = true;

				StringBuilder sbLines = new StringBuilder ();

				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					if (line.Contains(":"))
					{
						string[] lineParts = line.Split(new char[] { ':' }, 2);
						if (lineParts.Length == 2)
						{
							string headerName = lineParts[0];
							string headerValue = lineParts[1];
							packet.Headers.Add(headerName.Trim(), headerValue.Trim());
						}
						continue;
					} else if (String.IsNullOrEmpty (line)) {
						if (!readingHeaders) {
							break;
						}
						readingHeaders = false;
						continue;
					}

					sbLines.AppendLine (line);
				}
				packet.Content = sbLines.ToString ();
			}
			return packet;
		}

		private string mvarContent = String.Empty;
		public string Content { get { return mvarContent; } set { mvarContent = value; } }
	}
}
