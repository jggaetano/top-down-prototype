using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class NPC : IXmlSerializable {

    Area area
    {
        get { return AreaController.Instance.area; }
    }

    // NPC Traits
    public string Name { get; protected set; }
    public string Sprite { get; protected set; }
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public Tile CurrentTile { get; protected set; }

    public bool pause = false;

    // Movement variables
    float speed = 2f;	
    float distThisFrame;
    float movementTimer = 0;

    // Scripted movement variables
    Movement currentDirection;
    List<Move> moves;
    int currentMove = 0;

    // Dialog 
    List<string> dialog;

    Action<NPC> cbNpcChanged;

    public NPC() {
        moves = new List<Move>();
        dialog = new List<string>();
    }

    public NPC(string name, int x, int y) {
        Name = name;
        X = x;
        Y = y + 0.5f;
        moves = new List<Move>();
        dialog = new List<string>();
    }

    public void Update(float deltaTime) {

		if (pause)
            return;

        if (moves.Count == 0)
            return;

        currentDirection = moves[currentMove].Direction;

        if (Move(moves[currentMove], deltaTime))
        {
            currentMove++;
            if (currentMove == moves.Count)
                currentMove = 0;
        }

        UpdateTile();

        if (cbNpcChanged != null)
            cbNpcChanged(this);

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="timer">Amount of time to move</param>
    /// <param name="deltaTime">Pass the time interval since last frame</param>
    /// <returns>When current move has been completed it returns true otherwise returns false</returns>
    private bool Move(Move move, float deltaTime)
    {
        
        // Setup Timer
        if (movementTimer <= 0)
            movementTimer = move.Time;

        distThisFrame = deltaTime / speed;
        switch (move.Direction)
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

        // update amount of time left to move.
        movementTimer -= deltaTime;
        if (movementTimer <= 0)
            return true;

        return false;
    }

    public Movement GetDirection() {
        return currentDirection;
    }

    public void UpdateTile() {
        int tileX = Mathf.FloorToInt(X);
        int tileY = Mathf.FloorToInt(Y);
        CurrentTile = area.GetTileAt(tileX, tileY);
    }

    public void Speak() {
        if (dialog.Count == 0)
            return;
        MessagesManager.AddMessage(Name, dialog[UnityEngine.Random.Range(0, dialog.Count)]);        
    }

    public void FaceMe(Movement direction)
    {
        currentDirection = global::Move.Opposite(direction);

        if (cbNpcChanged != null)
            cbNpcChanged(this);
    }

    public void RegisterOnChangedCallback(Action<NPC> cb)
    {
        cbNpcChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<NPC> cb)
    {
        cbNpcChanged -= cb;
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("NPC");
        writer.WriteAttributeString("Name", Name);
        writer.WriteAttributeString("Sprite", Sprite);
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        if (moves.Count != 0)
        {
            writer.WriteStartElement("Moves");
            writer.WriteAttributeString("MovementTimer", movementTimer.ToString());
            writer.WriteAttributeString("CurrentMove", currentMove.ToString());
            foreach (Move move in moves)
            {
                move.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
        if (dialog.Count != 0)
        {
            writer.WriteStartElement("Dialog");
            foreach (string text in dialog)
            {
                writer.WriteStartElement("Text");
                writer.WriteString(text);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

    }

    public void ReadXml(XmlReader reader)
    {
        Name = reader.GetAttribute("Name");
        Sprite = reader.GetAttribute("Sprite");
        X = float.Parse(reader.GetAttribute("X")); 
        Y = float.Parse(reader.GetAttribute("Y"));

		if (reader.IsEmptyElement)
			return;

	    while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "Moves":
                        movementTimer = float.Parse(reader.GetAttribute("MovementTimer"));
                        currentMove = int.Parse(reader.GetAttribute("CurrentMove"));
                        ReadXml_Moves(reader);
                        break;
                    case "Dialog":
                        ReadXml_Dialog(reader);
                        break;
                }
            }
            else {
                if (reader.Name == "NPC")
                    break;
            }
        }
    }

    void ReadXml_Moves(XmlReader reader) {

        Move m;

        if (reader.ReadToDescendant("Move"))
        {
            do
            {
                m = new Move();
                m.ReadXml(reader);
                moves.Add(m);
            } while (reader.ReadToNextSibling("Move"));
        }

    }

    void ReadXml_Dialog(XmlReader reader)
    {
        if (reader.ReadToDescendant("Text"))
        {
            do
            {
                dialog.Add(reader.ReadString());
            } while (reader.ReadToNextSibling("Text"));
        }
    }
}
