using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Coding with Unity Scriptable Object Inventory System
//https://www.youtube.com/watch?v=_IqTeruf3-s&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&index=1&ab_channel=CodingWithUnity

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public Inventory defaultInv;
    public List<InventorySlot> Container = new List<InventorySlot>();
    public InventorySlot[] materials;
    private string craft;
    private bool update = false;
    private int itemSelected = 0;
    private bool suppress = false;
    public ItemObject starlight;

    public void Awake()
    {
        if (defaultInv)
        {
            foreach (InventorySlot s in defaultInv.Container)
            {
                Container.Add(new InventorySlot(s.item, s.amount));
            }
            update = true;
        }
        suppress = false;
    }

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
        else if (Container[itemSelected].hasItem("Sword")) return 50;
        else if (Container[itemSelected].hasItem("Dagger")) return 10;
        else return 1;
    }
    public int getKnockback()
    {
        if (itemSelected >= Container.Count) return 2;
        else if (Container[itemSelected].hasItem("Sword")) return 10;
        else if (Container[itemSelected].hasItem("Dagger")) return 5;
        else return 2;
    }

    private bool stackItem(ItemObject item, int amount)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].hasItem(item))
            {
                materials[i].AddAmount(amount);
                return true;
            }
        }
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
    public bool hasItem(ItemObject item)
    {
        foreach (InventorySlot s in materials)
        {
            if (s.hasItem(item)) return true;
        }
        foreach (InventorySlot slot in Container)
        {
            if (slot.hasItem(item)) return true;
        }
        return false;
    }
    public bool hasItem(string itemName)
    {
        foreach (InventorySlot s in materials)
        {
            if (s.hasItem(itemName)) return true;
        }
        foreach (InventorySlot slot in Container)
        {
            if (slot.hasItem(itemName)) return true;
        }
        return false;
    }
    public bool hasItem(ItemObject item, int amount)
    {
        int invAmount = 0;
        foreach (InventorySlot slot in materials)
        {
            if (slot.hasItem(item)) invAmount += slot.getAmount();
        }
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
        AddItem(recipe.result,recipe.numResult);
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
        ensureCollector();
    }
    public bool removeItem(ItemObject item, int amount)
    {
        if (!hasItem(item, amount)) return false;
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].hasItem(item) && amount >= 0)
            {
                if (materials[i].AddAmount(-amount)) return true;
                else
                {
                    amount -= materials[i].getAmount(); 
                }
            }
        }
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].hasItem(item) && amount>=0)
            {
                if (Container[i].AddAmount(-amount)) return true;
                else
                {
                    amount -= Container[i].getAmount();
                    if (item.name!="Drop of Starlight") Container.RemoveAt(i);
                }
            }
        }
        update = true;
        ensureCollector();
        return amount <= 0;
    }
    public int getAmount(string itemName)
    {
        int amount = 0;
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].hasItem(itemName)) amount += materials[i].getAmount();
        }
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
