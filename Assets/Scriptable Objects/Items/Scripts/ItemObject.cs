using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Coding with Unity Scriptable Object Inventory System
//https://www.youtube.com/watch?v=_IqTeruf3-s&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&index=1&ab_channel=CodingWithUnity
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
