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

    public void LoadInventory()
    {
        foreach (Item item in Character.Instance.Items)
        {
            GameObject newItem = Instantiate(ItemPrefab, ItemsPanel.transform);
            Item newItemScript = newItem.GetComponent<Item>();

            // We use predefined images from the resources folder to load each
            // item's sprites from outside the game and assign it to the new item.
            // Debug.Log(item.Name);
            Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + item.AssetsImageName);

            newItem.GetComponent<Image>().sprite = sprite;
            newItemScript.Name = item.Name;
            newItemScript.Description = item.Description;
            newItemScript.Active = item.Active;
            newItemScript.AssetsImageName = item.AssetsImageName;
        }
    }
}