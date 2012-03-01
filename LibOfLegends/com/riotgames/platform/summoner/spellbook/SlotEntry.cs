using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner.spellbook
{
	public class SlotEntry : AbstractDomainObject
	{
		public int runeId;
		public int runeSlotId;
		//Not sure about type as it is null
		public object runeSlot;
		//Not sure about type as it is null
		public object rune;
	}
}