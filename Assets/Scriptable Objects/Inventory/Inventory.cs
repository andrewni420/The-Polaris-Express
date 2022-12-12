using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Coding with Unity Scriptable Object Inventory System
//https://www.youtube.com/watch?v=_IqTeruf3-s&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&index=1&ab_channel=CodingWithUnity

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    private string craft;
    private bool update = false;
    private int itemSelected = 0;
    private bool suppress = false;
    public ItemObject starlight;

    public string getCraft() { return craft; }
    public void setCraft(string item) { craft = item; }
    public void AddItem(ItemObject item, int amount)
    {
        if (!stackItem(item,amount))
        {
            Container.Add(new InventorySlot(item, amount));
        }
        update = true;

    }
    public bool updateNeeded() { return update; }
    public void setUpdate(bool update) { this.update = update; }
    public int getSize() { return Container.Count; }
    public int getSelection() { return itemSelected; }
    public void setSelection(int selection) { this.itemSelected = selection; }
    public int getDamage()
    {
        if (itemSelected >= Container.Count) return 1;
        else if (Container[itemSelected].hasItem("sword")) return 100;
        else if (Container[itemSelected].hasItem("dagger")) return 10;
        else return 1;
    }
    public int getKnockback()
    {
        if (itemSelected >= Container.Count) return 2;
        else if (Container[itemSelected].hasItem("sword")) return 10;
        else if (Container[itemSelected].hasItem("dagger")) return 5;
        else return 2;
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
        update = true;
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
    public bool hasItem(ItemObject item, int amount)
    {
        int invAmount = 0;
        foreach (InventorySlot slot in Container)
        {
            if (slot.hasItem(item)) invAmount+=slot.getAmount();
        }
        return invAmount>=amount;
    }
    public bool hasItem(string itemName, int amount)
    {
        int invAmount = 0;
        foreach (InventorySlot slot in Container)
        {
            if (slot.hasItem(itemName)) invAmount += slot.getAmount();
        }
        return invAmount>=amount;
    }
    public void craftRecipe(Recipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            removeItem(recipe.ingredients[i], recipe.numIngredients[i]);
        }
        Container.Add(new InventorySlot(recipe.result,recipe.numResult));
        update = true;
    }
    public bool canCraft(Recipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            if (!hasItem(recipe.ingredients[i], recipe.numIngredients[i])) return false;
        }
        return true;
    }
    public void removeItem(ItemObject item)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].hasItem(item))
            {
                Container.RemoveAt(i);
                return;
            }
        }
        update = true;
    }
    public bool removeItem(ItemObject item, int amount)
    {
        if (!hasItem(item, amount)) return false;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].hasItem(item) && amount>=0)
            {
                if (Container[i].AddAmount(-amount)) return true;
                else
                {
                    amount = -Container[i].getAmount();
                    if (item.name!="Drop of Starlight") Container.RemoveAt(i);
                }
            }
        }
        update = true;
        return amount <= 0;
    }
    public int getAmount(string itemName)
    {
         int amount = 0;
         for (int i = 0; i < Container.Count; i++)
         {
            if (Container[i].hasItem(itemName)) amount += Container[i].getAmount();
         }
        return amount;
    }
    public bool isSuppressed() { return suppress; }
    public void setSuppressed(bool s) { suppress = s; }

    public void ensureCollector()
    {
        foreach (InventorySlot slot in Container)
        {
            if (slot.hasItem("Drop of Starlight")) return;
        }
        AddItem(starlight,0);
        update = true;
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    //public int capacity
    //Add crafting - repair swords, make items for combat, consumables, curEquipped, different kinds of weapons, skills.


    public InventorySlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
    public bool AddAmount(int value)
    {
        amount += value;
        return amount>0;
    }
    public bool hasItem(ItemObject item)
    {
        return this.item.itemName == item.itemName;
    }
    public bool hasItem(string itemName)
    {
        return this.item.itemName == itemName;
    }
    public bool hasAmount(int amount)
    {
        return this.amount >= amount;
    }
    public int getAmount() { return amount; }

}
