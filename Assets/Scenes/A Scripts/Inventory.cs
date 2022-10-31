using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    public static string[] itemList = new string[] {
    "rope","wood","iron","stone","stone_sword","iron_sword","stone_hammer","iron_hammer","bowl",
    "apple","apple_juice","health" };
    //public static Dictionary<string, List<(string, int)>> recipes = new Dictionary<string, List<(string,int)>> {
    //    {"stone_sword", new (string,int)[]{("wood_sword",1), ("stone",1)} },
    //    {"iron_sword", new (string,int)[]{("stone_sword",1), ("iron",1)} }
    //};
    public static Dictionary<string, List<(string,int)>> recipes = new Dictionary<string, List<(string,int)>>
    {
        {"stone_sword", new List<(string,int)>{("wood",1),("stone",2), ("rope",1) } },
        {"iron_sword", new List<(string,int)>{("wood",1),("iron",2), ("rope", 1) }  },
        {"stone_hammer", new List<(string,int)>{("wood",2),("stone",2), ("rope", 1) }  },
        {"iron_hammer", new List<(string,int)>{("wood",2),("iron",2), ("rope", 1) }  },
        {"bowl", new List<(string,int)>{("wood",3)}  },
        {"apple_juice", new List<(string,int)>{("bowl",1),("apple",3)}  }
    };
    public int capacity = 10;
    private List<string> items;
    // Start is called before the first frame update
    void Start()
    {
        items = new List<string>(capacity);

    }

    //returns False and does not pick up item if capacity reached
    //returns True and picks up item if capacity not reached
    bool pickUp(string item)
    {
        if (items.Count == capacity) return false;
        items.Add(item);
        return true;
    }

    bool remove(string item)
    {
        if (items.Contains(item)){
            items.Remove(item);
            return true;
        }
        return false;
    }

    


}
