using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Each book unlocks a set of recipes
[CreateAssetMenu(fileName = "New Recipe Book", menuName = "Crafting System/RecipeBook")]
public class RecipeBook : ScriptableObject
{
    public string description;
    public List<Recipe> recipes;
}
