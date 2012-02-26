using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.summoner;

namespace com.riotgames.platform.gameclient.domain
{
	public class SummonerDefaultSpells
	{
		public int dataVersion;
		public string summonerDefaultSpellsJson;
		public Dictionary<string, SummonerGameModeSpells> summonerDefaultSpellMap;
		//public Dictionary<string, object> summonerDefaultSpellMap;
		public int summonerId;
		public object futureData;
	}
}
