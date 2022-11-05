using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    private string craft;
    private bool update = false;

    public string getCraft()
    {
        return craft;
    }
    public void setCraft(string item)
    {
        craft = item;
    }
    public void AddItem(ItemObject item, int amount)
    {
        if (!stackItem(item,amount))
        {
            Container.Add(new InventorySlot(item, amount));
        }
        update = true;

    }
    public bool updateNeeded()
    {
        return update;
    }
    public void setUpdate(bool update)
    {
        this.update = update;
    }
    private bool stackItem(ItemObject item, int amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].hasItem(item))
            {
                Container[i].AddAmount(amount);
                return true;
            }
        }
        return false;
    }
    public bool craftSword()
    {
        int daggerSlot = -1;
        int metalSlot = -1;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].hasItem("Dagger")) daggerSlot = i;
            if (Container[i].hasItem("Metal")) metalSlot = i;
        }
        if (daggerSlot>=0 && metalSlot>=0)
        {
            Container.RemoveAt(Math.Max(daggerSlot,metalSlot));
            Container.RemoveAt(Math.Min(daggerSlot, metalSlot));
            GameObject sword = GameObject.Find("Sword");
            var item = sword.GetComponent<Item>();
            this.AddItem(item.getItem(), item.getAmount());
            Destroy(sword);
            craft = "";
            update = true;
            return true;
        }
        return false;
    }
    public bool canCraftSword()
    {
        int daggerSlot = -1;
        int metalSlot = -1;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].hasItem("Dagger")) daggerSlot = i;
            if (Container[i].hasItem("Metal")) metalSlot = i;
        }
        return daggerSlot >= 0 && metalSlot >= 0;
    }
    public bool hasItem(ItemObject item)
    {
        foreach (InventorySlot slot in Container)
        {
            if (slot.hasItem(item)) return true;
        }
        return false;
    }
    public bool hasItem(string itemName)
    {
        foreach (InventorySlot slot in Container)
        {
            if (slot.hasItem(itemName)) return true;
        }
        return false;
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    //public int capacity


    public InventorySlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
    public bool AddAmount(int value)
    {
        amount += value;
        return true;
    }
    public bool hasItem(ItemObject item)
    {
        return this.item.itemName == item.itemName;
    }
    public bool hasItem(string itemName)
    {
        return this.item.itemName == itemName;
    }
}
