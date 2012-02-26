using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.gameclient.domain
{
	public class Spell
	{
		public string displayName;
		public bool active;
		public string name;
		public List<object> gameModes;
		public string description;
		public int spellId;
		public int minLevel;
	}
}
