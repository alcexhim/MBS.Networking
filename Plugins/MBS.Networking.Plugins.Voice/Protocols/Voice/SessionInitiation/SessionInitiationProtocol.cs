using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.NetworkInformation;

using MBS.Networking.Protocols.UniversalDatagram;
using MBS.Networking.Protocols.Voice.SessionInitiation.Requests;
using MBS.Networking.Protocols.PlainText;

namespace MBS.Networking.Protocols.Voice.SessionInitiation
{
	public class SessionInitiationProtocol : UniversalDatagramProtocol
	{
		public override int DefaultPort => 5060;

		private string mvarDomainName = String.Empty;
		public string DomainName { get { return mvarDomainName; } set { mvarDomainName = value; } }

		private string getLocalIP()
		{
			string Localip = null;
			foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
			{

				var defaultGateway = from nics in NetworkInterface.GetAllNetworkInterfaces()


									 from props in nics.GetIPProperties().GatewayAddresses
									 where nics.OperationalStatus == OperationalStatus.Up
									 select props.Address.ToString(); // this sets the default gateway in a variable

				GatewayIPAddressInformationCollection prop = netInterface.GetIPProperties().GatewayAddresses;

				if (defaultGateway.First() != null)
				{

					IPInterfaceProperties ipProps = netInterface.GetIPProperties();

					foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
					{

						if (addr.Address.ToString().Contains(defaultGateway.First().Remove(defaultGateway.First().LastIndexOf(".")))) // The IP address of the computer is always a bit equal to the default gateway except for the last group of numbers. This splits it and checks if the ip without the last group matches the default gateway
						{

							if (Localip == null) // check if the string has been changed before
							{
								Localip = addr.Address.ToString(); // put the ip address in a string that you can use.
							}
						}

					}
				}
			}
			return Localip;
		}

		private string mvarUserName = String.Empty;
		public string UserName { get { return mvarUserName; } set { mvarUserName = value; } }

		private string mvarPassword = String.Empty;
		public string Password { get { return mvarPassword; } set { mvarPassword = value; } }

		private string GenerateNonce()
		{
			Random random = new Random();
			StringBuilder sbNonce = new StringBuilder();
			
			byte[] nonce = new byte[4];
			random.NextBytes(nonce);
			
			for (int i = 0; i < nonce.Length; i++)
			{
				sbNonce.Append(nonce[i].ToString("x").ToLower().PadLeft(2, '0'));
			}
			return sbNonce.ToString();
		}

		public void test()
		{
			base.UnderlyingClient.Connect(System.Net.IPAddress.Parse("10.0.1.51"), 5060);

			SIPRequest req = new SIPRequest();
			req.Method = "REGISTER";
			req.Path = "sip:" + mvarDomainName;
			req.Headers.Add("CSeq", "164 REGISTER");
			
			string nonce = GenerateNonce();
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

			string callID = Guid.NewGuid().ToString();

			req.Headers.Add("Call-ID", callID + "@" + System.Environment.MachineName);
			req.Headers.Add("To", "<sip:" + mvarUserName + "@asterisk.becker-family.mbn>");
			req.Headers.Add("Contact", "<sip:" + mvarUserName + "@10.0.1.103:5060>;q=1, <sip:" + mvarUserName + "@192.168.56.1:5060>;q=0.667, <sip:" + mvarUserName + "@169.254.80.80:5060>;q=0.334");
			req.Headers.Add("Allow", "INVITE,ACK,OPTIONS,BYE,CANCEL,SUBSCRIBE,NOTIFY,REFER,MESSAGE,INFO,PING,PRACK");
			req.Headers.Add("Expires", "3600");
			req.Headers.Add("Content-Length", "0");
			req.Headers.Add("Max-Forwards", "70");

			Send(req);
			Packet pkt = Receive();

			if (pkt is SIPResponse)
			{
				SIPResponse resp = (pkt as SIPResponse);
				if (resp.ResponseCode == 401)
				{
					req.Headers["CSeq"].Value = "165 REGISTER";

					// unauthorized, try with digest
					string authenticate = resp.Headers["WWW-Authenticate"].Value;
					string[] authParams = authenticate.Split(new char[] { ',' });
					string nonceParam = authParams[authParams.Length - 1];
					string[] nonceParams = nonceParam.Split(new char[] { '=' });
					nonce = nonceParams[1].Substring(1, nonceParams[1].Length - 2);

					string realm = "asterisk";
					string ha1 = md5.HashString(mvarUserName + ":" + realm + ":" + mvarPassword);
					string ha2 = md5.HashString(req.Method + ":" + req.Path);
					string hash = md5.HashString(ha1 + ":" + nonce + ":" + ha2);
					req.Headers.Insert(3, "Authorization", "Digest username=\"" + mvarUserName + "\", realm=\"" + realm + "\", nonce=\"" + nonce + "\", uri=\"" + req.Path + "\", algorithm=MD5, response=\"" + hash + "\"");
					
					Send(req);
					pkt = Receive();

					if (pkt is Response)
					{

					}
					else if (pkt is Request)
					{
						Request req2 = (pkt as Request);
						if (req2.Method == "OPTIONS")
						{
							SIPResponse resp2 = new SIPResponse();
							resp2.ResponseCode = 200;
							resp2.ResponseText = "OK";
							resp2.Headers.Add("CSeq", req2.Headers["CSeq"].Value);
							resp2.Headers.Add("Call-ID", req2.Headers["Call-ID"].Value);
							Send(resp2);

							pkt = Receive();

							SIPInviteRequest invite = new SIPInviteRequest();
							invite.Headers.Add("CSeq", "1 INVITE");
							invite.Subject = "sip:51200@asterisk.becker-family.mbn";
							invite.Headers.Add("v", "SIP/2.0/UDP 10.0.1.103:5060;branch=z9hG4bK4545d10a-bb10-1910-984c-0a0027000000;rport");
							invite.Headers.Add("f", "\"Mike Becker\" <sip:" + mvarUserName + "@asterisk.becker-family.mbn>;tag=f840d10a-bb10-1910-984a-0a0027000000");
							invite.Headers.Add("i", "f840d10a-bb10-1910-984a-0a0027000000@LENOVO-PC");
							invite.Headers.Add("k", "100rel,replaces");
							invite.Headers.Add("m", "\"Mike Becker\" <sip:" + mvarUserName + "@10.0.1.103:5060>");
							invite.Headers.Add("Allow", "INVITE,ACK,OPTIONS,BYE,CANCEL,SUBSCRIBE,NOTIFY,REFER,MESSAGE,INFO,PING,PRACK");
							invite.Headers.Add("l", "864");
							invite.Headers.Add("c", "application/sdp");
							invite.Headers.Add("Max-Forwards", "70");
							
							string w = @"v=0
o=- 1443585841 1 IN IP4 10.0.1.103
s=Ekiga/4.0.2
c=IN IP4 10.0.1.103
t=0 0
m=audio 5070 RTP/AVP 93 0 8 101
a=sendrecv
a=rtpmap:93 Speex/16000/1
a=rtpmap:0 PCMU/8000/1
a=rtpmap:8 PCMA/8000/1
a=rtpmap:101 telephone-event/8000
a=fmtp:101 0-16,32,36
a=maxptime:20
m=video 5072 RTP/AVP 91 31 34 110 113 112 118
b=AS:4096
b=TIAS:4096000
a=sendrecv
a=rtpmap:91 theora/90000
a=fmtp:91 height=576;width=704
a=rtpmap:31 h261/90000
a=fmtp:31 CIF=1;QCIF=1
a=rtpmap:34 H263/90000
a=fmtp:34 F=1;CIF=1;CIF4=1;QCIF=1
a=rtpmap:110 H263-1998/90000
a=fmtp:110 D=1;F=1;I=1;J=1;CIF=1;CIF4=1;QCIF=1
a=rtpmap:113 H264/90000
a=fmtp:113 max-fs=6336;max-mbps=190080;profile-level-id=42801e
a=rtpmap:112 H264/90000
a=fmtp:112 packetization-mode=1;max-fs=6336;max-mbps=190080;profile-level-id=42801e
a=rtpmap:118 MP4V-ES/90000
a=fmtp:118 profile-level-id=5";
							invite.Content = w;
							Send(invite);

							pkt = Receive();

							SIPSubscribeRequest subscribe = new SIPSubscribeRequest();
							subscribe.Subject = "sip:4072271633@asterisk.becker-family.mbn";
							subscribe.Contact = "sip:" + mvarUserName + "@10.0.1.103:5060";
							Send(subscribe);

							pkt = Receive();
						}
					}
				}
			}
		}

		public void Send(Packet packet)
		{
			packet.Prepare();

			string localIPAddress = getLocalIP();
			packet.Headers.Add("Via", "SIP/2.0/UDP " + localIPAddress + ":" + DefaultPort);
			packet.Headers.Add("User-Agent", "Indigo/1.02");
			packet.Headers.Add("From", "<sip:" + mvarUserName + "@asterisk.becker-family.mbn>;tag=adf591b0-ba10-1910-9806-0a0027000000");
			
			StringBuilder sb = new StringBuilder();
			if (packet is Request)
			{
				Request req = (packet as Request);
				sb.AppendLine(req.Method + " " + req.Path + " SIP/2.0");
			}
			else if (packet is SIPResponse)
			{
				SIPResponse resp = (packet as SIPResponse);
				sb.AppendLine("SIP/2.0 " + resp.ResponseCode.ToString() + " " + resp.ResponseText);
			}

			foreach (Header header in packet.Headers)
			{
				sb.AppendLine(header.Name + ": " + header.Value);
			}
			sb.AppendLine();
			if (!String.IsNullOrEmpty(packet.Content))
			{
				sb.Append(packet.Content);
			}

			byte[] data = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
			base.UnderlyingClient.Send(data, data.Length);

			while (base.UnderlyingClient.Available == 0)
			{
				System.Threading.Thread.Sleep(500);
			}

			Packet packetR = Receive();
		}

		private Packet Receive()
		{
			System.Net.IPEndPoint remoteEP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("10.0.1.51"), 5060);
			byte[] data = base.UnderlyingClient.Receive(ref remoteEP);

			string dataText = System.Text.Encoding.UTF8.GetString(data);
			Packet response = Packet.Parse(dataText, "SIP/2.0");
			return response;
		}

	}
}
