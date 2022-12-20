using System;
namespace MBS.Networking.Services.FileSystem
{
	public class FileSystemService : Service
	{
		public event EventHandler<FileRequestedEventArgs> FileRequested;
		protected virtual void OnFileRequested(FileRequestedEventArgs e)
		{
			FileRequested?.Invoke(this, e);
		}

		public File.FileCollection Files { get; } = new File.FileCollection();

		public System.Collections.Specialized.StringCollection IndexFileNames { get; } = new System.Collections.Specialized.StringCollection();

		public FileSystemService()
		{
			IndexFileNames.Add("index.html");
			IndexFileNames.Add("default.html");
		}

		public File GetFile(string path)
		{
			File file = Files[path];

			FileRequestedEventArgs ee = new FileRequestedEventArgs(file, path);
			OnFileRequested(ee);

			if (ee.Cancel)
				return null;

			file = ee.File;

			if (file == null)
			{
				if (path.EndsWith("/"))
				{
					for (int i = 0; i < IndexFileNames.Count; i++)
					{
						File tryFile = GetFile(path + IndexFileNames[i]);
						if (tryFile != null)
							return tryFile;
					}
				}
			}

			if (file != null)
				return file;

			return null;
		}
	}
}
