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
    private List<int> _sortingLayersToRestore = new List<int>();

    private int _currentInstructionIndex;
    private TextMeshProUGUI _instructionsText;

    private void Start()
    {
        NextHighlight();

        _objectsToActivateAtTheEnd = ObjectsToDeactivateAtTheStart;
        _objectsToDeactivateAtTheEnd = ObjectsToActivateAtTheStart;

        foreach (GameObject obj in ObjectsToDeactivateAtTheStart)
        {
            obj.SetActive(false);
        }
    }

    public void NextLine(TextMeshProUGUI textUI)
    {
        _instructionsText = textUI;
        _currentInstructionIndex++;

        NextHighlight();

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                CheckLengthOfLines(EnglishInstructions);
                textUI.text = EnglishInstructions[_currentInstructionIndex];
                break;
            case "Dutch":
                CheckLengthOfLines(DutchInstructions);
                textUI.text = DutchInstructions[_currentInstructionIndex];
                break;
        }
    }

    private void CheckLengthOfLines(string[] lines)
    {
        if (_currentInstructionIndex > lines.Length - 1)
        {
            _currentInstructionIndex = 0;
            _instructionsText.text = EnglishInstructions[_currentInstructionIndex];

            _sortingLayersToRestore.Clear();
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

    private void NextHighlight()
    {
        // Restores the layers after they were modified to highlight those items.
        if (_sortingLayersToRestore.Count > 0)
        {
            for (int i = 0; i < _sortingLayersToRestore.Count; i++)
            {
                if (ObjectsToHighlight[i].GetComponent<SpriteRenderer>())
                {
                    ObjectsToHighlight[i].GetComponent<SpriteRenderer>().sortingOrder = _sortingLayersToRestore[i];
                }
                else if (ObjectsToHighlight[i].GetComponent<Image>())
                {
                    Canvas canvas = ObjectsToHighlight[i].GetComponent<Canvas>();

                    ObjectsToHighlight[i].GetComponent<Canvas>().sortingOrder = _sortingLayersToRestore[i];
                }
            }
        }

        if (_currentHighlightIndex < ObjectsToHighlight.Count)
        {
            // Highlights and hides the objects in the next sequence of instructions.
            for (int i = 0; i < ObjectsToHighlight.Count; i++)
            {
                if (i == _currentHighlightIndex)
                {
                    if (ObjectsToHighlight[i] != null)
                    {
                        if (ObjectsToHighlight[i].GetComponent<SpriteRenderer>())
                        {
                            SpriteRenderer spriteRenderer = ObjectsToHighlight[i].GetComponent<SpriteRenderer>();

                            _sortingLayersToRestore.Add(spriteRenderer.sortingOrder);
                            spriteRenderer.sortingOrder = 100;
                        }
                        else if (ObjectsToHighlight[i].GetComponent<Image>())
                        {
                            Canvas canvas = ObjectsToHighlight[i].GetComponent<Canvas>();

                            _sortingLayersToRestore.Add(canvas.sortingOrder);
                            canvas.sortingOrder = 100;
                        }
                    }
                }
            }
            _currentHighlightIndex++;
        }
    }
}
