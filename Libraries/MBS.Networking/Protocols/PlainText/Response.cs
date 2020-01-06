using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UniversalEditor.Accessors;
using UniversalEditor.IO;

namespace MBS.Networking.Protocols.PlainText
{
	public class Response : Packet
	{
		private int mvarResponseCode = 0;
		public int ResponseCode { get { return mvarResponseCode; } set { mvarResponseCode = value; } }

		private string mvarResponseText = String.Empty;
		public string ResponseText { get { return mvarResponseText; } set { mvarResponseText = value; } }

		public Response()
		{

		}
		public Response(int responseCode, string responseText, string protocol)
		{
			mvarResponseCode = responseCode;
			mvarResponseText = responseText;
			base.Protocol = protocol;
		}

		public override string ToString()
		{
			return base.Protocol + " " + mvarResponseCode.ToString() + " " + mvarResponseText;
		}
	}
}
