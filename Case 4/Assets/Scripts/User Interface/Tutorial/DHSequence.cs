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
    private List<dynamic> _sortingLayersToRestore = new List<dynamic>();

    private int _currentInstructionIndex;
    public TextMeshProUGUI InstructionsText;

    public void InitiateSequence()
    {
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

                _sortingLayersToRestore.Add(new
                {
                    Name = obj.name,
                    SortingOrder = spriteRenderer.sortingOrder,
                });
            }
            else if (obj.GetComponent<Image>())
            {
                Canvas canvas = obj.GetComponent<Canvas>();

                _sortingLayersToRestore.Add(new
                {
                    Name = obj.name,
                    SortingOrder = canvas.sortingOrder,
                });
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
        NextHighlight();
    }

    private void CheckLengthOfLines(string[] lines)
    {
        if (_currentInstructionIndex > lines.Length - 1)
        {
            _currentInstructionIndex = 0;
            _currentHighlightIndex = 0;

            for (int i = 0; i < ObjectsToHighlight.Count; i++)
            {
                if (ObjectsToHighlight[i].name == _sortingLayersToRestore[i].Name)
                {
                    if (ObjectsToHighlight[i].GetComponent<SpriteRenderer>())
                    {
                        SpriteRenderer spriteRenderer = ObjectsToHighlight[i].GetComponent<SpriteRenderer>();

                        spriteRenderer.sortingOrder = _sortingLayersToRestore[i].SortingOrder;
                    }
                    else if (ObjectsToHighlight[i].GetComponent<Image>())
                    {
                        Canvas canvas = ObjectsToHighlight[i].GetComponent<Canvas>();

                        canvas.sortingOrder = _sortingLayersToRestore[i].SortingOrder;
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
        // Restores the layers after they were modified to highlight those items.
        if (_currentHighlightIndex < ObjectsToHighlight.Count)
        {
            for (int i = 0; i < _sortingLayersToRestore.Count; i++)
            {
                if (ObjectsToHighlight[_currentHighlightIndex].name == _sortingLayersToRestore[i].Name)
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
                } else
                {
                    if (i < ObjectsToHighlight.Count - 1)
                    {
                        if (ObjectsToHighlight[i].GetComponent<SpriteRenderer>())
                        {
                            SpriteRenderer spriteRenderer = ObjectsToHighlight[i].GetComponent<SpriteRenderer>();

                            spriteRenderer.sortingOrder = _sortingLayersToRestore[i].SortingOrder;
                        }
                        else if (ObjectsToHighlight[i].GetComponent<Image>())
                        {
                            Canvas canvas = ObjectsToHighlight[i].GetComponent<Canvas>();

                            canvas.sortingOrder = _sortingLayersToRestore[i].SortingOrder;
                        }
                    }
                }
            }
        }
    }
}
