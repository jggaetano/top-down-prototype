using System.Collections.Generic;
using UnityEngine;

// TODO add coments to code.
public class HeroController : MonoBehaviour {

	public float closeness = 0.5f;
    public bool frozen;
    public static HeroController Instance;

    Area area { get { return AreaController.Instance.area; } }
	CharacterSpriteController csc { get { return CharacterSpriteController.Instance; } }
    MenuController menu { get { return MenuController.Instance; } } 
    
    List<Movement> blockers;
    Collider2D heroCollider;
    RaycastHit2D hit;
    Vector2 north, east, south, west;
    GameObject touching;

    void Start() {
        if (Instance == null)
            Instance = this;

        frozen = false;
        heroCollider = GetComponent<Collider2D>();
    }

    void Initialize() {
        blockers = new List<Movement>();
        
    }

    void Update() {

        if (blockers == null)
            Initialize();

        if (!frozen)
        {
            if (area.hero != null)
            {
                if (!blockers.Contains(GetDirection()))
                {
                    area.hero.currentMovement = GetDirection();
                }
                else if (blockers.Contains(GetDirection()))
                {
                    area.hero.currentMovement = Movement.NONE;
                }

            }

			// Check if something is blocking the direction the hero can move.
            CheckBlockages();

			// Check if the hero is facing something or someone that can be 
            CheckTouch();
            if (Input.GetButtonUp("Interact"))
            {
                //Input.ResetInputAxes();
                if (touching != null)
                {
                    switch(touching.tag)
                    {
                        case "NPC":
                            csc.GetNPC(touching).Speak();
                            break;
                        default:
                            area.fixtureManager.Interact(touching);
                            break;
                    }
                    
                }
            }
        }

        // Get Inputs from User.
        if (Input.GetButtonUp("Menu"))
            menu.Toggle();
       
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();

    }

    void CheckBlockages() {

        north = new Vector2(heroCollider.bounds.center.x, heroCollider.bounds.center.y + heroCollider.bounds.extents.y + 0.1f);
        hit = Physics2D.Raycast(north, Vector2.up, closeness, LayerMask.GetMask("Default"));
        if (hit.collider != null)
            AddBlocker(Movement.NORTH);
        else
            RemoveBlocker(Movement.NORTH);

        east = new Vector2(heroCollider.bounds.center.x + heroCollider.bounds.extents.x + 0.1f, heroCollider.bounds.center.y);
        hit = Physics2D.Raycast(east, Vector2.right, closeness, LayerMask.GetMask("Default"));
        if (hit.collider != null)
            AddBlocker(Movement.EAST);
        else
            RemoveBlocker(Movement.EAST);

        south = new Vector2(heroCollider.bounds.center.x, heroCollider.bounds.center.y - heroCollider.bounds.extents.y - 0.1f);
        hit = Physics2D.Raycast(south, Vector2.down, closeness, LayerMask.GetMask("Default"));
        if (hit.collider != null)
            AddBlocker(Movement.SOUTH);
        else
            RemoveBlocker(Movement.SOUTH);

        west = new Vector2(heroCollider.bounds.center.x - heroCollider.bounds.extents.x - 0.1f, heroCollider.bounds.center.y);
        hit = Physics2D.Raycast(west, Vector2.left, closeness, LayerMask.GetMask("Default"));
        if (hit.collider != null)
            AddBlocker(Movement.WEST);
        else
            RemoveBlocker(Movement.WEST);

    }

	void AddBlocker(Movement direction) {   
	
		if (blockers.Contains (direction))
			return;

		blockers.Add(direction);
	
	}

	void RemoveBlocker(Movement direction) {

		if (blockers.Contains (direction) == false)
			return;

		blockers.Remove(direction);
	
	}

    Movement GetDirection()
    {

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            area.hero.facing = Movement.EAST;
            return Movement.EAST;
        }
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            area.hero.facing = Movement.WEST;
            return Movement.WEST;
        }
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            area.hero.facing = Movement.NORTH;
            return Movement.NORTH;
        }
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            area.hero.facing = Movement.SOUTH;
            return Movement.SOUTH;
        }

        return Movement.NONE;
    }

    void CheckTouch() {

        if (area.hero.facing != Movement.NONE)
        {
            switch (area.hero.facing)
            {
                case Movement.NORTH:
                    north = new Vector2(heroCollider.bounds.center.x, heroCollider.bounds.center.y + heroCollider.bounds.extents.y + 0.1f);
                    hit = Physics2D.Raycast(north, Vector2.up, closeness, LayerMask.GetMask("Default"));
                    break;
                case Movement.EAST:
                    east = new Vector2(heroCollider.bounds.center.x + heroCollider.bounds.extents.x + 0.1f, heroCollider.bounds.center.y);
                    hit = Physics2D.Raycast(east, Vector2.right, closeness, LayerMask.GetMask("Default"));
                    break;
                case Movement.SOUTH:
                    south = new Vector2(heroCollider.bounds.center.x, heroCollider.bounds.center.y - heroCollider.bounds.extents.y - 0.1f);
                    hit = Physics2D.Raycast(south, Vector2.down, closeness, LayerMask.GetMask("Default"));
                    break;
                case Movement.WEST:
                    west = new Vector2(heroCollider.bounds.center.x - heroCollider.bounds.extents.x - 0.1f, heroCollider.bounds.center.y);
                    hit = Physics2D.Raycast(west, Vector2.left, closeness, LayerMask.GetMask("Default"));
                    break;
            }


			if (hit.collider != null && hit.collider.gameObject.tag == "Interactable")
				touching = hit.collider.gameObject.transform.parent.gameObject; //area.GetTileAt(hit.collider.transform.position);
			else if (hit.collider != null && hit.collider.gameObject.tag == "NPC") {
				touching = hit.collider.gameObject.transform.gameObject; // The NPC GameObject used later 
                csc.GetNPC (touching).pause = true;
                csc.GetNPC(touching).FaceMe(area.hero.facing);
			} else {
                if (touching != null && touching.tag == "NPC")
                    csc.GetNPC(touching).pause = false;
				touching = null;
			}

        }

    }

    public void Toggle() {
        frozen = !frozen;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.name == "Open")
            area.fixtureManager.WalkThroughDoor(other.transform.parent.gameObject);

        if (other.gameObject.name == "DoorSensor")
            area.fixtureManager.TripSensor(other.transform.parent.gameObject);

    }

    void OnTriggerExit2D(Collider2D other)
    {

        if (other.gameObject.name == "DoorSensor")
            area.fixtureManager.SensorClear(other.transform.parent.gameObject);

    }

    

}
