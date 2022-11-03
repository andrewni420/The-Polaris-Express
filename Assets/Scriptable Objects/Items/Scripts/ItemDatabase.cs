using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;
    public Dictionary<ItemObject, int> GetID = new Dictionary<ItemObject, int>();

    public void OnAfterDeserialize()
    {
        GetID = new Dictionary<ItemObject, int>();
        for (int i = 0; i < Items.Length; i++)
        {
            GetID.Add(Items[i], i);
        }
    }
    public void OnBeforeSerialize()
    {

    }
}
