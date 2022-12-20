using System;
using System.Collections.Generic;

namespace MBS.Networking.Plugins.Chat.Services.Chat
{
	public class ChatChannel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public ChatUser.ChatUserCollection Users { get; } = new ChatUser.ChatUserCollection();
		public string Password { get; set; } = null;

		public ChatChannel(string name, string description, string password = null)
		{
			Name = name;
			Description = description;
			Password = password;
		}

		public class ChatChannelCollection
			: System.Collections.ObjectModel.Collection<ChatChannel>
		{
			private Dictionary<string, ChatChannel> _itemsByName = new Dictionary<string, ChatChannel>();
			public ChatChannel this[string name]
			{
				get
				{
					if (_itemsByName.ContainsKey(name))
						return _itemsByName[name];
					return null;
				}
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				_itemsByName.Clear();
			}
			protected override void InsertItem(int index, ChatChannel item)
			{
				base.InsertItem(index, item);
				_itemsByName[item.Name] = item;
			}
			protected override void RemoveItem(int index)
			{
				_itemsByName.Remove(this[index].Name);
				base.RemoveItem(index);
			}

			public bool Contains(string name)
			{
				return _itemsByName.ContainsKey(name);
			}
		}
	}
}
