using UnityEngine;
using System;
using System.Collections.Generic;

public class Menu {
    
    public string Header { get; protected set; }
    public List<string> Options { get; protected set; }
    public Action menuAction;

    public Menu()
    {
    }

}
