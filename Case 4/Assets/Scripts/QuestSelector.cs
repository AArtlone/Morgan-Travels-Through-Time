using UnityEngine;

public class QuestSelector : MonoBehaviour
{
    private InterfaceManager _interfaceManager;

    private void Start()
    {
        _interfaceManager = FindObjectOfType<InterfaceManager>();
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
