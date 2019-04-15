using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Loot : IXmlSerializable
{

    public int Money { get; protected set; }
    public List<Item> items;

    public Loot()
    {
        items = new List<Item>();
    }

    public Loot(int money, List<Item> items)
    {
        Money = money;
        this.items = items;
    }



    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Loot");
        writer.WriteAttributeString("Money", Money.ToString());
        foreach (Item item in items)
            item.WriteXml(writer);
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
        Item item = null;

        Money = int.Parse(reader.GetAttribute("Money"));

        if (reader.IsEmptyElement)
            return;

        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                if (reader.Name == "Item")
                {
                    item = new Item();
                    item.ReadXml(reader);
                    items.Add(item);
                }
            }
            else
            {
                if (reader.Name == "Loot")
                    return;
            }
        }
        //if (reader.ReadToDescendant("Item"))
        //{
        //    do
        //    {
        //        item = new Item();
        //        item.ReadXml(reader);
        //        items.Add(item);

        //    } while (reader.ReadToNextSibling("Item"));
        //    reader.ReadEndElement();
        //}
        
    }

}
