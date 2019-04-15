using UnityEngine;
using System.Collections;

public class Fixture  {

    Area area { get { return AreaController.Instance.area; } }

    public string Sprite { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public string State { get; protected set; }
    public Tile MyTile { get { return area.GetTileAt(X, Y); } }

    public Fixture() {
    }

    public Fixture(string sprite, int x, int y, string state) {
        Sprite = sprite;
        X = x;
        Y = y;
        State = state;
    }

    protected Fixture(Fixture other) {
        Sprite = other.Sprite;
        X      = other.X;
        Y      = other.Y;
        State  = other.State;
    }

    virtual public Fixture Clone() {
        return new Fixture(this);
    }

    virtual public void Interact()
    {
        switch (State)
        {
            case "Open":
                State = "Closed";
                break;
            case "Closed":
                State = "Open";
                break;
            default:
                State = "";
                break;
        }
    }

    virtual public void Open() {
        if (State == "Closed")
            State = "Open";
    }

    virtual public void Close()
    {
        if (State == "Open")
            State = "Closed";
    }

}
