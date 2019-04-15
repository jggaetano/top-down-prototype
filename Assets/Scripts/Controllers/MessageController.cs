using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessageController : MonoBehaviour
{

    public GameObject popUpPrefab;
    public GameObject hud;

    HeroController hc { get { return HeroController.Instance; } }
    public static MessageController Instance;

    static GameObject popUp;
    bool showNext = true;

	void Start()
    {
        if (Instance == null)
            Instance = this;
        
        MessagesManager.RegisterShowMessage(ShowMessage);

        if (popUp == null)
        {
            popUp = (GameObject)Instantiate(popUpPrefab);
            popUp.transform.SetParent(hud.transform, false);
            popUp.SetActive(false);
            popUp.name = "PopUp";
        }

    }

    void Update() {

        if (MessagesManager.HasMessages() && showNext)
        {
            MessagesManager.GetNextMessage();
            showNext = false;
        }

        // TODO Need to be able to use Interact, but will require a more robust handler for both menu and messages. UIController.
        if (Input.GetKeyUp(KeyCode.E)) 
        {
            showNext = true;
            if (MessagesManager.HasMessages() == false)
                Disable();
        }

    }

    void Disable()
    {
        popUp.SetActive(false);
        if (hc != null)
            hc.frozen = false;
    }

    void ShowMessage(Message message) {

        hc.frozen = true;
        popUp.SetActive(true);
        popUp.transform.GetChild(0).GetComponent<Text>().text = message.MainText;
        popUp.transform.GetChild(1).GetComponent<Text>().text = message.Title;

    }


}
