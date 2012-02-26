using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

using FluorineFx.AMF3;

namespace com.riotgames.platform.systemstate
{
	[DataContract]
	class ClientSystemStatesNotification : IExternalizable
	{
		[DataMember]
		public bool practiceGameEnabled;
		[DataMember]
		public bool advancedTutorialEnabled;
		[DataMember]
		public int minNumPlayersForPracticeGame;
		[DataMember]
		public List<int> practiceGameTypeConfigIdList;
		[DataMember]
		public List<int> freeToPlayChampionIdList;
		[DataMember]
		public List<int> inactiveChampionIdList;

		public void ReadExternal(IDataInput input)
		{
			string json = input.ReadUTF();
			System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ClientSystemStatesNotification));

			Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(json));

			ClientSystemStatesNotification cssn = (ClientSystemStatesNotification)ser.ReadObject(s);

			practiceGameEnabled = cssn.practiceGameEnabled;
			advancedTutorialEnabled = cssn.advancedTutorialEnabled;
			minNumPlayersForPracticeGame = cssn.minNumPlayersForPracticeGame;
			practiceGameTypeConfigIdList = cssn.practiceGameTypeConfigIdList;
			freeToPlayChampionIdList = cssn.freeToPlayChampionIdList;
			inactiveChampionIdList = cssn.inactiveChampionIdList;
		}

		public void WriteExternal(IDataOutput output)
		{
			throw new NotImplementedException();
		}
	}
}