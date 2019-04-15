using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Hero : IXmlSerializable {

    Area area
    {
        get { return AreaController.Instance.area; }
    }

    public string Name { get; protected set; }
    public string Sprite { get; protected set; }
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public Vector2 Position
    {
        get { return new Vector2(X, Y); }
        protected set { }
    }
    public Vector2 Direction {

        get {
            switch (currentMovement) {
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
        protected set { }
    }

    float movementPercentage;
    float speed = 0.25f;	
    float distThisFrame;

    Action<Hero> cbHeroChanged;

    public Movement currentMovement;
    public Movement facing;
  
    public Hero() {
        currentMovement = Movement.NONE;
    }

    public Hero(string name, string sprite, float x, float y) {
        currentMovement = Movement.NONE;
        facing = Movement.SOUTH;
        Name = name;
        Sprite = sprite;
        X = x;
		Y = y;
    }

    protected Hero(Hero other, float x, float y) {
        currentMovement = Movement.NONE;
        facing = other.facing;
        Name = other.Name;
        Sprite = other.Sprite;
		X = x;
		Y = y;
    }

    virtual public Hero Clone(float x = 0, float y = 0) {
        return new Hero(this, x, y);
    }

    public void Update(float deltaTime) {

        distThisFrame = deltaTime / speed;

        switch (currentMovement)
        {
            case Movement.NORTH:
                Y += distThisFrame;
                break;
            case Movement.EAST:
                X += distThisFrame;
                break;
            case Movement.SOUTH:
                Y -= distThisFrame;
                break;
            case Movement.WEST:
                X -= distThisFrame;
                break;

        }

        if (cbHeroChanged != null)
            cbHeroChanged(this);

    }

    public Vector2 GetNextPosition()
    {

        float distThisFrame = 1.0f / speed;
        float x = 0, y = 0;

        switch (currentMovement)
        {
            case Movement.NORTH:
                y = Y + distThisFrame;
                break;
            case Movement.EAST:
                x = X + distThisFrame;
                break;
            case Movement.SOUTH:
                y = Y + -distThisFrame;
                break;
            case Movement.WEST:
                x = X + -distThisFrame;
                break;

        }

        return new Vector2(x, y);

    }

    public void Move(Tile tile) {
        Move(tile.X, tile.Y);
    }

    public void Move(int x, int y)
    {
        X = x;
		Y = y + 0.5f;
    }

    public void Move(float x, float y) {
        X = x;
        Y = y; 
    }

    public void RegisterOnChangedCallback(Action<Hero> cb)
    {
        cbHeroChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Hero> cb)
    {
        cbHeroChanged -= cb;
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Hero");
        writer.WriteAttributeString("Name", Name);
        writer.WriteAttributeString("Sprite", Sprite);
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
        Name = reader.GetAttribute("Name");
        Sprite = reader.GetAttribute("Sprite");
        X = int.Parse(reader.GetAttribute("X"));
        Y = int.Parse(reader.GetAttribute("Y"));

    }


}
