using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.gameclient.domain
{
	public class AllPublicSummonerDataDTO
	{
		/*
		public SpellBook spellBook;
		public SummonerTalentsAndPoints summonerTalentsAndPoints;
		public SummonerLevelAndPoints summonerLevelAndPoints;
		public SummonerLevel summonerLevel;
		*/
		public BasePublicSummonerDTO summoner;
		public object spellBook;
		public SummonerDefaultSpells summonerDefaultSpells;
		public object summonerTalentsAndPoints;
		public object summonerLevelAndPoints;
		public object summonerLevel;
	}
}
