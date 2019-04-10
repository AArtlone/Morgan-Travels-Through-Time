using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsLoader : MonoBehaviour
{
    public GameObject ItemsPanel;
    public GameObject ItemPrefab;

    public void Start()
    {
        Character.Instance.SetupWorldData();
        LoadInventory();
    }

    /// <summary>
    /// Updates the panel on the inventory in the game based on the player's items.
    /// </summary>
    public void LoadInventory()
    {
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                InstantiateItemsFormatToPanel(Character.Instance.Items);
                break;
            case "Dutch":
                InstantiateItemsFormatToPanel(Character.Instance.ItemsDutch);
                break;
        }
    }
    
    /// <summary>
    /// This function creates the items in the inventory panel based on the player's
    /// language configuration.
    /// </summary>
    /// <param name="itemsForPanel"></param>
    private void InstantiateItemsFormatToPanel(List<Item> itemsForPanel)
    {
        for (int i = 0; i < GameObject.FindGameObjectWithTag("Items Panel").transform.childCount; i++)
        {
            Destroy(GameObject.FindGameObjectWithTag("Items Panel").transform.GetChild(i).gameObject);
        }

        foreach (Item item in itemsForPanel)
        {
            GameObject newItem = Instantiate(ItemPrefab, ItemsPanel.transform);
            Item newItemScript = newItem.GetComponent<Item>();

            // We use predefined images from the resources folder to load each
            // item's sprites from outside the game and assign it to the new item.
            // Debug.Log(item.Name);
            Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + item.AssetsImageName);

            newItem.GetComponent<Image>().sprite = sprite;
            switch (SettingsManager.Instance.Language)
            {
                case "English":
                    newItemScript.Name = item.Name;
                    newItemScript.Description = item.Description;
                    newItemScript.Active = item.Active;
                    break;
                case "Dutch":
                    newItemScript.NameDutch = item.NameDutch;
                    newItemScript.DescriptionDutch = item.DescriptionDutch;
                    newItemScript.ActiveDutch = item.ActiveDutch;
                    break;
            }
            newItemScript.AssetsImageName = item.AssetsImageName;
        }
    }
}