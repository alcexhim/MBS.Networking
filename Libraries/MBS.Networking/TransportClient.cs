using System;
namespace MBS.Networking
{
	public abstract class TransportClient
	{
		public abstract int Available { get; }
		public abstract bool IsConnected { get; }

		protected abstract void ConnectInternal(System.Net.IPAddress addr, int port);
		public void Connect(System.Net.IPAddress addr, int port)
		{
			ConnectInternal(addr, port);
		}

		protected abstract System.IO.Stream GetStreamInternal();
		public System.IO.Stream GetStream() { return GetStreamInternal(); }

		public void Write(byte[] data)
		{
			Write(data, 0, data.Length);
		}
		public void Write(byte[] data, int offset, int length)
		{
			WriteInternal(data, offset, length);
		}
		protected abstract void WriteInternal(byte[] data, int offset, int length);

		public void Flush()
		{
			FlushInternal();
		}
		protected abstract void FlushInternal();

		public byte[] ReadAvailable()
		{
			byte[] data = new byte[Available];
			Read(data, 0, data.Length);
			return data;
		}
		public void Read(byte[] data)
		{
			Read(data, 0, data.Length);
		}
		public void Read(byte[] data, int offset, int length)
		{
			ReadInternal(data, offset, length);
		}
		protected abstract void ReadInternal(byte[] data, int offset, int length);
	}
}
