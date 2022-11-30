using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayCrafting : MonoBehaviour
{
    public GameObject menuPrefab;
    private (GameObject menu, GameObject selector, GameObject materials) objects = (null,null,null);
    public GameObject materialsPrefab;
    public string[] materialNames = {"Mat1", "Mat2", "Mat3", "Mat4", "Mat5", "Mat6" };
    public List<RecipeBook> recipeBooks;
    public List<Recipe> recipes;
    private List<(Recipe recipe, int index, GameObject display)> recipesDisplayed = new List<(Recipe recipe, int index, GameObject display)>();
    public List<bool> craftable;
    public Inventory inventory;
    public int maxRecipes = 5;
    int recipeSelected = 0;
    bool update = false;
    bool active = false;
    //top left
    //Need to select recipe using up/down/key, outline its display
    //All recipes go in well defined areas, so which one is highlighted just draw a lil box around it.
    //Ingredients will show in the right side of the book
    //Need to grey out uncraftable recipes
    //Need to craft recipe upon enter

    // Start is called before the first frame update
    void startCrafting()
    {
        active = true;
        inventory.setSuppressed(true);
        CreateMaterials();
        CreateMenu();
        CreateDisplay();
    }
    void endCrafting()
    {
        active = false;
        inventory.setSuppressed(false);
        Destroy(objects.menu);
        Destroy(objects.selector);
        Destroy(objects.materials);
    }

    // Update is called once per frame
    void Update()
    {
        getKeyInput();
        if (update)
        {
            update = false;
            RectTransform rectTransform = objects.selector.GetComponent<RectTransform>();
            setPosition(rectTransform, recipeSelected);
        }
    }

    bool canCraft(Recipe recipe)
    {
        return inventory.canCraft(recipe);
    }
    void updateCraftable()
    {
        for (int i = 0; i < craftable.Count; i++)
        {
            craftable[i] = canCraft(recipes[i]);
            //Reinstantiate
        }
    }

    void craft(Recipe recipe)
    {
        inventory.craftRecipe(recipe);
        updateCraftable();
    }
    public void CreateMaterials()
    {
        objects = (objects.menu, objects.selector, Instantiate(materialsPrefab, Vector3.zero, Quaternion.identity, transform));
        RectTransform rectTransform = objects.materials.GetComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2(0.3f, 0.9f);
        rectTransform.anchorMax = new Vector2(0.7f, 0.97f);
        rectTransform.offsetMin = new Vector2();
        rectTransform.offsetMax = new Vector2();

        Transform t = objects.materials.transform;
        foreach (string s in materialNames)
        {
            t.Find(s).GetComponentInChildren<TextMeshProUGUI>().text = inventory.getAmount(s).ToString();
        }
    }
    public void CreateMenu()
    {
        objects = (Instantiate(menuPrefab, Vector3.zero, Quaternion.identity, transform),objects.selector, objects.materials);
        RectTransform rectTransform = objects.menu.GetComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2(0.2f, 0.25f);
        rectTransform.anchorMax = new Vector2(0.8f, 0.9f);
        rectTransform.offsetMin = new Vector2();
        rectTransform.offsetMax = new Vector2();

        recipeSelected = 0;
        objects = (objects.menu, objects.menu.transform.Find("CraftingLeft").Find("Selector").gameObject, objects.materials);
    }
    public void CreateDisplay()
    {
        for(int i = 0; i < maxRecipes; i++)
        {
            GameObject display = createRecipe(i, recipes[i]);
            recipesDisplayed.Add((recipes[i], i, display));
            craftable.Add(canCraft(recipes[i]));
        }
    }

    public void incrementDisplay()
    {
        int n = recipesDisplayed.Count;
        int lastIndex = recipesDisplayed[n - 1].index;
        if (lastIndex + 1 == recipes.Count) return;
        Destroy(recipesDisplayed[0].display);
        recipesDisplayed.RemoveAt(0);
        for (int j = 0; j < recipesDisplayed.Count; j++) setPosition(recipesDisplayed[j].display.GetComponent<RectTransform>(), j);
        GameObject display = createRecipe(maxRecipes - 1, recipes[lastIndex + 1]);
        recipesDisplayed.Add((recipes[lastIndex+1],lastIndex+1,display));
    }

    public void decrementDisplay()
    {
        int n = recipesDisplayed.Count;
        int firstIndex = recipesDisplayed[0].index;
        if (firstIndex - 1 <= 0) return;
        Destroy(recipesDisplayed[n-1].display);
        recipesDisplayed.RemoveAt(n-1);
        for (int j = 0; j < recipesDisplayed.Count; j++) setPosition(recipesDisplayed[j].display.GetComponent<RectTransform>(), j + 1);
        GameObject display = createRecipe(0, recipes[firstIndex-1]);
        recipesDisplayed.Insert(0, (recipes[firstIndex - 1], firstIndex - 1, display));
    }

    public GameObject createRecipe(int position, Recipe recipe)
    {
        var obj = Instantiate(recipe.leftPrefab, Vector3.zero, Quaternion.identity, objects.menu.transform.Find("CraftingLeft"));
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        setPosition(rectTransform, position);
        return obj;
    }
    public (Vector2 min, Vector2 max) GetPosition(int i)
    {
        Vector2 min = new Vector2(0, 1 - (i+1) / (float)maxRecipes);
        Vector2 max = new Vector2(1, 1 - i / (float)maxRecipes);
        return (min, max);

    }
    private void setPosition(RectTransform rectTransform, int i)
    {
        rectTransform.anchorMin = GetPosition(i).min;
        rectTransform.anchorMax = GetPosition(i).max;
        rectTransform.offsetMin = new Vector2();
        rectTransform.offsetMax = new Vector2();
    }
    private void getKeyInput()
    {
        if (!active)
        {
            if (Input.GetKeyDown(KeyCode.E)) startCrafting();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) setSelection(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) setSelection(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) setSelection(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) setSelection(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) setSelection(4);

        if (Input.GetKeyDown(KeyCode.DownArrow)) changeSelection(1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) changeSelection(-1);

        if (Input.GetKeyDown(KeyCode.Return) && canCraft(recipes[recipeSelected])) craft(recipes[recipeSelected]);
        if (Input.GetKeyDown(KeyCode.E)) endCrafting();
    }
    private void setSelection(int i)
    {
        int temp = recipeSelected;
        recipeSelected = i;
        update = temp!=recipeSelected;
    }
    private void changeSelection(int i)
    {
        int temp = recipeSelected;
        recipeSelected = Math.Min(Math.Max(0,temp+i), maxRecipes-1);
        update = temp != recipeSelected;
    }

}
