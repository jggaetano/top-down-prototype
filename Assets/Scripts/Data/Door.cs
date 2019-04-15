using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public enum DoorTypes { SINGLE, DOUBLE, EXIT };
public enum DoorAction { SWINGING, SLIDING, NONE };

public class Door : Fixture, IXmlSerializable {

	public int ID { get; protected set; }
	public DoorTypes Type { get; protected set; }
    public DoorAction Action { get; protected set; }
    public string LinkToArea { get; protected set; }
    public int LinkToDoor { get; protected set; }
    public float ExitX { get; protected set; }
    public float ExitY { get; protected set; }
    public bool Sensor { get; protected set; }

    // Traits for Sliding Doors
    public float LeftOpenness { get; protected set; }
    public float RightOpenness { get; protected set; }

    // Traits for Door Sensors 
    //      false = not tripped
    //      true  = tripped
    public bool SensorState { get; set; }

    // Control Time
    float speed = 1f;
    float openDuration = 2.0f;
    float openTimer = 0f;
    
    Action<Door> cbDoorChanged;

    public Door() : base() {
        SensorState = false;
    }

    public Door(string sprite, int x, int y, string state, int id, DoorTypes type, string linktoarea, int linktodoor, int exitx, int exity, bool sensor) : base(sprite, x, y, state) {
        ID = id;
        Type = type;
        LinkToArea = linktoarea;
        LinkToDoor = linktodoor;
        ExitX = exitx;
        ExitY = exity;
        Sensor = sensor;
        SensorState = false;
    }

    protected Door(Door other) : base(other)
    {
        ID         = other.ID;
        Type       = other.Type;
        LinkToArea = other.LinkToArea;
        LinkToDoor = other.LinkToDoor;
        ExitX      = other.ExitX;
        ExitY      = other.ExitY;
        Sensor     = other.Sensor;
    }

    public override Fixture Clone() {

        return new Door(this);
    }

    public void Update(float deltaTime)
    {
        switch (Action)
        {
            case DoorAction.SWINGING:
                AutomaticallyCloseDoor(deltaTime);
                break;
            case DoorAction.SLIDING:
                AutomaticSlidingDoor(deltaTime);
                break;
        }
    }

    void AutomaticallyCloseDoor(float deltaTime) {
        if (base.State == "Closed")
        {
            openTimer = openDuration;
        }
        if (base.State == "Open" && SensorState == false)
            openTimer -= deltaTime;
        if (openTimer <= 0)
            Close();
    }

    void AutomaticSlidingDoor(float deltaTime) {

        if (base.State == "Closed")
        {
            openTimer = openDuration;
            return;
        }

        if (base.State == "Opening")
        {
            LeftOpenness -= deltaTime / speed;
            RightOpenness += deltaTime / speed;
        }
        if (base.State == "Closing")
        {
            LeftOpenness += deltaTime / speed;
            RightOpenness -= deltaTime / speed;
        }
        LeftOpenness = Mathf.Clamp(LeftOpenness, -1.0f, 0f);
        RightOpenness = Mathf.Clamp(RightOpenness, 0f, 1f);
  
        if (base.State == "Open" && SensorState == false) 
        {
            openTimer -= deltaTime;
            if (openTimer <= 0)
                base.State = "Closing";
        }
        else
        {
            if (LeftOpenness >= 0)
                base.State = "Closed";
            if (LeftOpenness <= -1)
                base.State = "Open";
        }

        if (cbDoorChanged != null)
            cbDoorChanged(this);

    }

	public override void Interact() {

        switch (Action)
        {
            case DoorAction.SWINGING:
                base.Interact();
                break;
            case DoorAction.SLIDING:
                base.State = "Opening";
                break;
            case DoorAction.NONE:
                base.Interact();
                break;
        }

        if (cbDoorChanged != null)
            cbDoorChanged(this);

    }
    
    public override void Open()
    {
        base.Open();
        if (cbDoorChanged != null)
            cbDoorChanged(this);
    }

    public override void Close()
    {
        base.Close();
        if (cbDoorChanged != null)
            cbDoorChanged(this);
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void RegisterOnChangedCallback(Action<Door> cb)
    {
        cbDoorChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Door> cb)
    {
        cbDoorChanged -= cb;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Door");
		writer.WriteAttributeString("ID", ID.ToString ());
		writer.WriteAttributeString("Type", Type.ToString ());
        writer.WriteAttributeString("Action", Action.ToString());
        writer.WriteAttributeString("Sprite", Sprite);
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        writer.WriteAttributeString("LinkToArea", LinkToArea);
        writer.WriteAttributeString("LinkToDoor", LinkToDoor.ToString());
        if (State != null)
            writer.WriteAttributeString("State", State);
        writer.WriteAttributeString("ExitX", ExitX.ToString());
        writer.WriteAttributeString("ExitY", ExitY.ToString());
        writer.WriteAttributeString("Sensor", Sensor.ToString());
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
		ID = int.Parse(reader.GetAttribute("ID"));

        if (reader.GetAttribute("Type") == null)
            Type = DoorTypes.EXIT;
        else
        {
            if (Enum.IsDefined(typeof(DoorTypes), reader.GetAttribute("Type")) == true)
                Type = (DoorTypes)Enum.Parse(typeof(DoorTypes), reader.GetAttribute("Type"));
            else
                Debug.LogError("XML Document has bad DoorTypes Enum");
        }

        if (reader.GetAttribute("Action") == null)
            Action = DoorAction.NONE;
        else
        {
            if (Enum.IsDefined(typeof(DoorAction), reader.GetAttribute("Action")) == true)
                Action = (DoorAction)Enum.Parse(typeof(DoorAction), reader.GetAttribute("Action"));
            else
                Debug.LogError("XML Document has bad DoorAction Enum");
        }

        Sprite = reader.GetAttribute("Sprite");
        X = int.Parse(reader.GetAttribute("X"));
        Y = int.Parse(reader.GetAttribute("Y"));
        LinkToArea = reader.GetAttribute("LinkToArea");
        LinkToDoor = int.Parse(reader.GetAttribute("LinkToDoor"));
        State = reader.GetAttribute("State");
        ExitX = float.Parse(reader.GetAttribute("ExitX"));
        ExitY = float.Parse(reader.GetAttribute("ExitY"));
        bool temp;
        if (bool.TryParse(reader.GetAttribute("Sensor"), out temp) == false)
            Sensor = false;
        else
            Sensor = temp;

    }
}
