using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluorineFx.AMF3;
using System.IO;
using System.Runtime.Serialization;

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
            System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(BroadcastNotification));

            Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(json));

            BroadcastNotification bn = (BroadcastNotification)ser.ReadObject(s);
            broadcastMessages = bn.broadcastMessages;
        }

        public void WriteExternal(IDataOutput output)
        {
            throw new NotImplementedException();
        }
    }
}
