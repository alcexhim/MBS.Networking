using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Networking.Protocols.PlainText
{
	public class Header
	{
		public class HeaderCollection
			: System.Collections.ObjectModel.Collection<Header>
		{
			public Header Add(string name, string value)
			{
				Header item = new Header();
				item.Name = name;
				item.Value = value;
				Add(item);
				return item;
			}

			public Header this[string name]
			{
				get
				{
					foreach (Header item in this)
					{
						if (item.Name == name) return item;
					}
					return null;
				}
			}

			public bool Contains(string name)
			{
				return (this[name] != null);
			}

			public Header Insert(int index, string name, string value)
			{
				Header item = new Header();
				item.Name = name;
				item.Value = value;
				Insert(index, item);
				return item;
			}
		}

		private string mvarName = String.Empty;
		public string Name { get { return mvarName; } set { mvarName = value; } }

		private string mvarValue = String.Empty;
		public string Value { get { return mvarValue; } set { mvarValue = value; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(mvarName);
			sb.Append(": ");
			sb.Append(mvarValue);
			return sb.ToString();
		}
	}
}
