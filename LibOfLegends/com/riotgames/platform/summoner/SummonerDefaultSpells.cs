using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner
{
	public class SummonerDefaultSpells : AbstractDomainObject
	{
		public string summonerDefaultSpellsJson;
		//This is still broken, it doesn't get mapped properly and results in an empty dictionary unless the object type is used
		public Dictionary<string, SummonerGameModeSpells> summonerDefaultSpellMap;
		//public Dictionary<string, object> summonerDefaultSpellMap;
		public int summonerId;
	}
}
