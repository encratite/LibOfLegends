using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SpellBook : AbstractDomainObject
	{
		public List<SpellBookPage> spellBookPages;
		public string spellBookPagesJson;
		public string dateString;
		public int summonerId;
	}
}