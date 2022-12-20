using System;
using MBS.Networking.Accessors;
using UniversalEditor;
using UniversalEditor.Accessors;

namespace MBS.Networking.Plugins.Chat.Protocols.IRC
{
	public enum IRCChatMessageType
	{
		Welcome = 001,
		YourHost = 002,
		Created = 003,
		MyInfo = 004,
		ISupport = 005,

		IsOn = 303,

		WhoisUser = 311,
		WhoisEnd = 318,

		ListStart = 321,
		List = 322,
		ListEnd = 323,

		NoTopic = 331,
		Topic = 332,

		NameReply = 353,
		NamesEnd = 366,

		Motd = 372,
		MotdStart = 375,
		MotdEnd = 376,

		NoSuchNick = 401,
		NoSuchChannel = 403,

		NoMotd = 422,
		NicknameAlreadyInUse = 433,

		NotOnChannel = 442,

		BadChannelKey = 475
	}
}