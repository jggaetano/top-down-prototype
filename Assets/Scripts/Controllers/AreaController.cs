using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaController : MonoBehaviour {

    public string first;
    public Vector2 startPosition;
    public static AreaController Instance { get; protected set; }
    public Area area { get; protected set; }

    static bool start = true;
    static string nextArea;
    static Hero nextAreaHero;
    static List<string> loadedAreas;
	public static int nextAreaDoor;

    // Use this for initialization
    void OnEnable()
    {

        if (Instance != null)
        {
            Debug.LogError("There should never be two area controllers.");
        }
        Instance = this;

        if (start)
        {
            loadedAreas = new List<string>();
            area = LoadArea(first);
            loadedAreas.Add(first);
            Hero hero = new Hero("Cecil", "Hero", startPosition.x, startPosition.y);
            area.hero = hero;
            start = false;
        }
        else
        {
            area = LoadArea(nextArea);
            area.hero = nextAreaHero;
            Door door = area.GetDoor(nextAreaDoor);
            if (door == null)
            {
                Debug.LogError("OnEnable -- Something went wrong with the door linkage.");
                return;
            }
			area.hero.Move (door.ExitX, door.ExitY);
        }

        
    }

    void Update() {
        area.Update(Time.deltaTime);
    }

    public void SaveArea(string place) {

        // Serialize class into XML format.
        XmlSerializer serializer = new XmlSerializer(typeof(Area));
        TextWriter writer = new StringWriter();
        serializer.Serialize(writer, area);

        using (StreamWriter stream = new StreamWriter("Assets\\Resources\\Areas\\SaveGame00_" + place + ".xml")) {
            stream.WriteLine(writer.ToString());
        }

        writer.Close();

        //Debug.Log(writer.ToString());
        //PlayerPrefs.SetString(place, writer.ToString());


    }

    public Area LoadArea(string place) {

        Area area;

        TextReader reader;
        if (loadedAreas.Contains(place) == false)
        {
            reader = new StreamReader("Assets\\Resources\\Areas\\" + place + ".xml");
            loadedAreas.Add(place);
        }
        else
            reader = new StreamReader("Assets\\Resources\\Areas\\SaveGame00_" + place + ".xml");

        //Debug.Log(reader.ToString());
        XmlSerializer serializer = new XmlSerializer(typeof(Area));
        area = (Area)serializer.Deserialize(reader);
        reader.Close();

        return area;
    }

    public void NextArea(string location) {
        SaveArea(area.Place);
        nextArea = location;
		nextAreaHero = area.hero.Clone();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
		
}
