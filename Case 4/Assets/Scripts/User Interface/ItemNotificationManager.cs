using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotificationManager : MonoBehaviour
{
    public static ItemNotificationManager Instance;
    public GameObject ItemNotificationPrefab;
    private Animator _itemNotificationPrefabAnimator;

    void Start()
    {
        _itemNotificationPrefabAnimator = ItemNotificationPrefab.GetComponent<Animator>();
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    public void GetItem(Item item)
    {
        string name = string.Empty;

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                name = item.Name;
                break;
            case "Dutch":
                name = item.NameDutch;
                break;
        }
        ItemNotificationPrefab.GetComponentInChildren<TextMeshProUGUI>().text = name;

        ItemNotificationPrefab.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Items/Inventory/" + item.AssetsImageName);

        _itemNotificationPrefabAnimator.SetBool("Toggle", true);
        StartCoroutine(ReturnAchievement());
    }

    public void GetItem(Clothing item)
    {
        string name = string.Empty;

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                name = item.Name;
                break;
            case "Dutch":
                name = item.NameDutch;
                break;
        }
        ItemNotificationPrefab.GetComponentInChildren<TextMeshProUGUI>().text = name;

        ItemNotificationPrefab.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Items/Inventory/" + item.Icon);

        _itemNotificationPrefabAnimator.SetBool("Toggle", true);
        StartCoroutine(ReturnAchievement());
    }

    private IEnumerator ReturnAchievement()
    {
        yield return new WaitForSeconds(7f);
        _itemNotificationPrefabAnimator.SetBool("Toggle", false);
    }
}
