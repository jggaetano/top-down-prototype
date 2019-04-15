using UnityEngine;
using System.Collections.Generic;
using System;

public class TileSpriteController : MonoBehaviour {

    public static TileSpriteController Instance { get; protected set; }

    Dictionary<string, Sprite> tileSpriteMap;
    Dictionary<Tile, GameObject> tileGameObjectMap;

    Area area
    {
        get { return AreaController.Instance.area; }
    }

    void OnEnable()
    {

        if (Instance != null)
        {
            Debug.LogError("There should never be two TileSpriteControllers.");
        }
        Instance = this;

    }

    void Start () {

        LoadSprites();
        MapGameObjects();

    }

    void LoadSprites() {

        // Instantiate dictionary that tracks sprites loaded from the Resources folder.
        tileSpriteMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Tiles");
        if (sprites.Length == 0) {
            Debug.LogError("No sprites loaded!");
            return;
        }

        foreach (Sprite s in sprites)
        {
            tileSpriteMap[s.name] = s;
        }
    }

    void MapGameObjects() {

        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < area.Width; x++)
        {
            for (int y = 0; y < area.Height; y++)
            {
                // Get the tile data
                Tile tile_data = area.GetTileAt(x, y);

                // This creates a new GameObject and adds it to our scene.
                GameObject tile_go = new GameObject();

                // Add our tile/GO pair to the dictionary.
                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform.GetChild(0).transform, true);
                tile_go.tag = "Tile";

                // Add a Sprite Renderer
                // Add a default sprite for empty tiles.
                SpriteRenderer sr = tile_go.AddComponent<SpriteRenderer>();

                if (!tileSpriteMap.ContainsKey(tile_data.Type.ToString()))
                {
                    Debug.LogError("Missing Sprite in Resource/Sprites");
                    return;
                }
                sr.sprite = tileSpriteMap[tile_data.Type.ToString()];
                sr.sortingLayerName = "Tiles";

                BoxCollider2D bc = tile_go.AddComponent<BoxCollider2D>();
                bc.offset = new Vector2(0.5f, 0.5f);
                bc.enabled = !tile_data.Walkable;
                //Rigidbody2D rb = tile_go.AddComponent<Rigidbody2D>();
                //rb.isKinematic = true;

                //if (tile_data.Walkable == false) {
                //    BoxCollider2D bc = tile_go.AddComponent<BoxCollider2D>();
                //    bc.offset = new Vector2(0.5f, 0.5f);
                //    Rigidbody2D rb = tile_go.AddComponent<Rigidbody2D>();
                //    rb.isKinematic = true;
                //}

                //UpdateWalkable(tile_data);
            }
        }
    }

    //public void UpdateWalkable(Tile tile)
    //{

    //    GameObject tile_go = tileGameObjectMap[tile];
    //    BoxCollider2D bc = tile_go.GetComponent<BoxCollider2D>();
    //    bc.enabled = !tile.Walkable;

    //}

    public GameObject GetTileGameObject(Tile tile)
    {
        if (tileGameObjectMap.ContainsKey(tile) == false)
            return null;

        return tileGameObjectMap[tile];
    }


}
