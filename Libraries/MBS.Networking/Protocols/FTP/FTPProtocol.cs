using System;
using MBS.Networking.Services.FileSystem;
using UniversalEditor.Accessors;

namespace MBS.Networking.Protocols.HTTP
{
	public class FTPProtocol : PlainText.PlainTextProtocol
	{
		public override int DefaultPort => 22;

		protected override void OnClientConnected(ClientConnectedEventArgs e)
		{
			MemoryAccessor maIn = new MemoryAccessor();
			maIn.Writer.WriteLine("220 ftp.tet.com Alterian ready.");
			e.Client.Write(maIn.ToArray());

			while (e.Client.Available == 0)
			{
				System.Threading.Thread.Sleep(50);
				continue;
			}

			while (true)
			{
				byte[] data = e.Client.ReadAvailable();
				MemoryAccessor maOut = new MemoryAccessor(data);

				string line = maOut.Reader.ReadLine();
				string userName = null, passWord = null;
				if (line.StartsWith("USER "))
				{
					userName = line.Substring("USER ".Length);

					maIn = new MemoryAccessor();
					maIn.Writer.WriteLine(String.Format("331 User {0} okay, need password", userName));
					e.Client.Write(maIn.ToArray());
				}
				else if (line.StartsWith("PASS "))
				{
					passWord = line.Substring("PASS ".Length);

					maIn = new MemoryAccessor();
					maIn.Writer.WriteLine("230 Logged in.");
					e.Client.Write(maIn.ToArray());
				}
				else if (line.StartsWith("SYST "))
				{
					passWord = line.Substring("SYST ".Length);

					maIn = new MemoryAccessor();
					maIn.Writer.WriteLine("215 UNIX Type: L8");
					e.Client.Write(maIn.ToArray());
				}
			}
		}
	}
}
