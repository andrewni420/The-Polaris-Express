using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCrafting : MonoBehaviour
{
    public List<RecipeBook> recipeBooks;
    public List<Recipe> recipes;
    public List<(Recipe recipe, int index, GameObject display)> recipesDisplayed;
    public List<int> uncraftable;
    public int maxRecipes = 5;
    public float yshift = 100;
    int recipeSelected;
    //top left
    public float xStart;
    public float yStart;
    //Need to select recipe using up/down/key, outline its display
    //All recipes go in well defined areas, so which one is highlighted just draw a lil box around it.
    //Ingredients will show in the right side of the book
    //Need to grey out uncraftable recipes
    //Need to craft recipe upon enter

    // Start is called before the first frame update
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < maxRecipes; i++)
        {
            createRecipe(i,recipes[i]);
        }
    }

    public void incrementDisplay()
    {
        int n = recipesDisplayed.Count;
        int lastIndex = recipesDisplayed[n - 1].index;
        if (lastIndex + 1 == recipes.Count) return;
        Destroy(recipesDisplayed[0].display);
        recipesDisplayed.RemoveAt(0);
        for (int j=0; j < recipesDisplayed.Count; j++) recipesDisplayed[j].display.GetComponent<RectTransform>().localPosition += new Vector3(0f, yshift, 0f);
        GameObject display = createRecipe(maxRecipes - 1, recipes[lastIndex + 1]);
        recipesDisplayed.Add((recipes[lastIndex+1],lastIndex+1,display));
    }

    //public void decrementDisplay(int i)
    //{

    //}

    public GameObject createRecipe(int position, Recipe recipe)
    {
        var obj = Instantiate(recipe.prefab, Vector3.zero, Quaternion.identity, transform);
        obj.GetComponent<RectTransform>().localPosition = GetPosition(position, obj.GetComponent<RectTransform>().localPosition);
        //obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
        return obj;
    }
    public Vector3 GetPosition(int i, Vector3 pos)
    {
        return new Vector3(pos.x + xStart,pos.y+ yStart + yshift*i, 0f);
    }


}
