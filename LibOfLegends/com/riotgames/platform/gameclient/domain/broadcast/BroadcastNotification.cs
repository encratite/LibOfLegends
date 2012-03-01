using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using FluorineFx.AMF3;

namespace com.riotgames.platform.gameclient.domain.broadcast
{
	[DataContract]
	public class BroadcastNotification : IExternalizable
	{
		[DataMember]
		public ArrayCollection broadcastMessages;

		public void ReadExternal(IDataInput input)
		{
			string json = input.ReadUTF();
			DataContractJsonSerializer serialiser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(BroadcastNotification));

			Stream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(json));

			BroadcastNotification notification = (BroadcastNotification)serialiser.ReadObject(stream);
			broadcastMessages = notification.broadcastMessages;
		}

		public void WriteExternal(IDataOutput output)
		{
			throw new NotImplementedException();
		}
	}
}
