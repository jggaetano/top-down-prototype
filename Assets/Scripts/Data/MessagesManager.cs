using System;
using UnityEngine;
using System.Collections.Generic;

public static class MessagesManager {

    static Queue<Message> messageQueue = new Queue<Message>();
    static Action<Message> cbShowMessage;

    public static void AddMessage(string message) {
        messageQueue.Enqueue(new Message("", message));
    }

    public static void AddMessage(string title, string maintext)
    {
        messageQueue.Enqueue(new Message(title, maintext));
    }

    public static void AddMessage(Message message)
    {
        messageQueue.Enqueue(message);
    }

    public static void GetNextMessage() {

        if (cbShowMessage != null)
            cbShowMessage(messageQueue.Dequeue());
    }

    public static bool HasMessages()
    {
        return messageQueue.Count != 0;
    }


    public static void RegisterShowMessage(Action<Message> callbackfunc)
    {
        cbShowMessage += callbackfunc;
    }

    public static void UnregisterShowMessage(Action<Message> callbackfunc)
    {
        cbShowMessage -= callbackfunc;
    }

}
