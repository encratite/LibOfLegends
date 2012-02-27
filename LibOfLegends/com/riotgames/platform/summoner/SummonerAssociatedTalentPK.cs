using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner
{
	public class SummonerAssociatedTalentPK : AbstractDomainObject
	{
		public int sumId;
		public int tltId;
	}
}