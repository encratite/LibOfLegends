using System;

#pragma warning disable 0169

namespace com.riotgames.platform.gameclient.domain.broadcast.game
{
	namespace com.riotgames.platform.gameclient.domain.game
	{
		public class GameReconnectionInfo
		{
			object game;
			int reconnectDelay;
			object playerCredentials;
		}
	}
}
