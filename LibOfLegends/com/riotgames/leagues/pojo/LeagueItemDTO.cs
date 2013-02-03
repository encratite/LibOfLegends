namespace com.riotgames.leagues.pojo
{
	public class LeagueItemDTO
	{
		public int previousDayLeaguePosition;
		public bool hotStreak;
		// Only non-null for people who are playing their advancement series
		public MiniSeriesDTO miniSeries;
		public bool freshBlood;
		public string tier;
		// Not sure about this one, possibly a date sometimes
		public long lastPlayed;
		public string playerOrTeamId;
		public int leaguePoints;
		public bool inactive;
		public string rank;
		public bool veteran;
		public string queueType;
		public int losses;
		public string playerOrTeamName;
		public int wins;
	}
}
