using System;
using System.Collections.Generic;

namespace MBS.Networking.Services.FileSystem
{
	public class File
	{
		public event EventHandler<DataRequestedEventArgs> DataRequested;

		private byte[] _Data = null;
		public byte[] Data
		{
			get
			{
				if (DataRequested != null)
				{
					DataRequestedEventArgs e = new DataRequestedEventArgs();
					DataRequested(this, e);
					if (!e.Cancel)
					{
						return e.Data;
					}
				}
				return _Data;
			}
			set
			{
				_Data = value;
			}
		}

		public byte[] GetData(Dictionary<string, object> arguments)
		{
			if (DataRequested != null)
			{
				DataRequestedEventArgs e = new DataRequestedEventArgs(arguments);
				DataRequested(this, e);
				if (!e.Cancel)
				{
					return e.Data;
				}
			}
			return _Data;
		}

		public string[] PathParts { get; set; } = null;
		public string Path
		{
			get { if (PathParts != null) return String.Join("/", PathParts); return null; }
			set { PathParts = value.Split(new char[] { '/' }); }
		}

		public string Name
		{
			get { if (PathParts == null) return null; return PathParts[PathParts.Length - 1]; }
			set { PathParts[PathParts.Length - 1] = value; }
		}

		public class FileCollection
			: System.Collections.ObjectModel.Collection<File>
		{

			private Dictionary<string, File> _itemsByPath = new Dictionary<string, File>();

			public File this[string path]
			{
				get
				{
					if (_itemsByPath.ContainsKey(path))
						return _itemsByPath[path];
					return null;
				}
			}

			public File Add(string filename, byte[] content)
			{
				File file = new File();
				file.Path = filename;
				file.Data = content;
				Add(file);
				return file;
			}
			public File Add(string filename, string content)
			{
				byte[] contentData = System.Text.Encoding.UTF8.GetBytes(content);
				return Add(filename, contentData);
			}
			public File Add(string filename, EventHandler<DataRequestedEventArgs> content)
			{
				File file = new File();
				file.Path = filename;
				file.DataRequested += content;
				Add(file);
				return file;
			}
			public File Add(string filename, Uri sourceUri)
			{
				File file = new File();
				file.Path = filename;
				file.DataRequested += delegate (object sender, DataRequestedEventArgs e)
				{
					if (sourceUri.Scheme == "file")
					{
						string fn = sourceUri.AbsolutePath;
						e.Data = System.IO.File.ReadAllBytes(fn);
						return;
					}
					throw new NotSupportedException();
				};
				Add(file);
				return file;
			}

			protected override void InsertItem(int index, File item)
			{
				base.InsertItem(index, item);
				_itemsByPath[item.Path] = item;
			}
			protected override void RemoveItem(int index)
			{
				_itemsByPath.Remove(this[index].Path);
				base.RemoveItem(index);
			}
		}
		public File()
		{
		}
		public File(string path, byte[] content)
		{
			Path = path;
			Data = content;
		}
		public File(string path, string content)
		{
			Path = path;
			Data = System.Text.Encoding.UTF8.GetBytes(content);
		}
	}
}
