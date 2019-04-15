using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterSpriteController : MonoBehaviour
{

    public GameObject heroPrefab;
    public GameObject npcPrefab;

    Dictionary<Hero, GameObject> heroGameObjectMap;
    Dictionary<GameObject, Hero> gameObjectHeroMap;
    Dictionary<NPC, GameObject> npcGameObjectMap;
    Dictionary<GameObject, NPC> gameObjectNpcMap;
    Dictionary<string, Sprite> characterSprites;
    List<GameObject> sortOrder;
    Dictionary<GameObject, float> gameObjectPositionMap;

    Area area
    {
        get { return AreaController.Instance.area; }
    }

    Dictionary<Fixture, GameObject> fixtures {
        get { return FixtureSpriteController.Instance.fixtureGameObjectMap;  }
    }

    public static CharacterSpriteController Instance { get; protected set; }

    // Use this for initialization
    void Start()
    {

        if (Instance == null)
            Instance = this;

        LoadSprites();

        // Instantiate our dictionary that tracks which GameObject is rendering which Hero data.
        heroGameObjectMap = new Dictionary<Hero, GameObject>();
        gameObjectHeroMap = new Dictionary<GameObject, Hero>();
        npcGameObjectMap = new Dictionary<NPC, GameObject>();
        gameObjectNpcMap = new Dictionary<GameObject, NPC>();

        // Register our callback so that our GameObject gets updated whenever
        // the tile's type changes.
        area.RegisterHeroCreated(OnHeroCreated);
        area.RegisterNpcCreated(OnNpcCreated);

        // Check for pre-existing characters, which won't do the callback.
        OnHeroCreated(area.hero);
        OnHeroChanged(area.hero);

        foreach (NPC n in area.npcs)
        {
            OnNpcCreated(n);
            OnNpcChanged(n);
        }

    }

    void LoadSprites()
    {
        characterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Characters/");

        //Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites)
        {
            //Debug.Log(s);
            characterSprites[s.name] = s;
        }
    }

    public void OnHeroCreated(Hero hero)
    {

        // Create a visual GameObject linked to this data.
        // This creates a new GameObject and adds it to our scene.
        GameObject char_go = (GameObject)Instantiate(heroPrefab);

        // Add our tile/GO pair to the dictionary.
        heroGameObjectMap.Add( hero, char_go );
        gameObjectHeroMap.Add( char_go, hero );

        char_go.name = hero.Name;
		char_go.transform.position = new Vector3(hero.X, hero.Y, 0);
        char_go.transform.SetParent(this.transform.GetChild(1).transform, true);
        char_go.tag = "Player";
  
        SpriteRenderer sr = char_go.AddComponent<SpriteRenderer>();
        
        if (!characterSprites.ContainsKey(hero.Sprite + "_" + area.hero.facing.ToString())) {
            Debug.LogError("characterSprites does not contain " + hero.Sprite + "_" + area.hero.facing.ToString());
            return;
        }
        sr.sprite = characterSprites[hero.Sprite + "_" + area.hero.facing.ToString()];
        sr.sortingLayerName = "Characters";
        
        hero.RegisterOnChangedCallback(OnHeroChanged);

    }

    public void OnNpcCreated(NPC npc)
    {

        // Create a visual GameObject linked to this data.
        // This creates a new GameObject and adds it to our scene.
        GameObject char_go = Instantiate(npcPrefab);

        // Add our tile/GO pair to the dictionary.
        npcGameObjectMap.Add(npc, char_go);
        gameObjectNpcMap.Add(char_go, npc);

        char_go.name = npc.Name;
        char_go.transform.position = new Vector3(npc.X, npc.Y, 0);
        char_go.transform.SetParent(this.transform.GetChild(1).transform, true);
        char_go.tag = "NPC";

        Movement currentDirection = npc.GetDirection();
        if (currentDirection == Movement.STOP)
            currentDirection = Movement.SOUTH;

        SpriteRenderer sr = char_go.AddComponent<SpriteRenderer>();

        if (!characterSprites.ContainsKey(npc.Sprite + "_" + currentDirection))
        {
            Debug.LogError("OnNpcCreated: characterSprites does not contain " + npc.Sprite + "_" + currentDirection);
            return;
        }
        sr.sprite = characterSprites[npc.Sprite + "_" + currentDirection];
        sr.sortingLayerName = "Characters";

        npc.UpdateTile();

        npc.RegisterOnChangedCallback(OnNpcChanged);

    }

    void OnHeroChanged(Hero hero)
    {

        if (heroGameObjectMap.ContainsKey(hero) == false)
        {
            Debug.LogError("OnHeroChanged -- trying to change visuals for character not in our map.");
            return;
        }

        GameObject char_go = heroGameObjectMap[hero];
    
        char_go.transform.position = new Vector3(hero.X, hero.Y, 0);

        SpriteRenderer sr = char_go.GetComponent<SpriteRenderer>();

        if (!characterSprites.ContainsKey(hero.Sprite + "_" + area.hero.facing.ToString()))
        {
            Debug.LogError("characterSprites does not contain " + hero.Sprite + "_" + area.hero.facing.ToString());
            return;
        }
        sr.sprite = characterSprites[hero.Sprite + "_" + area.hero.facing.ToString()];

        AdjustLayer();
    }

    void OnNpcChanged(NPC npc)
    {

        if (npcGameObjectMap.ContainsKey(npc) == false)
        {
            Debug.LogError("OnNpcChanged -- trying to change visuals for character not in our map.");
            return;
        }

        GameObject char_go = npcGameObjectMap[npc];

        char_go.transform.position = new Vector3(npc.X, npc.Y, 0);

        Movement currentDirection = npc.GetDirection();
        if (currentDirection == Movement.STOP)
            currentDirection = Movement.SOUTH;

        SpriteRenderer sr = char_go.GetComponent<SpriteRenderer>();

        if (!characterSprites.ContainsKey(npc.Sprite + "_" + currentDirection.ToString()))
        {
            Debug.LogError("OnNpcChanged: characterSprites does not contain " + npc.Sprite + "_" + currentDirection.ToString());
            return;
        }
        sr.sprite = characterSprites[npc.Sprite + "_" + currentDirection.ToString()];

    }

    void AdjustLayer() {

        gameObjectPositionMap = new Dictionary<GameObject, float>();

        foreach (Hero hero in heroGameObjectMap.Keys) {
            gameObjectPositionMap[heroGameObjectMap[hero]] = heroGameObjectMap[hero].transform.position.y;
        }
        foreach (NPC npc in npcGameObjectMap.Keys)
        {
            gameObjectPositionMap[npcGameObjectMap[npc]] = npcGameObjectMap[npc].transform.position.y;
        }
        foreach (Fixture fixture in fixtures.Keys)
        {
            if (fixture.GetType() == typeof(Container))
                gameObjectPositionMap[fixtures[fixture as Container]] = fixtures[fixture as Container].transform.position.y;
        }

        sortOrder = gameObjectPositionMap.OrderByDescending(kp => kp.Value).Select(kp => kp.Key).ToList();
        for (int i = 0; i < sortOrder.Count; i++) {
            if (sortOrder[i].GetComponent<SpriteRenderer>() != null)
               sortOrder[i].GetComponent<SpriteRenderer>().sortingOrder = i;
        }
         
    }

    public GameObject GetCharacter(Hero hero) {

        if (hero == null)
            return null;

        if (!heroGameObjectMap.ContainsKey(hero))
        {
            Debug.LogError("This hero is not in the map!");
            return null;
        }

        return heroGameObjectMap[hero];

    }

    public GameObject GetCharacter(NPC npc) {

        if (npc == null)
            return null;

        if (!npcGameObjectMap.ContainsKey(npc))
        {
            Debug.LogError("This npc is not in the map!");
            return null;
        }

        return npcGameObjectMap[npc];

    }

    public Hero GetHero(GameObject obj) {

        if (obj == null)
            return null;

        if (!gameObjectHeroMap.ContainsKey(obj))
        {
            // Not a Hero return null
            return null;
        }

        return gameObjectHeroMap[obj];
    }

    public NPC GetNPC(GameObject obj)
    {
        if (obj == null)
            return null;

        if (!gameObjectNpcMap.ContainsKey(obj))
        {
            //Not a NPC return null
            return null;
        }

        return gameObjectNpcMap[obj];
    }

    public GameObject GetNpcGamObject(NPC npc)
    {
        if (npc == null)
            return null;

        if (!npcGameObjectMap.ContainsKey(npc))
        {
            //Not a NPC return null
            return null;
        }

        return npcGameObjectMap[npc];
    }
}
