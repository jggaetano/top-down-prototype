using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public enum Movement { NORTH, EAST, SOUTH, WEST, STOP, NONE };

public class Move : IXmlSerializable { 

    public Movement Direction { get; set; }
    public float Time { get; set;  }

    public Move() {
        Direction = Movement.NONE;
        Time = 0.0f;
    }

    public Move(Movement direction, float time) {
        Direction = direction;
        Time = time;
    }

    public static Movement Opposite(Movement direction) {

        switch (direction) {
            case Movement.NORTH:
                return Movement.SOUTH;
            case Movement.EAST:
                return Movement.WEST;
            case Movement.SOUTH:
                return Movement.NORTH;
            case Movement.WEST:
                return Movement.EAST;
        }

        return Movement.NONE;

    }

    public static Vector2 VectorDirection(Movement direction) {

        switch (direction)
        {
            case Movement.NORTH:
                return Vector2.up;
            case Movement.EAST:
                return Vector2.right;
            case Movement.SOUTH:
                return Vector2.down;
            case Movement.WEST:
                return Vector2.left;
        }

        return Vector2.zero;

    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Move");
        writer.WriteAttributeString("Direction", Direction.ToString());
        writer.WriteAttributeString("Time", Time.ToString());
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
        if (Enum.IsDefined(typeof(Movement), reader.GetAttribute("Direction")) == true) 
            Direction = (Movement)Enum.Parse(typeof(Movement), reader.GetAttribute("Direction"));
        else
            Debug.LogError("XML Document has bad Movement Enum");


        Time = float.Parse(reader.GetAttribute("Time"));
     
    }

}
