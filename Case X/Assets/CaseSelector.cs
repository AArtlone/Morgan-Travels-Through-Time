using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseSelector : MonoBehaviour
{
    private InterfaceManager _interfaceManager;

    private void Start()
    {
        _interfaceManager = GameObject.FindGameObjectWithTag("Interface Manager").GetComponent<InterfaceManager>();
    }

    public void LoadCaseToInterface(Object obj)
    {
        _interfaceManager.DisplayCaseDetails(obj);
    }
}
