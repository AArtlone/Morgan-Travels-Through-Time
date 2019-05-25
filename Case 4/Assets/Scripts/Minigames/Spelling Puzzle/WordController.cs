using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class WordController : MonoBehaviour
{
    public List<WordToFindFormat> WordFormats = new List<WordToFindFormat>();

    private void Start()
    {
        if (transform.parent.transform.parent.name != "Fields")
        {
            foreach (WordToFindFormat format in WordFormats)
            {
                if (format.Format == SettingsManager.Instance.Language)
                {
                    transform.parent.GetComponentInChildren<TextMeshProUGUI>().text = format.ExpectedWord;
                }
            }
        }
    }
}
