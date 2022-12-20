using System;
using System.Collections.Generic;

namespace MBS.Networking.Plugins.Chat.Services.Chat
{
	public class ChatUser
	{
		public Client Client { get; private set; } = null;
		public string Name { get; set; }
		public string RealName { get; set; }
		public string LocalUserName { get; set; }
		public string LocalServer { get; set; }
		public string Identifier { get { return String.Format("{0}!{1}@{2}", Name, LocalUserName, LocalServer); } }

		public ChatUser(Client client, string name, string localUserName, string localServer, string realName = null)
		{
			Client = client;
			Name = name;
			LocalUserName = localUserName;
			LocalServer = localServer;
			RealName = realName;
		}

		public class ChatUserCollection
			: System.Collections.ObjectModel.Collection<ChatUser>
		{
			private Dictionary<string, ChatUser> _itemsByName = new Dictionary<string, ChatUser>();
			private Dictionary<Client, ChatUser> _itemsByClient = new Dictionary<Client, ChatUser>();

			public ChatUser this[string name]
			{
				get
				{
					if (_itemsByName.ContainsKey(name))
						return _itemsByName[name];
					return null;
				}
			}
			public ChatUser this[Client client]
			{
				get
				{
					if (_itemsByClient.ContainsKey(client))
						return _itemsByClient[client];
					return null;
				}
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				_itemsByName.Clear();
				_itemsByClient.Clear();
			}
			protected override void InsertItem(int index, ChatUser item)
			{
				base.InsertItem(index, item);
				_itemsByName[item.Name] = item;
				_itemsByClient[item.Client] = item;
			}
			protected override void RemoveItem(int index)
			{
				_itemsByName.Remove(this[index].Name);
				_itemsByClient.Remove(this[index].Client);
				base.RemoveItem(index);
			}

			public bool Contains(string name)
			{
				return _itemsByName.ContainsKey(name);
			}

			public bool Contains(Client client)
			{
				return _itemsByClient.ContainsKey(client);
			}
			public bool Remove(Client client)
			{
				ChatUser item = this[client];
				if (item == null)
					return false;

				Remove(item);
				return true;
			}
		}
	}
}
