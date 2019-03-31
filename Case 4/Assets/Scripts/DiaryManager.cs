using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    // References to the buttons and pages related to the diary and toggling.
    [SerializeField]
    private List<GameObject> _buttonsForPages = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _pages = new List<GameObject>();

    public void SelectPage(Object selectedButton)
    {
        GameObject buttonObj = (GameObject)selectedButton;
        
        foreach (GameObject button in _buttonsForPages)
        {
            Debug.Log("A");
            if (button == buttonObj)
            {
                button.SetActive(true);
                button.GetComponentInChildren<Image>().color = new Color(255, 255, 255, 1);

                foreach (GameObject page in _pages)
                {
                    Debug.Log("E");
                    if (button.name == "Quest Button")
                    {
                        if (page.name == "Quests Page")
                        {
                            page.SetActive(true);
                        } else
                        {
                            page.SetActive(false);
                        }
                    } else if (button.name == "Blueprints Button")
                    {
                        if (page.name == "Blueprints Page")
                        {
                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    } else if (button.name == "Help Button")
                    {
                        if (page.name == "Help Page")
                        {
                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    }
                }
            } else
            {
                button.GetComponentInChildren<Image>().color = new Color(255, 255, 255, 0.5f);
            }
        }
    }

}
