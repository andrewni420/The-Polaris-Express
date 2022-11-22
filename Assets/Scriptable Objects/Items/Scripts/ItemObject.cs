using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Food,
    Equipment,
    Default
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject InvPrefab;
    public GameObject EnvPrefab;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public string itemName;


}
