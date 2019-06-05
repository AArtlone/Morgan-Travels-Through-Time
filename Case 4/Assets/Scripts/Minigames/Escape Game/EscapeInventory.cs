using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EscapeInventory : MonoBehaviour
{
    private int _currentIDCount;


    public bool AddItem(Item item)
    {
        foreach (KeyValuePair<int, Item> pair in InterfaceManager.Instance.Items)
        {
            if (pair.Value.Name == item.Name)
            {
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
            }
        }

        InterfaceManager.Instance.Items.Remove(itemToRemove);
        _currentIDCount--;
        SortInventory();
    }

    public void SortInventory()
    {
        RefreshPanel();
    }

    public void CraftItem(Item item1, Item item2)
    {
        TextAsset itemCombinations = Resources.Load<TextAsset>("Default World Data/EscapeItemCombinations");
        JsonData itemCombinationsJson = JsonMapper.ToObject(itemCombinations.text);

        for (int i = 0; i < itemCombinationsJson["Combinations"].Count; i++)
        {
            for (int j = 0; j < itemCombinationsJson["Combinations"][i]["Recipe"].Count; j++)
            {
                if ((item1.Type.ToString() == itemCombinationsJson["Combinations"][i]["Recipe"][j]["RequieredItem1Type"].ToString() || item1.Type.ToString() == itemCombinationsJson["Combinations"][i]["Recipe"][j]["RequieredItem2Type"].ToString()) && (item2.Type.ToString() == itemCombinationsJson["Combinations"][i]["Recipe"][j]["RequieredItem1Type"].ToString() || item2.Type.ToString() == itemCombinationsJson["Combinations"][i]["Recipe"][j]["RequieredItem2Type"].ToString()))
                {
                    JsonData newItemJsonData = itemCombinationsJson["Combinations"][i]["Recipe"][j]["NewItem"];

                    Item newItem = new Item(newItemJsonData["Name"].ToString(), newItemJsonData["NameDutch"].ToString(), newItemJsonData["Description"].ToString(), newItemJsonData["DescriptionDutch"].ToString(), newItemJsonData["Active"].ToString(), newItemJsonData["ActiveDutch"].ToString(), newItemJsonData["AssetsImageName"].ToString());

                    Item.ItemType newItemType = (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), newItemJsonData["Type"].ToString());
                    newItem.Type = newItemType;

                    RemoveItem(item1);
                    RemoveItem(item2);
                    AddItem(newItem);
                }
            }
        }
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
            Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + InterfaceManager.Instance.Items[i].Value.AssetsImageName);

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

