using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting System/Recipe")]
public class Recipe : ScriptableObject
{
    public GameObject prefab;
    public ItemObject[] ingredients;
    public int[] numIngredients;
    public ItemObject result;
    [TextArea(15, 20)]
    public string description;
}
