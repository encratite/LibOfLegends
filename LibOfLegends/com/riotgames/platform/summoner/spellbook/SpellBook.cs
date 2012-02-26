using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SpellBook
	{
		public int dataVersion;
		public List<SpellBookPage> spellBookPages;
		public string spellBookPagesJson;
		public string dateString;
		public int summonerId;
		public object futureData;
	}
}