using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
    public Inventory inventory;
    public float xPad;
    public int numCols=5;
    public float yPad;
    private float xStart = -343f;
    private float yStart = -1f;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    public TextMeshProUGUI craftPrompt;
    private bool crafting;

    // Start is called before the first frame update
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.updateNeeded()) UpdateDisplay();
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
        var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
        obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
        itemsDisplayed.Add(inventory.Container[i], obj);
    }
    public void UpdateDisplay()
    {
        inventory.setUpdate(false);
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
        if (inventory.canCraftSword())
        {
            craftPrompt.text = "Press E to craft Sword";
        } else
        {
            craftPrompt.text = "";
        }
        
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(xStart+ xPad * (i % numCols), yStart + (-yPad) * (i / numCols), 0f);
    }

    //public void openCrafting()

}
