using System;
using UniversalEditor;
using UniversalEditor.IO;

namespace MBS.Networking.Accessors
{
	public class NetworkAccessor : Accessor
	{
		public Client Client { get; private set; } = null;

		public NetworkAccessor(Client client)
		{
			Client = client;
		}

		public override long Length { get => Client.Available; set => throw new NotImplementedException(); }

		public override void Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		protected override void CloseInternal()
		{
			Client.Close();
		}

		protected override long GetPosition()
		{
			return pos;
		}

		protected override Accessor GetRelativeInternal(string filename, string prefix = null)
		{
			throw new NotSupportedException();
		}

		protected override void OpenInternal()
		{
			throw new NotSupportedException();
		}

		private long pos = 0;

		protected override int ReadInternal(byte[] buffer, int start, int count)
		{
			Client.Read(buffer, start, count);
			pos += count;
			return count;
		}

		protected override int WriteInternal(byte[] buffer, int start, int count)
		{
			Client.Write(buffer, start, count);
			pos += count;
			return count;
		}
	}
}
