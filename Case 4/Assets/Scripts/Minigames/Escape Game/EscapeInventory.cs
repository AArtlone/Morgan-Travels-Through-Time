using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeInventory : MonoBehaviour
{
    private int _currentIDCount;

    public void TempFunc1()
    {
        RemoveItem(InterfaceManager.Instance.Items[0].Value);
    }
    public void TempFunc2()
    {
        RemoveItem(InterfaceManager.Instance.Items[1].Value);
    }
    public void TempFunc3()
    {
        RemoveItem(InterfaceManager.Instance.Items[2].Value);
    }

    public bool AddItem(Item item)
    {
        foreach (KeyValuePair<int, Item> pair in InterfaceManager.Instance.Items)
        {
            if (pair.Value.Name == item.Name)
            {
                //Debug.Log("Item " + item.Name + " already exists in the inventory!");
                return false;
            }
        }
        _currentIDCount++;
        InterfaceManager.Instance.Items.Add(new KeyValuePair<int, Item>(_currentIDCount, item));
        SortInventory();
        return true;
    }

    public void RemoveItem(Item item)
    {
        KeyValuePair<int, Item> itemToRemove = new KeyValuePair<int, Item>();
        foreach (KeyValuePair<int, Item> pair in InterfaceManager.Instance.Items)
        {
            if (pair.Value.Name == item.Name)
            {
                itemToRemove = pair;
                //Debug.Log("Removed " + pair.Value);
            }
        }

        InterfaceManager.Instance.Items.Remove(itemToRemove);
        _currentIDCount--;
        SortInventory();
    }

    public void SortInventory()
    {
        //InterfaceManager.Instance.Items.Sort();
        RefreshPanel();
    }

    public void RefreshPanel()
    {
        if (InterfaceManager.Instance.InventoryPanel.transform.childCount > 0)
        {
            for (int i = 0; i < InterfaceManager.Instance.InventoryPanel.transform.childCount; i++)
            {
                Destroy(InterfaceManager.Instance.InventoryPanel.transform.GetChild(i).gameObject);
            }
        }

        List<KeyValuePair<int, Item>> TempInventory = new List<KeyValuePair<int, Item>>();

        for (int i = 0; i < InterfaceManager.Instance.Items.Count; i++)
        {
            GameObject newItem = Instantiate(InterfaceManager.Instance.ItemPrefab, InterfaceManager.Instance.InventoryPanel.transform);
            Item newItemScript = newItem.GetComponent<Item>();
            // We use predefined images from the resources folder to load each
            // item's sprites from outside the game and assign it to the new item.
            // Debug.Log(item.Name);
            Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + InterfaceManager.Instance.Items[i].Value.GetComponent<Item>().AssetsImageName);

            newItem.GetComponent<Image>().sprite = sprite;
            newItemScript.Type = InterfaceManager.Instance.Items[i].Value.Type;
            newItemScript.Name = InterfaceManager.Instance.Items[i].Value.Name;
            newItemScript.Description = InterfaceManager.Instance.Items[i].Value.Description;
            newItemScript.Active = InterfaceManager.Instance.Items[i].Value.Active;
            newItemScript.NameDutch = InterfaceManager.Instance.Items[i].Value.NameDutch;
            newItemScript.DescriptionDutch = InterfaceManager.Instance.Items[i].Value.DescriptionDutch;
            newItemScript.ActiveDutch = InterfaceManager.Instance.Items[i].Value.ActiveDutch;
            newItemScript.AssetsImageName = InterfaceManager.Instance.Items[i].Value.AssetsImageName;
            TempInventory.Add(new KeyValuePair<int, Item>(i, newItemScript));
        }

        InterfaceManager.Instance.Items.Clear();    
        foreach (KeyValuePair<int, Item> pair in TempInventory)
        {
            InterfaceManager.Instance.Items.Add(new KeyValuePair<int, Item>(pair.Key, pair.Value));
        }
    }
}

