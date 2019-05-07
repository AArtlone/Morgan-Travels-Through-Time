using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSelector : MonoBehaviour
{
    private InterfaceManager _interfaceManager;
    private TextMeshProUGUI _questLabel;

    public Image StatusIcon;
    [Space(10)]
    [Header("Icons that indicate the status of this button's quest")]
    public Sprite AvailableIcon;
    public Sprite OngoingIcon;
    public Sprite CompleteIcon;

    private void Start()
    {
        _interfaceManager = FindObjectOfType<InterfaceManager>();
        _questLabel = GetComponentInChildren<TextMeshProUGUI>();

        SetStatusIcon();
    }

    public void SetStatusIcon()
    {
        string questProgress = string.Empty;
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                foreach (Quest quest in Character.Instance.AllQuests)
                {
                    if (quest.Name == _questLabel.text)
                    {
                        questProgress = quest.ProgressStatus;
                    }
                }
                break;
            case "Dutch":
                foreach (Quest quest in Character.Instance.AllQuestsDutch)
                {
                    if (quest.Name == _questLabel.text)
                    {
                        questProgress = quest.ProgressStatus;
                    }
                }
                break;
        }

        if (questProgress == "Ongoing" || questProgress == "Nog niet gedaan")
        {
            StatusIcon.sprite = OngoingIcon;
        }
        else if (questProgress == "Complete" || questProgress == "Gedaan")
        {
            StatusIcon.sprite = CompleteIcon;
        }
    }

    /// <summary>
    /// Displays the currently selected by the player quest from the
    /// items inventory menu.
    /// </summary>
    /// <param name="obj"></param>
    public void LoadQuestToInterface(Object obj)
    {
        _interfaceManager.DisplayQuestDetails(obj);
    }
}
