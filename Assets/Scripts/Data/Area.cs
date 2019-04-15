using UnityEngine;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Area : IXmlSerializable
{

    public string Place { get; set; }
    Tile[,] tiles;

    // The tile width of the world.
    public int Width { get; protected set; }

    // The tile height of the world
    public int Height { get; protected set; }

    // Keep track whom is in the area.
    public Hero hero;
    public List<NPC> npcs;
    public List<Fixture> fixtures;

    public FixtureManager fixtureManager;

    Action<Hero> cbHeroCreated;
    Action<NPC> cbNpcCreated;
    Action<Door> cbDoorCreated;
    Action<Container> cbContainerCreated;

    public Area()
    {
        hero = new Hero();
        npcs = new List<NPC>();
        fixtures = new List<Fixture>();

        fixtureManager = new FixtureManager();
    }

    public Area(string place, int width, int height)
    {

        Place = place;
        Width = width;
        Height = height;
        InitializeTiles();

    }

    void InitializeTiles()
    {
        tiles = new Tile[Width, Height];

        for (int w = 0; w < Width; w++)
        {
            for (int h = 0; h < Height; h++)
            {
                tiles[w, h] = new Tile(TileType.Empty, w, h, true);
            }
        }
    }

    public void Update(float deltaTime)
    {

        // Update Hero
        hero.Update(deltaTime);

        // Update NPC
        foreach (NPC n in npcs)
        {
            n.Update(deltaTime);
        }

        // Update Doors
        foreach (Fixture f in fixtures)
        {
            if (f.GetType() == typeof(Door))
                (f as Door).Update(deltaTime);
        }
    }

    /// <summary>
	/// Gets the tile data at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            //Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public Tile GetTileAt(float x, float y)
    {
        int intX = Mathf.FloorToInt(x);
        int intY = Mathf.FloorToInt(y);

        return GetTileAt(intX, intY);
    }

    public Tile GetTileAt(Vector3 tile)
    {
        int intX = Mathf.FloorToInt(tile.x);
        int intY = Mathf.FloorToInt(tile.y);

        return GetTileAt(intX, intY);
    }

    public NPC GetNpcAt(Tile tile)
    {

        foreach (NPC npc in npcs)
        {
            if (npc.CurrentTile == tile)
                return npc;
        }

        return null;
    }

    public Door GetDoor(int ID)
    {

        foreach (Fixture fixture in fixtures)
        {
            if (fixture.GetType() == typeof(Door))
                if ((fixture as Door).ID == ID)
                    return (fixture as Door);
        }

        return null;

    }

    public void RegisterHeroCreated(Action<Hero> callbackfunc)
    {
        cbHeroCreated += callbackfunc;
    }

    public void UnregisterHeroCreated(Action<Hero> callbackfunc)
    {
        cbHeroCreated -= callbackfunc;
    }

    public void RegisterNpcCreated(Action<NPC> callbackfunc)
    {
        cbNpcCreated += callbackfunc;
    }

    public void UnregisterNpcCreated(Action<NPC> callbackfunc)
    {
        cbNpcCreated -= callbackfunc;
    }

    public void RegisterDoorCreated(Action<Door> callbackfunc)
    {
        cbDoorCreated += callbackfunc;
    }

    public void UnregisterDoorCreated(Action<Door> callbackfunc)
    {
        cbDoorCreated -= callbackfunc;
    }

    public void RegisterContainerCreated(Action<Container> callbackfunc)
    {
        cbContainerCreated += callbackfunc;
    }

    public void UnregisterContainerCreated(Action<Container> callbackfunc)
    {
        cbContainerCreated -= callbackfunc;
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Place", Place);
        writer.WriteAttributeString("Width", Width.ToString());
        writer.WriteAttributeString("Height", Height.ToString());
        writer.WriteStartElement("Tiles");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                writer.WriteStartElement("Tile");
                tiles[x, y].WriteXml(writer);
                writer.WriteEndElement();
               
            }
        }
        writer.WriteEndElement();

        // Start Characters
        writer.WriteStartElement("Characters");
        //hero.WriteXml(writer);

        if (npcs.Count != 0)
        {
            writer.WriteStartElement("NPCs");
            foreach (NPC n in npcs)
            {
                n.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        // Start Fixtures
        if (fixtures.Count != 0)
        {
            writer.WriteStartElement("Fixtures");

            if (fixtures.Exists(element => element.GetType() == typeof(Door)))
            {
                writer.WriteStartElement("Doors");
                foreach (Fixture fixture in fixtures)
                {
                    if (fixture.GetType() == typeof(Door))
                    {
                        (fixture as Door).WriteXml(writer);
                    }
                }
                writer.WriteEndElement();
            }

            if (fixtures.Exists(element => element.GetType() == typeof(Container)))
            {
                writer.WriteStartElement("Containers");
                foreach (Fixture fixture in fixtures)
                {
                    if (fixture.GetType() == typeof(Container))
                    {
                        (fixture as Container).WriteXml(writer);
                    }
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }


    }

    public void ReadXml(XmlReader reader)
    {
        Place = reader.GetAttribute("Place");
        Width = int.Parse(reader.GetAttribute("Width"));
        Height = int.Parse(reader.GetAttribute("Height"));

        InitializeTiles();

        while (reader.Read())
        {

            if (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "Tiles":
                        ReadXml_Tiles(reader);
                        break;
                    case "Characters":
                        ReadXml_Characters(reader);
                        break;
                    case "Fixtures":
                        ReadXml_Fixtures(reader);
                        break;
                }
            }

        }

        //if (reader.ReadToDescendant("Tiles"))
        //{
        //    ReadXml_Tiles(reader.ReadSubtree());
        //}
        //if (reader.ReadToNextSibling("Characters"))
        //{
        //    ReadXml_Characters(reader.ReadSubtree());
        //}
        //if (reader.ReadToNextSibling("Fixtures"))
        //{
        //    ReadXml_Fixtures(reader.ReadSubtree());
        //}
    }

    void ReadXml_Tiles(XmlReader reader)
    {

        int x, y, start, end;

        if (reader.ReadToDescendant("Tile"))
        {
            do
            {
                if (reader.GetAttribute("X") != null && reader.GetAttribute("Y") != null)
                {
                    x = int.Parse(reader.GetAttribute("X"));
                    y = int.Parse(reader.GetAttribute("Y"));
                    if (x > Width || x < 0 || y > Height || y < 0)
                    {
                        Debug.LogError("(X,Y) in XML is Out of Bounds for Area (" + x + ", " + y + ")");
                        continue;
                    }
                    tiles[x, y].ReadXml(reader);
                }
                else if (reader.GetAttribute("Fill") != null)
                {
                    string fillType = reader.GetAttribute("Fill");
                    x = int.Parse(reader.GetAttribute(fillType));
                    start = int.Parse(reader.GetAttribute("Start"));
                    end = int.Parse(reader.GetAttribute("End"));
                    if (fillType == "Row")
                    {
                        for (int i = start; i <= end; i++)
                        {
                            tiles[i, x].ReadXml(reader);
                        }
                    }
                    else if (fillType == "Column")
                    {
                        for (int i = start; i <= end; i++)
                        {
                            tiles[x, i].ReadXml(reader);
                        }
                    }
                }
                else if (reader.GetAttribute("Row") != null || reader.GetAttribute("Column") != null)
                {
                    if (reader.GetAttribute("Row") != null)
                    {
                        y = int.Parse(reader.GetAttribute("Row"));
                        for (int i = 0; i < Width; i++)
                        {
                            tiles[i, y].ReadXml(reader);
                        }
                    }
                    else if (reader.GetAttribute("Column") != null)
                    {
                        x = int.Parse(reader.GetAttribute("Column"));
                        for (int i = 0; i < Height; i++)
                        {
                            tiles[x, i].ReadXml(reader);
                        }
                    }
                }
                else
                {
                    for (int w = 0; w < Width; w++)
                    {
                        for (int h = 0; h < Height; h++)
                        {
                            tiles[w, h].ReadXml(reader);
                        }
                    }
                }
            } while (reader.ReadToNextSibling("Tile"));

        }

    }

    void ReadXml_Characters(XmlReader reader)
    {

        if (reader.IsEmptyElement)
            return;

        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                
                switch (reader.Name)
                {
                    case "NPCs":
                        ReadXml_NPCs(reader);
                        break;
                }
            }
            else {
                if (reader.Name == "Characters")
                    return;
            }
 
        }


        //if (reader.ReadToDescendant("Hero"))
        //{
        //    ReadXml_Hero(reader);

        //    if (reader.ReadToNextSibling("NPCs"))
        //    {
        //        ReadXml_NPCs(reader);
        //    }
        //    reader.ReadEndElement();
        //}
        /*else */

        //if (reader.ReadToDescendant("NPCs"))
        //{
        //    ReadXml_NPCs(reader);
        //    reader.ReadEndElement();
        //}


    }

    void ReadXml_Hero(XmlReader reader)
    {

        Hero hero = new Hero();

        hero.ReadXml(reader);
        this.hero = hero;
        if (cbHeroCreated != null)
            cbHeroCreated(hero);

    }


    void ReadXml_NPCs(XmlReader reader)
    {

        NPC npc;

        if (reader.ReadToDescendant("NPC"))
        {
            do
            {
                npc = new NPC();
                npc.ReadXml(reader);
                npcs.Add(npc);

                if (cbNpcCreated != null)
                    cbNpcCreated(npc);

            } while (reader.ReadToNextSibling("NPC"));
        }

    }

    void ReadXml_Fixtures(XmlReader reader)
    {

        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "Doors":
                        ReadXml_Doors(reader);
                        break;
                    case "Containers":
                        ReadXml_Containers(reader);
                        break;
                }
            }
            else
            {
                if (reader.Name == "Fixtures")
                    return;
            }

        }


        //if (reader.ReadToDescendant("Doors"))
        //{
        //    ReadXml_Doors(reader);

        //    if (reader.ReadToNextSibling("Containers"))
        //    {
        //        ReadXml_Containers(reader);
        //    }
        //    reader.ReadEndElement();
        //}
        //else if (reader.ReadToDescendant("Containers"))
        //{
        //    ReadXml_Containers(reader);

        //    if (reader.ReadToNextSibling("Doors"))
        //    {
        //        ReadXml_Doors(reader);
        //    }
        //    reader.ReadEndElement();

        //}

    }

    void ReadXml_Doors(XmlReader reader)
    {

        Door door;

        if (reader.ReadToDescendant("Door"))
        {
            do
            {
                door = new Door();
                door.ReadXml(reader);
                fixtures.Add(door);

                if (cbDoorCreated != null)
                    cbDoorCreated(door);

            } while (reader.ReadToNextSibling("Door"));
        }

    }

    void ReadXml_Containers(XmlReader reader)
    {

        Container container;
    
        if (reader.ReadToDescendant("Container"))
        {
            do
            {
                container = new Container();
                container.ReadXml(reader);
                fixtures.Add(container);

                if (cbContainerCreated != null)
                    cbContainerCreated(container);

            } while (reader.ReadToNextSibling("Container"));
        }

    }

}
