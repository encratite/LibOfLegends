using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.team
{
	public class CreatedTeam : AbstractDomainObject
	{
		//originally a double
		public long timeStamp;
		public TeamId teamId;
	}
}