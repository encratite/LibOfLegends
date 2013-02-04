using System.Collections.Generic;

namespace com.riotgames.leagues.pojo
{
	public class LeagueListDTO
	{
		public string queue;
		public string name;
		public string tier;
		public string requestorsRank;
		public List<LeagueItemDTO> entries;
		public string requestorsName;
	}
}
