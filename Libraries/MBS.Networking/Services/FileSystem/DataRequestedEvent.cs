using System;
namespace MBS.Networking.Services.FileSystem
{
	public class DataRequestedEventArgs : System.ComponentModel.CancelEventArgs
	{
		public System.Collections.Generic.Dictionary<string, object> Arguments { get; private set; } = new System.Collections.Generic.Dictionary<string, object>();
		public byte[] Data { get; set; } = null;

		public DataRequestedEventArgs(System.Collections.Generic.Dictionary<string, object> arguments = null)
		{
			if (arguments != null)
				Arguments = arguments;
		}
	}
}
