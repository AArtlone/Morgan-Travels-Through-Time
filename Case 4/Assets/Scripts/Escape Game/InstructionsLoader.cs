using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionsLoader : MonoBehaviour
{
    [TextArea(0, 100)]
    public List<string> EnglishLines;
    [TextArea(0, 100)]
    public List<string> DutchLines;

    private int _currentLineIndex;
    private TextMeshProUGUI _instructionsText;
    
    private void Start()
    {
        if (Character.Instance.TutorialCompleted == false)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void NextLine(TextMeshProUGUI textUI)
    {
        _instructionsText = textUI;
        _currentLineIndex++;
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                CheckLengthOfLines(EnglishLines);
                textUI.text = EnglishLines[_currentLineIndex];
                break;
            case "Dutch":
                CheckLengthOfLines(DutchLines);
                textUI.text = DutchLines[_currentLineIndex];
                break;
        }
    }

    private void CheckLengthOfLines(List<string> lines)
    {
        if (_currentLineIndex > lines.Count - 1)
        {
            _currentLineIndex = 0;
            _instructionsText.text = EnglishLines[_currentLineIndex];
            gameObject.SetActive(false);
        }
    }
}
