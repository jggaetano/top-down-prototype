using UnityEngine;
using System.Collections;

public class Message  {

    public string Title { get; protected set; }
    public string MainText { get; protected set; }

    public Message() {
    }

    public Message(string title, string maintext) {
        Title = title;
        MainText = maintext;
    }
    
}
