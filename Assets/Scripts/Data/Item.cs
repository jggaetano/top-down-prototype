using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Item : IXmlSerializable {

    public string Type { get; protected set; }
    public int Amount { get; set; }

    public Item() { }

    public Item(string type)
    {
        Type = type;
        Amount = 1;
    }

    public Item(string type, int amount)
    {
        Type = type;
        Amount = amount;
    }


    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Item");
        writer.WriteAttributeString("Type", Type);
        writer.WriteAttributeString("Amount", Amount.ToString());
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
        Type   = reader.GetAttribute("Type");
        Amount = int.Parse(reader.GetAttribute("Amount"));
    }
}
