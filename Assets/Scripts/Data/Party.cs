using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Party  {

    static int Wallet;
    public static Dictionary<string, Item> Pockets = new Dictionary<string, Item>();

    public static void AddMoney(int amount) {

        MessagesManager.AddMessage("Money", "You received " + amount.ToString() + " Gil.");
        Wallet += amount;
    }

    /// <summary>
    /// Try to subtract amount for current pool of money. If successful, bank will be deducted passed amount.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>Returns false if not enough money in the bank. Returns true if enough money in the bank and deducts amount.</returns>
    public static bool UseMoney(int amount)
    {

        if ((Wallet - amount) < 0)
            return false;

        Wallet -= amount;
        return true;

    }

    public static string ShowWallet() {
        return Wallet.ToString();
    }

    public static void AddItems(Item item) {

        if (item == null)
            return;

        if (Pockets.ContainsKey(item.Type) == false)
            Pockets.Add(item.Type, new Item());

        Pockets[item.Type].Amount += item.Amount;
        MessagesManager.AddMessage("Item", "You received " + item.Amount.ToString() + " " + item.Type + ".");


    }

    public static void AddItems(List<Item> items) {

        if (items == null)
            return;

        foreach (Item item in items)
        {
            AddItems(item);
        }
    }

    public static void RemoveItems(Item item) {

        if (item == null)
            return;

        if (Pockets.ContainsKey(item.Type) == false)
            return;

        Pockets[item.Type].Amount-= item.Amount;
        if (Pockets[item.Type].Amount == 0)
            Pockets.Remove(item.Type);
      
    }

    public static void RemoveItems(List<Item> items) {

        if (items == null)
            return;

        foreach (Item item in items)
        {
            RemoveItems(item);
        }

    }

    public static void RemoveItems(string item) {

        if (item == null)
            return;

        RemoveItems(new Item(item, 1));

    }

    public static void RemoveItems(string item, int amount)
    {

        if (item == null)
            return;

        RemoveItems(new Item(item, amount));

    }


}
