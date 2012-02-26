using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SpellBookPage
	{
		public int dataVersion;
		public int pageId;
		public string name;
		public List<SlotEntry> slotEntries;
		public bool isCurrent;
		public DateTime createDate;
		public int summonerId;
		public object futureData;
	}
}