using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking.Services.Voice
{
	public class VoiceService : Service
	{
		public void Call(string phoneNumber)
		{
			// if (!phoneNumber.Contains("@")) phoneNumber += "@" + mvarDomainName;
			// SendCommand("INVITE sip:" + phoneNumber);
		}
	}
}
