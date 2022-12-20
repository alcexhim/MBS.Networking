using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking.Protocols.Voice.SessionInitiation.Requests
{
	public class SIPInviteRequest : SIPRequest
	{
		private string mvarSubject = String.Empty;
		public string Subject { get { return mvarSubject; } set { mvarSubject = value; } }

		protected override void OnPrepare()
		{
			base.OnPrepare();
			Method = "INVITE";
			Path = mvarSubject;
			Headers.Add("t", "<" + Subject + ">");
		}
	}
}
