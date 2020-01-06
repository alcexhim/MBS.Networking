using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking.Protocols.PlainText
{
	public class Request : Packet
	{
		private string mvarMethod = String.Empty;
		public string Method { get { return mvarMethod; } set { mvarMethod = value; } }

		private string mvarPath = String.Empty;
		public string Path { get { return mvarPath; } set { mvarPath = value; } }

		public override string ToString()
		{
			return mvarMethod + " " + mvarPath + " " + base.Protocol;
		}
	}
}
