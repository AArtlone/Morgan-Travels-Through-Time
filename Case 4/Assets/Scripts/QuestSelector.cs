using UnityEngine;

public class QuestSelector : MonoBehaviour
{
    private InterfaceManager _interfaceManager;

    private void Start()
    {
        _interfaceManager = GameObject.FindGameObjectWithTag("Interface Manager").GetComponent<InterfaceManager>();
    }

    public void LoadQuestToInterface(Object obj)
    {
        _interfaceManager.DisplayQuestDetails(obj);
    }
}
