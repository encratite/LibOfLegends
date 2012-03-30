using System;
using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SpellBookPage : AbstractDomainObject
	{
		public int pageId;
		public string name;
		public List<SlotEntry> slotEntries;
		public bool current;
		public DateTime createDate;
		public int summonerId;
	}
}