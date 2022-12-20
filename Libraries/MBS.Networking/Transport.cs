using System;
namespace MBS.Networking
{
	public abstract class Transport
	{
		protected abstract TransportClient CreateClientInternal(Protocol protocol);
		public TransportClient CreateClient(Protocol protocol) { return CreateClientInternal(protocol); }

		protected abstract TransportServer CreateServerInternal(Protocol protocol);
		public TransportServer CreateServer(Protocol protocol) { return CreateServerInternal(protocol); }
	}
}
