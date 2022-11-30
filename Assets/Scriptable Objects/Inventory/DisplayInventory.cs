using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

//Coding with Unity Scriptable Object Inventory System
//https://www.youtube.com/watch?v=_IqTeruf3-s&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&index=1&ab_channel=CodingWithUnity
public class DisplayInventory : MonoBehaviour
{
    public Inventory inventory;
    private float xStart = 0.005f;
    private float yStart = 0.05f;
    private float width = 0.09f;
    private float height = 0.9f;
    private int numSlots = 10;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    public TextMeshProUGUI craftPrompt;
    private bool crafting;
    public GameObject itemSelector;
    private int itemSelected = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.updateNeeded()) UpdateDisplay();

        getKeyInput();
        
    }

    private void getKeyInput()
    {
        if (inventory.isSuppressed()) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) setSelection(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) setSelection(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) setSelection(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) setSelection(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) setSelection(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) setSelection(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) setSelection(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) setSelection(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) setSelection(8);
        if (Input.GetKeyDown(KeyCode.Alpha0)) setSelection(9);

        if (Input.GetKeyDown(KeyCode.Mouse1)) useItem(itemSelected);
    }

    //--------------------
    public void useItem(int item)
    {
        
    }
    public void openCrafting()
    {

    }
    //---------------------

    public void changeSelection(int i)
    {
        int temp = itemSelected;
        itemSelected = Math.Max(Math.Min(itemSelected + i, numSlots), 0);
        inventory.setUpdate(temp != itemSelected);
        inventory.setSelection(itemSelected);
    }
    public void setSelection(int i)
    {
        changeSelection(i - itemSelected);
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            CreateItem(i);
        }
    }
    public void CreateItem(int i)
    {
        var obj = Instantiate(inventory.Container[i].item.InvPrefab, Vector3.zero, Quaternion.identity, transform);
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        setPosition(rectTransform, i);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
        itemsDisplayed.Add(inventory.Container[i], obj);
    }
    private void setPosition(RectTransform rectTransform, int i, bool offset = true)
    {
        rectTransform.anchorMin = GetPosition(i,offset).min;
        rectTransform.anchorMax = GetPosition(i,offset).max;
        rectTransform.offsetMin = new Vector2();
        rectTransform.offsetMax = new Vector2();
    }
    public void UpdateDisplay()
    {
        inventory.setUpdate(false);

        //Remove nonexisting items
        List<InventorySlot> toRemove = new List<InventorySlot>();
        foreach (InventorySlot key in itemsDisplayed.Keys)
        {
            if (!inventory.Container.Contains(key))
            {
                GameObject.Destroy(itemsDisplayed[key]);
                toRemove.Add(key);
            }
        }
        foreach(InventorySlot key in toRemove)
        {
            itemsDisplayed.Remove(key);
        }

        //Add undisplayed items
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (itemsDisplayed.ContainsKey(inventory.Container[i]))
            {
                itemsDisplayed[inventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
            }
            else
            {
                CreateItem(i);
            }
        }

        //Craft prompt
        if (inventory.canCraftSword())
        {
            craftPrompt.text = "Press E to craft Sword";
        }
        else
        {
            craftPrompt.text = "";
        }

        //Change item selected
        RectTransform rectTransform = itemSelector.GetComponent<RectTransform>();
        setPosition(rectTransform, itemSelected, false);

    }

    public (Vector2 min, Vector2 max) GetPosition(int i, bool offset=true)
    {
        if (offset)
        {
            Vector2 min = new Vector2(xStart + i / (float)numSlots, yStart);
            Vector2 max = new Vector2(xStart + width + i / (float)numSlots, yStart + height);
            return (min, max);
        }
        else
        {
            Vector2 min = new Vector2(i / (float)numSlots, 0);
            Vector2 max = new Vector2((i + 1) / (float)numSlots, 1);
            return (min, max);
        }
        
    }

    //public void openCrafting()

}
