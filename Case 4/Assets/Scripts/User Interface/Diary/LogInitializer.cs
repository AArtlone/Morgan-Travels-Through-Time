using TMPro;
using UnityEngine;

public class LogInitializer : MonoBehaviour
{
    private TextMeshProUGUI _textComponent;
    private DiaryDialogueLogManager _diaryDialogueLogManager;

    private void Start()
    {
        _textComponent = GetComponentInChildren<TextMeshProUGUI>();
        _diaryDialogueLogManager = FindObjectOfType<DiaryDialogueLogManager>();
    }

    public void LoadLogEntry()
    {
        _diaryDialogueLogManager.SetupLogEntries(_textComponent.text);
    }
}
