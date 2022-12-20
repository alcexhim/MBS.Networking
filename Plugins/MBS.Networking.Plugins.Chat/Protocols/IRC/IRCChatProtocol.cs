using System;
using System.Collections.Generic;
using System.Text;
using MBS.Networking.Accessors;
using MBS.Networking.Plugins.Chat.Services.Chat;
using UniversalEditor;
using UniversalEditor.Accessors;

namespace MBS.Networking.Plugins.Chat.Protocols.IRC
{
	public class IRCChatProtocol : Protocol
	{
		public IRCChatProtocol()
		{
		}

		public override int DefaultPort => 6667;
		public string ServerName { get; set; } = "localhost";
		public DateTime CreationDateTime { get; } = DateTime.Now;

		private void SendRaw(Client client, string raw)
		{
			MemoryAccessor acc = new MemoryAccessor();
			// acc.Writer.WriteLine(String.Format(":{0} {1} {2} :{3}", ServerName, ((int)messageType).ToString().PadLeft(3, '0'), nickName, message));

			// FIXME: why do some messages have the ':' and some don't?
			acc.Writer.WriteLine(raw);
			acc.Close();

			client.Write(acc.ToArray());
			client.Flush();
		}
		private void Send(Client client, string message)
		{
			ChatService chat = (Server.Service as ChatService);
			SendRaw(client, String.Format(":{0} {1} {2}", ServerName, chat.Users[client].Name, message));
		}
		private void Send(Client client, IRCChatMessageType messageType, string message)
		{
			ChatService chat = (Server.Service as ChatService);
			SendRaw(client, String.Format(":{0} {1} {2} {3}", ServerName, ((int)messageType).ToString().PadLeft(3, '0'), chat.Users[client].Name, message));
		}

		protected override void OnClientConnected(ClientConnectedEventArgs e)
		{
			base.OnClientConnected(e);

			// Accessor acc = new NetworkAccessor(e.Client);
			byte[] data = e.Client.ReadAvailable();
			Accessor acc = new MemoryAccessor(data);
			acc.Writer.NewLineSequence = UniversalEditor.IO.NewLineSequence.CarriageReturnLineFeed;

			string[] userInfo = null;
			string emailAddress = null;
			ChatService chat = (Server.Service as ChatService);

			while (true)
			{
				if (e.Client.Available > 0)
				{
					data = e.Client.ReadAvailable();
					acc = new MemoryAccessor(data);
				}

				string line = acc.Reader.ReadLine();
				if (String.IsNullOrEmpty(line))
				{
					System.Threading.Thread.Sleep(500);
					continue;
				}

				line = line.Trim();

				string cmd = line;
				string parms = String.Empty;

				if (line.Contains(" "))
				{
					cmd = line.Substring(0, line.IndexOf(' '));
					parms = line.Substring(line.IndexOf(' ') + 1);
				}

				string trailer = null;
				if (parms.Contains(":"))
				{
					trailer = parms.Substring(parms.IndexOf(':') + 1);
				}

				string nickName = null;
 				switch (cmd)
				{
					case "USER":
					{
						userInfo = parms.Split(new char[] { ' ' }, 4);
						userInfo[3] = trailer;
						break;
					}
					case "NICK":
 					{
						nickName = parms;
						if (userInfo != null)
						{
							chat.Users.Add(new ChatUser(e.Client, nickName, userInfo[0], userInfo[2], userInfo[3]));
						}

						Send(e.Client, IRCChatMessageType.Welcome, String.Format("Welcome to the Internet Relay Network {0}!{1}", nickName, emailAddress));
						Send(e.Client, IRCChatMessageType.YourHost, String.Format("Your host is {0}, running version {1}", ServerName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
						Send(e.Client, IRCChatMessageType.Created, String.Format("This server was created {0}", CreationDateTime));
						Send(e.Client, IRCChatMessageType.MyInfo, String.Format("a b c d e f", CreationDateTime));

						SendMotd(e.Client, chat.Description);
						break;
					}
					case "LIST":
					{
						SendList(e.Client);
						break;
					}
					case "JOIN":
					{
						JoinChannel(e.Client, parms);
						break;
					}
					case "PART":
					{
						PartChannel(e.Client, parms);
						break;
					}
					case "INVITE":
					{
						break;
					}
					case "ISON":
					{
						string[] nicks = parms.Split(new char[] { ' ' });
						List<string> isOnList = new List<string>();
						for (int i = 0; i < nicks.Length; i++)
						{
							if (chat.Users.Contains(nicks[i]))
							{
								isOnList.Add(nicks[i]);
							}
						}
						Send(e.Client, IRCChatMessageType.IsOn, String.Join(" ", isOnList.ToArray()));
						break;
					}
					case "PRIVMSG":
					{
						string recipient = parms.Substring(0, parms.IndexOf(' '));
						string message = parms.Substring(recipient.Length + 2); //extra for colon
						if (chat.Users.Contains(recipient))
						{
							SendPrivateMessage(chat.Users[e.Client], chat.Users[recipient], message);
						}
						else if (chat.Channels.Contains(recipient))
						{
							SendPrivateMessage(chat.Users[e.Client], chat.Channels[recipient], message);
						}
						break;
					}
					case "PING":
					{
						SendRaw(e.Client, String.Format("PONG {0}", ServerName));
						break;
					}
					case "WHOIS":
					{
						ChatUser user = chat.Users[parms];
						if (user == null)
						{
							Send(e.Client, IRCChatMessageType.NoSuchNick, String.Format("{0} :No such nick/channel", parms));
							break;
						}
						else
						{
							Send(e.Client, IRCChatMessageType.WhoisUser, String.Format("{0} {1} {2} * :{3}", parms, user.LocalUserName, user.LocalServer, user.RealName));
							Send(e.Client, IRCChatMessageType.WhoisEnd, String.Format("{0} :End of WHOIS list", parms));
						}
						break;
					}
					case "QUIT":
					{
						chat.Users.Remove(e.Client);

						foreach (ChatUser user in chat.Users)
						{

						}
						break;
					}
					case "TOPIC":
					{
						string channelName = parms.Substring(0, parms.IndexOf(' '));
						string description = trailer;

						if (chat.Channels[channelName] != null)
						{
							chat.Channels[channelName].Description = description;
							SendChannelTopic(e.Client, chat.Channels[channelName]);
						}
						else
						{
							Send(e.Client, IRCChatMessageType.NoSuchChannel, String.Format("{0} :No such channel", channelName));
						}
						break;
					}
				}
			}
		}

		private void SendPrivateMessage(ChatUser sender, ChatUser receiver, string message)
		{
			ChatService chat = (Server.Service as ChatService);
			SendRaw(receiver.Client, String.Format(":{0}!{1}@{2} PRIVMSG {3} :{4}", sender.Name, sender.LocalUserName, sender.LocalServer, receiver.Name, message));
		}
		private void SendPrivateMessage(ChatUser sender, ChatChannel receiver, string message)
		{
			ChatService chat = (Server.Service as ChatService);
			foreach (ChatUser user in receiver.Users)
			{
				if (user == sender) continue;
				SendRaw(user.Client, String.Format(":{0}!{1}@{2} PRIVMSG {3} :{4}", sender.Name, sender.LocalUserName, sender.LocalServer, receiver.Name, message));
			}
		}

		private void PartChannel(Client client, string channelName)
		{
			ChatService chat = (Server.Service as ChatService);
			ChatChannel chan = chat.Channels[channelName];
			if (chan == null)
			{
				Send(client, IRCChatMessageType.NoSuchChannel, String.Format("{0} :No such channel", channelName));
				return;
			}

			if (!chan.Users.Contains(client))
			{
				Send(client, IRCChatMessageType.NotOnChannel, String.Format("{0} :You're not on that channel", channelName));
				return;
			}

			chan.Users.Remove(client);
			foreach (ChatUser user in chan.Users)
			{
				SendRaw(user.Client, String.Format(":{0} PART {1} :{2}", chat.Users[client].Identifier, chan.Name, String.Empty));
			}
		}
		private void JoinChannel(Client client, string parms)
		{
			ChatService chat = (Server.Service as ChatService);

			string[] parmz = parms.Split(new char[] { ' ' }, 2);
			string name = parmz[0];
			string pass = parmz.Length > 1 ? parmz[1] : null;

			ChatChannel chan = chat.Channels[name];
			if (chan == null)
			{
				Send(client, IRCChatMessageType.NoSuchChannel, String.Format("{0} :No such channel", name));
				return;
			}

			if (chan.Password != pass)
			{
				Send(client, IRCChatMessageType.BadChannelKey, String.Format("{0} :Cannot join channel (+k)", name));
				return;
			}

			chan.Users.Add(chat.Users[client]);
			foreach (ChatUser user in chan.Users)
			{
				SendRaw(user.Client, String.Format(":{0} JOIN {1}", chat.Users[client].Identifier, chan.Name));
			}

			SendChannelTopic(client, chan);

			StringBuilder sb = new StringBuilder();
			sb.Append('=');
			sb.Append(chan.Name);
			sb.Append(' ');
			sb.Append(':');
			for (int i = 0;  i < chan.Users.Count;  i++)
			{
				//sb.Append('+');
				sb.Append(chan.Users[i].Name);
				if (i < chan.Users.Count - 1)
					sb.Append(' ');
			}

			// FIXME: this dun work
			Send(client, IRCChatMessageType.NameReply, sb.ToString());
			Send(client, IRCChatMessageType.NamesEnd, "End of NAMES list");
		}

		private void SendChannelTopic(Client client, ChatChannel chan)
		{
			if (chan.Description == null)
			{
				Send(client, IRCChatMessageType.NoTopic, String.Format("{0} :{1}", chan.Name, "No topic is set"));
			}
			else
			{
				Send(client, IRCChatMessageType.Topic, String.Format("{0} :{1}", chan.Name, chan.Description));
			}
		}

		private void SendList(Client client)
		{
			Send(client, IRCChatMessageType.ListStart, "Channel :Users Name");

			ChatService chat = (Server.Service as ChatService);
			foreach (ChatChannel chan in chat.Channels)
			{
				Send(client, IRCChatMessageType.List, String.Format("{0} {1} :{2}", chan.Name, chan.Users.Count, chan.Description));
			}
			Send(client, IRCChatMessageType.ListEnd, "End of /LIST");
		}

		private void SendMotd(Client client, string v)
		{
			if (v == null)
			{
				Send(client, IRCChatMessageType.NoMotd, ":MOTD File is missing");
			}
			else
			{
				Send(client, IRCChatMessageType.MotdStart, String.Format("{0} Message of the day - ", ServerName));
				string[] lines = v.Split(new string[] { "\r\n" });
				for (int i = 0; i < lines.Length; i++)
				{
					Send(client, IRCChatMessageType.Motd, lines[i]);
				}
				Send(client, IRCChatMessageType.MotdEnd, "End of /MOTD command.");
			}
		}
	}
}
