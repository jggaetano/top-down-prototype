using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FixtureSpriteController : MonoBehaviour
{

    // Door Prefabs
    public GameObject singlePrefab;
    public GameObject singleSlidingPrefab;
	public GameObject doublePrefab;
    public GameObject doubleSlidingPrefab;
	public GameObject exitPrefab;

    // Container Prefabs
    public GameObject containerPrefab;

    static public FixtureSpriteController Instance;

    public Dictionary<Fixture, GameObject> fixtureGameObjectMap;
    public Dictionary<GameObject, Fixture> gameObjectFixtureMap;
    //public Dictionary<Container, GameObject> containerGameObjectMap;
    Dictionary<string, Sprite> fixtureSprites;

    Area area
    {
        get { return AreaController.Instance.area; }
    }

    // Use this for initialization
    void Start()
    {

        if (Instance == null)
            Instance = this;

        LoadSprites();

        // Instantiate our dictionary that tracks which GameObject is rendering which Fixture data.
        fixtureGameObjectMap = new Dictionary<Fixture, GameObject>();
        gameObjectFixtureMap = new Dictionary<GameObject, Fixture>();
        //containerGameObjectMap = new Dictionary<Container, GameObject>();

        // Register our callback so that our GameObject gets updated whenever
        // the door changes
        area.RegisterDoorCreated(OnDoorCreated);
        area.RegisterContainerCreated(OnContainerCreated);

        foreach (Fixture fixture in area.fixtures)
        {
            if (fixture.GetType() == typeof(Door))
            {
                OnDoorCreated(fixture as Door);
                OnDoorChanged(fixture as Door);
            }
            if (fixture.GetType() == typeof(Container))
            {
                OnContainerCreated(fixture as Container);
                OnContainerChanged(fixture as Container);
            }
        }

        //foreach (Container c in area.containers)
        //{
        //    OnContainerCreated(c);
        //    OnContainerChanged(c);
        //}

    }

    void LoadSprites()
    {
        fixtureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Fixtures/");

        foreach (Sprite s in sprites)
        {
            fixtureSprites[s.name] = s;
        }
    }

    public void OnDoorCreated(Door door)
    {

        // Create a visual GameObject linked to this data.
        // This creates a new GameObject and adds it to our scene.
        GameObject fixture_go = null;
        switch (door.Type)
        {
            case DoorTypes.SINGLE:
                fixture_go = CreateSingle(door); 
                break;
            case DoorTypes.DOUBLE:
                fixture_go = CreateDouble(door);
                break;
            case DoorTypes.EXIT:
                fixture_go = (GameObject)Instantiate(exitPrefab);
                break;
        }

        // Add our tile/GO pair to the dictionary.
        fixtureGameObjectMap.Add(door, fixture_go);
        gameObjectFixtureMap.Add(fixture_go, door);

        fixture_go.name = "DoorTo" + door.LinkToArea + "_" + door.LinkToDoor;
        fixture_go.transform.position = new Vector3(door.X, door.Y, 0);
        fixture_go.transform.SetParent(this.transform.GetChild(2).transform, true);
        fixture_go.tag = "Door";

        fixture_go.transform.GetChild(0).gameObject.SetActive(door.Sensor);
        fixture_go.transform.GetChild(1).gameObject.SetActive(false);
        fixture_go.transform.GetChild(2).gameObject.SetActive(false);
        switch (door.State)
        {
            case "Open":
                fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case "Closed":
                fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                break;
           case null:
                fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                break;
        }


        door.RegisterOnChangedCallback(OnDoorChanged);

    }

    GameObject CreateSingle(Door door) {

        GameObject single_go = null;
        switch (door.Action)
        {
            case DoorAction.NONE:
            case DoorAction.SWINGING:
                single_go = (GameObject)Instantiate(singlePrefab);
                break;
            case DoorAction.SLIDING:
                single_go = (GameObject)Instantiate(singleSlidingPrefab);
                break;
        }

        if (door.Sprite != "NONE")
        {
            switch (door.Action)
            {
                case DoorAction.SWINGING:
                case DoorAction.NONE:
                    SpriteRenderer sr = single_go.GetComponent<SpriteRenderer>();
                    if (!fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_" + door.State))
                    {
                        Debug.LogError("fixtureSprites does not contain " + door.Sprite + "_" + door.Type.ToString() + "_" + door.State);
                        return null;
                    }
                    sr.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_" + door.State];
                    sr.sortingLayerName = "Fixtures";
                    break;
                case DoorAction.SLIDING:
                    SpriteRenderer doorframe    = single_go.GetComponent<SpriteRenderer>();
                    SpriteRenderer door_open    = single_go.transform.GetChild(1).GetComponent<SpriteRenderer>();
                    SpriteRenderer door_close   = single_go.transform.GetChild(2).GetComponent<SpriteRenderer>();
                    if (!fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_Doorframe") && 
                        !fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_Door"))
                    {
                        Debug.LogError("fixtureSprites does not contain sliding door parts!");
                        return null;
                    }
                    doorframe.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_Doorframe"];
                    door_open.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_Door"];
                    door_close.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_Door"];
                    // Offet Door
                    
                    // Set Layers
                    doorframe.sortingLayerName = "Fixtures";
                    door_open.sortingLayerName = "Fixtures";
                    door_open.sortingOrder = 1;
                    door_close.sortingLayerName = "Fixtures";
                    door_close.sortingOrder = 1;
                    break;
            }
            
        }

        return single_go;

    }

    GameObject CreateDouble(Door door)
    {

        GameObject double_go = null;
        switch (door.Action)
        {
            case DoorAction.NONE:
            case DoorAction.SWINGING:
                double_go = (GameObject)Instantiate(doublePrefab);
                break;
            case DoorAction.SLIDING:
                double_go = (GameObject)Instantiate(doubleSlidingPrefab);
                break;
        }

        if (door.Sprite != "NONE")
        {
            switch (door.Action)
            {
                case DoorAction.NONE:
                case DoorAction.SWINGING:
                    SpriteRenderer sr = double_go.GetComponent<SpriteRenderer>();
                    if (!fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_" + door.State))
                    {
                        Debug.LogError("fixtureSprites does not contain " + door.Sprite + "_" + door.Type.ToString() + "_" + door.State);
                        return null;
                    }
                    sr.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_" + door.State];
                    sr.sortingLayerName = "Fixtures";
                    break;
                case DoorAction.SLIDING:
                    SpriteRenderer doorframe = double_go.GetComponent<SpriteRenderer>();
                    SpriteRenderer leftDoor  = double_go.transform.GetChild(2).GetComponent<SpriteRenderer>();
                    SpriteRenderer rightDoor = double_go.transform.GetChild(3).GetComponent<SpriteRenderer>();
                    if (!fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_Doorframe") &&
                        !fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_LeftDoor") &&
                        !fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_RightDoor"))
                    {
                        Debug.LogError("fixtureSprites does not contain sliding door parts!");
                        return null;
                    }
                    doorframe.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_Doorframe"];
                    leftDoor.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_LeftDoor"];
                    rightDoor.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_RightDoor"];
                    doorframe.sortingLayerName = "Fixtures";
                    leftDoor.sortingLayerName = "Fixtures";
                    leftDoor.sortingOrder = 1;
                    rightDoor.sortingLayerName = "Fixtures";
                    rightDoor.sortingOrder = 1;
                    break;
            }

        }

        return double_go;

    }

    void OnDoorChanged(Door door)
    {

        if (fixtureGameObjectMap.ContainsKey(door) == false)
        {
            Debug.LogError("OnDoorChanged -- trying to change visuals for fixture not in our map.");
            return;
        }

        GameObject fixture_go = fixtureGameObjectMap[door];
    
        fixture_go.transform.position = new Vector3(door.X, door.Y, 0);

        switch (door.Type)
        {
            case DoorTypes.SINGLE:
            case DoorTypes.EXIT:
                ChangeSingle(door, fixture_go);
                break;
            case DoorTypes.DOUBLE:
                ChangeDouble(door, fixture_go);
                break;
        }

    }

    void ChangeSingle(Door door, GameObject fixture_go) {

        if (door.Sprite != "NONE")
        {
            switch (door.Action)
            {
                case DoorAction.NONE:
                case DoorAction.SWINGING:
                    SpriteRenderer sr = fixture_go.GetComponent<SpriteRenderer>();

                    if (!fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_" + door.State))
                    {
                        Debug.LogError("fixtureSprites does not contain " + door.Sprite + door.State);
                        return;
                    }
                    sr.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_" + door.State];

                    fixture_go.transform.GetChild(1).gameObject.SetActive(false);
                    fixture_go.transform.GetChild(2).gameObject.SetActive(false);
                    switch (door.State)
                    {
                        case "Open":
                            fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                        case "Closed":
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            break;
                        case null:
                            fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                    }

                    break;
                case DoorAction.SLIDING:

                    fixture_go.transform.GetChild(1).gameObject.SetActive(false);
                    fixture_go.transform.GetChild(2).gameObject.SetActive(false);
                    switch (door.State)
                    {
                        case "Open":
                            fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(2).localPosition = new Vector2(door.LeftOpenness, 0);
                            break;
                        case "Closed":
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(2).localPosition = new Vector2(door.LeftOpenness, 0);
                            break;
                        case "Opening":
                        case "Closing":
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(2).localPosition = new Vector2(door.LeftOpenness, 0);
                            break;
                        case null:
                            fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                    }

                    break;
            }
        }

    }

    void ChangeDouble(Door door, GameObject fixture_go) {

        if (door.Sprite != "NONE")
        {
            switch (door.Action)
            {
                case DoorAction.NONE:
                case DoorAction.SWINGING:
                    SpriteRenderer sr = fixture_go.GetComponent<SpriteRenderer>();

                    if (!fixtureSprites.ContainsKey(door.Sprite + "_" + door.Type.ToString() + "_" + door.State))
                    {
                        Debug.LogError("fixtureSprites does not contain " + door.Sprite + door.State);
                        return;
                    }
                    sr.sprite = fixtureSprites[door.Sprite + "_" + door.Type.ToString() + "_" + door.State];

                    fixture_go.transform.GetChild(1).gameObject.SetActive(false);
                    fixture_go.transform.GetChild(2).gameObject.SetActive(false);
                    switch (door.State)
                    {
                        case "Open":
                            fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                        case "Closed":
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            break;
                        case null:
                            fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                    }
                    break;
                case DoorAction.SLIDING:

                    fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                    fixture_go.transform.GetChild(2).gameObject.SetActive(false);
                    fixture_go.transform.GetChild(3).gameObject.SetActive(false);
                    switch (door.State)
                    {
                        case "Open":
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(2).localPosition = new Vector2(door.LeftOpenness, 0);
                            fixture_go.transform.GetChild(3).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(3).localPosition = new Vector2(door.RightOpenness, 0);
                            break;
                        case "Closed":
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(2).localPosition = new Vector2(door.LeftOpenness, 0);
                            fixture_go.transform.GetChild(3).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(3).localPosition = new Vector2(door.RightOpenness, 0);
                            break;
                        case "Opening":
                        case "Closing":
                            fixture_go.transform.GetChild(2).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(2).localPosition = new Vector2(door.LeftOpenness, 0);
                            fixture_go.transform.GetChild(3).gameObject.SetActive(true);
                            fixture_go.transform.GetChild(3).localPosition = new Vector2(door.RightOpenness, 0);
                            break;
                        case null:
                            fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                    }

                    break;
            }
        }

    }

    public void OnContainerCreated(Container container)
    {

        // Create a visual GameObject linked to this data.
        // This creates a new GameObject and adds it to our scene.
        GameObject fixture_go = (GameObject)Instantiate(containerPrefab);

        // Add our tile/GO pair to the dictionary.
        fixtureGameObjectMap.Add(container, fixture_go);
        gameObjectFixtureMap.Add(fixture_go, container);

        fixture_go.name = "Container_" + container.Sprite;
        fixture_go.transform.position = new Vector3(container.X, container.Y, 0);
        fixture_go.transform.SetParent(this.transform.GetChild(2).transform, true);
        fixture_go.tag = "Container";

        if (container.Sprite != "NONE")
        {
            SpriteRenderer sr = fixture_go.AddComponent<SpriteRenderer>();

            if (!fixtureSprites.ContainsKey(container.Sprite + "_" + container.State))
            {
                Debug.LogError("OnContainerCreated: fixtureSprites does not contain " + container.Sprite + "_" + container.State);
                return;
            }
            sr.sprite = fixtureSprites[container.Sprite + "_" + container.State];
            sr.sortingLayerName = "Characters";
        }

        fixture_go.transform.GetChild(0).gameObject.SetActive(false);
        fixture_go.transform.GetChild(1).gameObject.SetActive(false);
        switch (container.State)
        {
            case "Open":
                fixture_go.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Closed":
                fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case null:
                fixture_go.transform.GetChild(0).gameObject.SetActive(true);
                break;
        }

        container.RegisterOnChangedCallback(OnContainerChanged);

    }

    void OnContainerChanged(Container container)
    {

        if (fixtureGameObjectMap.ContainsKey(container) == false)
        {
            Debug.LogError("OnContainerChanged -- trying to change visuals for fixture not in our map.");
            return;
        }

        GameObject fixture_go = fixtureGameObjectMap[container];

        fixture_go.transform.position = new Vector3(container.X, container.Y, 0);

        if (container.Sprite != "NONE")
        {
            SpriteRenderer sr = fixture_go.GetComponent<SpriteRenderer>();
            if (!fixtureSprites.ContainsKey(container.Sprite + "_" + container.State))
            {
                Debug.LogError("OnContainerChanged: fixtureSprites does not contain " + container.Sprite + "_" + container.State);
                return;
            }
            sr.sprite = fixtureSprites[container.Sprite + "_" + container.State];
        }

        fixture_go.transform.GetChild(0).gameObject.SetActive(false);
        fixture_go.transform.GetChild(1).gameObject.SetActive(false);
        switch (container.State)
        {
            case "Open":
                fixture_go.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Closed":
                fixture_go.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case null:
                fixture_go.transform.GetChild(0).gameObject.SetActive(true);
                break;
        }

    }

}
