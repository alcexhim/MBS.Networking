using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking.Protocols.Voice.SessionInitiation.Requests
{
	public class SIPSubscribeRequest : SIPRequest
	{
		private System.Collections.Specialized.StringCollection mvarAcceptedContentTypes = new System.Collections.Specialized.StringCollection();
		public System.Collections.Specialized.StringCollection AcceptedContentTypes { get { return mvarAcceptedContentTypes; } }

		private string mvarSubject = String.Empty;
		public string Subject { get { return mvarSubject; } set { mvarSubject = value; } }

		private string mvarContact = String.Empty;
		public string Contact { get { return mvarContact; } set { mvarContact = value; } }

		public SIPSubscribeRequest()
		{
			AcceptedContentTypes.Add("application/pidf+xml");
			AcceptedContentTypes.Add("multipart/related");
			AcceptedContentTypes.Add("application/rlmi+xml");
		}

		protected override void OnPrepare()
		{
			base.OnPrepare();
			
			Method = "SUBSCRIBE";
			Path = mvarSubject;

			Headers.Add("CSeq", "1 SUBSCRIBE");
			Headers.Add("To", "<" + mvarSubject + ">");
			foreach (string contentType in mvarAcceptedContentTypes)
			{
				Headers.Add("Accept", contentType);
			}
			if (!String.IsNullOrEmpty(mvarContact)) Headers.Add("Contact", "<" + mvarContact + ">");
			Headers.Add("Allow", "INVITE,ACK,OPTIONS,BYE,CANCEL,SUBSCRIBE,NOTIFY,REFER,MESSAGE,INFO,PING,PRACK");
			Headers.Add("Expires", "300");
			Headers.Add("Event", "presence");
			Headers.Add("Content-Length", "0");
			Headers.Add("Max-Forwards", "70");
		}

	}
}
