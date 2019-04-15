using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

    HeroController hero { get { return HeroController.Instance; } }

    public static MenuController Instance;

    public GameObject hud;
    public GameObject menu;
    public GameObject menuWindowPrefab;
    public GameObject optionPrefab;

    Stack<GameObject> menus;
    Dictionary<GameObject, Toggle[]> menuToOptionMap;
    Toggle[] options;
    int currentOption = 0;
    bool active = false;

	void Start ()
    {
        if (Instance == null)
            Instance = this;

        menus = new Stack<GameObject> ();
        menus.Push(menu);

        menuToOptionMap = new Dictionary<GameObject, UnityEngine.UI.Toggle[]>();
        options = menu.GetComponentsInChildren<Toggle>(true);

        // First Menu - Main Menu
        menuToOptionMap.Add(menu, options);
    }

    void Update()
    {
        if (!active)
            return;

        // Used to move cursor up and now in current menu.
        ScrollMenu();

        if (Input.GetButtonUp("Interact"))
        {
            if (menuToOptionMap[menus.Peek()][currentOption].name == "Back")
            {
                GameObject rMenu = menus.Pop();
                menuToOptionMap.Remove(rMenu);
                Destroy(rMenu);
                Clear();
            }
            else if (menuToOptionMap[menus.Peek()][currentOption].name == "Exit")
            {
                Toggle();
            }
            else 
            {
           
                GameObject menu_go = (GameObject)Instantiate(menuWindowPrefab);
                menu_go.SetActive(true);

                menu_go.name = menuToOptionMap[menus.Peek()][currentOption].name;
                menu_go.transform.GetChild(0).GetComponent<Text>().text = menuToOptionMap[menus.Peek()][currentOption].name;

                menu_go.transform.SetParent(hud.transform, false);

                menu_go.transform.position = menus.Peek().transform.position + (Vector3.right * 50.0f);

                // need to add options and attach to map.
                GameObject opt1 = (GameObject)Instantiate(optionPrefab);
                GameObject opt2 = (GameObject)Instantiate(optionPrefab);
                GameObject opt3 = (GameObject)Instantiate(optionPrefab);

                opt1.transform.SetParent(menu_go.transform.GetChild(1).transform);
                opt2.transform.SetParent(menu_go.transform.GetChild(1).transform);
                opt3.transform.SetParent(menu_go.transform.GetChild(1).transform);

                opt1.name = "Item 1";
                opt2.name = "Item 2";
                opt3.name = "Back";

                opt1.GetComponentInChildren<Text>().text = "Item 1";
                opt2.GetComponentInChildren<Text>().text = "Item 2";
                opt3.GetComponentInChildren<Text>().text = "Back";

                options = menu_go.GetComponentsInChildren<Toggle>(true);

                //RectTransform rt = menu_go.GetComponent<RectTransform>();
                //rt.transform.bo
                //menu_go.GetComponent<RectTransform>().rect.yMax = (float)options.Length * 40f; 

                menus.Push(menu_go);
                menuToOptionMap.Add(menu_go, options);
                Clear();
            }
            
        }

        


    }

    /// <summary>
    /// Used to move cursor on menu up and down. 
    /// </summary>
    void ScrollMenu()
    {

        menuToOptionMap[menus.Peek()][currentOption].isOn = false;

        if (Input.GetButtonUp("Up"))
        {
            currentOption--;
        }
        if (Input.GetButtonUp("Down"))
        {
            currentOption++;
        }

        if (currentOption >= menuToOptionMap[menus.Peek()].Length)
            currentOption = 0;
        if (currentOption < 0)
            currentOption = menuToOptionMap[menus.Peek()].Length - 1;

        menuToOptionMap[menus.Peek()][currentOption].isOn = true;
    }

    /// <summary>
    /// Used to open the main menu, or close all currently open menus.
    /// </summary>
    public void Toggle() {

        GameObject rMenu;

        // Sub-menus open, close all.
        while (menus.Count > 1)
        {
            rMenu = menus.Pop();
            menuToOptionMap.Remove(rMenu);
            Destroy(rMenu);
        }

        active = !active;
        if (active)
            Clear();

        menus.Peek().SetActive(active);

        hero.Toggle();
    }

    void Clear() {
        foreach (Toggle option in menuToOptionMap[menus.Peek()])
            option.isOn = false;

        currentOption = 0;
        menuToOptionMap[menus.Peek()][currentOption].isOn = true;

    }

}
