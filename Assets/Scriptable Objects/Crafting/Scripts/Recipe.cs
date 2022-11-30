using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting System/Recipe")]
public class Recipe : ScriptableObject
{
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public ItemObject[] ingredients;
    public int[] numIngredients;
    public ItemObject result;
    public int numResult = 1;
    [TextArea(15, 20)]
    public string description;

}
