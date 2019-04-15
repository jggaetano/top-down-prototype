using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InfoManager : MonoBehaviour {

    Text moneyText;
    Text itemText;

	// Use this for initialization
	void Start () {

        moneyText = transform.GetChild(0).gameObject.GetComponent<Text>();
        itemText = transform.GetChild(1).gameObject.GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {


        moneyText.text = "Gil: " + Party.ShowWallet();
        itemText.text = "Items: \n";
        foreach (string item in Party.Pockets.Keys)
        {
            if (Party.Pockets[item].Amount > 1)
                itemText.text += item + " (x" + Party.Pockets[item].Amount.ToString() + ")";
            else
                itemText.text += item;
            itemText.text += "\n";

        }



    }

}
