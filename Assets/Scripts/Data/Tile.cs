using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public enum TileType { Empty, Ground, Water };

public class Tile : IXmlSerializable {

    public TileType Type { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public bool Walkable { get; set; }

    Area area
    {
        get { return AreaController.Instance.area; }
    }

    public Tile() {
        Type = TileType.Empty;
        X = 0;
        Y = 0;
        Walkable = true;
    }

    public Tile(TileType type, int x, int y, bool walkable) {
        Type = type;
        X = x;
        Y = y;
        Walkable = walkable;
    }

    protected Tile(Tile tile) {
        Type = tile.Type;
        X = tile.X;
        Y = tile.Y;
        Walkable = tile.Walkable;

    }

    virtual public Tile Clone() {
        return new Tile(this);
    }

    /// <summary>
	/// Gets the neighbours.
	/// </summary>
	/// <returns>The neighbours.</returns>
	public Tile[] GetNeighbours()
    {
        Tile[] ns = new Tile[4];
        Tile n;

        n = area.GetTileAt(X, Y + 1);
        ns[0] = n;  // Could be null, but that's okay.
        n = area.GetTileAt(X + 1, Y);
        ns[1] = n;  // Could be null, but that's okay.
        n = area.GetTileAt(X, Y - 1);
        ns[2] = n;  // Could be null, but that's okay.
        n = area.GetTileAt(X - 1, Y);
        ns[3] = n;  // Could be null, but that's okay.

        return ns;
    }

    public Tile North()
    {
        return area.GetTileAt(X, Y + 1);
    }
    public Tile NorthEast()
    {
        return area.GetTileAt(X + 1, Y + 1);
    }
    public Tile East()
    {
        return area.GetTileAt(X + 1, Y);
    }
    public Tile SouthEast()
    {
        return area.GetTileAt(X + 1, Y - 1);
    }
    public Tile South()
    {
        return area.GetTileAt(X, Y - 1);
    }
    public Tile SouthWest()
    {
        return area.GetTileAt(X - 1, Y - 1);
    }
    public Tile West()
    {
        return area.GetTileAt(X - 1, Y);
    }
    public Tile NorthWest()
    {
        return area.GetTileAt(X - 1, Y + 1);
    }

    public bool IsNorth(Tile tile) {
        return Y > tile.Y;
    }

    public bool IsEast(Tile tile)
    {
        return X > tile.X;
    }

    public bool IsSouth(Tile tile)
    {
        return Y < tile.Y;
    }

    public bool IsWest(Tile tile)
    {
        return X < tile.X;
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("Type", Type.ToString());
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        writer.WriteAttributeString("Walkable", Walkable.ToString());
    }

    public void ReadXml(XmlReader reader) {
        Type = (TileType)Enum.Parse(typeof(TileType), reader.GetAttribute("Type"));
        Walkable = bool.Parse(reader.GetAttribute("Walkable"));
    }

}
