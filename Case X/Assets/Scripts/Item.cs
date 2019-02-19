using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

[System.Serializable]
public class Item : MonoBehaviour
{
    public string Name;
    public string Description;
    public string Active;
    public string AssetsImageName;

    private Character _characterScript;
    private Transform _itemsPanel;
    private Transform _canvas;

    private void Start()
    {
        _characterScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        _itemsPanel = GameObject.Find("Items").transform;
        _canvas = GameObject.Find("Canvas").transform;
    }

    public Item(string Name, string Description, string Active, string AssetsImageName)
    {
        this.Name = Name;
        this.Description = Description;
        this.Active = Active;
        this.AssetsImageName = AssetsImageName;
    }

    public void DragItem()
    {
        Touch touch = Input.GetTouch(0);
        transform.parent = _canvas;
        transform.position = new Vector2(touch.position.x, touch.position.y);
    }

    public void DropItem()
    {
        transform.parent = _itemsPanel;
        _characterScript.ReloadInventory();
    }
}
