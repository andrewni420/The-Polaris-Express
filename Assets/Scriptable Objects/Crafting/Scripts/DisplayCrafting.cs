using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayCrafting : MonoBehaviour
{//Crashes when you press arrow too many times while changing recipesDisplayed
    public GameObject menuPrefab;
    private (GameObject menu, GameObject selector, GameObject materials) objects = (null,null,null);
    private (GameObject left, GameObject right) menu = (null,null);
    public GameObject materialsPrefab;
    public string[] materialNames = {"Stick", "Grass", "Wheat", "Metal", "Spoiled Meat", "Rope" };
    public List<RecipeBook> recipeBooks = new List<RecipeBook>();
    public List<Recipe> recipes = new List<Recipe>();
    private List<(Recipe recipe, int index, GameObject display)> recipesDisplayed = new List<(Recipe recipe, int index, GameObject display)>();
    private List<bool> craftable = new List<bool>();
    private Dictionary<int, GameObject> uncraftables = new Dictionary<int, GameObject>();
    public GameObject uncraftableSelector;
    public Inventory inventory;
    private int maxRecipes = 5;
    int recipeSelected = 0;
    bool update = false;
    bool active = false;

    public Spawner spawner;
    public PlayerMovement playerMovement;
    //top left
    //Need to select recipe using up/down/key, outline its display
    //All recipes go in well defined areas, so which one is highlighted just draw a lil box around it.
    //Ingredients will show in the right side of the book
    //Need to grey out uncraftable recipes
    //Need to craft recipe upon enter
    void Start()
    {
        foreach (RecipeBook book in recipeBooks) readBook(book);
    }

    // Start is called before the first frame update
    void startCrafting()
    {
        active = true;
        inventory.setSuppressed(true);
        spawner.setSuppressed(true);
        playerMovement.setSuppressed(true);
        createCraftable();
        //CreateMaterials();
        CreateMenu();
        CreateDisplay();
        update = true;
    }
    void endCrafting()
    {
        active = false;
        inventory.setSuppressed(false);
        spawner.setSuppressed(false);
        playerMovement.setSuppressed(false);
        Destroy(objects.menu);
        Destroy(objects.selector);
        //Destroy(objects.materials);
    }

    void readBook(RecipeBook book)
    {
        foreach (Recipe r in book.recipes) recipes.Add(r);
    }
    

    // Update is called once per frame
    void Update()
    {
        getKeyInput();
        if (update)
        {
            update = false;

            int count = 0;
            while (recipeSelected < recipesDisplayed[0].index && count<10)
            {
                decrementDisplay();
                count++;
            }

            while (recipeSelected > recipesDisplayed[maxRecipes - 1].index&&count<10)
            {
                incrementDisplay();
                count++;
            }
            if (count == 10) Debug.Log((count,recipeSelected, recipesDisplayed[0]));

            int position = -1;
            for (int i = 0; i < maxRecipes; i++)
            {
                if (recipesDisplayed[i].index == recipeSelected)
                {
                    position = i;
                    break;
                }
            }
            RectTransform rectTransform = objects.selector.GetComponent<RectTransform>();
            setPosition(rectTransform, position);


            displayRight();

            //updateMaterials();

        }
    }

    void displayRight()
    {
        foreach (Transform t in menu.right.transform) Destroy(t.gameObject);
        var obj = Instantiate(recipes[recipeSelected].rightPrefab, Vector3.zero, Quaternion.identity, menu.right.transform);
        RectTransform rt = obj.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0.05f, 0.05f);
        rt.anchorMax = new Vector2(0.95f, 0.95f);
        rt.offsetMin = new Vector2();
        rt.offsetMax = new Vector2();
    }

    void createCraftable()
    {
        foreach (Recipe r in recipes) craftable.Add(canCraft(r));
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
            if (craftable[i])
            {
                if (uncraftables.ContainsKey(i))
                {
                    Destroy(uncraftables[i]);
                    uncraftables.Remove(i);
                }
            }
            else
            {
                if (!uncraftables.ContainsKey(i))
                {
                    for (int j = 0; j < recipesDisplayed.Count; j++)
                    {
                        if (recipesDisplayed[j].index == i)
                        {
                            GameObject s = Instantiate(uncraftableSelector, Vector3.zero, Quaternion.identity, recipesDisplayed[j].display.transform);
                            RectTransform rectTransform = s.GetComponent<RectTransform>();
                            rectTransform.offsetMin = new Vector2();
                            rectTransform.offsetMax = new Vector2();
                            uncraftables.Add(i, s);
                        }
                    }
                }
            }
        }
    }

    void craft(Recipe recipe)
    {
        inventory.craftRecipe(recipe);
        updateCraftable();
        //updateMaterials();
    }
    public void CreateMaterials()
    {
       
        objects = (objects.menu, objects.selector, Instantiate(materialsPrefab, Vector3.zero, Quaternion.identity, transform));
        RectTransform rectTransform = objects.materials.GetComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2(0.3f, 0.9f);
        rectTransform.anchorMax = new Vector2(0.7f, 0.97f);
        rectTransform.offsetMin = new Vector2();
        rectTransform.offsetMax = new Vector2();

        updateMaterials();
    }
    public void updateMaterials()
    {
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
        menu = (objects.menu.transform.Find("CraftingLeft").gameObject, objects.menu.transform.Find("CraftingRight").gameObject);
        objects = (objects.menu, menu.left.transform.Find("Selector").gameObject, objects.materials);
        
    }
    public void CreateDisplay()
    {
        for(int i = 0; i < maxRecipes; i++)
        {
            GameObject display = createRecipe(i, recipes[i]);
            recipesDisplayed.Add((recipes[i], i, display));
            if (!craftable[i])
            {
                GameObject s = Instantiate(uncraftableSelector, Vector3.zero, Quaternion.identity, display.transform);
                RectTransform rectTransform = s.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2();
                rectTransform.offsetMax = new Vector2();
                uncraftables.Add(i,s);
            }
        }
    }

    public void incrementDisplay()
    {
        int n = recipesDisplayed.Count;
        int lastIndex = recipesDisplayed[n - 1].index;
        if (lastIndex + 1 >= recipes.Count) return;
        Destroy(recipesDisplayed[0].display);
        uncraftables.Remove(recipesDisplayed[0].index);
        recipesDisplayed.RemoveAt(0);
        for (int j = 0; j < recipesDisplayed.Count; j++) setPosition(recipesDisplayed[j].display.GetComponent<RectTransform>(), j,0.015f);
        GameObject display = createRecipe(maxRecipes - 1, recipes[lastIndex + 1]);
        recipesDisplayed.Add((recipes[lastIndex+1],lastIndex+1,display));

        if (!craftable[lastIndex+1])
        {
            GameObject s = Instantiate(uncraftableSelector, Vector3.zero, Quaternion.identity, display.transform);
            RectTransform rectTransform = s.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2();
            rectTransform.offsetMax = new Vector2();
            uncraftables.Add(lastIndex+1, s);
        }
    }

    public void decrementDisplay()
    {
        int n = recipesDisplayed.Count;
        int firstIndex = recipesDisplayed[0].index;
        if (firstIndex - 1 < 0) return;
        Destroy(recipesDisplayed[n-1].display);
        uncraftables.Remove(recipesDisplayed[n - 1].index);
        recipesDisplayed.RemoveAt(n-1);
        for (int j = 0; j < recipesDisplayed.Count; j++) setPosition(recipesDisplayed[j].display.GetComponent<RectTransform>(), j + 1,0.015f);
        GameObject display = createRecipe(0, recipes[firstIndex-1]);
        recipesDisplayed.Insert(0, (recipes[firstIndex - 1], firstIndex - 1, display));

        if (!craftable[firstIndex-1])
        {
            GameObject s = Instantiate(uncraftableSelector, Vector3.zero, Quaternion.identity, display.transform);
            RectTransform rectTransform = s.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2();
            rectTransform.offsetMax = new Vector2();
            uncraftables.Add(firstIndex-1, s);
        }
    }

    public GameObject createRecipe(int position, Recipe recipe)
    {
        var obj = Instantiate(recipe.leftPrefab, Vector3.zero, Quaternion.identity, menu.left.transform);
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        setPosition(rectTransform, position,0.015f);
        return obj;
    }
    public (Vector2 min, Vector2 max) GetPosition(int i)
    {
        Vector2 min = new Vector2(0, 1 - (i+1) / (float)maxRecipes);
        Vector2 max = new Vector2(1, 1 - i / (float)maxRecipes);
        return (min, max);
    }

    private void setPosition(RectTransform rectTransform, int i, float offset = 0)
    {
        Vector2 offsetVector = new Vector2(0, offset);
        rectTransform.anchorMin = GetPosition(i).min+offsetVector;
        rectTransform.anchorMax = GetPosition(i).max-offsetVector;
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
        update = true;
    }
    private void changeSelection(int i)
    {
        int temp = recipeSelected;
        recipeSelected = Math.Min(Math.Max(0,temp+i), recipes.Count-1);
        if (temp == recipeSelected) return;
        update = true;
    }

}
