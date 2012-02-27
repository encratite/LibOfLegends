using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SpellBookPage : AbstractDomainObject
	{
		public int pageId;
		public string name;
		public List<SlotEntry> slotEntries;
		public bool isCurrent;
		public DateTime createDate;
		public int summonerId;
	}
}