using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemObject item;
    public int amount=1;

    public ItemObject getItem()
    {
        return item;
    }
    public int getAmount()
    {
        return amount;
    }
    public bool hasItem(ItemObject item)
    {
        return item.itemName == this.item.itemName;
    }
}
