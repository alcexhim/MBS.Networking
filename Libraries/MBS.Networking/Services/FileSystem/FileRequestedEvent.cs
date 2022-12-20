using System;
using System.ComponentModel;

namespace MBS.Networking.Services.FileSystem
{
	public class FileRequestedEventArgs : CancelEventArgs
	{
		public File File { get; set; } = null;
		public string Path { get; set; } = null;

		public FileRequestedEventArgs(File file, string path)
		{
			File = file;
			Path = path;
		}
	}
}
