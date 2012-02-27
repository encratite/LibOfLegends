using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LibOfLegends
{
	[XmlRoot("dictionary")]
	public class SerialisableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		static string
				ItemTag = "Item",
				KeyTag = "Key",
				ValueTag = "Value";

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			XmlSerializer keySerialiser = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerialiser = new XmlSerializer(typeof(TValue));

			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();

			if (wasEmpty)
				return;

			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				reader.ReadStartElement(ItemTag);

				reader.ReadStartElement(KeyTag);
				TKey key = (TKey)keySerialiser.Deserialize(reader);
				reader.ReadEndElement();

				reader.ReadStartElement(ValueTag);
				TValue value = (TValue)valueSerialiser.Deserialize(reader);
				reader.ReadEndElement();

				this.Add(key, value);

				reader.ReadEndElement();
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			XmlSerializer keySerialiser = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerialiser = new XmlSerializer(typeof(TValue));

			foreach (TKey key in this.Keys)
			{
				writer.WriteStartElement(ItemTag);

				writer.WriteStartElement(KeyTag);
				keySerialiser.Serialize(writer, key);
				writer.WriteEndElement();

				writer.WriteStartElement(ValueTag);
				TValue value = this[key];
				valueSerialiser.Serialize(writer, value);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
		}
	}
}
