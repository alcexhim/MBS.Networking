using System;
namespace MBS.Networking.Protocols.HyperTextTransfer
{
	public class Header
	{
		public class HeaderCollection
			: System.Collections.ObjectModel.Collection<Header>
		{
			public void Add(string name, string value)
			{
				Header header = new Header();
				header.Name = name;
				header.Value = value;
				Add(header);
			}
		}

		public string Name { get; set; } = String.Empty;
		public string Value { get; set; } = String.Empty;
	}
}
