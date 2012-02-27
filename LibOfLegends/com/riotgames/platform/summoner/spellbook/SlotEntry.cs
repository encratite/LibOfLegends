using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SlotEntry
	{
		public int dataVersion;
		public int runeId;
		public int runeSlotId;
		//Not sure about type as it is null
		public object runeSlot;
		//Not sure about type as it is null
		public object rune;
		public object futureData;
	}
}