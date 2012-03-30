using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SpellBook : AbstractDomainObject
	{
		//Unknown type, always null
		public object bookPagesJson;
		public List<SpellBookPage> bookPages;
		public string dateString;
		public int summonerId;
	}
}