using UnityEngine;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Container : Fixture, IXmlSerializable
{

    Area area { get { return AreaController.Instance.area; } }

    Loot loot;
    
    Action<Container> cbContainerChanged;

    public Container() : base()
    {
    }

    protected Container(Container other) : base(other)
    {
        loot = other.loot;
    }

    public override Fixture Clone()
    {
        return new Container(this);
    }

    public override void Interact()
    {

        // Check for Loot and add to party.
        if (loot != null)
        {
            Party.AddMoney(loot.Money);
            Party.AddItems(loot.items);

            // Loot is gone, set to null.
            loot = null;
        }

        base.Interact();

        if (cbContainerChanged != null)
            cbContainerChanged(this);
    }

    public void RegisterOnChangedCallback(Action<Container> cb)
    {
        cbContainerChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Container> cb)
    {
        cbContainerChanged -= cb;
    }


    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Container");
        writer.WriteAttributeString("Sprite", Sprite);
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        writer.WriteAttributeString("State", State);
        if (loot != null)
            loot.WriteXml(writer);
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
        Sprite = reader.GetAttribute("Sprite");
        X = int.Parse(reader.GetAttribute("X"));
        Y = int.Parse(reader.GetAttribute("Y"));
        State = reader.GetAttribute("State");

        if (reader.IsEmptyElement)
            return;

        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                if (reader.Name == "Loot")
                {
                    loot = new Loot();
                    loot.ReadXml(reader);
                }
            }
            else {
                if (reader.Name == "Container")
                    return;
            }
        }
    }
}
