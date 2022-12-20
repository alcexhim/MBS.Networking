using System;
namespace MBS.Networking.Plugins.Chat.Services.Chat
{
	public class ChatService : Service
	{
		public ChatChannel.ChatChannelCollection Channels { get; } = new ChatChannel.ChatChannelCollection();
		public ChatUser.ChatUserCollection Users { get; } = new ChatUser.ChatUserCollection();

		/// <summary>
		/// Gets or sets a description or "Message of the Day" (MOTD), which is usually displayed when a client logs on to the chat server.
		/// </summary>
		/// <value>The description or "Message of the Day" (MOTD).</value>
		public string Description { get; set; } = null;
	}
}
