using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionsController : MonoBehaviour
{
    // Settings that will be loaded into the interface window
    private int _currentResolutionIndex;
    private string _currentResolutionName;
    public List<string> Resolutions = new List<string>();
    public TextMeshProUGUI PurrentResolutionText;

    #region The following functions run whenever the player moves forwards and backwards in the resolution settings field.
    public void ChangeResolution(string direction)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        GetCurrentResolution();

        if (direction == "Next")
        {
            _currentResolutionIndex++;
            if (_currentResolutionIndex > Resolutions.Count - 1)
            {
                _currentResolutionIndex = Resolutions.Count - 1;
            }

            PurrentResolutionText.text = Resolutions[_currentResolutionIndex];
        } else if (direction == "Previous")
        {
            GetCurrentResolution();

            _currentResolutionIndex--;
            if (_currentResolutionIndex <= 0)
            {
                _currentResolutionIndex = 0;
            }

            PurrentResolutionText.text = Resolutions[_currentResolutionIndex];
        }
    }

    public void GetCurrentResolution()
    {
        _currentResolutionName = string.Empty;
        _currentResolutionName += Screen.currentResolution.width + "x" + Screen.currentResolution.height;

        for (int i = 0; i < Resolutions.Count; i++)
        {
            if (_currentResolutionName == Resolutions[i])
            {
                _currentResolutionIndex = i;
                break;
            }
        }
    }
    #endregion
}
