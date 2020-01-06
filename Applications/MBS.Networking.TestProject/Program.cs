using System;
using System.Configuration;
using System.Windows.Forms;

namespace MBS.Networking.TestProject
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Client client = new Client();
			client.Protocol = new Protocols.Voice.SessionInitiation.SessionInitiationProtocol();
			client.Service = new Services.Voice.VoiceService();

			Services.Voice.VoiceService service = (client.Service as Services.Voice.VoiceService);
			Protocols.Voice.SessionInitiation.SessionInitiationProtocol sip = (client.Protocol as Protocols.Voice.SessionInitiation.SessionInitiationProtocol);

			sip.DomainName = "asterisk.becker-family.mbn";

			sip.UserName = ConfigurationManager.AppSettings["SIP.UserName"];
			sip.Password = ConfigurationManager.AppSettings["SIP.Password"];
			
			sip.test();

			// service.Register();
			// service.Call("51200");

			Application.Run();
		}
	}
}
