using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class DHSequence : MonoBehaviour
{
    public string Name;
    [Tooltip("This list of objects will be activated at the start of the instructions!")]
    public List<GameObject> ObjectsToActivateAtTheStart = new List<GameObject>();
    // This list of objects will be activated at the end of the instructions!
    private List<GameObject> _objectsToActivateAtTheEnd = new List<GameObject>();
    [Space(10)]
    [Tooltip("This list of objects will be DEactivated at the start of the instructions!")]
    public List<GameObject> ObjectsToDeactivateAtTheStart = new List<GameObject>();
    // This list of objects will be DEactivated at the end of the instructions!
    private List<GameObject> _objectsToDeactivateAtTheEnd = new List<GameObject>();
    [TextArea(0, 100)]
    public string[] EnglishInstructions;
    [TextArea(0, 100)]
    public string[] DutchInstructions;

    private int _currentHighlightIndex;
    public List<GameObject> ObjectsToHighlight = new List<GameObject>();
    private List<KeyValuePair<string, int>> _sortingLayersToRestore = new List<KeyValuePair<string, int>>();

    private int _currentInstructionIndex;
    public TextMeshProUGUI InstructionsText;

    public void InitiateSequence()
    {
        _sortingLayersToRestore.Clear();
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                CheckLengthOfLines(EnglishInstructions);
                InstructionsText.text = EnglishInstructions[_currentInstructionIndex];
                break;
            case "Dutch":
                CheckLengthOfLines(DutchInstructions);
                InstructionsText.text = DutchInstructions[_currentInstructionIndex];
                break;
        }

        foreach (GameObject obj in ObjectsToHighlight)
        {
            if (obj.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

                _sortingLayersToRestore.Add(new KeyValuePair<string, int>(obj.name, spriteRenderer.sortingOrder));
            }
            else if (obj.GetComponent<Image>())
            {
                Canvas canvas = obj.GetComponent<Canvas>();

                _sortingLayersToRestore.Add(new KeyValuePair<string, int>(obj.name, canvas.sortingOrder));
            }
        }

        NextHighlight();

        _objectsToActivateAtTheEnd = ObjectsToDeactivateAtTheStart;
        _objectsToDeactivateAtTheEnd = ObjectsToActivateAtTheStart;

        foreach (GameObject obj in ObjectsToDeactivateAtTheStart)
        {
            obj.SetActive(false);
        }
    }

    public void NextLine()
    {
        _currentInstructionIndex++;
        _currentHighlightIndex++;

        NextHighlight();
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                CheckLengthOfLines(EnglishInstructions);
                InstructionsText.text = EnglishInstructions[_currentInstructionIndex];
                break;
            case "Dutch":
                CheckLengthOfLines(DutchInstructions);
                InstructionsText.text = DutchInstructions[_currentInstructionIndex];
                break;
        }
    }

    private void CheckLengthOfLines(string[] lines)
    {
        if (_currentInstructionIndex > lines.Length - 1)
        {
            _currentInstructionIndex = 0;
            _currentHighlightIndex = 0;

            foreach (GameObject obj in ObjectsToHighlight)
            {
                foreach (KeyValuePair<string, int> pair in _sortingLayersToRestore)
                {
                    if (obj.name == pair.Key)
                    {
                        if (obj.GetComponent<SpriteRenderer>())
                        {
                            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

                            spriteRenderer.sortingOrder = pair.Value;
                        }
                        else if (obj.GetComponent<Image>())
                        {
                            Canvas canvas = obj.GetComponent<Canvas>();

                            canvas.sortingOrder = pair.Value;
                        }
                    }
                }
            }

            foreach (GameObject obj in _objectsToActivateAtTheEnd)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in _objectsToDeactivateAtTheEnd)
            {
                obj.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }

    public void NextHighlight()
    {
        Debug.Log(_currentHighlightIndex);
        // Restores the layers after they were modified to highlight those items.
        if (_currentHighlightIndex < ObjectsToHighlight.Count)
        {
            int i = 0;
            foreach (KeyValuePair<string, int> pair in _sortingLayersToRestore)
            {
                if (ObjectsToHighlight[_currentHighlightIndex].name == pair.Key)
                {
                    if (ObjectsToHighlight[_currentHighlightIndex].GetComponent<SpriteRenderer>())
                    {
                        SpriteRenderer spriteRenderer = ObjectsToHighlight[_currentHighlightIndex].GetComponent<SpriteRenderer>();

                        spriteRenderer.sortingOrder = 100;
                    }
                    else if (ObjectsToHighlight[_currentHighlightIndex].GetComponent<Image>())
                    {
                        Canvas canvas = ObjectsToHighlight[_currentHighlightIndex].GetComponent<Canvas>();

                        canvas.sortingOrder = 100;
                    }
                }
                else
                {
                    if (i < ObjectsToHighlight.Count - 1)
                    {
                        if (ObjectsToHighlight[i].GetComponent<SpriteRenderer>())
                        {
                            SpriteRenderer spriteRenderer = ObjectsToHighlight[i].GetComponent<SpriteRenderer>();

                            spriteRenderer.sortingOrder = pair.Value;
                        }
                        else if (ObjectsToHighlight[i].GetComponent<Image>())
                        {
                            Canvas canvas = ObjectsToHighlight[i].GetComponent<Canvas>();

                            canvas.sortingOrder = pair.Value;
                        }
                    }
                }
                i++;
            }
        }
        else
        {
            foreach (KeyValuePair<string, int> pair in _sortingLayersToRestore)
            {
                foreach (GameObject obj in ObjectsToHighlight)
                {
                    if (obj.name == pair.Key)
                    {
                        if (obj.GetComponent<SpriteRenderer>())
                        {
                            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

                            spriteRenderer.sortingOrder = pair.Value;
                        }
                        else if (obj.GetComponent<Image>())
                        {
                            Canvas canvas = obj.GetComponent<Canvas>();

                            canvas.sortingOrder = pair.Value;
                        }
                    }
                }
            }
        }
    }
}
